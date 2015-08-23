using Net.FrozenExports.PlayingCards.Poker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Net.FrozenExports.PokerDemo.WebFrontEnd.ViewModels
{
    /// <summary>
    ///     Container class for data needed to render the evaluation of poker hands.
    /// </summary>
    public class PokerEvaluationViewModel
    {
        /// <summary>
        ///     The verdict rendered based on the data provided.
        /// </summary>
        public PokerHandVerdict Verdict;

        /// <summary>
        ///     The best hand processed that didn't win.
        /// </summary>
        public PokerHand RunnerUpHand;

        /// <summary>
        ///     The error that occurred during hand evaluation (if any).
        /// </summary>
        public Exception ThrownException;
    }
}