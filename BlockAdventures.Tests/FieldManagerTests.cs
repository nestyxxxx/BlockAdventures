using System.Drawing;
using BlockAdventures.GameLogic;
using BlockAdventures.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlockAdventures.Tests
{
    [TestClass]
    public class FieldManagerTests
    {
        private FigureModel CreateOneCellFigure(BonusColor color)
        {
            var cells = new bool[3, 3];
            cells[0, 0] = true;

            return new FigureModel(cells, color);
        }

        [TestMethod]
        public void CanPutFigure_EmptyField_ReturnsTrue()
        {
            var fieldManager = new FieldManager(5, 5);
            var figure = CreateOneCellFigure(BonusColor.Red);

            var result = fieldManager.CanPutFigure(figure, 2, 2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanPutFigure_BusyCell_ReturnsFalse()
        {
            var fieldManager = new FieldManager(5, 5);
            var figure = CreateOneCellFigure(BonusColor.Red);

            fieldManager.PutFigure(figure, 2, 2);

            var result = fieldManager.CanPutFigure(figure, 2, 2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PutFigure_OneCellFigure_PaintsNeededCell()
        {
            var fieldManager = new FieldManager(5, 5);
            var figure = CreateOneCellFigure(BonusColor.Blue);

            fieldManager.PutFigure(figure, 1, 3);

            Assert.AreEqual(BonusColor.Blue, fieldManager.Cells[1, 3].Value);
        }

        [TestMethod]
        public void ClearFilledRows_FullRow_ReturnsOneAndClearsRow()
        {
            var fieldManager = new FieldManager(3, 2);
            var figure = CreateOneCellFigure(BonusColor.Green);

            fieldManager.PutFigure(figure, 0, 0);
            fieldManager.PutFigure(figure, 1, 0);
            fieldManager.PutFigure(figure, 2, 0);

            var clearedRows = fieldManager.ClearFilledRows();

            Assert.AreEqual(1, clearedRows);
            Assert.IsNull(fieldManager.Cells[0, 0]);
            Assert.IsNull(fieldManager.Cells[1, 0]);
            Assert.IsNull(fieldManager.Cells[2, 0]);
        }

        [TestMethod]
        public void ClearFilledColumns_FullColumn_ReturnsOneAndClearsColumn()
        {
            var fieldManager = new FieldManager(2, 3);
            var figure = CreateOneCellFigure(BonusColor.Yellow);

            fieldManager.PutFigure(figure, 0, 0);
            fieldManager.PutFigure(figure, 0, 1);
            fieldManager.PutFigure(figure, 0, 2);

            var clearedColumns = fieldManager.ClearFilledColumns();

            Assert.AreEqual(1, clearedColumns);
            Assert.IsNull(fieldManager.Cells[0, 0]);
            Assert.IsNull(fieldManager.Cells[0, 1]);
            Assert.IsNull(fieldManager.Cells[0, 2]);
        }

        [TestMethod]
        public void ClearColor_RemovesOnlyNeededColor()
        {
            var fieldManager = new FieldManager(4, 4);
            var redFigure = CreateOneCellFigure(BonusColor.Red);
            var blueFigure = CreateOneCellFigure(BonusColor.Blue);

            fieldManager.PutFigure(redFigure, 0, 0);
            fieldManager.PutFigure(redFigure, 1, 1);
            fieldManager.PutFigure(blueFigure, 2, 2);

            fieldManager.ClearColor(BonusColor.Red);

            Assert.IsNull(fieldManager.Cells[0, 0]);
            Assert.IsNull(fieldManager.Cells[1, 1]);
            Assert.IsNotNull(fieldManager.Cells[2, 2]);
            Assert.AreEqual(BonusColor.Blue, fieldManager.Cells[2, 2].Value);
        }

        [TestMethod]
        public void ClampFigurePosition_LeftTopOutside_ReturnsZeroZero()
        {
            var fieldManager = new FieldManager(5, 5);
            var figure = CreateOneCellFigure(BonusColor.Red);

            var point = fieldManager.ClampFigurePosition(figure, -10, -10);

            Assert.AreEqual(0, point.X);
            Assert.AreEqual(0, point.Y);
        }

        [TestMethod]
        public void HasAnyPlaceForFigure_FullField_ReturnsFalse()
        {
            var fieldManager = new FieldManager(1, 1);
            var figure = CreateOneCellFigure(BonusColor.Red);

            fieldManager.PutFigure(figure, 0, 0);

            var result = fieldManager.HasAnyPlaceForFigure(figure);

            Assert.IsFalse(result);
        }
    }
}