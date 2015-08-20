using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.FrozenExports.PlayingCards
{
    /// <summary>
    ///     A set of cards belonging to a player.
    /// </summary>
    public class CardHand
    {
        /// <summary>
        ///     The cards belonging to the player.
        /// </summary>
        public List<Card> Cards { get; private set; }

        /// <summary>
        ///     The name of the player who holds the cards in this hand.
        /// </summary>
        public String PlayerName { get; set; }

        /// <summary>
        ///     Simple constructor used to set up list of cards that will contain the
        ///     objects belonging to this hand.
        /// </summary>
        public CardHand()
        {
            Cards = new List<Card>();
        }

        /// <summary>
        ///     Determines whether or not this hand contains a card with the provided
        ///     rank and suit.
        /// </summary>
        /// <param name="Rank">The rank of the card for which a check is being requested.</param>
        /// <param name="Suit">The suit of the card for which a check is being requested.</param>
        /// <returns></returns>
        public bool ContainsCard(CardRank Rank, CardSuit Suit)
        {
            return
                Cards.Where(card => card.Rank == Rank && card.Suit == Suit).Count() > 0;
        }

        /// <summary>
        ///     Groups cards by their rank for easier analysis. The lower ranks will appear
        ///     first in this grouping.
        /// </summary>
        /// <returns>The cards in the hand grouped by their rank.</returns>
        public Dictionary<CardRank, List<Card>> GroupCardsByRank()
        {
            Dictionary<CardRank, List<Card>> cardsByRank = new Dictionary<CardRank, List<Card>>();

            Cards.OrderBy(card => card.Rank).ToList().ForEach(card =>
            {
                if (card.Rank.HasValue)
                {
                    if (!cardsByRank.ContainsKey(card.Rank.Value))
                        cardsByRank.Add(card.Rank.Value, new List<Card>());

                    cardsByRank[card.Rank.Value].Add(card);
                }
            });

            return cardsByRank;
        }
    }
}
