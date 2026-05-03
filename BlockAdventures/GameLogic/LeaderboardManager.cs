using System;
using System.Collections.Generic;
using System.IO;
using BlockAdventures.Models;

namespace BlockAdventures.GameLogic
{
    public static class LeaderboardManager
    {
        private static readonly string filePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "leaderboard.txt");

        public static List<PlayerScore> LoadScores()
        {
            var scores = new List<PlayerScore>();

            if (!File.Exists(filePath))
            {
                return scores;
            }

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var parts = line.Split('|');

                if (parts.Length != 2)
                {
                    continue;
                }

                var name = parts[0].Trim();
                var scoreText = parts[1].Trim();

                int score;
                if (int.TryParse(scoreText, out score))
                {
                    scores.Add(new PlayerScore(name, score));
                }
            }

            return NormalizeScores(scores);
        }

        public static bool IsHighScore(int score)
        {
            var scores = LoadScores();

            if (ContainsScore(scores, score))
            {
                return false;
            }

            if (scores.Count < 10)
            {
                return true;
            }

            return score > scores[scores.Count - 1].Score;
        }

        public static void AddOrUpdateScore(string name, int score)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Игрок";
            }

            var scores = LoadScores();
            var existingPlayerIndex = FindPlayerIndex(scores, name);

            if (existingPlayerIndex != -1)
            {
                if (score <= scores[existingPlayerIndex].Score)
                {
                    return;
                }

                if (ContainsScoreExceptPlayer(scores, score, existingPlayerIndex))
                {
                    return;
                }

                scores[existingPlayerIndex].Score = score;
                SaveScores(NormalizeScores(scores));
                return;
            }

            if (ContainsScore(scores, score))
            {
                return;
            }

            if (!IsHighScore(score))
            {
                return;
            }

            scores.Add(new PlayerScore(name, score));
            SaveScores(NormalizeScores(scores));
        }

        private static List<PlayerScore> NormalizeScores(List<PlayerScore> scores)
        {
            scores.Sort(CompareScores);

            var uniqueScores = new List<PlayerScore>();

            foreach (var score in scores)
            {
                if (ContainsScore(uniqueScores, score.Score))
                {
                    continue;
                }

                uniqueScores.Add(score);

                if (uniqueScores.Count == 10)
                {
                    break;
                }
            }

            return uniqueScores;
        }

        private static int CompareScores(PlayerScore first, PlayerScore second)
        {
            if (first.Score != second.Score)
            {
                return second.Score.CompareTo(first.Score);
            }

            return string.Compare(first.Name, second.Name, StringComparison.OrdinalIgnoreCase);
        }

        private static bool ContainsScore(List<PlayerScore> scores, int score)
        {
            foreach (var item in scores)
            {
                if (item.Score == score)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsScoreExceptPlayer(List<PlayerScore> scores, int score, int playerIndex)
        {
            for (var i = 0; i < scores.Count; i++)
            {
                if (i == playerIndex)
                {
                    continue;
                }

                if (scores[i].Score == score)
                {
                    return true;
                }
            }

            return false;
        }

        private static int FindPlayerIndex(List<PlayerScore> scores, string name)
        {
            for (var i = 0; i < scores.Count; i++)
            {
                if (scores[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        private static void SaveScores(List<PlayerScore> scores)
        {
            var lines = new List<string>();

            foreach (var score in scores)
            {
                lines.Add(score.Name + "|" + score.Score);
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}