using System.Drawing;
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
        }

        [TestMethod]
        public void DifferentColors_DoNotAffectEachOther()
        {
            var bonusManager = new BonusManager();

            bonusManager.AddProgress(BonusColor.Red, 100);

            Assert.IsTrue(bonusManager.IsReady(BonusColor.Red));
            Assert.IsFalse(bonusManager.IsReady(BonusColor.Blue));
            Assert.IsFalse(bonusManager.IsReady(BonusColor.Green));
            Assert.IsFalse(bonusManager.IsReady(BonusColor.Yellow));
        }

        [TestMethod]
        public void YellowBonus_TwoSteps_IsNotReadyYet()
        {
            var bonusManager = new BonusManager();

            bonusManager.AddProgress(BonusColor.Yellow, 25);
            bonusManager.AddProgress(BonusColor.Yellow, 25);

            Assert.IsFalse(bonusManager.IsReady(BonusColor.Yellow));
        }

        [TestMethod]
        public void GetClickedBonusColor_RedArea_ReturnsRed()
        {
            var bonusManager = new BonusManager();

            var result = bonusManager.GetClickedBonusColor(new Point(120, 100), 280, 220);

            Assert.AreEqual(BonusColor.Red, result);
        }

        [TestMethod]
        public void GetClickedBonusColor_OutsideFigure_ReturnsNone()
        {
            var bonusManager = new BonusManager();

            var result = bonusManager.GetClickedBonusColor(new Point(10, 10), 280, 220);

            Assert.AreEqual(BonusColor.None, result);
        }
    }
}