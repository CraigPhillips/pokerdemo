using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Net.FrozenExports.PlayingCards.Poker.Tests
{
    /// <summary>
    ///     Container for tests that verify PokerHand objects are working correctly.
    /// </summary>
    [TestClass]
    public class PokerHandTests
    {
        private PokerHand testHand;

        /// <summary>
        ///     Creates new PokerHand object to use during each test.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            testHand = new PokerHand();
        }

        /// <summary>
        ///     Verifies that, in a hand containing a straight, the first card in that straight will
        ///     be returned.
        /// </summary>
        [TestMethod]
        public void SimpleFindFirstCardInStraight()
        {
            testHand.Cards.Add(new Card() { Rank = CardRank.Five, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Six, Suit = CardSuit.Diamonds });
            testHand.Cards.Add(new Card() { Rank = CardRank.Seven, Suit = CardSuit.Hearts });
            testHand.Cards.Add(new Card() { Rank = CardRank.Four, Suit = CardSuit.Spades });
            testHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Clubs });

            Card firstCardInStraight = testHand.FindFirstCardInStraight();

            Assert.IsNotNull(firstCardInStraight, "First found in straight not found.");
            Assert.AreEqual(
                CardRank.Four,
                firstCardInStraight.Rank,
                "Incorrect card returned as first card in straight.");
        }

        /// <summary>
        ///     Verifies that a null is returned in the case a hand does not contain a flush.
        /// </summary>
        [TestMethod]
        public void FindFirstCardInStraightFailure()
        {
            testHand.Cards.Add(new Card() { Rank = CardRank.Five, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Six, Suit = CardSuit.Diamonds });
            testHand.Cards.Add(new Card() { Rank = CardRank.Seven, Suit = CardSuit.Hearts });
            testHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Spades });
            testHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Clubs });

            Card firstCardInStraight = testHand.FindFirstCardInStraight();

            Assert.IsNull(firstCardInStraight, "Found first card in straight where no straight exists");
        }

        /// <summary>
        ///     Verifies that a hand containing a flush is correctly identified.
        /// </summary>
        [TestMethod]
        public void SimpleFlushDetection()
        {
            testHand.Cards.Add(new Card() { Rank = CardRank.Ace, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Five, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Four, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Jack, Suit = CardSuit.Clubs });

            Assert.IsTrue(testHand.ContainsHandType(PokerHandType.Flush), "Flush detection failed.");
        }

        /// <summary>
        ///     Verifies that a hand holding four of a kind is correctly identified.
        /// </summary>
        [TestMethod]
        public void SimpleFourOfAKindDetection()
        {
            testHand.Cards.Add(new Card() { Rank = CardRank.Seven, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Seven, Suit = CardSuit.Diamonds });
            testHand.Cards.Add(new Card() { Rank = CardRank.Seven, Suit = CardSuit.Hearts });
            testHand.Cards.Add(new Card() { Rank = CardRank.Seven, Suit = CardSuit.Spades });
            testHand.Cards.Add(new Card() { Rank = CardRank.Ace, Suit = CardSuit.Clubs });

            Assert.IsTrue(
                testHand.ContainsHandType(PokerHandType.FourOfAKind), "Four of a kind detection failed.");
        }

        /// <summary>
        ///     Verifies that a hand holding a full house is correctly identified. 
        /// </summary>
        [TestMethod]
        public void SimpleFullHouseDetection()
        {
            testHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Diamonds });
            testHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Hearts });
            testHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Diamonds });

            Assert.IsTrue(
                testHand.ContainsHandType(PokerHandType.FullHouse), "Full house detection failed.");
        }

        /// <summary>
        ///     Verifies that a hand holding a pair is correctly identified.
        /// </summary>
        [TestMethod]
        public void SimplePairDetection()
        {
            testHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Hearts });
            testHand.Cards.Add(new Card() { Rank = CardRank.Two, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Three, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Four, Suit = CardSuit.Diamonds });

            Assert.IsTrue(
                testHand.ContainsHandType(PokerHandType.Pair), "Pair detection failed.");
        }

        /// <summary>
        ///     Verifies that a hand holding a straight is correctly identified.
        /// </summary>
        [TestMethod]
        public void SimpleStraightDetection()
        {
            testHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Ten, Suit = CardSuit.Diamonds });
            testHand.Cards.Add(new Card() { Rank = CardRank.Jack, Suit = CardSuit.Hearts });
            testHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Spades });
            testHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Clubs });

            Assert.IsTrue(
                testHand.ContainsHandType(PokerHandType.Straight), "Straight detection failed.");
        }

        /// <summary>
        ///     Verifies that a hand holding a straight flush is correctly identified.
        /// </summary>
        [TestMethod]
        public void SimpleStraightFlushDetection()
        {
            testHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Ten, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Jack, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Clubs });

            Assert.IsTrue(
                testHand.ContainsHandType(PokerHandType.StraightFlush), "Straight flush detection failed.");
        }

        /// <summary>
        ///     Verifies that a hand with a straight that is not a straight flush is properly detected.
        /// </summary>
        [TestMethod]
        public void StraightFlushFailureOnNonFlush()
        {
            testHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Ten, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Jack, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Spades });
            testHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Clubs });

            Assert.IsFalse(
                testHand.ContainsHandType(
                    PokerHandType.StraightFlush), 
                    "Straight flush without flush flagged incorrectly.");
        }

        [TestMethod]
        public void ThreeOfAKindDetection()
        {
            testHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Spades });
            testHand.Cards.Add(new Card() { Rank = CardRank.Six, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Hearts});
            testHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Diamonds });

            Assert.IsTrue(
                testHand.ContainsHandType(PokerHandType.ThreeOfAKind), "Three of a kind detection failed.");
        }

        /// <summary>
        ///     Verifies that a hand with two pair is properly detected.
        /// </summary>
        [TestMethod]
        public void SimpleTwoPairDetection()
        {
            testHand.Cards.Add(new Card() { Rank = CardRank.Five, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Five, Suit = CardSuit.Hearts });
            testHand.Cards.Add(new Card() { Rank = CardRank.Three, Suit = CardSuit.Diamonds });
            testHand.Cards.Add(new Card() { Rank = CardRank.Three, Suit = CardSuit.Hearts });
            testHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Clubs });

            Assert.IsTrue(
                testHand.ContainsHandType(PokerHandType.TwoPair), "Two pair detection failed.");
        }

        /// <summary>
        ///     Validates that a hand with only a high card is detected as such.
        /// </summary>
        [TestMethod]
        public void NoHandTypeDetection()
        {
            testHand.Cards.Add(new Card() { Rank = CardRank.Five, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Ace, Suit = CardSuit.Hearts });
            testHand.Cards.Add(new Card() { Rank = CardRank.Four, Suit = CardSuit.Diamonds });
            testHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Hearts });
            testHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Clubs });

            Assert.IsTrue(
                testHand.ContainsHandType(PokerHandType.None), "No hand type detection failed.");
        }

        /// <summary>
        ///     Verifies that the generic hand type detection call works correctly.
        /// </summary>
        [TestMethod]
        public void HandTypeDetection()
        {
            testHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Diamonds });
            testHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Hearts });
            testHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Clubs });
            testHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Diamonds });

            PokerHandType? detectedHandType = testHand.DetermineHandType();

            Assert.IsNotNull(detectedHandType, "Detected hand type result was null.");
            Assert.IsTrue(detectedHandType.HasValue, "Detected hand type had not value.");
            Assert.AreEqual(
                PokerHandType.FullHouse,
                detectedHandType.Value,
                "Full house not detected during generic hand detection test.");
        }
    }
}
