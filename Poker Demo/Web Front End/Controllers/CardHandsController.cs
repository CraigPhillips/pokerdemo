using Net.FrozenExports.PlayingCards;
using Net.FrozenExports.PokerDemo.WebFrontEnd.Serialization.Json;
using Net.FrozenExports.PokerDemo.WebFrontEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Net.FrozenExports.PokerDemo.WebFrontEnd.Controllers
{
    /// <summary>
    ///     Responsible for generating card hands.
    /// </summary>
    public class CardHandsController : Controller
    {
        private Func<CardDeck> cardDeckFactory;
        private Func<CardHandsViewModel> viewModelFactory;

        /// <summary>
        ///     Simple constructor.
        /// </summary>
        /// <param name="CardDeckFactory">Creates new card decks.</param>
        /// <param name="ViewModelFactory">Creates new view models for containing user data.</param>
        public CardHandsController(Func<CardDeck> CardDeckFactory, Func<CardHandsViewModel> ViewModelFactory)
        {
            if (CardDeckFactory == null) throw new ArgumentNullException("CardDeckFactory");
            if (ViewModelFactory == null) throw new ArgumentNullException("ViewModelFactory");

            cardDeckFactory = CardDeckFactory;
            viewModelFactory = ViewModelFactory;
        }

        /// <summary>
        ///     Used to retrieve randomly dealt hands of cards.
        /// </summary>
        /// <param name="HandCount">The number of hands to deal.</param>
        /// <param name="CardsPerHand">The number of cards per hand.</param>
        /// <returns>
        ///     The set of requested card hands. If either the number of hands
        ///     requested or the number of cards per hand is not greater than 0,
        ///     this list will be empty.
        ///     
        ///     If the total number of requested cards is more than is in a started deck,
        ///     the list will be empty.
        ///     
        ///     The list is wrapped in an object which will cause the MVC framework to
        ///     generate the JSON representation of the list of hands.
        ///     
        ///     GET: /CardHands?HandCount={HandCount}&CardsPerHand={CardsPerHand}
        /// </returns>
        public ActionResult GeneratedCardHands(int HandCount, int CardsPerHand)
        {
            CardHandsViewModel viewModel = viewModelFactory();
            viewModel.RequestedHands = new List<List<Card>>();

            if(HandCount > 0 && CardsPerHand > 0)
            {
                CardDeck deck = cardDeckFactory();

                try { viewModel.RequestedHands = deck.Deal(HandCount, CardsPerHand); }
                /// Simply swallows exceptions so that an empty list will be returned.
                catch (Exception) { }
            }

            // This must be manually allowed since retrieving JSON via GET requests can lead
            // to cross-site scripting arrays. This is only a danger if raw arraws are being
            // returned at the root level of the request which is not the case here.
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Overridden JSON-generating method to hook into the JSON.Net serialization instead
        ///     of the default ASP.Net MVC JSON serialization.
        ///     
        ///     This code largely taken from here: 
        ///     
        ///     http://stackoverflow.com/questions/17244774/proper-json-serialization-in-mvc-4
        /// </summary>
        /// <param name="Data">Data to be serialized</param>
        /// <param name="ContentType">The content type to be sent with the response.</param>
        /// <param name="ContentEncoding"><The content encoding to use when preparing the response./param>
        /// <param name="Behavior">JSON-specific behavior to use during serialization.</param>
        /// <returns></returns>
        protected override JsonResult Json(
            object Data, 
            string ContentType, 
            Encoding ContentEncoding, 
            JsonRequestBehavior Behavior)
        {
            return new JsonDotNetResult
            {
                Data = Data,
                ContentType = ContentType,
                ContentEncoding = ContentEncoding,
                JsonRequestBehavior = Behavior
            };
        }
    }
}
