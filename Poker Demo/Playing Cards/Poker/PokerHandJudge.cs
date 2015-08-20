using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.FrozenExports.PlayingCards.Poker
{
    /// <summary>
    ///     Compares a set of poker hands and decides which is the best and why.
    /// </summary>
    public class PokerHandJudge
    {
        private Func<PokerHandVerdict> verdictFactory;

        /// <summary>
        ///     Simple constructor.
        /// </summary>
        /// <param name="VerdictFactory">Responsible for creating new verdict objects.</param>
        public PokerHandJudge(
            Func<PokerHandVerdict> VerdictFactory)
        {
            if (VerdictFactory == null) throw new ArgumentNullException("VerdictFactory");

            verdictFactory = VerdictFactory;
        }

        /// <summary>
        ///     Renders a verdict based on the provided hands.
        ///     
        ///     Both hands must not be null and contain more than zero cards. Otherwise,
        ///     the verdict will be rendered as a tie and no other values on it will be set.
        /// </summary>
        /// <param name="LeftHand">The first hand to be compared.</param>
        /// <param name="RightHand">The second hand to be compared.</param>
        /// <returns>The judgement of the provided hands.</returns>
        public PokerHandVerdict Judge(PokerHand LeftHand, PokerHand RightHand)
        {
            PokerHandVerdict verdict = verdictFactory();
            verdict.VerdictType = PokerHandVerdictType.Tie;

            if(LeftHand != null && LeftHand.Cards.Count > 0 &&
                RightHand != null && RightHand.Cards.Count > 0)
            {
                // It is safe to assume that these values are set since both
                // hands have cards in them.
                PokerHandType leftHandType = LeftHand.DetermineHandType().Value;
                PokerHandType rightHandType = RightHand.DetermineHandType().Value;

                bool leftHandWins = false;
                bool isTie = false;
                KeyValuePair<int, KeyValuePair<Card, Card>?>? compareResult = null;
                // If the two hands are of differing types, simply compare those types.
                if(leftHandType != rightHandType)
                {
                    leftHandWins = leftHandType > rightHandType;

                    verdict.DecidingHandTypePair = new KeyValuePair<PokerHandType, PokerHandType>(
                        leftHandWins ? leftHandType : rightHandType,
                        leftHandWins ? rightHandType : leftHandType);
                    verdict.VerdictType = PokerHandVerdictType.WinByHandType;
                }
                // If the two hands are of the same type, tie-breakers need to be considered.
                else
                {
                    Dictionary<CardRank, List<Card>> leftHandRanked = LeftHand.GroupCardsByRank();
                    Dictionary<CardRank, List<Card>> rightHandRanked = RightHand.GroupCardsByRank();

                    switch(leftHandType)
                    {
                        case PokerHandType.Flush:
                        case PokerHandType.None:
                        case PokerHandType.Straight:
                        case PokerHandType.StraightFlush:
                            compareResult = Compare(LeftHand.Cards, RightHand.Cards);
                            if (compareResult.Value.Key == 0) isTie = true;
                            else
                                verdict.VerdictType =
                                    leftHandType != PokerHandType.None ?
                                        PokerHandVerdictType.WinByInTypeOrdering :
                                        PokerHandVerdictType.WinByExtraTypeOrdering;

                            break;
                        case PokerHandType.FourOfAKind:
                        case PokerHandType.ThreeOfAKind:
                            compareResult = Compare(
                                GetCardListOfSize(
                                    leftHandRanked, leftHandType == PokerHandType.ThreeOfAKind? 3 : 4),
                                GetCardListOfSize(
                                    rightHandRanked, leftHandType == PokerHandType.ThreeOfAKind? 3 : 4));

                            verdict.VerdictType = PokerHandVerdictType.WinByInTypeOrdering;

                            break;
                        case PokerHandType.FullHouse:
                            compareResult = Compare(
                                GetCardListOfSize(
                                    leftHandRanked, PokerHand.NumberOfCardsInMajorityOfFullHouse),
                                GetCardListOfSize(
                                    rightHandRanked, PokerHand.NumberOfCardsInMajorityOfFullHouse));

                            verdict.VerdictType = PokerHandVerdictType.WinByInTypeOrdering;

                            break;
                        case PokerHandType.Pair:
                            // First the cards that make up the pair are considered.
                            compareResult = Compare(
                                GetCardListOfSize(leftHandRanked, 2),
                                GetCardListOfSize(rightHandRanked, 2));

                            if (compareResult.Value.Key != 0)
                                verdict.VerdictType = PokerHandVerdictType.WinByInTypeOrdering;
                            // If this doesn't give a decision about which is better,
                            // consider the rest of the cards next.
                            else
                            {
                                compareResult = Compare(
                                    GetCardListOfSize(leftHandRanked, 1),
                                    GetCardListOfSize(rightHandRanked, 1));

                                // If these are also the same, the result is a tie
                                if (compareResult.Value.Key == 0)
                                    isTie = true;
                                else
                                    verdict.VerdictType = PokerHandVerdictType.WinByExtraTypeOrdering;
                            }

                            break;
                        case PokerHandType.TwoPair:
                            // First, compare the results of the higher of each hands pairs.
                            compareResult = Compare(
                                leftHandRanked
                                    .Where(entry => entry.Value.Count == 2)
                                    .Last()
                                    .Value,
                                rightHandRanked
                                    .Where(entry => entry.Value.Count == 2)
                                    .Last()
                                    .Value);

                            if (compareResult.Value.Key != 0)
                                verdict.VerdictType = PokerHandVerdictType.WinByInTypeOrdering;
                            else
                            {
                                // If the higher pairs are the same, compares the lower pairs.
                                compareResult = Compare(
                                    leftHandRanked
                                        .Where(entry => entry.Value.Count == 2)
                                        .First()
                                        .Value,
                                    rightHandRanked
                                        .Where(entry => entry.Value.Count == 2)
                                        .First()
                                        .Value);

                                if (compareResult.Value.Key != 0)
                                    verdict.VerdictType = PokerHandVerdictType.WinByInTypeOrdering;
                                else
                                {
                                    // If both pairs are equal, compares the remaining card(s)
                                    compareResult = Compare(
                                        GetCardListOfSize(leftHandRanked, 1),
                                        GetCardListOfSize(rightHandRanked, 1));

                                    if (compareResult.Value.Key == 0)
                                        isTie = true;
                                    else
                                        verdict.VerdictType = PokerHandVerdictType.WinByExtraTypeOrdering;
                                }
                            }

                            break;
                    }
                }

                if (!isTie)
                {
                    if(compareResult != null)
                    {
                        leftHandWins = compareResult.Value.Key > 0;

                        verdict.DecidingCardPair = new KeyValuePair<Card, Card>(
                            // This is a bit confusing since both KeyValuePairs and nullable type
                            // are being used but the chain of values sorts through the objects like:
                            // Nullable compare result -> non-nullable -> Nullable card pair -> non-nullable
                            compareResult.Value.Value.Value.Key,
                            compareResult.Value.Value.Value.Value);
                    }

                    verdict.WinningHand =
                        leftHandWins ? LeftHand : RightHand;
                }
            }

            return verdict;
        }

        /// <summary>
        ///     Determines how two lists of cards are ranked against each other based on the CardRanks
        ///     of their individual members.
        ///     
        ///     Both lists must have the same number of cards and must not be null or exceptions will
        ///     result.
        /// </summary>
        /// <param name="LeftCardList">The first list of cards to be compared.</param>
        /// <param name="RightCardList">The second list of cards to be compared.</param>
        /// <returns>
        ///     A KeyValuePair where the key is the comparison value: -1 if the left list is ranked 
        ///     lower than the right, 0 if they are ranked equally, otherwise 1. The value is
        ///     another KeyValuePair where the key is the winning CardRank and the value is the
        ///     losing CardRank.
        /// </returns>
        private KeyValuePair<int, KeyValuePair<Card, Card>?> 
            Compare(List<Card> LeftCardList, List<Card> RightCardList)
        {
            int comparisonValue = 0;
            KeyValuePair<Card, Card>? differingCards = null;

            List<Card> orderedLeftList = LeftCardList.OrderByDescending(card => card.Rank).ToList();
            List<Card> orderedRightList = RightCardList.OrderByDescending(card => card.Rank).ToList();

            for (int cardsConsidered = 0; cardsConsidered < LeftCardList.Count; cardsConsidered++)
            {
                Card leftCard = orderedLeftList[cardsConsidered];
                Card rightCard = orderedRightList[cardsConsidered];

                if (leftCard.Rank > rightCard.Rank)
                {
                    comparisonValue = 1;
                    differingCards = new KeyValuePair<Card, Card>(leftCard, rightCard);
                }
                else if (leftCard.Rank < rightCard.Rank)
                {
                    comparisonValue = -1;
                    differingCards = new KeyValuePair<Card, Card>(rightCard, leftCard);
                }

                // If a differing value was found, we no longer need to compare cards, so stops 
                // looping.
                if (comparisonValue != 0) break;
            }

            return new KeyValuePair<int, KeyValuePair<Card, Card>?>(comparisonValue, differingCards);
        }

        /// <summary>
        ///     Pulls the list of cards out of the provided dictionary which has the number
        ///     of cards specified in the Size argument. 
        /// </summary>
        /// <param name="Target">The target from which the list should be pulled.</param>
        /// <param name="Size">The size of the desired list.</param>
        /// <returns>
        ///     A list of cards of the give size, if it can be found. Otherwise, null. If multiple
        ///     lists of the given size are found, combines those lists and returns the joined
        ///     result.
        /// </returns>
        private List<Card> GetCardListOfSize(Dictionary<CardRank, List<Card>> Target, int Size)
        {
            List<Card> target = null;

            if(Size > 0 && Target != null)
            {
                List<List<Card>> targets =
                    Target
                        .Where(entry => entry.Value.Count == Size)
                        .Select(entry => entry.Value)
                        .ToList();

                if (targets.Count == 1)
                    target = targets.First();
                else
                {
                    target = new List<Card>();
                    targets.ForEach(list => target.AddRange(list));
                }
            }

            return target;
        }
    }
}
