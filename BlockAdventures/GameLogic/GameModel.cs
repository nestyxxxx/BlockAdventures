using BlockAdventures.Models;

namespace BlockAdventures.GameLogic
{
    public class GameModel
    {
        private readonly FieldManager fieldManager;
        private readonly BonusManager bonusManager;

        public int FieldCols { get; }
        public int FieldRows { get; }

        public int Score { get; private set; }

        public FigureModel CurrentFigure { get; private set; }
        public TaskModel CurrentTask { get; private set; }

        public BonusColor?[,] Cells
        {
            get { return fieldManager.Cells; }
        }

        public GameModel(int fieldCols, int fieldRows)
        {
            FieldCols = fieldCols;
            FieldRows = fieldRows;

            fieldManager = new FieldManager(fieldCols, fieldRows);
            bonusManager = new BonusManager();

            CurrentFigure = FigureGenerator.Generate();
            CurrentTask = TaskGenerator.Generate();
        }

        public int GetFigureChangeCost()
        {
            if (Score <= 0)
            {
                return 0;
            }

            return (int)System.Math.Ceiling(Score * 0.10);
        }

        public int GetTaskChangeCost()
        {
            if (Score <= 0)
            {
                return 0;
            }

            return (int)System.Math.Ceiling(Score * 0.20);
        }

        public bool ChangeFigure()
        {
            var cost = GetFigureChangeCost();

            if (!TrySpendScore(cost))
            {
                return false;
            }

            CurrentFigure = FigureGenerator.Generate();
            return true;
        }

        public bool ChangeTask()
        {
            var cost = GetTaskChangeCost();

            if (!TrySpendScore(cost))
            {
                return false;
            }

            CurrentTask = TaskGenerator.Generate();
            return true;
        }

        public bool CanPlaceCurrentFigure(int startCol, int startRow)
        {
            return fieldManager.CanPutFigure(CurrentFigure, startCol, startRow);
        }

        public bool PlaceCurrentFigure(int startCol, int startRow)
        {
            if (!fieldManager.CanPutFigure(CurrentFigure, startCol, startRow))
            {
                return false;
            }

            fieldManager.PutFigure(CurrentFigure, startCol, startRow);

            var taskIsDone = TaskChecker.CheckTask(
                CurrentTask,
                fieldManager.Cells,
                FieldCols,
                FieldRows
            );

            if (taskIsDone)
            {
                Score += CurrentTask.Reward;
                bonusManager.AddProgress(CurrentTask.BonusColor, 25);
                CurrentTask = TaskGenerator.Generate();
            }

            fieldManager.ClearFilledRows();
            fieldManager.ClearFilledColumns();

            CurrentFigure = FigureGenerator.Generate();
            return true;
        }

        public bool HasAnyPlaceForCurrentFigure()
        {
            return fieldManager.HasAnyPlaceForFigure(CurrentFigure);
        }

        public int GetBonusProgress(BonusColor bonusColor)
        {
            return bonusManager.GetProgress(bonusColor);
        }

        public bool IsBonusReady(BonusColor bonusColor)
        {
            return bonusManager.IsReady(bonusColor);
        }

        public bool UseBonus(BonusColor bonusColor)
        {
            if (!bonusManager.IsReady(bonusColor))
            {
                return false;
            }

            fieldManager.ClearColor(bonusColor);
            bonusManager.Reset(bonusColor);
            return true;
        }

        private bool TrySpendScore(int points)
        {
            if (Score < points)
            {
                return false;
            }

            Score -= points;
            return true;
        }
    }
}