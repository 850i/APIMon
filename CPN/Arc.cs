using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using APIMonLib;
namespace CPN
{
	//TODO Split Arc class into two classes P2T arc class and T2P arc class.

	//DONE
	////TODO Make fast Arc for P2T and leave this implementation for T2P connections
	////Tune Arc in such a way that it delegates to FastArc unless Generating or filtering expression was used.
	////If so it instantiates heavy arc and delegates to it.


    public class Arc : IComparable<Arc>
    {

        public const int MIN_WEIGHT = 0;
        public const int MAX_WEIGHT = DEFAULT_WEIGHT*2;
        public const int DEFAULT_WEIGHT = 1000;

        private int _weight = DEFAULT_WEIGHT;

        public int weight
        {
            get { return _weight; }
        }

        private Node _from;
        public Node from { get { return _from; } }


        private Node _to;
        public Node to { get { return _to; } }

        public string name
        {
            get
            {
                return "arc from " + _from.name_ + " to " + _to.name_;
            }
        }

        /// <summary>
        /// When set to true, remove method actually removes tokens from the input node
        /// </summary>
        private bool remove_enabled = false;

        /// <summary>
        /// Enables removal of tokens 
        /// </summary>
        /// <returns></returns>
        public Arc enableTokenRemovalFromInput() {
            remove_enabled = true;
            return this;
        }

        public Arc setTokenRemoval(bool value) {
            remove_enabled = value;
            return this;
        }

        #region Filtering expressions management

        /// <summary>
        /// When set to true, generating or filtering expression can not be changed
        /// </summary>
        private bool is_fast_arc = false;

        private Func<Tuple, bool> DEFAULT_FILTERING_EXPRESSION = tuple => true;

        private Func<Tuple, Token> DEFAULT_GENERATING_EXPRESSION = tuple => {
            throw new Exception("Generating expression was not specified. Reference tuple: " + tuple.toString("   "));
        //    return new Token(tuple.getSingleToken()); 
        };

        private Func<Tuple, bool> _filtering_expression = null;

        public Func<Tuple, bool> filtering_expression
        {
            //get { return _filtering_expression; }
            set
            {
                enforceFastArcImmutability();
                _filtering_expression = value;
                enableFullProcessingMode();
            }
        }

        private Func<Tuple, Token> _generating_expression = null;

        public Func<Tuple, Token> generating_expression
        {
            //get { return _generating_expression; }
            set
            {
                enforceFastArcImmutability();
                _generating_expression = value;
                enableFullProcessingMode();
            }
        }

        /// <summary>
        /// Checks if expression is null and sets default values for it
        /// </summary>
        private void checkAndSetDefaultValuesForExpressions(){
            if (_generating_expression == null) _generating_expression = DEFAULT_GENERATING_EXPRESSION;
            if (_filtering_expression == null) _filtering_expression = DEFAULT_FILTERING_EXPRESSION;
        }

        /// <summary>
        /// If someone tries to modify fast arc it will raise the exception
        /// </summary>
        private void enforceFastArcImmutability(){
            if (is_fast_arc)
            {
                throw new WrongConfigurationException("Generating expression can not be changed for the fast Place to Transition arc");
            };
        }


        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for the Arc from Place to Transition
        /// This kind of arc can not have generating expression
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Arc(Place from, Transition to)
            : this(from, to, DEFAULT_WEIGHT)
        {
        }


        /// <summary>
        /// Constructor for the Arc from Transition to Place
        /// This kind of Arc usually have generating expression specified
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Arc(Transition from, Place to)
            : this(from, to, DEFAULT_WEIGHT)
        {
        }

        public Arc(Place from, Transition to, int weight)
        {
            _from = from;
            _to = to;
			_weight = weight;
            _from.addOutputArc(this);
            _to.addInputArc(this);

            enableNoProcessingMode();//gives double speed up
            //enableFullProcessingMode();
        }

        public Arc(Transition from, Place to, int weight)
        {
            _from = from;
            _to = to;
			_weight = weight;
            _from.addOutputArc(this);
            _to.addInputArc(this);

            enableFullProcessingMode();
        }
        #endregion

        public delegate IEnumerable<Token> GetTokens();

        public GetTokens getTokens = null;

        private void enableNoProcessingMode(){
            is_fast_arc = true;
            getTokens = new GetTokens(getTokensNoProcessing);
        }

        private void enableFullProcessingMode()
        {
            is_fast_arc = false;
            checkAndSetDefaultValuesForExpressions();
            getTokens = new GetTokens(getTokensFullProcessing);
        }

        /// <summary>
        /// Computes output of the Arc. It applies filtering expression and then generating expression.
        /// Computation is deferred
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Token> getTokensFullProcessing()
        {
            return _from.getTuples().Where(_filtering_expression).Select(_generating_expression);
        }

        /// <summary>
        /// Use this method when no processing is required. Just go through is needed
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Token> getTokensNoProcessing()
        {
            return ((Place)_from).getTokens();
        }

        public void removeToken(Token token)
        {
            if (remove_enabled) from.removeToken(token);
        }

        internal void nodeAtInputHasChangedEvent()
        {
            //TODO Probably we should check filtering expression here and only if it works fire the event. Downside is that in getTokens we compute expression again.
            //if we implement caching here everything should work fine w/o useless firings.
            _to.arcHasChangedEvent(this);
        }

        public int CompareTo(Arc other)
        {
			//if (this._weight.Equals(other._weight))
			//{
			//    return this.GetHashCode().Equals(other.GetHashCode()) ? 0 : this.GetHashCode() < other.GetHashCode() ? -1 : 1;
			//} else return this._weight < other._weight ? -1 : 1;
			int result = this._weight.CompareTo(other._weight);
			if (result != 0) return result;
			else return this.GetHashCode().CompareTo(other.GetHashCode());
        }
    }
}