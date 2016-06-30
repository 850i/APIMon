using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using APIMonLib;
using System.IO;

namespace CPN
{
    public class Place : Node
    {

		public enum PrintLevel : byte {
			Low = 0,
			Medium = 1,
			High = 2
		}

        public static List<Place> places_created = new List<Place>();

        private static Func<int, string, string> expand = (num, what) => num <= 0 ? what : expand(num - 1, what) + what;

        public static void printStatistics(PrintLevel print_level) {
            Console.WriteLine(expand(10, "========="));
            foreach (Place p in places_created) {
				if (p.print_level >= print_level) {
					Console.WriteLine(expand(30, "---"));
					Console.Write("(");
					printNoZeroDifferentColor(p.id, ConsoleColor.DarkYellow);
					Console.WriteLine(")" + p.name_);
					Console.Write("\tCurrent: ");
					printNoZeroDifferentColor(p.Count, ConsoleColor.Red);
					Console.Write("\tPut: ");
					printNoZeroDifferentColor(p.counters.token_put, ConsoleColor.Green);
					Console.Write("\t Taken: ");
					printNoZeroDifferentColor(p.counters.token_removed, ConsoleColor.Blue);
					Console.WriteLine();
				}
            }
            Console.WriteLine(expand(10, "=>=-^-=<="));
        }

        public static IEnumerable<Place> getDetectionPlaces() {
            IEnumerable<Place> result = (from place in places_created where place is DetectionPlace select place).ToArray();
            return result;
        }

        private const string STAT_DELIM = ",";

		public static void writeStatistics(PrintLevel print_level, StreamWriter file) {
            file.WriteLine("ID" + STAT_DELIM + "Name" + STAT_DELIM + "Current #" + STAT_DELIM + "Put #" + STAT_DELIM + "Taken #" + STAT_DELIM);
			foreach (Place p in places_created) {
				if (p.print_level >= print_level) {
					file.WriteLine(
								"" + 
                                p.id + STAT_DELIM +
								p.name_ + STAT_DELIM+
								p.Count+STAT_DELIM+
								p.counters.token_put + STAT_DELIM +
								p.counters.token_removed + STAT_DELIM
								);
				}
			}
		}

		private static void printNoZeroDifferentColor(int num, ConsoleColor color) {
			if (num != 0) {
				ConsoleColor old = Console.ForegroundColor;
				Console.ForegroundColor = color;
				Console.Write(num);
				Console.ForegroundColor = old;
			} else {
				Console.Write(num);
			}
		}

		//TODO Freeze/cancel all operations for cleaning or reset not only Places but also Transitions and Arcs which might be working at that moment
		public static void clearAllPlaces() {
			foreach (Place place in places_created) {
				place.clear();
			}
		}

		private PrintLevel place_print_level = PrintLevel.Medium;

		public PrintLevel print_level {
			get { return place_print_level; }
			set { place_print_level = value; }
		}

		public Place setPrintLevel(PrintLevel print_level) {
			place_print_level = print_level;
			return this;
		}


        //TODO implement concept of a color s.t. it will allow sorting over some fields.
        //So each token has link onto its color and color actually impose some restriction onto large collections of tokens.
        //It looks like token is a row in table and color is a columnt in table
        List<Token> tokens = new List<Token>();



        //TODO It might be useful to introduce color checking in here. Especially when CPN is under development. We can check for existence of fields and probably it's values. Use speciall class color for it.


        /// <summary>
        /// Creates place. Registers it in the registry of places.
        /// </summary>
        /// <param name="name">Specify unique name for this place in the whole CPN</param>
        public Place(String name)
            : base(name)
        {
            places_created.Add(this);
        }

        private class Counters {
            public int token_put = 0;
            public int token_put_silently = 0;
            public int token_requested_to_remove = 0;
            public int token_removed = 0;

			public void reset() {
				token_put = 0;
                token_put_silently = 0;
				token_requested_to_remove = 0;
				token_removed = 0;
			}
        };

        private Counters counters=new Counters();

        /// <summary>
        /// returns false when place never had tokens in it
        /// </summary>
        /// <returns></returns>
        public bool isVirgin() {
            return (counters.token_put == 0) && (counters.token_put_silently == 0);
        }

        /// <summary>
        /// This method adds token to this place
        /// </summary>
        /// <param name="t"></param>
        public void putToken(Token t)
        {
            counters.token_put++;
            tokens.Add(t);
            t.setCurrentPlace(this);
            if (put_reaction != null) put_reaction(this, t);
            base.notifyOutputArcs();
        }

		/// <summary>
		/// Resets place completely. All counters a set to 0. All stored tokens disposed
		/// </summary>
		protected void clear() {
			counters.reset();
			tokens.Clear();
		}

        /// <summary>
        /// This method adds token to this place w/o
        /// notification any interested parties. Usually used for debug and testing purposes
        /// </summary>
        /// <param name="t"></param>
        public void putTokenSilently(Token t)
        {
            counters.token_put_silently++;
            tokens.Add(t);
            t.setCurrentPlace(this);
            if (put_reaction != null) put_reaction(this, t);
        }

        public string toString() {
            string result = string.Empty;
            result += "Place (" + this.name_ + ") " + this.name_+"\n";
            foreach(Token token in tokens){
                result+=expand(10, "---------")+"\n";
                result += token;
            }
            return result;
        }

        public override string ToString()
        {
            return "Place " + name_ + " ";
        }

        public delegate void Reaction(Place place, Token t);

        private Reaction put_reaction = null;

        public Place addPutReaction(Reaction r)
        {
            put_reaction += r;
			return this;
        }

        private Reaction remove_reaction = null;

        public void addRemoveReaction(Reaction r) {
            remove_reaction += r;
        }

        public override void arcHasChangedEvent(Arc arc)
        {
            IEnumerable<Token> tokens = arc.getTokens();
            foreach (Token token in tokens)
            {
                putToken(token);
            }
        }

        public override void removeToken(Token token)
        {
            counters.token_requested_to_remove++;
            if (tokens.Remove(token)) {                
                counters.token_removed++;
                if (remove_reaction != null) remove_reaction(this, token);
            }
        }

        public void flushAllTokens()
        {
            //tokens.RemoveAll(item => true);
			tokens.Clear();
        }

        public override IEnumerable<Tuple> getTuples()
        {
            return tokens.Select(token => new Tuple(token));
        }

        public IEnumerable<Token> getTokens(){
            return tokens;
        }

        public int Count { get{return tokens.Count;}  }


        public static Place[] operator +(Place a, Place b) {
            return new Place[] { a, b };
        }
    }
}
