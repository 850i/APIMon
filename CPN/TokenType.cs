using System;
using System.Collections.Generic;
using System.Text;

namespace CPN
{
    /// <summary>
    /// This class is used for reinforcing token types at certain
    /// places
    /// </summary>
    public class TokenType
    {
        private static bool REINFORCE_TYPES=true;


        public HashSet<String> fields = new HashSet<string>();

        private Place place = null;

        public TokenType(Place location){
            place = location;
        }

        public bool isTypeCorrect(Token token)
        {
            return fields.IsSubsetOf(token.getFields());
        }

        public void reinforceType(Token token)
        {
            if (REINFORCE_TYPES)
            {
                if (!isTypeCorrect(token))
                {
                    throw new TypeCheckException("Type is not correct at "+place.name_);
                }
            }
        }
    }
}
