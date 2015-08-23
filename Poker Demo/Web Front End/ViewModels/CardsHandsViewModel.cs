using Net.FrozenExports.PlayingCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Net.FrozenExports.PokerDemo.WebFrontEnd.ViewModels
{
    /// <summary>
    ///     Simple wrapper for card hands being returned to a user.
    /// </summary>
    public class CardHandsViewModel
    {
        /// <summary>
        ///     The hands being returned to the user.
        /// </summary>
        public List<List<Card>> RequestedHands { get; set; }
    }
}