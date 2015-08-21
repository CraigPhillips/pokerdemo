using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.FrozenExports.PlayingCards
{
    /// <summary>
    ///     Represents a single playing card.
    /// </summary>
    public class Card
    {
        /// <summary>
        ///     The suit of this card.
        /// </summary>
        public CardSuit? Suit { get; set; }

        /// <summary>
        ///     The rank of this card.
        /// </summary>
        public CardRank? Rank { get; set; }
    }
}
