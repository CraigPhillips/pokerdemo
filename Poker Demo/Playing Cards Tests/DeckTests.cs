using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Net.FrozenExports.PlayingCards.Tests
{
    /// <summary>
    ///     Verifies that deck behavior is functioning as intended.
    /// </summary>
    [TestClass]
    public class DeckTests
    {
        private CardDeck testDeck; 

        /// <summary>
        ///     Creates new test deck for each test.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            testDeck = new CardDeck(() => { return new Card(); });
        }

        /// <summary>
        ///     Verifies that dealt hands appear correct.
        /// </summary>
        [TestMethod]
        public void Deal()
        {
            List<List<Card>> dealtHands = testDeck.Deal(4, 12);

            Assert.AreEqual(
                4,
                dealtHands.Count,
                "Incorrect number of hands dealt.");
            Assert.AreEqual(
                12,
                dealtHands[3].Count,
                "Incorrect number of cards dealt.");
        }

        /// <summary>
        ///     Verifies that the correct exception is thrown if a null value is provided
        ///     for the CardFactory argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FactoryNullOnInitialize()
        {
            new CardDeck(null);
        }
    }
}
