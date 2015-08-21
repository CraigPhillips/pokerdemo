using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Net.FrozenExports.PlayingCards.Poker.Tests
{
    /// <summary>
    ///     Verifies that class responsible for judging PokerHand objects behaves
    ///     correctly.
    /// </summary>
    [TestClass]
    public class PokerHandJudgeTests
    {
        private PokerHandJudge testJudge;

        /// <summary>
        ///     Creates a new judge object for each test.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            testJudge = new PokerHandJudge(
                () => { return new PokerHandVerdict(); });
        }

        /// <summary>
        ///     Verifies that a null argument exception is thrown if PokerHandVerdict factory
        ///     isn't provided.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullVerdictFactoryTest()
        {
            testJudge = new PokerHandJudge(null);
        }

        /// <summary>
        ///     Verifies that the correct winner is declared for two hands of different types.
        /// </summary>
        [TestMethod]
        public void WinByHandTypeTest()
        {
            PokerHand fullHouseHand = new PokerHand() { PlayerName = "Full House Hand" };
            PokerHand threeOfAKindHand = new PokerHand() { PlayerName = "Three of a Kind Hand" };

            fullHouseHand.Cards.Add(new Card() { Rank = CardRank.Jack, Suit = CardSuit.Hearts });
            fullHouseHand.Cards.Add(new Card() { Rank = CardRank.Jack, Suit = CardSuit.Diamonds });
            fullHouseHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Hearts });
            fullHouseHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Diamonds });
            fullHouseHand.Cards.Add(new Card() { Rank = CardRank.Jack, Suit = CardSuit.Clubs });

            threeOfAKindHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Clubs });
            threeOfAKindHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Diamonds });
            threeOfAKindHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Hearts });
            threeOfAKindHand.Cards.Add(new Card() { Rank = CardRank.Five, Suit = CardSuit.Clubs });
            threeOfAKindHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Clubs });

            PokerHandVerdict verdict = testJudge.Judge(fullHouseHand, threeOfAKindHand);

            Assert.AreEqual(
                PokerHandVerdictType.WinByHandType,
                verdict.VerdictType,
                "Incorrect verdict type determined.");
            Assert.AreEqual(
                new KeyValuePair<PokerHandType, PokerHandType>(
                    PokerHandType.FullHouse, PokerHandType.ThreeOfAKind),
                verdict.DecidingHandTypePair,
                "Incorrect winning hand type pair determined.");
            Assert.AreEqual(
                fullHouseHand,
                verdict.WinningHand,
                "Incorrect hand determined to be the winner.");
        }

        /// <summary>
        ///     Verifies that the correct winner is determined when both hands have a flush.
        /// </summary>
        [TestMethod]
        public void TwoFlushesTests()
        {
            PokerHand winningFlushHand = new PokerHand() { PlayerName = "Winning Flush Hand" };
            PokerHand losingFlushHand = new PokerHand() { PlayerName = "Losing Flush Hand" };

            Card queenOfClubs = new Card() { Rank = CardRank.Queen, Suit = CardSuit.Clubs };
            Card jackOfHearts = new Card() { Rank = CardRank.Jack, Suit = CardSuit.Hearts };
            
            winningFlushHand.Cards.Add(new Card() { Rank = CardRank.Ace, Suit = CardSuit.Clubs });
            winningFlushHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Clubs });
            winningFlushHand.Cards.Add(new Card() { Rank = CardRank.Two, Suit = CardSuit.Clubs });
            winningFlushHand.Cards.Add(queenOfClubs);
            winningFlushHand.Cards.Add(new Card() { Rank = CardRank.Jack, Suit = CardSuit.Clubs });

            losingFlushHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Hearts });
            losingFlushHand.Cards.Add(new Card() { Rank = CardRank.Ace, Suit = CardSuit.Hearts });
            losingFlushHand.Cards.Add(new Card() { Rank = CardRank.Five, Suit = CardSuit.Hearts });
            losingFlushHand.Cards.Add(jackOfHearts);
            losingFlushHand.Cards.Add(new Card() { Rank = CardRank.Two, Suit = CardSuit.Hearts });

            PokerHandVerdict verdict = testJudge.Judge(winningFlushHand, losingFlushHand);

            Assert.AreEqual(
                PokerHandVerdictType.WinByInTypeOrdering,
                verdict.VerdictType.Value,
                "Incorrect verdict type determined when winning hand is present.");
            Assert.AreEqual(
                new KeyValuePair<Card, Card>(queenOfClubs, jackOfHearts),
                verdict.DecidingCardPair,
                "Incorrect deciding pair found when winning hand is present.");
            Assert.AreEqual(
                winningFlushHand,
                verdict.WinningHand,
                "Incorrect winning hand found when winning hand is present.");

            losingFlushHand.Cards.RemoveAt(2);
            losingFlushHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Hearts });
            verdict = testJudge.Judge(winningFlushHand, losingFlushHand);
            Assert.AreEqual(
                PokerHandVerdictType.Tie,
                verdict.VerdictType,
                "Tied flush hands not properly detected.");
        }

        /// <summary>
        ///     Verifies that the correct winner is determined when both hands have a four of a kind.
        /// </summary>
        [TestMethod]
        public void TwoFourOfAKindTests()
        {
            PokerHand winningFourOfAKind = new PokerHand() { PlayerName = "Winning four of a kind." };
            PokerHand losingFourOfAKind = new PokerHand() { PlayerName = "Losing four of a kind." };

            Card aceOfClubs = new Card() { Rank = CardRank.Ace, Suit = CardSuit.Clubs };
            Card kingOfClubs = new Card() { Rank = CardRank.King, Suit = CardSuit.Clubs };

            winningFourOfAKind.Cards.Add(aceOfClubs);
            winningFourOfAKind.Cards.Add(new Card() { Rank = CardRank.Ace, Suit = CardSuit.Diamonds });
            winningFourOfAKind.Cards.Add(new Card() { Rank = CardRank.Ace, Suit = CardSuit.Hearts });
            winningFourOfAKind.Cards.Add(new Card() { Rank = CardRank.Ace, Suit = CardSuit.Spades });
            winningFourOfAKind.Cards.Add(new Card() { Rank = CardRank.Two, Suit = CardSuit.Clubs });

            losingFourOfAKind.Cards.Add(kingOfClubs);
            losingFourOfAKind.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Diamonds });
            losingFourOfAKind.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Hearts });
            losingFourOfAKind.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Spades });
            losingFourOfAKind.Cards.Add(new Card() { Rank = CardRank.Three, Suit = CardSuit.Clubs });

            PokerHandVerdict verdict = testJudge.Judge(losingFourOfAKind, winningFourOfAKind);

            Assert.AreEqual(
                PokerHandVerdictType.WinByInTypeOrdering,
                verdict.VerdictType,
                "Incorrect verdict type detected.");
            Assert.AreEqual(
                new KeyValuePair<Card, Card>(aceOfClubs, kingOfClubs),
                verdict.DecidingCardPair,
                "Incorrect correct pair found.");
            Assert.AreEqual(
                winningFourOfAKind,
                verdict.WinningHand,
                "Incorrect winning hand detected.");
        }

        /// <summary>
        ///     Verifies that behavior when two full houses are present is correct.
        /// </summary>
        [TestMethod]
        public void TwoFullHouseTests()
        {
            PokerHand winningHand = new PokerHand() { PlayerName = "Winning hand." };
            PokerHand losingHand = new PokerHand() { PlayerName = "Losing hand." };

            Card aceOfClubs = new Card() { Rank = CardRank.Ace, Suit = CardSuit.Clubs };
            Card nineOfHearts = new Card() { Rank = CardRank.Nine, Suit = CardSuit.Hearts };

            winningHand.Cards.Add(aceOfClubs);
            winningHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Clubs });
            winningHand.Cards.Add(new Card() { Rank = CardRank.Ace, Suit = CardSuit.Diamonds });
            winningHand.Cards.Add(new Card() { Rank = CardRank.Ace, Suit = CardSuit.Hearts });
            winningHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Hearts });

            losingHand.Cards.Add(nineOfHearts);
            losingHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Hearts });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Spades });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Spades });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Diamonds });

            PokerHandVerdict verdict = testJudge.Judge(winningHand, losingHand);

            Assert.AreEqual(
                PokerHandVerdictType.WinByInTypeOrdering,
                verdict.VerdictType,
                "Incorrect verdict type detected.");
            Assert.AreEqual(
                new KeyValuePair<Card, Card>(aceOfClubs, nineOfHearts),
                verdict.DecidingCardPair,
                "Incorrect deciding pair detected.");
            Assert.AreEqual(
                winningHand,
                verdict.WinningHand,
                "Incorrect winner detected.");
        }

        /// <summary>
        ///     Verifies that behavior when two hands with only high cards to be considered
        ///     is correct.
        /// </summary>
        [TestMethod]
        public void TwoHighCardTests()
        {
            PokerHand winningHand = new PokerHand() { PlayerName = "Winning hand." };
            PokerHand losingHand = new PokerHand() { PlayerName = "Losing hand." };

            Card queenOfClubs = new Card() { Rank = CardRank.Queen, Suit = CardSuit.Clubs };
            Card nineOfHearts = new Card() { Rank = CardRank.Nine, Suit = CardSuit.Hearts };

            winningHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Clubs });
            winningHand.Cards.Add(queenOfClubs);
            winningHand.Cards.Add(new Card() { Rank = CardRank.Jack, Suit = CardSuit.Hearts});
            winningHand.Cards.Add(new Card() { Rank = CardRank.Four, Suit = CardSuit.Diamonds });
            winningHand.Cards.Add(new Card() { Rank = CardRank.Two, Suit = CardSuit.Hearts });

            losingHand.Cards.Add(nineOfHearts);
            losingHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Hearts });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Spades });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Seven, Suit = CardSuit.Spades });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Six, Suit = CardSuit.Diamonds });

            PokerHandVerdict verdict = testJudge.Judge(winningHand, losingHand);

            Assert.AreEqual(
                PokerHandVerdictType.WinByExtraTypeOrdering,
                verdict.VerdictType,
                "Incorrect verdict type detected.");
            Assert.AreEqual(
                new KeyValuePair<Card, Card>(queenOfClubs, nineOfHearts),
                verdict.DecidingCardPair,
                "Incorrect deciding pair detected.");
            Assert.AreEqual(
                winningHand,
                verdict.WinningHand,
                "Incorrect winner detected.");
        }

        /// <summary>
        ///     Verifies that behavior when two hands with a single pair each is correct.
        /// </summary>
        [TestMethod]
        public void TwoHandsWithAPairTests()
        {
            PokerHand winningHand = new PokerHand() { PlayerName = "Winning hand." };
            PokerHand losingHand = new PokerHand() { PlayerName = "Losing hand." };

            Card queenOfClubs = new Card() { Rank = CardRank.Queen, Suit = CardSuit.Clubs };
            Card kingOfClubs = new Card() { Rank = CardRank.King, Suit = CardSuit.Clubs };
            Card nineOfClubs = new Card() { Rank = CardRank.Nine, Suit = CardSuit.Clubs };

            winningHand.Cards.Add(queenOfClubs);
            winningHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Diamonds });
            winningHand.Cards.Add(kingOfClubs);
            winningHand.Cards.Add(new Card() { Rank = CardRank.Four, Suit = CardSuit.Diamonds });
            winningHand.Cards.Add(new Card() { Rank = CardRank.Two, Suit = CardSuit.Hearts });

            losingHand.Cards.Add(nineOfClubs);
            losingHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Hearts });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Spades });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Four, Suit = CardSuit.Spades });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Two, Suit = CardSuit.Diamonds });

            PokerHandVerdict verdict = testJudge.Judge(winningHand, losingHand);

            Assert.AreEqual(
                PokerHandVerdictType.WinByInTypeOrdering,
                verdict.VerdictType,
                "Incorrect verdict type detected.");
            Assert.AreEqual(
                new KeyValuePair<Card, Card>(queenOfClubs, nineOfClubs),
                verdict.DecidingCardPair,
                "Incorrect deciding pair detected.");
            Assert.AreEqual(
                winningHand,
                verdict.WinningHand,
                "Incorrect winner detected.");

            // Alters hand so that each hand's pair is of the same rank and then tests again.
            losingHand.Cards.RemoveAt(2);
            losingHand.Cards.RemoveAt(1);
            losingHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Hearts });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Queen, Suit = CardSuit.Spades });
            verdict = testJudge.Judge(winningHand, losingHand);

            Assert.AreEqual(
                PokerHandVerdictType.WinByExtraTypeOrdering,
                verdict.VerdictType,
                "Incorrect verdict type detected.");
            Assert.AreEqual(
                new KeyValuePair<Card, Card>(kingOfClubs, nineOfClubs),
                verdict.DecidingCardPair,
                "Incorrect deciding pair detected.");
            Assert.AreEqual(
                winningHand,
                verdict.WinningHand,
                "Incorrect winner detected.");

            // Alters hands so the two are of equal value.
            losingHand.Cards.RemoveAt(0);
            losingHand.Cards.Add(new Card() { Rank = CardRank.King, Suit = CardSuit.Hearts });
            verdict = testJudge.Judge(losingHand, winningHand);

            Assert.AreEqual(
                PokerHandVerdictType.Tie,
                verdict.VerdictType,
                "Incorrect verdict type detected.");
        }

        /// <summary>
        ///     Verifies that behavior when boths hands have ordinary straights is 
        ///     correct.
        /// </summary>
        [TestMethod]
        public void BothHandsHaveOrdinaryStraights()
        {
            PokerHand winningHand = new PokerHand() { PlayerName = "Winning hand." };
            PokerHand losingHand = new PokerHand() { PlayerName = "Losing hand." };

            Card queenOfClubs = new Card() { Rank = CardRank.Queen, Suit = CardSuit.Clubs };
            Card nineOfHearts = new Card() { Rank = CardRank.Nine, Suit = CardSuit.Hearts };

            winningHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Clubs });
            winningHand.Cards.Add(queenOfClubs);
            winningHand.Cards.Add(new Card() { Rank = CardRank.Jack, Suit = CardSuit.Clubs });
            winningHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Diamonds });
            winningHand.Cards.Add(new Card() { Rank = CardRank.Ten, Suit = CardSuit.Diamonds });

            losingHand.Cards.Add(nineOfHearts);
            losingHand.Cards.Add(new Card() { Rank = CardRank.Five, Suit = CardSuit.Hearts });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Spades });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Seven, Suit = CardSuit.Spades });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Six, Suit = CardSuit.Spades });

            PokerHandVerdict verdict = testJudge.Judge(winningHand, losingHand);

            Assert.AreEqual(
                PokerHandVerdictType.WinByInTypeOrdering,
                verdict.VerdictType,
                "Incorrect verdict type detected.");
            Assert.AreEqual(
                new KeyValuePair<Card, Card>(queenOfClubs, nineOfHearts),
                verdict.DecidingCardPair,
                "Incorrect deciding pair detected.");
            Assert.AreEqual(
                winningHand,
                verdict.WinningHand,
                "Incorrect winner detected.");

            // Sets up hands to tie and retests
            losingHand.Cards[0].Rank = CardRank.Queen;
            losingHand.Cards[1].Rank = CardRank.Jack;
            losingHand.Cards[2].Rank = CardRank.Ten;
            losingHand.Cards[3].Rank = CardRank.Nine;
            losingHand.Cards[4].Rank = CardRank.Eight;
            verdict = testJudge.Judge(winningHand, losingHand);

            Assert.AreEqual(
                PokerHandVerdictType.Tie,
                verdict.VerdictType,
                "Tie not detected correctly.");
        }

        /// <summary>
        ///     Verifies that behavior when boths hands have straight flushes is 
        ///     correct.
        /// </summary>
        [TestMethod]
        public void BothHandsHaveStraighFlushes()
        {
            PokerHand winningHand = new PokerHand() { PlayerName = "Winning hand." };
            PokerHand losingHand = new PokerHand() { PlayerName = "Losing hand." };

            Card queenOfClubs = new Card() { Rank = CardRank.Queen, Suit = CardSuit.Clubs };
            Card nineOfHearts = new Card() { Rank = CardRank.Nine, Suit = CardSuit.Hearts };

            winningHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Clubs });
            winningHand.Cards.Add(queenOfClubs);
            winningHand.Cards.Add(new Card() { Rank = CardRank.Jack, Suit = CardSuit.Clubs });
            winningHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Clubs});
            winningHand.Cards.Add(new Card() { Rank = CardRank.Ten, Suit = CardSuit.Clubs });

            losingHand.Cards.Add(nineOfHearts);
            losingHand.Cards.Add(new Card() { Rank = CardRank.Five, Suit = CardSuit.Hearts });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Hearts });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Seven, Suit = CardSuit.Hearts });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Six, Suit = CardSuit.Hearts });

            PokerHandVerdict verdict = testJudge.Judge(winningHand, losingHand);

            Assert.AreEqual(
                PokerHandVerdictType.WinByInTypeOrdering,
                verdict.VerdictType,
                "Incorrect verdict type detected.");
            Assert.AreEqual(
                new KeyValuePair<Card, Card>(queenOfClubs, nineOfHearts),
                verdict.DecidingCardPair,
                "Incorrect deciding pair detected.");
            Assert.AreEqual(
                winningHand,
                verdict.WinningHand,
                "Incorrect winner detected.");

            // Sets up hands to tie and retests
            losingHand.Cards[0].Rank = CardRank.Queen;
            losingHand.Cards[1].Rank = CardRank.Jack;
            losingHand.Cards[2].Rank = CardRank.Ten;
            losingHand.Cards[3].Rank = CardRank.Nine;
            losingHand.Cards[4].Rank = CardRank.Eight;
            verdict = testJudge.Judge(winningHand, losingHand);

            Assert.AreEqual(
                PokerHandVerdictType.Tie,
                verdict.VerdictType,
                "Tie not detected correctly.");
        }

        /// <summary>
        ///     Verifies that behavior when boths hands have three of a kind is correct.
        /// </summary>
        [TestMethod]
        public void BothHandsHaveThreeOfAKind()
        {
            PokerHand winningHand = new PokerHand() { PlayerName = "Winning hand." };
            PokerHand losingHand = new PokerHand() { PlayerName = "Losing hand." };

            Card eightOfSpades = new Card() { Rank = CardRank.Eight, Suit = CardSuit.Spades };
            Card fiveOfHearts = new Card() { Rank = CardRank.Five, Suit = CardSuit.Hearts };

            winningHand.Cards.Add(eightOfSpades);
            winningHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Diamonds });
            winningHand.Cards.Add(new Card() { Rank = CardRank.Eight, Suit = CardSuit.Hearts });
            winningHand.Cards.Add(new Card() { Rank = CardRank.Nine, Suit = CardSuit.Diamonds });
            winningHand.Cards.Add(new Card() { Rank = CardRank.Ten, Suit = CardSuit.Diamonds });

            losingHand.Cards.Add(fiveOfHearts);
            losingHand.Cards.Add(new Card() { Rank = CardRank.Five, Suit = CardSuit.Clubs });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Five, Suit = CardSuit.Diamonds });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Four, Suit = CardSuit.Clubs });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Three, Suit = CardSuit.Clubs });

            PokerHandVerdict verdict = testJudge.Judge(winningHand, losingHand);

            Assert.AreEqual(
                PokerHandVerdictType.WinByInTypeOrdering,
                verdict.VerdictType,
                "Incorrect verdict type detected.");
            Assert.AreEqual(
                new KeyValuePair<Card, Card>(eightOfSpades, fiveOfHearts),
                verdict.DecidingCardPair,
                "Incorrect deciding pair detected.");
            Assert.AreEqual(
                winningHand,
                verdict.WinningHand,
                "Incorrect winner detected.");
        }

        /// <summary>
        ///     Verifies that behavior with both hands having two pairs is correct.
        /// </summary>
        [TestMethod]
        public void BothHandsHaveTwoPairTests()
        {
            PokerHand winningHand = new PokerHand() { PlayerName = "Winning Hand" };
            PokerHand losingHand = new PokerHand() { PlayerName = "Losing Hand" };

            Card sevenOfSpades = new Card() { Rank = CardRank.Seven, Suit = CardSuit.Spades };
            Card sixOfSpades = new Card() { Rank = CardRank.Six, Suit = CardSuit.Spades };
            Card fiveOfClubs = new Card() { Rank = CardRank.Five, Suit = CardSuit.Clubs };
            Card fourOfHearts = new Card() { Rank = CardRank.Four, Suit = CardSuit.Hearts };
            Card threeOfClubs = new Card() { Rank = CardRank.Three, Suit = CardSuit.Clubs };

            winningHand.Cards.Add(sevenOfSpades);
            winningHand.Cards.Add(new Card() { Rank = CardRank.Seven, Suit = CardSuit.Hearts });
            winningHand.Cards.Add(sixOfSpades);
            winningHand.Cards.Add(new Card() { Rank = CardRank.Six, Suit = CardSuit.Hearts });
            winningHand.Cards.Add(fourOfHearts);

            losingHand.Cards.Add(fiveOfClubs);
            losingHand.Cards.Add(new Card() { Rank = CardRank.Five, Suit = CardSuit.Diamonds });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Four, Suit = CardSuit.Clubs });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Four, Suit = CardSuit.Diamonds });
            losingHand.Cards.Add(threeOfClubs);

            PokerHandVerdict verdict = testJudge.Judge(losingHand, winningHand);

            Assert.AreEqual(
                PokerHandVerdictType.WinByInTypeOrdering,
                verdict.VerdictType,
                "Incorrect verdict type detected when larger pair should have won.");
            Assert.AreEqual(
                new KeyValuePair<Card, Card>(sevenOfSpades, fiveOfClubs),
                verdict.DecidingCardPair,
                "Incorrect card pair detected when larger pair should have won.");
            Assert.AreEqual(
                winningHand,
                verdict.WinningHand,
                "Incorrect winning hand identified when larger pair should have won.");

            // Alters losing hand so that winning hand must win based on the smaller pair.
            losingHand.Cards.RemoveAt(3);
            losingHand.Cards.RemoveAt(2);
            losingHand.Cards.Add(new Card() { Rank = CardRank.Seven, Suit = CardSuit.Diamonds });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Seven, Suit = CardSuit.Clubs });
            verdict = testJudge.Judge(winningHand, losingHand);

            Assert.AreEqual(
                PokerHandVerdictType.WinByInTypeOrdering,
                verdict.VerdictType,
                "Incorrect verdict type detected when smaller pair should have won.");
            Assert.AreEqual(
                new KeyValuePair<Card, Card>(sixOfSpades, fiveOfClubs),
                verdict.DecidingCardPair,
                "Incorrect card pair detected when smaller pair should have won.");
            Assert.AreEqual(
                winningHand,
                verdict.WinningHand,
                "Incorrect winning hand identified when smaller pair should have won.");

            // Alters losing hand so that winning hand must win based on cards not in either pair.
            losingHand.Cards.RemoveAt(1);
            losingHand.Cards.RemoveAt(0);
            losingHand.Cards.Add(new Card() { Rank = CardRank.Six, Suit = CardSuit.Clubs });
            losingHand.Cards.Add(new Card() { Rank = CardRank.Six, Suit = CardSuit.Diamonds });
            verdict = testJudge.Judge(losingHand, winningHand);

            Assert.AreEqual(
                PokerHandVerdictType.WinByExtraTypeOrdering,
                verdict.VerdictType,
                "Incorrect verdict type detected when non-pair card should have won.");
            Assert.AreEqual(
                new KeyValuePair<Card, Card>(fourOfHearts, threeOfClubs),
                verdict.DecidingCardPair,
                "Incorrect card pair detected when non-pair card should have won.");
            Assert.AreEqual(
                winningHand,
                verdict.WinningHand,
                "Incorrect winning hand identified when non-pair card should have won.");

            // Alters losing hand so that two hands tie.
            losingHand.Cards.RemoveAt(0);
            losingHand.Cards.Add(new Card() { Rank = CardRank.Four, Suit = CardSuit.Clubs });
            verdict = testJudge.Judge(winningHand, losingHand);

            Assert.AreEqual(
                PokerHandVerdictType.Tie,
                verdict.VerdictType,
                "Tie not detected correctly.");
        }
    }
}
