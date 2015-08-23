using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.FrozenExports.PlayingCards.Poker
{
    /// <summary>
    ///     The types of verdicts that can be created when poker hands are compared.
    /// </summary>
    public enum PokerHandVerdictType
    {
        WinByHandType,
        WinByInTypeOrdering,
        WinByExtraTypeOrdering,
        Tie
    }
}
