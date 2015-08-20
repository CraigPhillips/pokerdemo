using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.FrozenExports.PlayingCards.Poker
{
    /// <summary>
    ///     The result of comparing multiple poker hands.
    /// </summary>
    public class PokerHandVerdict
    {
        /// <summary>
        ///     If the decision was made based on individual cards, the winner and runner up
        ///     cards.
        /// </summary>
        public KeyValuePair<Card, Card> DecidingCardPair { get; set; }

        /// <summary>
        ///     If the decision was made based on types of hands, the winner and runner
        ///     up hand types.
        /// </summary>
        public KeyValuePair<PokerHandType, PokerHandType> DecidingHandTypePair { get; set; }

        /// <summary>
        ///     The type of verdict rendered.
        /// </summary>
        public PokerHandVerdictType? VerdictType { get; set; }

        /// <summary>
        ///     The best hands considered for this verdict.
        /// </summary>
        public PokerHand WinningHand { get; set; }
    }
}
