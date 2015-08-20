using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.FrozenExports.PlayingCards.Poker
{
    /// <summary>
    ///     The types of hands that a player can achieve in a standard game of poker.
    /// </summary>
    public enum PokerHandType
    {
        None,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush
    }
}
