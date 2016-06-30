using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using APIMonLib;

namespace CPN
{
    public class Transition : Node
    {
        //key is selector and value is a filter
        private Dictionary<Node, Func<Token, bool>> unary_filtering_expressions = new Dictionary<Node, Func<Token, bool>>();

        //key is an array of 2 strings first is -from- and second is -to-
        private SortedDictionary<Node[], Func<Token, Token, bool>> binary_filtering_expressions = new SortedDictionary<Node[], Func<Token, Token, bool>>(new SelectorComparer());

        /// <summary>
        /// this field is used by this transition as temporary storage of tokens obtained
        /// through enabling process. It's valid only during the process of notification of
        /// other nodes.
        /// </summary>
        private IEnumerable<Tuple> temporary_store;

        public Transition(String name)
            : base(name)
        {
        }

        /// <summary>
        /// <para>Selector and filter applied in a following way. First we select all 
        /// tokens satisfying selection criteria. Than we apply filter and drop
        /// tokens that does NOT satisfy filter. Hence selector limits dropping
        /// capability of filter. Tokens NOT satisfying selection criteria go through unchanged.</para>
        /// <para>In other words if filter applies only to tokens from the place equal to filter selector
        /// Other tokens go through unchanged.</para>para>
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="filter"></param>
        public Transition addUnaryExpression(Place selector, Func<Token, bool> filter)
        {
            unary_filtering_expressions.Add(selector, filter);
            return this;
        }

        /// <summary>
        /// Adds binary filtering expression to the transition. All expressions will be applied on the AND basis. 
        /// In other words only tuples satisfying all conditions at the same time will be returned
        /// <para>Rules for writting filters:</para>
        /// <para>1. There should be a filter for each input attached to transition </para>
        /// <para>2. All filters should overlap. There shouldn't be filters working on two independent slices of input. For example if we have
        ///     four inputs (i1,i2,i3,i4) there should be filters for example (i1,i2) (i2,i3) (i3,i4) but not (i1,i2) (i3,i4) </para>
        /// <para>3. Only one filter for each pair of inputs should exist</para>
        /// <para>4. Order of selectors is important. In this order data will be passed to filter</para>
        /// </summary>
        /// <param name="selector">this function should return array of two strings 
        /// corresponding to the use of it in the filter. Order DOES matter</param>
        /// <param name="filter">This function should return true if two tokens satisfy some mutual condition</param>
        public Transition addBinaryExpression(Place[] selector, Func<Token, Token, bool> filter)
        {
            if (selector.Length != 2)
            {
                throw new WrongSelectorProvidedException("Selector provided contains " + selector.Length+" items instead of 2");
            }
            if (binary_filtering_expressions.ContainsKey(selector))
            {
                throw new WrongSelectorProvidedException("There is a filter with the same selector (" + selector[0]+","+selector[1]+")");
            }
            binary_filtering_expressions.Add(selector, filter);
            return this;
        }

        public override IEnumerable<Tuple> getTuples()
        {
            return temporary_store.ToArray();
        }

        public override void arcHasChangedEvent(Arc in_arc)
        {
            Dictionary<Node, IEnumerable<Token>> cumulative_input = new Dictionary<Node, IEnumerable<Token>>();
            foreach (Arc arc in this.input_arcs.Values)
            {
                cumulative_input.Add(arc.from, arc.getTokens());
            }
            IEnumerable<Tuple> cumulative_output = processExpressions(cumulative_input);
            if ((cumulative_output!=null) && (cumulative_output.Count() > 0))
            {
                //cumulative_output = filterOutRepetitiveOccurenceOfTokenInDifferentTuples(cumulative_output);
                //if (cumulative_output.Count() > 0)
                //{
                    this.enable(cumulative_output);
                //}
            }
        }


        private void enable(IEnumerable<Tuple> cumulative_output)
        {
            removeTokensFromIncomingArcs(cumulative_output);
            initializeTemporaryStore(cumulative_output);
            foreach (Arc output_arc in this.output_arcs.Keys)
            {
                output_arc.nodeAtInputHasChangedEvent();
            }
            initializeTemporaryStore(null);
        }

        /// <summary>
        /// This method fills temporary storage of the current transition with received tuples
        /// </summary>
        /// <param name="cumulative_output"></param>
        private void initializeTemporaryStore(IEnumerable<Tuple> tuples)
        {
            temporary_store = tuples;
        }

        private IEnumerable<Tuple> processExpressions(Dictionary<Node, IEnumerable<Token>> in_tokens)
        {
            Dictionary<Node, IEnumerable<Token>> step1 = checkUnaryExpressions(in_tokens);
            IEnumerable<Tuple> step2 = checkBinaryExpressions(step1);
            return step2;
        }

        /// <summary>
        /// Selector and filter applied in a following way. First we select all 
        /// tokens satisfying selection criteria. Than we apply filter and drop
        /// tokens that does NOT satisfy filter. Hence selector limits dropping
        /// capability of filter. Tokens NOT satisfying selection criteria go through unchanged.
        /// In other words filter applies only to tokens from the place equal to filter selector
        /// Other tokens go through unchanged.
        /// </summary>
        /// <param name="in_tokens">unfiltered set of tokens indexed by place name</param>
        /// <returns>filtered set of tokens</returns>
        private Dictionary<Node, IEnumerable<Token>> checkUnaryExpressions(Dictionary<Node, IEnumerable<Token>> in_tokens)
        {
            //TODO It might be possible not to copy tokens but remove them from initial dictionary
            Dictionary<Node, IEnumerable<Token>> out_tokens = new Dictionary<Node, IEnumerable<Token>>();

            foreach (Node place in in_tokens.Keys)
            {
                Func<Token, bool> filter = null;
                IEnumerable<Token> tokens_to_check = in_tokens[place];
                if (unary_filtering_expressions.TryGetValue(place, out filter))//only when appropriate selector exists
                {
                    IEnumerable<Token> result = (from token in tokens_to_check where filter(token) select token).ToArray();
                    out_tokens.Add(place, result);
                }
                else
                {//no selector found therefore add tokens unchanged
                    out_tokens.Add(place, tokens_to_check);
                }
            }
            return out_tokens;
        }

        private IEnumerable<Tuple> checkBinaryExpressions(Dictionary<Node, IEnumerable<Token>> in_tokens)
        {
            IEnumerable<Tuple> result = null;
            bool some_binary_filters_defined = false;
            foreach (Node[] selector in binary_filtering_expressions.Keys)
            {
                Func<Token, Token, bool> filter = binary_filtering_expressions[selector];
                if (result == null)
                {
                    result = checkInitialBinaryExpression(in_tokens, selector, filter);
                }
                else
                {
                    result = checkSingleBinaryExpression(result, in_tokens, selector, filter);
                }
                some_binary_filters_defined = true;
            }
            //TODO IMPORTANT check if the program checks if all inputs covered by filters
            if (!some_binary_filters_defined)
            {//no filters are defined so we just create wrap tokens provided into tuples
                //Why do we raise this exception? Otherwise we need to return full cartesian product of inputs
                if (in_tokens.Keys.Count != 1)
                {
                    throw new InvalidOperationException("No binary filters provided. Input must be of a single dimensionality. We do not return cartesian products");
                }
                foreach (Node key in in_tokens.Keys)
                {//As it easy to see this statement should and will run only ONCE
                    result = in_tokens[key].Select(token => new Tuple(token)).ToArray();
                }
            }
            return result;
        }

        /// <summary>
        /// Creates tuple out of two tokens provided
        /// </summary>
        private Func<Token, Token, Tuple> tuple_create = (token1, token2) =>
        {
            Tuple tuple = new Tuple();
            tuple.Add(token1.place, token1);
            tuple.Add(token2.place, token2);
            return tuple;
        };

        /// <summary>
        /// Checks first binary expression in the list. Creates tuples of results
        /// </summary>
        /// <param name="in_tokens"></param>
        /// <param name="selector"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IEnumerable<Tuple> checkInitialBinaryExpression(Dictionary<Node, IEnumerable<Token>> in_tokens,
                                                                                Node[] selector,
                                                                                Func<Token, Token, bool> filter
                                                                              )
        {
            //IEnumerable<Tuple> result = null;
            List<Tuple> result = new List<Tuple>();
            IEnumerable<Token> tokens1 = in_tokens[selector[0]];
            IEnumerable<Token> tokens2 = in_tokens[selector[1]];
            //TODO It might be good to check which input is longer and put it into inner loop
            foreach(Token outer_token in tokens1){
                foreach (Token inner_token in tokens2)
                {
                    if (filter(outer_token, inner_token)) result.Add(tuple_create(outer_token, inner_token));
                }
            }
            //result = from token1 in tokens1 from token2 in tokens2 where filter(token1, token2) select tuple_create(token1, token2);
            //Tuple[] array_result = result.ToArray();
            if (result.Count == 0) return null;
            else return result;
        }

        //private IEnumerable<Tuple> checkInitialBinaryExpression(Dictionary<Node, IEnumerable<Token>> in_tokens,
        //                                                                        Node[] selector,
        //                                                                        Func<Token, Token, bool> filter
        //                                                                      )
        //{
        //    IEnumerable<Tuple> result = null;
        //    IEnumerable<Token> tokens1 = in_tokens[selector[0]];
        //    IEnumerable<Token> tokens2 = in_tokens[selector[1]];
        //    result = from token1 in tokens1 from token2 in tokens2 where filter(token1, token2) select tuple_create(token1, token2);
        //    Tuple[] array_result = result.ToArray();
        //    if (array_result.Length == 0) return null;
        //    else return array_result;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuples">set of tuples we've already built</param>
        /// <param name="in_tokens">set of collections of tokens indexed by their Place of origin</param>
        /// <returns>set of tuple extended with new token according to the rules provided for binary expressions</returns>
        private IEnumerable<Tuple> checkSingleBinaryExpression(IEnumerable<Tuple> tuples,
                                                                                Dictionary<Node, IEnumerable<Token>> in_tokens,
                                                                                Node[] selector,
                                                                                Func<Token, Token, bool> filter
                                                                              )
        {
            if (tuples == null) return null;
            ////first we should check that tuples collection is not empty
            //if (tuples.Count() == 0)
            //{
            //    return null;
            //}

            Tuple first_tuple = tuples.First();
            IEnumerable<Tuple> result = null;
            if (first_tuple.ContainsKey(selector[0]))
            {
                if (first_tuple.ContainsKey(selector[1]))
                {//tuple contains both type of tokens to check. 
                    //actually control flow should never be here. Dictionary of rules contains only one rule for particular selector.
                    throw new Exception("Dictionary of rules should contain only one rule for particular selector. Strange that we are here.");

                    //result = from tuple in tuples where filter(tuple[selector[0]], tuple[selector[1]]) select tuple;
                }
                else
                {//tuple doesn't contain second type (selector[1]) so we will try to expand tuple by token of this type
                    IEnumerable<Token> tokens = in_tokens[selector[1]];
                    result = from tuple in tuples from token in tokens where filter(tuple[selector[0]], token) select tuple_expand(tuple, token);
                }
            }
            else
            {
                if (first_tuple.ContainsKey(selector[1]))
                {//tuple doesn't contain first type (selector[0]) so we will try to expand tuple by token of this type
                    selector = swap_selector(selector);
                    IEnumerable<Token> tokens = in_tokens[selector[1]];
                    result = from tuple in tuples from token in tokens where filter(token, tuple[selector[0]]) select tuple_expand(tuple, token);
                }
                else
                {//tuple does NOT contain any type of tokens to check.
                    //actually control flow should never be here. Tuples are build on incremental basis.
                    //There might be two nonintersecting subsets of filters s.t. we'll get this situation. Now we just require not to do that!!!
                    throw new Exception("Tuples are build on incremental basis. Check");
                }
            }
            Tuple[] array_result = result.ToArray();
            if (array_result.Length == 0) return null;
            else return array_result;
        }

        //private IEnumerable<Tuple> filterOutRepetitiveOccurenceOfTokenInDifferentTuples(IEnumerable<Tuple> tuples)
        //{
        //    Tuple first_tuple = tuples.First();
        //    IEnumerable<Tuple> result = tuples;
        //    foreach (Node place_name in first_tuple.Keys)
        //    {
        //        result = result.Distinct(new ParametrizedTupleComparer(place_name)).ToArray();
        //    }
        //    return result;
        //}


        private void removeTokensFromIncomingArcs(IEnumerable<Tuple> tuples)
        {
            foreach (Tuple tuple in tuples)
            {
                foreach (Node node in tuple.Keys)
                {
                    try {
                        this.input_arcs[node].removeToken(tuple[node]);
                    } catch (KeyNotFoundException ex) {
                        throw ex;
                    }
                }
            }
        }



        /// <summary>
        /// Expands tuple with new token. If token with has the same origin as one of the tokens inside
        /// tuple new token will replace older one and tuple dimension will NOT be increased.
        /// </summary>
        private Func<Tuple, Token, Tuple> tuple_expand = (tuple, token) =>
        {
            Tuple new_tuple = new Tuple(tuple);
            new_tuple.Add(token.place, token);
            return new_tuple;
        };

        /// <summary>
        /// This function swaps selector such that the first selector becomse the second and vice versa
        /// </summary>
        private Func<Node[], Node[]> swap_selector = (node_array) =>
        {
            return new Node[] { node_array[1], node_array[0] };
        };





        /// <summary>
        /// This class implements comparer for selectors: arrays consisting of two strings
        /// </summary>
        private class SelectorComparer : IComparer<Node[]>
        {

            #region IComparer<Node[]> Members

            int IComparer<Node[]>.Compare(Node[] x, Node[] y)
            {
                int hr = x[0].id-y[0].id;
                if (hr == 0)
                {
                    return x[1].id- y[1].id;
                }
                else
                {
                    return hr;
                }

            }

            #endregion
        }



        ///// <summary>
        ///// This class implements comparer for selectors: arrays consisting of two strings
        ///// </summary>
        //private class SelectorComparer : IComparer<string[]>
        //{
        //    StringComparer comparer;

        //    public SelectorComparer(StringComparer comparer)
        //    {
        //        this.comparer = comparer;
        //    }

        //    #region IComparer<string[]> Members

        //    int IComparer<string[]>.Compare(string[] x, string[] y)
        //    {
        //        int hr = comparer.Compare(x[0], y[0]);
        //        if (hr == 0)
        //        {
        //            return comparer.Compare(x[1], y[1]);
        //        }
        //        else
        //        {
        //            return hr;
        //        }

        //    }

        //    #endregion
        //}

        // Custom comparer for the Tuple class.
        private class ParametrizedTupleComparer : IEqualityComparer<Tuple>
        {
            private Node comparison_field = null;

            public ParametrizedTupleComparer(Node comparison_field)
            {
                this.comparison_field = comparison_field;
            }

            // Tuples are equal they contain the same token under comparison field slice
            public bool Equals(Tuple x, Tuple y)
            {
                return x[comparison_field] == y[comparison_field];
            }

            // If Equals() returns true for a pair of objects,
            // GetHashCode must return the same value for these objects.

            public int GetHashCode(Tuple tuple)
            {
                // Calculate the hash code for the tuple.
                return tuple[comparison_field].GetHashCode();
            }

        }
    }
}