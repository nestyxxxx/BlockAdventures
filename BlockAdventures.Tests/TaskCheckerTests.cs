using BlockAdventures.GameLogic;
using BlockAdventures.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlockAdventures.Tests
{
    [TestClass]
    public class TaskCheckerTests
    {
        [TestMethod]
        public void CheckTask_FillTopRowYellow_ReturnsTrue()
        {
            var field = new System.Drawing.Color?[10, 8];
            var yellow = TaskChecker.GetYellowColor();

            for (var col = 0; col < 10; col++)
            {
                field[col, 0] = yellow;
            }

            var task = new TaskModel(
                TaskType.FillTopRowYellow,
                "",
                70,
                BonusColor.Yellow
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckTask_FillTopRowYellow_WithWrongColor_ReturnsFalse()
        {
            var field = new System.Drawing.Color?[10, 8];
            var yellow = TaskChecker.GetYellowColor();
            var blue = TaskChecker.GetBlueColor();

            for (var col = 0; col < 9; col++)
            {
                field[col, 0] = yellow;
            }

            field[9, 0] = blue;

            var task = new TaskModel(
                TaskType.FillTopRowYellow,
                "",
                70,
                BonusColor.Yellow
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CheckTask_FillCornersRed_ReturnsTrue()
        {
            var field = new System.Drawing.Color?[10, 8];
            var red = TaskChecker.GetRedColor();

            field[0, 0] = red;
            field[9, 0] = red;
            field[0, 7] = red;
            field[9, 7] = red;

            var task = new TaskModel(
                TaskType.FillCornersRed,
                "",
                100,
                BonusColor.Red
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckTask_FillCornersRed_WithoutOneCorner_ReturnsFalse()
        {
            var field = new System.Drawing.Color?[10, 8];
            var red = TaskChecker.GetRedColor();

            field[0, 0] = red;
            field[9, 0] = red;
            field[0, 7] = red;

            var task = new TaskModel(
                TaskType.FillCornersRed,
                "",
                100,
                BonusColor.Red
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CheckTask_FillRowRed_ReturnsTrue()
        {
            var field = new System.Drawing.Color?[10, 8];
            var red = TaskChecker.GetRedColor();

            for (var col = 0; col < 10; col++)
            {
                field[col, 3] = red;
            }

            var task = new TaskModel(
                TaskType.FillRowRed,
                "",
                70,
                BonusColor.Red
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckTask_FillRightColumnBlue_ReturnsTrue()
        {
            var field = new System.Drawing.Color?[10, 8];
            var blue = TaskChecker.GetBlueColor();

            for (var row = 0; row < 8; row++)
            {
                field[9, row] = blue;
            }

            var task = new TaskModel(
                TaskType.FillRightColumnBlue,
                "",
                70,
                BonusColor.Blue
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckTask_FillRightColumnBlue_WithOneEmptyCell_ReturnsFalse()
        {
            var field = new System.Drawing.Color?[10, 8];
            var blue = TaskChecker.GetBlueColor();

            for (var row = 0; row < 7; row++)
            {
                field[9, row] = blue;
            }

            var task = new TaskModel(
                TaskType.FillRightColumnBlue,
                "",
                70,
                BonusColor.Blue
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CheckTask_FillCenterGreen_ReturnsTrue()
        {
            var field = new System.Drawing.Color?[10, 8];
            var green = TaskChecker.GetGreenColor();

            field[4, 3] = green;
            field[5, 3] = green;
            field[4, 4] = green;
            field[5, 4] = green;

            var task = new TaskModel(
                TaskType.FillCenterGreen,
                "",
                80,
                BonusColor.Green
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckTask_FillColumnGreen_ReturnsTrue()
        {
            var field = new System.Drawing.Color?[10, 8];
            var green = TaskChecker.GetGreenColor();

            for (var row = 0; row < 8; row++)
            {
                field[2, row] = green;
            }

            var task = new TaskModel(
                TaskType.FillColumnGreen,
                "",
                70,
                BonusColor.Green
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckTask_FillCenterYellow_ReturnsTrue()
        {
            var field = new System.Drawing.Color?[10, 8];
            var yellow = TaskChecker.GetYellowColor();

            field[4, 3] = yellow;
            field[5, 3] = yellow;
            field[4, 4] = yellow;
            field[5, 4] = yellow;

            var task = new TaskModel(
                TaskType.FillCenterYellow,
                "",
                80,
                BonusColor.Yellow
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsTrue(result);
        }
    }
}