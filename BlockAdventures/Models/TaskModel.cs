namespace BlockAdventures.Models
{
    public enum BonusColor
    {
        None,
        Red,
        Green,
        Yellow,
        Blue
    }

    public enum TaskType
    {
        FillCornersRed,
        FillRowRed,

        FillRightColumnBlue,
        FillCornersBlue,

        FillCenterGreen,
        FillColumnGreen,

        FillTopRowYellow,
        FillCenterYellow
    }

    public class TaskModel
    {
        public TaskType Type { get; }
        public string Text { get; }
        public int Reward { get; }
        public BonusColor BonusColor { get; }

        public TaskModel(TaskType type, string text, int reward, BonusColor bonusColor)
        {
            Type = type;
            Text = text;
            Reward = reward;
            BonusColor = bonusColor;
        }
    }
}