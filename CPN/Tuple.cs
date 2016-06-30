using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPN
{
    /// <summary>
    /// This class defines abstraction of the tuple of tokens indexed by token placename
    /// </summary>
    public class Tuple : Dictionary<Node, Token>, PrefixPrintable
    {

        public Tuple()
        {
        }

        public Tuple(Token token)
        {
            this.Add(token.place, token);
        }

        public Tuple(Tuple tuple):base(tuple)
        {
            
        }

        public new Token this[Node index]   // Indexer declaration
        {
            get {
                try {
                    return base[index];
                } catch (KeyNotFoundException) {
                    //Node[] fields = this.Keys;
                    string fields_str = string.Empty;
                    foreach(Node node in this.Keys){
                        fields_str += "  ("+node.name_ + ") ";
                    }
                    throw new KeyNotFoundException("Can not find key \"" + index.name_ + "\" among (" + fields_str + ")");
                }
            }
            set {
                if (index != null) {
                    if (value != null) {
                        base[index] = value;
                    } else {
                        this.Remove(index);
                    }
                }
            }
        }

        /// <summary>
        /// This method ensures that tuple contains only one token and returns it
        /// </summary>
        /// <returns></returns>
        public Token getSingleToken()
        {
            if (this.Values.Count != 1)
            {
                throw new InvalidOperationException("Tuple contains more than one token (" + this.Values.Count + " tokens). Something is wrong with a current program state. Probably you forgot replace default generating expression.");
            }
            return this.Values.First();
        }

        public string toString(string prefix)
        {
            String result = "Tuple:";
            String expander = "    ";
            prefix += expander + "|"; 
            foreach (Node key in this.Keys)
            {
                if (this[key] is PrefixPrintable)
                {
                    result += "\n" + prefix + "---" + key + " = \n"+prefix+expander + ((PrefixPrintable)this[key]).toString(prefix);
                }
                else
                {
                    result += "\n" + prefix + "---" + key + " = " + this[key];
                }
            }
            return result;
        }
    }
}
