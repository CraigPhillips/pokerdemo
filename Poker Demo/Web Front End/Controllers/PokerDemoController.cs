using Net.FrozenExports.PlayingCards;
using Net.FrozenExports.PlayingCards.Poker;
using Net.FrozenExports.PokerDemo.WebFrontEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Net.FrozenExports.PokerDemo.WebFrontEnd.Controllers
{
    /// <summary>
    ///     Simple controller used to gather information needed for the poker hand
    ///     input and evaluation pages and then trigger the rendering of those views
    ///     with the relevant data.
    /// </summary>
    public class PokerDemoController : Controller
    {
        private Func<Card> cardFactory;
        private Func<PokerHand> pokerHandFactory;
        private PokerHandJudge pokerHandJudge;
        private Func<PokerEvaluationViewModel> viewModelFactory;

        /// <summary>
        ///     Simple constructor. No arguments may be null.
        /// </summary>
        /// <param name="CardFactory">Responsible for creating new Card objects.</param>
        /// <param name="PokerHandFactory">Responsible for creating new PokerHand objects.</param>
        /// <param name="PokerHandJudge">
        ///     Used by the controller to evaluate poker hands provided to it.
        /// </param>
        /// <param name="ViewModelFactory">Creates new view models for containing evaluation data.</param>
        public PokerDemoController(
            Func<Card> CardFactory,
            Func<PokerHand> PokerHandFactory,
            PokerHandJudge PokerHandJudge,
            Func<PokerEvaluationViewModel> ViewModelFactory)
        {
            if (CardFactory == null) throw new ArgumentNullException("CardFactory");
            if (PokerHandFactory == null) throw new ArgumentNullException("PokerHandFactory");
            if (PokerHandJudge == null) throw new ArgumentNullException("PokerHandJudge");
            if (ViewModelFactory == null) throw new ArgumentNullException("ViewModelFactory");

            cardFactory = CardFactory;
            pokerHandFactory = PokerHandFactory;
            pokerHandJudge = PokerHandJudge;
            viewModelFactory = ViewModelFactory;
        }

        /// <summary>
        ///     Retrieves information needed to display the poker hand input page and
        ///     then triggers the rendering of that page.
        /// </summary>
        /// <returns>
        ///     The view, coupled with data needed to render that view, that the MVC framework should 
        ///     use to display the poker hand input page.
        /// </returns>
        public ActionResult PokerInput()
        {
            return View();
        }

        /// <summary>
        ///     Processes a form that contains data about two poker hands that need to
        ///     be evaluated and the data and view needed by the MVC framework to render
        ///     this evaluation for the user.
        /// </summary>
        /// <param name="PokerHandsForm">
        ///     The form data that contains information about the hands being submitted
        ///     for evaluation. Must not be null.
        /// </param>
        /// <returns>
        ///     The view, coupled with the evaluation data needed to render that view that
        ///     the MVC framework should use to display the results of evaluating the poker hands.
        /// </returns>
        public ActionResult PokerEvaluation(FormCollection PokerHandsForm)
        {
            PokerEvaluationViewModel viewModel = viewModelFactory();
            PokerHand player1Hand = pokerHandFactory();
            PokerHand player2Hand = pokerHandFactory();

            try
            {
                if (PokerHandsForm == null) throw new ArgumentNullException("PokerHandsForm");

                for (int handBeingProcessed = 1; handBeingProcessed <= 2; handBeingProcessed++)
                {
                    PokerHand targetHand = handBeingProcessed == 1 ? player1Hand : player2Hand;
                    String playerNameField = 
                        String.Format("player-{0}-name", handBeingProcessed.ToString());

                    if (!PokerHandsForm.AllKeys.Contains(playerNameField) ||
                            String.IsNullOrEmpty(PokerHandsForm[playerNameField]) ||
                            String.IsNullOrEmpty(PokerHandsForm[playerNameField].Trim()))
                        throw new Exception(
                            String.Format(
                                "No name present for player {0}.",
                                handBeingProcessed.ToString()));

                    targetHand.PlayerName = PokerHandsForm[playerNameField];

                    for (int cardBeingProcessed = 1; cardBeingProcessed <= 5; cardBeingProcessed++)
                    {
                        Card card = cardFactory();

                        String cardRankField =
                            String.Format(
                                "rank-player-{0}-card-{1}",
                                handBeingProcessed.ToString(),
                                cardBeingProcessed.ToString());
                        String cardSuitField =
                            String.Format(
                                "suit-player-{0}-card-{1}",
                                handBeingProcessed.ToString(),
                                cardBeingProcessed.ToString());

                        if(!PokerHandsForm.AllKeys.Contains(cardRankField) ||
                                String.IsNullOrEmpty(PokerHandsForm[cardRankField]))
                            throw new Exception(
                                String.Format(
                                    "Missing rank value for card {0} for player \"{1}.\"",
                                    cardBeingProcessed.ToString(),
                                    targetHand.PlayerName));
                        if (!PokerHandsForm.AllKeys.Contains(cardSuitField) ||
                                String.IsNullOrEmpty(PokerHandsForm[cardSuitField]))
                            throw new Exception(
                                String.Format(
                                    "Missing suit value for card {0} for player \"{1}.\"",
                                    cardBeingProcessed.ToString(),
                                    targetHand.PlayerName));

                        CardRank rank = CardRank.Ace;
                        if (!Enum.TryParse<CardRank>(PokerHandsForm[cardRankField], true, out rank))
                            throw new Exception(
                                String.Format(
                                    "Invalid card rank value \"{0}\".",
                                    PokerHandsForm[cardRankField]));

                        CardSuit suit = CardSuit.Clubs;
                        if (!Enum.TryParse<CardSuit>(PokerHandsForm[cardSuitField], true, out suit))
                            throw new Exception(
                                String.Format(
                                    "Invalid card suit value \"{0}\".",
                                    PokerHandsForm[cardSuitField]));

                        card.Rank = rank;
                        card.Suit = suit;

                        targetHand.Cards.Add(card);
                    }
                }

                viewModel.Verdict = pokerHandJudge.Judge(player1Hand, player2Hand);
                viewModel.RunnerUpHand =
                    viewModel.Verdict.WinningHand == player1Hand ?
                        player2Hand : player1Hand;
            }
            catch(Exception ex)
            {
                viewModel.ThrownException = ex;
            }

            return View(viewModel);
        }
    }
}
