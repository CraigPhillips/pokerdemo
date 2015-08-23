using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.FrozenExports.PlayingCards
{
    /// <summary>
    ///     A set of playing cards that begins containing a single copy of all cards.
    /// </summary>
    public class CardDeck
    {
        private Stack<Card> remainingCards;

        /// <summary>
        ///     Creates a new deck of randomly ordered cards.
        /// </summary>
        /// <param name="CardFactory">Creates new Card objects.</param>
        public CardDeck(Func<Card> CardFactory)
        {
            if (CardFactory == null) throw new ArgumentNullException("CardFactory");

            remainingCards = new Stack<Card>();

            List<Card> allCards = new List<Card>();
            foreach(CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach(CardRank rank in Enum.GetValues(typeof(CardRank)))
                {
                    Card card = CardFactory();
                    card.Rank = rank;
                    card.Suit = suit;

                    allCards.Add(card);
                }
            }

            // Shuffles cards and then assembles the deck.
            Random shuffler = new Random();
            allCards.OrderBy(card => shuffler.Next()).ToList().ForEach(card =>
                remainingCards.Push(card));
        }

        /// <summary>
        ///     Creates sets of cards from the current deck. The number of hands and number
        ///     cards in a hand must be greater than zero and the total number of cards
        ///     must be less than or equal to the number of cards remaining in the deck.
        /// </summary>
        /// <param name="HandCount">The number of hands needed.</param>
        /// <param name="HandSize">The size of each hand.</param>
        /// <returns>The hands requested, if they can be generated, otherwise null.</returns>
        public List<List<Card>> Deal(int HandCount, int HandSize)
        {
            List<List<Card>> hands = null;

            if(HandCount > 0 && HandSize > 0 && HandCount * HandSize <= remainingCards.Count)
            {
                hands = new List<List<Card>>();

                for(int handsCreated = 0; handsCreated < HandCount; handsCreated++)
                {
                    List<Card> hand = new List<Card>();

                    for(int cardsInHandDealt = 0; cardsInHandDealt < HandSize; cardsInHandDealt++)
                    {
                        hand.Add(remainingCards.Pop());
                    }

                    hands.Add(hand);
                }
            }

            return hands;
        }
    }
}
