using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.FrozenExports.PlayingCards.Poker
{
    /// <summary>
    ///     A hand of cards belong to a player in a game of poker.
    /// </summary>
    public class PokerHand : CardHand
    {
        /// <summary>
        ///     The number of cards it takes to make a straight or flush.
        /// </summary>
        public const int NumberOfCardsInAFlushOrStraight = 5;

        /// <summary>
        ///     The number of cards with the same rank it takes to get the larger portion of a full house.
        /// </summary>
        public const int NumberOfCardsInMajorityOfFullHouse = 3;

        /// <summary>
        ///     The number of cards with the same rank it takes to get the smaller portion of a full house.
        /// </summary>
        public const int NumberOfCardsInMinorityOfFullHouse = 2;

        /// <summary>
        ///     Determines whether or not this player's hand contains
        ///     cards that make up the requested hand type.
        /// </summary>
        /// <param name="HandType">The type of hand for which the test is being run.</param>
        /// <returns>True if this hand contains the requested type. Otherwise, false.</returns>
        public bool ContainsHandType(PokerHandType HandType)
        {
            bool containsHandType = false;

            if(Cards.Count > 0)
            {
                switch(HandType)
                {
                    case PokerHandType.Flush:
                        containsHandType = ContainsFlush();
                        break;
                    case PokerHandType.FourOfAKind:
                        containsHandType =
                            GroupCardsByRank().Values.Where(cards => cards.Count >= 4).Count() > 0;
                        break;
                    case PokerHandType.FullHouse:
                        Dictionary<CardRank, List<Card>> cardsByRank = GroupCardsByRank();

                        cardsByRank
                            .Keys
                            .Where(
                                majorityRank =>
                                    cardsByRank[majorityRank].Count >= NumberOfCardsInMajorityOfFullHouse)
                            .ToList()
                            // Each of the ranks that reach this portion of the query will have 3 or
                            // more of that rank in the current hand.
                            .ForEach(majorityRank =>
                            {
                                if(cardsByRank
                                    .Keys
                                    .Where(
                                        minorityRank =>
                                            minorityRank != majorityRank &&
                                            cardsByRank[minorityRank].Count >= 
                                                NumberOfCardsInMinorityOfFullHouse)
                                    // If another rank exists (other than the majority rank) which has two
                                    // or more cards of that rank in the current hand, we have a full house.
                                    .Count() > 0)
                                {
                                    containsHandType = true;
                                }
                            });
                        break;
                    case PokerHandType.None:
                        containsHandType =
                            // Covers: pair, two pair, three of a kind, four of a kind, full house
                            !ContainsHandType(PokerHandType.Pair) &&
                            // Covers: straight & straight flush
                            !ContainsHandType(PokerHandType.Straight) &&
                            // Covers: flush & straight flush
                            !ContainsHandType(PokerHandType.Flush);
                        break;
                    case PokerHandType.Pair:
                        containsHandType =
                            GroupCardsByRank().Values.Where(cards => cards.Count >= 2).Count() > 0;
                        break;
                    case PokerHandType.Straight:
                        containsHandType =
                            FindFirstCardInStraight() != null;
                        break;
                    case PokerHandType.StraightFlush:
                        Card firstCardInStraight = FindFirstCardInStraight();
                        if(firstCardInStraight != null 
                            && firstCardInStraight.Rank != null && firstCardInStraight.Rank.HasValue
                            && firstCardInStraight.Suit != null && firstCardInStraight.Suit.HasValue)
                        {
                            bool straightFlushStillInPlay = true;

                            // Verifies that the rest of the cards in the straight are a flush.
                            for(int straightCardsInspected = 1; 
                                straightCardsInspected < NumberOfCardsInAFlushOrStraight;
                                straightCardsInspected++)
                            {
                                if (!ContainsCard(
                                    // This casting is safe to do because we've already determined that the
                                    // hand contains a straight and that means that the lowest card in
                                    // that straight will always be low enough that sum being calculated here
                                    // will fall within the enumeration's value.
                                    (CardRank)((int)firstCardInStraight.Rank.Value + straightCardsInspected),
                                    firstCardInStraight.Suit.Value))
                                {
                                    straightFlushStillInPlay = false;
                                }
                            }

                            containsHandType = straightFlushStillInPlay;
                        }
                        break;
                    case PokerHandType.ThreeOfAKind:
                        containsHandType =
                            GroupCardsByRank().Values.Where(cards => cards.Count >= 3).Count() > 0;
                        break;
                    case PokerHandType.TwoPair:
                        containsHandType =
                            GroupCardsByRank().Values.Where(cards => cards.Count >= 2).Count() > 1;
                        break;
                }
            }

            return containsHandType;
        }

        /// <summary>
        ///     If any cards have been added to the hand, determines the type of hand.
        /// </summary>
        /// <returns>
        ///     The type of poker hand represented by the current set of cards.
        /// 
        ///     If no cards have been added, returns null.
        /// </returns>
        public PokerHandType? DetermineHandType()
        {
            PokerHandType? foundHandType = null;

            if (Cards.Count > 0)
            {
                List<PokerHandType> handTypesHighestToLowest =
                    Enum.GetValues(typeof(PokerHandType)).Cast<PokerHandType>().ToList();
                handTypesHighestToLowest.Reverse();

                // Determines the type of hand. This will always select at least one hand type
                // even if that type is "None" indicating that the highest card should be
                // used for ranking.
                handTypesHighestToLowest.ForEach(handType =>
                {
                    if (foundHandType == null && ContainsHandType(handType))
                        foundHandType = handType;
                });
            }

            return foundHandType;
        }

        /// <summary>
        ///     If the current hand contains a straight, returns the lowest card in that straight.
        /// </summary>
        /// <returns>
        ///     The lowest card in the straight if one is found. Otherwise, null.
        /// </returns>
        public Card FindFirstCardInStraight()
        {
            Card lowestCardInStraight = null;

            if(Cards.Count >= NumberOfCardsInAFlushOrStraight)
            {
                List<Card> orderedCardList = Cards.OrderBy(card => card.Rank).ToList();

                int currentStraightCardCount = 0;
                Card potentialLowestCardInStraight = null;
                CardRank? lastCardRank = null;
                orderedCardList.ForEach(card =>
                {
                    if (card != null && card.Rank != null && lowestCardInStraight == null)
                    {
                        bool shouldResetCount = false;

                        // This is the first card being considered.
                        if (currentStraightCardCount == 0)
                            shouldResetCount = true;
                        else
                        {
                            // A sequence of cards continues.
                            if ((int)card.Rank.Value == (int)lastCardRank.Value + 1)
                            {
                                currentStraightCardCount++;
                                lastCardRank = card.Rank;
                            }
                            else shouldResetCount = true;
                        }

                        // Sets the current card as the first card in a potential new sequence.
                        if (shouldResetCount)
                        {
                            currentStraightCardCount = 1;
                            lastCardRank = card.Rank;
                            potentialLowestCardInStraight = card;
                        }
                        // Checks to see if enough cards have been encountered to consider this a
                        // straight.
                        else if(currentStraightCardCount == NumberOfCardsInAFlushOrStraight)
                            lowestCardInStraight = potentialLowestCardInStraight;
                    }
                });
            }

            return lowestCardInStraight;
        }

        /// <summary>
        ///     Determines whether or not the current hand contains a flush.
        /// </summary>
        /// <returns>True if the current hand contains a flush. Otherwise, false.</returns>
        private bool ContainsFlush()
        {
            bool containsFlush = false;

            if(Cards.Count >= NumberOfCardsInAFlushOrStraight)
            {
                foreach(CardSuit suit in Enum.GetValues(typeof(CardSuit)))
                {
                    if (Cards
                            .Where(card => card.Suit == suit)
                            .Count() >= NumberOfCardsInAFlushOrStraight)
                    {
                        containsFlush = true;
                        break;
                    }
                }
            }

            return containsFlush;
        }
    }
}
