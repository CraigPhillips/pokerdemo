using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Net.FrozenExports.PlayingCards.Tests
{
    [TestClass]
    public class CardHandTests
    {
        private CardHand testHand;

        /// <summary>
        ///     Used to initialize test resources before running any tests.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            testHand = new CardHand();
        }

        /// <summary>
        ///     Verifies that constructor behavior is executing correctly.
        /// </summary>
        [TestMethod]
        public void Initialization()
        {
            Assert.IsNotNull(
                testHand.Cards,
                "Cards collection is not created correctly.");
            Assert.AreEqual(
                0,
                testHand.Cards.Count,
                "Test hand was not created empty.");
            Assert.IsNull(
                testHand.PlayerName,
                "Test hand was not created with a null player name.");
        }
    }
}
