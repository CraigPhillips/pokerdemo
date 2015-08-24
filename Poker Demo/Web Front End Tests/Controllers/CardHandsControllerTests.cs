using Net.FrozenExports.PlayingCards;
using Net.FrozenExports.PokerDemo.WebFrontEnd.Tests.ControllerEvaluation;
using Net.FrozenExports.PokerDemo.WebFrontEnd.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Net.FrozenExports.PokerDemo.WebFrontEnd.Controllers.Tests
{
    /// <summary>
    ///     Tests for verifying that the CardsHandsController is behaving correctly.
    /// </summary>
    [TestClass]
    public class CardHandsControllerTests
    {
        private ActionResultShucker shucker;
        private ArgumentNullExceptionChecker nullExceptionChecker;
        private CardHandsController testController;

        /// <summary>
        ///     Sets up fresh test resources for each test.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            testController = new CardHandsController(
                () => { return new CardDeck(() => { return new Card(); }); },
                () => { return new CardHandsViewModel(); });

            nullExceptionChecker = new ArgumentNullExceptionChecker();
            shucker = new ActionResultShucker();
        }

        /// <summary>
        ///     Verifies that null arguments passed to constructor are handled correctly.
        /// </summary>
        [TestMethod]
        public void NullArguments()
        {
            nullExceptionChecker.CheckForArgumentNullException(() =>
            {
                new CardHandsController(null, null);
            });

            nullExceptionChecker.CheckForArgumentNullException(() =>
            {
                new CardHandsController(
                    () => { return new CardDeck(() => { return new Card(); }); }, null);
            });
        }

        /// <summary>
        ///     Verifies that the controller is able to generate hands as requsted.
        /// </summary>
        [TestMethod]
        public void BasicHandsRetrieval()
        {
            JsonResult result = testController.GeneratedCardHands(4, 10) as JsonResult;
            CardHandsViewModel viewModel = shucker.Shuck<CardHandsViewModel>(result);
            Assert.IsNotNull(viewModel, "Generated view model was null.");

            List<List<Card>> dealtHands = viewModel.RequestedHands;

            Assert.IsNotNull(dealtHands, "Hands not dealt correctly.");
            Assert.AreEqual(4, dealtHands.Count, "Incorrect number of hands generated.");
            Assert.AreEqual(10, dealtHands[2].Count, "Incorrect number of cards in dealth hand.");
        }

        /// <summary>
        ///     Verifies that the controller behaves correctly when too many cards are requested.
        /// </summary>
        [TestMethod]
        public void TooManyCards()
        {
            JsonResult result = testController.GeneratedCardHands(6, 10) as JsonResult;
            CardHandsViewModel viewModel = shucker.Shuck<CardHandsViewModel>(result);
            Assert.IsNotNull(viewModel, "Generated view model was null.");

            Assert.IsNull(viewModel.RequestedHands, "Hands generated when they shouldn't have been.");
        }
    }
}
