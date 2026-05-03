using BlockAdventures.Models;

namespace BlockAdventures.GameLogic
{
    public class BonusManager
    {
        private int redBonus = 0;
        private int greenBonus = 0;
        private int yellowBonus = 0;
        private int blueBonus = 0;

        public void AddProgress(BonusColor bonusColor, int value)
        {
            if (bonusColor == BonusColor.Red)
            {
                redBonus += value;
                if (redBonus > 100)
                {
                    redBonus = 100;
                }
            }

            if (bonusColor == BonusColor.Green)
            {
                greenBonus += value;
                if (greenBonus > 100)
                {
                    greenBonus = 100;
                }
            }

            if (bonusColor == BonusColor.Yellow)
            {
                yellowBonus += value;
                if (yellowBonus > 100)
                {
                    yellowBonus = 100;
                }
            }

            if (bonusColor == BonusColor.Blue)
            {
                blueBonus += value;
                if (blueBonus > 100)
                {
                    blueBonus = 100;
                }
            }
        }

        public int GetProgress(BonusColor bonusColor)
        {
            if (bonusColor == BonusColor.Red)
            {
                return redBonus;
            }

            if (bonusColor == BonusColor.Green)
            {
                return greenBonus;
            }

            if (bonusColor == BonusColor.Yellow)
            {
                return yellowBonus;
            }

            if (bonusColor == BonusColor.Blue)
            {
                return blueBonus;
            }

            return 0;
        }

        public bool IsReady(BonusColor bonusColor)
        {
            return GetProgress(bonusColor) == 100;
        }

        public void Reset(BonusColor bonusColor)
        {
            if (bonusColor == BonusColor.Red)
            {
                redBonus = 0;
            }

            if (bonusColor == BonusColor.Green)
            {
                greenBonus = 0;
            }

            if (bonusColor == BonusColor.Yellow)
            {
                yellowBonus = 0;
            }

            if (bonusColor == BonusColor.Blue)
            {
                blueBonus = 0;
            }
        }
    }
}