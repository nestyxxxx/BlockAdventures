using BlockAdventures.GameLogic;
using BlockAdventures.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlockAdventures.Tests
{
    [TestClass]
    public class BonusManagerTests
    {
        [TestMethod]
        public void RedBonus_BecomesReady_AfterFourSteps()
        {
            var bonusManager = new BonusManager();

            bonusManager.AddProgress(BonusColor.Red, 25);
            Assert.IsFalse(bonusManager.IsReady(BonusColor.Red));

            bonusManager.AddProgress(BonusColor.Red, 25);
            Assert.IsFalse(bonusManager.IsReady(BonusColor.Red));

            bonusManager.AddProgress(BonusColor.Red, 25);
            Assert.IsFalse(bonusManager.IsReady(BonusColor.Red));

            bonusManager.AddProgress(BonusColor.Red, 25);
            Assert.IsTrue(bonusManager.IsReady(BonusColor.Red));
        }

        [TestMethod]
        public void Reset_AfterReady_MakesBonusNotReady()
        {
            var bonusManager = new BonusManager();

            bonusManager.AddProgress(BonusColor.Blue, 100);
            Assert.IsTrue(bonusManager.IsReady(BonusColor.Blue));

            bonusManager.Reset(BonusColor.Blue);

            Assert.IsFalse(bonusManager.IsReady(BonusColor.Blue));
        }

        [TestMethod]
        public void Bonus_ProgressMoreThanHundred_StillStaysReady()
        {
            var bonusManager = new BonusManager();

            bonusManager.AddProgress(BonusColor.Green, 125);

            Assert.IsTrue(bonusManager.IsReady(BonusColor.Green));
            Assert.AreEqual(100, bonusManager.GetProgress(BonusColor.Green));
        }
    }
}