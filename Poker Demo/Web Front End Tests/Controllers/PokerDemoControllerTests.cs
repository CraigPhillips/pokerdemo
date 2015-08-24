using Net.FrozenExports.PlayingCards;
using Net.FrozenExports.PlayingCards.Poker;
using Net.FrozenExports.PokerDemo.WebFrontEnd.Tests.ControllerEvaluation;
using Net.FrozenExports.PokerDemo.WebFrontEnd.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;
using System.Web.Mvc;

namespace Net.FrozenExports.PokerDemo.WebFrontEnd.Controllers.Tests
{
    /// <summary>
    ///     Verifies that the behavior the PokerDemoController is correct.
    /// </summary>
    [TestClass]
    public class PokerDemoControllerTests
    {
        private ActionResultShucker shucker;
        private ArgumentNullExceptionChecker argumentNullChecker;
        private FormCollection testForm;
        private PokerDemoController testController;

        /// <summary>
        ///     Sets up testing resources.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            testController = new PokerDemoController(
                () => { return new Card(); },
                () => { return new PokerHand(); },
                new PokerHandJudge(() => { return new PokerHandVerdict(); }),
                () => { return new PokerEvaluationViewModel(); });

            NameValueCollection testFormData = new NameValueCollection();
            for(int playerBeingAdded = 1; playerBeingAdded <= 2; playerBeingAdded++)
            {
                testFormData.Add(
                    String.Format("player-{0}-name", playerBeingAdded.ToString()),
                    String.Format("Player {0}", playerBeingAdded.ToString()));

                for(int cardBeingAdded = 1; cardBeingAdded <= 5; cardBeingAdded++)
                {
                    testFormData.Add(
                        String.Format(
                            "rank-player-{0}-card-{1}",
                            playerBeingAdded.ToString(),
                            cardBeingAdded.ToString()),
                        Enum.GetValues(typeof(CardRank)).GetValue(cardBeingAdded).ToString());
                    testFormData.Add(
                        String.Format(
                            "suit-player-{0}-card-{1}",
                            playerBeingAdded.ToString(),
                            cardBeingAdded.ToString()),
                            cardBeingAdded == 1 ? CardSuit.Clubs.ToString() : CardSuit.Diamonds.ToString());
                }
            }
            testForm = new FormCollection(testFormData);

            argumentNullChecker = new ArgumentNullExceptionChecker();
            shucker = new ActionResultShucker();
        }

        [TestMethod]
        public void BasicView()
        {
            ViewResult result = testController.PokerInput() as ViewResult;
            Assert.IsNotNull(
                result,
                "Result was null.");
        }

        /// <summary>
        ///     Validates that a basic form processing will return the expected results.
        /// </summary>
        [TestMethod]
        public void BasicEval()
        {
            ViewResult result = testController.PokerEvaluation(testForm) as ViewResult;
            Assert.IsNotNull(
                result,
                "Result was either null or of the incorrect type.");

            PokerEvaluationViewModel viewModel = shucker.Shuck<PokerEvaluationViewModel>(result);
            Assert.IsTrue(
                viewModel != null && viewModel.Verdict != null &&
                    viewModel.TiedHands.Count == 2,
                "Contained view model was null, of the wrong type or had the incorrect data.");
        }

        /// <summary>
        ///     Validates that a form entry with a winner is processed correctly.
        /// </summary>
        [TestMethod]
        public void BasicEvalWithWin()
        {
            testForm["rank-player-1-card-1"] = "eight";
            ViewResult result = testController.PokerEvaluation(testForm) as ViewResult;
            PokerEvaluationViewModel viewModel = shucker.Shuck<PokerEvaluationViewModel>(result);

            Assert.IsTrue(
                viewModel != null && viewModel.Verdict != null &&
                    viewModel.Verdict.VerdictType == PokerHandVerdictType.WinByInTypeOrdering,
                "Result was null, empty or had the incorrect data.");
        }

        /// <summary>
        ///     Validates that an exception is thrown when a player name is missing.
        /// </summary>
        [TestMethod]
        public void MissingPlayerName()
        {
            testForm.Remove("player-1-name");
            ViewResult result = testController.PokerEvaluation(testForm) as ViewResult;
            PokerEvaluationViewModel viewModel = shucker.Shuck<PokerEvaluationViewModel>(result);

            Assert.IsTrue(
                viewModel != null && viewModel.ThrownException != null,
                "View model was generated incorrectly or exception was not recorded.");
        }

        /// <summary>
        ///     Validates that an exception is thrown when a card is missing its suit.
        /// </summary>
        [TestMethod]
        public void MissingSuit()
        {
            testForm.Remove("suit-player-1-card-1");
            ViewResult result = testController.PokerEvaluation(testForm) as ViewResult;
            PokerEvaluationViewModel viewModel = shucker.Shuck<PokerEvaluationViewModel>(result);

            Assert.IsTrue(
                viewModel != null && viewModel.ThrownException != null,
                "View model was generated incorrectly or exception was not recorded.");
        }

        /// <summary>
        ///     Validates that an exception is thrown when a card is missing its rank.
        /// </summary>
        [TestMethod]
        public void MissingRank()
        {
            testForm.Remove("rank-player-1-card-1");
            ViewResult result = testController.PokerEvaluation(testForm) as ViewResult;
            PokerEvaluationViewModel viewModel = shucker.Shuck<PokerEvaluationViewModel>(result);

            Assert.IsTrue(
                viewModel != null && viewModel.ThrownException != null,
                "View model was generated incorrectly or exception was not recorded.");
        }

        /// <summary>
        ///     Validates that an exception is thrown when an invalid suit is provided.
        /// </summary>
        [TestMethod]
        public void InvalidSuit()
        {
            testForm["suit-player-1-card-1"] = "invalid";
            ViewResult result = testController.PokerEvaluation(testForm) as ViewResult;
            PokerEvaluationViewModel viewModel = shucker.Shuck<PokerEvaluationViewModel>(result);

            Assert.IsTrue(
                viewModel != null && viewModel.ThrownException != null,
                "View model was generated incorrectly or exception was not recorded.");
        }

        /// <summary>
        ///     Validates that an exception is thrown when no form data is provided.
        /// </summary>
        [TestMethod]
        public void MissingForm()
        {
            ViewResult result = testController.PokerEvaluation(null) as ViewResult;
            PokerEvaluationViewModel viewModel = shucker.Shuck<PokerEvaluationViewModel>(result);

            Assert.IsTrue(
                viewModel != null && viewModel.ThrownException != null,
                "View model was generated incorrectly or exception was not recorded.");
        }

        /// <summary>
        ///     Validates that an exception is thrown if an invalid suit is provided.
        /// </summary>
        [TestMethod]
        public void InvalidRank()
        {
            testForm["rank-player-1-card-1"] = "invalid";
            ViewResult result = testController.PokerEvaluation(testForm) as ViewResult;
            PokerEvaluationViewModel viewModel = shucker.Shuck<PokerEvaluationViewModel>(result);

            Assert.IsTrue(
                viewModel != null && viewModel.ThrownException != null,
                "View model was generated incorrectly or exception was not recorded.");
        }

        /// <summary>
        ///     Validates that exceptions are thrown for null arguments.
        /// </summary>
        [TestMethod]
        public void NullArguments()
        {
            argumentNullChecker.CheckForArgumentNullException(() =>
            {
                new PokerDemoController(null, null, null, null);
            });
            argumentNullChecker.CheckForArgumentNullException(() =>
            {
                new PokerDemoController(
                    () => { return new Card(); }, 
                    null, null, null);
            }); 
            argumentNullChecker.CheckForArgumentNullException(() =>
            {
                new PokerDemoController(
                    () => { return new Card(); },
                    () => { return new PokerHand(); },
                    null, null);
            }); 
            argumentNullChecker.CheckForArgumentNullException(() =>
            {
                new PokerDemoController(
                    () => { return new Card(); },
                    () => { return new PokerHand(); },
                    new PokerHandJudge(() => { return new PokerHandVerdict(); }),
                    null);
            });
        }
    }
}
