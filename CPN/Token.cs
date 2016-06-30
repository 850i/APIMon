using System;
using System.Collections.Generic;
using System.Text;
using APIMonLib;

namespace CPN {
    [Serializable]
    public class Token : TransferUnit, PrefixPrintable {
        private Place _place = null;

        public Place place {
            get {
                return _place;
            }
        }

        public Token() {
        }

        public Token(TransferUnit tu)
            : base(tu) {
        }

        public Token(Token t, String[] keys_to_copy)
            : this() {
            this.loadFields(keys_to_copy, "old", t);
        }

        /// <summary>
        /// This method installs current place for this token
        /// </summary>
        /// <param name="place"></param>
        public void setCurrentPlace(Place place) {
            this._place = place;
        }

        public string toString(string prefix) {
            String result = "Token:";
            String expander = "    ";
            prefix += expander + "|";
            foreach (String key in this.getFields()) {
                if (this[key] is PrefixPrintable) {
                    result += "\n" + prefix + "---" + key + " =\n" + prefix + expander + ((PrefixPrintable)this[key]).toString(prefix);
                } else {
                    result += "\n" + prefix + "---" + key + " = " + this[key];
                }
            }
            return result;
        }

        public const string PREV = "prev";
        public Tuple prevTuple {

            get {
               return (Tuple)this[PREV];
            }
            set {
                this[PREV]=value;
            }
        }


        public override string ToString() {
            return toString("%");
        }
    }
}