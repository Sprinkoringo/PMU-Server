namespace Server.IO
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class IO
    {
        #region Methods

        public static void Initialize(string startFolder)
        {
            Paths.Initialize(startFolder);
        }

        public static bool DirectoryExists(string dirPath) {
            return System.IO.Directory.Exists(ProcessPath(dirPath));
        }

        public static void CreateDirectory(string dirPath) {
            if (DirectoryExists(dirPath) == false) {
                System.IO.Directory.CreateDirectory(ProcessPath(dirPath));
            }
        }

        public static bool FileExists(string filePath) {
            return System.IO.File.Exists(ProcessPath(filePath));
        }

        public static string ProcessPath(string path) {
            return path;
        }

        #endregion Methods
    }
}