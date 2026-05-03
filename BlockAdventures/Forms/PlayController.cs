using BlockAdventures.GameLogic;
using BlockAdventures.Models;

namespace BlockAdventures.Controllers
{
    public class PlayController
    {
        private GameModel gameModel;

        public int Score
        {
            get { return gameModel.Score; }
        }

        public FigureModel CurrentFigure
        {
            get { return gameModel.CurrentFigure; }
        }

        public TaskModel CurrentTask
        {
            get { return gameModel.CurrentTask; }
        }

        public BonusColor?[,] Cells
        {
            get { return gameModel.Cells; }
        }

        public PlayController(int fieldCols, int fieldRows)
        {
            gameModel = new GameModel(fieldCols, fieldRows);
        }

        public int GetFigureChangeCost()
        {
            return gameModel.GetFigureChangeCost();
        }

        public int GetTaskChangeCost()
        {
            return gameModel.GetTaskChangeCost();
        }

        public bool ChangeFigure()
        {
            return gameModel.ChangeFigure();
        }

        public bool ChangeTask()
        {
            return gameModel.ChangeTask();
        }

        public bool CanPlaceCurrentFigure(int startCol, int startRow)
        {
            return gameModel.CanPlaceCurrentFigure(startCol, startRow);
        }

        public bool PlaceCurrentFigure(int startCol, int startRow)
        {
            return gameModel.PlaceCurrentFigure(startCol, startRow);
        }

        public bool HasAnyPlaceForCurrentFigure()
        {
            return gameModel.HasAnyPlaceForCurrentFigure();
        }

        public int GetBonusProgress(BonusColor bonusColor)
        {
            return gameModel.GetBonusProgress(bonusColor);
        }

        public bool IsBonusReady(BonusColor bonusColor)
        {
            return gameModel.IsBonusReady(bonusColor);
        }

        public bool UseBonus(BonusColor bonusColor)
        {
            return gameModel.UseBonus(bonusColor);
        }
    }
}