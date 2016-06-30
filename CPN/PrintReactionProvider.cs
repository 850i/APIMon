using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPN {
    public class PrintReactionProvider {

        /// <summary>
        /// Returns printing reaction provider. All the reactions are designed to print something.
        /// </summary>
        /// <returns>Printing provider</returns>
        public static PrintReactionProvider getProvider() {
            return new PrintReactionProvider();
        }

        /// <summary>
        /// Returns printing reaction provider. All the reactions are designed to print something.
        /// </summary>
        /// <param name="color">choose the color for printing</param>
        /// <returns>Printing provider</returns>
        public static PrintReactionProvider getProvider(ConsoleColor color) {
            return new PrintReactionProvider(color);
        }

        private ConsoleColor _color = Console.ForegroundColor;
        private bool color_was_set = false;

        private PrintReactionProvider() {
            Console.ResetColor();
        }

        private PrintReactionProvider(ConsoleColor color) {
            Console.ResetColor();
            _color = color;
            color_was_set = true;
        }

        public void simplePrintToken(Place place, Token t) {
            if (color_was_set) {
                Console.ForegroundColor = _color;
                Console.WriteLine("\n Place: " + place.name_ + "\t received token");
                Console.ResetColor();
            } else {
                Console.WriteLine("\n Place: " + place.name_ + "\t received token");
            }
        }

        public void advancedPrintToken(Place place, Token token) {
            if (color_was_set) {
                Console.ForegroundColor = _color;
                _printCauseToken(place, token);
                Console.ResetColor();
            } else {
                _printCauseToken(place, token);
            }
        }

        private void _printCauseToken(Place place, Token token) {
            Func<int, string> spaces = null;
            spaces = (len) => len > 1 ? spaces(len - 1) + " " : " ";
            Console.WriteLine(place.ToString() + "(" + place.Count + ")\n   " + token.toString(
                                                                spaces(3)
                                                              )
                              );
        }
    }
}
