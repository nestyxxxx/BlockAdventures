using System.IO;
using System.Windows.Forms;
using WMPLib;

namespace BlockAdventures
{
    public static class MusicManager
    {
        private static WindowsMediaPlayer player = new WindowsMediaPlayer();
        private static string currentPath = "";

        public static void Play(string fileName, int volume)
        {
            var fullPath = FindMusicPath(fileName);

            if (fullPath == "")
            {
                MessageBox.Show("Файл музыки не найден");
                return;
            }

            if (currentPath != fullPath)
            {
                currentPath = fullPath;
                player.URL = fullPath;
                player.settings.setMode("loop", true);
            }

            SetVolume(volume);
            player.controls.play();
        }

        private static string FindMusicPath(string fileName)
        {
            var path1 = Path.Combine(Application.StartupPath, fileName);
            if (File.Exists(path1))
            {
                return path1;
            }

            var path2 = Path.Combine(Application.StartupPath, "Resources", fileName);
            if (File.Exists(path2))
            {
                return path2;
            }

            var path3 = Path.Combine(Application.StartupPath, @"..\..\Resources", fileName);
            path3 = Path.GetFullPath(path3);

            if (File.Exists(path3))
            {
                return path3;
            }

            return "";
        }

        public static void SetVolume(int volume)
        {
            if (volume < 0)
            {
                volume = 0;
            }

            if (volume > 100)
            {
                volume = 100;
            }

            player.settings.volume = volume;
        }

        public static void Stop()
        {
            player.controls.stop();
        }
    }
}