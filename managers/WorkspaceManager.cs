using IpisCentralDisplayController.Models;
using System;
using System.IO;
using Newtonsoft.Json;

namespace IpisCentralDisplayController.managers
{
    public static class WorkspaceManager
    {
        public static string GetWorkspacePath()
        {
            return Properties.Settings.Default.WorkspacePath;
        }

        public static void SetWorkspacePath(string path)
        {
            Properties.Settings.Default.WorkspacePath = path;
            Properties.Settings.Default.Save();
            EnsureWorkspaceDirectoriesExist(path);
        }

        public static bool IsValidWorkspace(string path)
        {
            return Directory.Exists(path);
        }

        public static void DeleteWorkspace()
        {
            Properties.Settings.Default.WorkspacePath = "";
            Properties.Settings.Default.Save();
        }

        public static void EnsureWorkspaceDirectoriesExist(string path)
        {
            // List of required base directories
            string[] requiredDirectories = {
        "DB",
        "Recordings",
        "Audio",
        "Sounds",
        "Reports",
        "Alerts",
        "Media",
        "Renders",
        "Backup",
        "Fonts",
        "Internal"
    };

            // Ensure base directories exist
            foreach (string dir in requiredDirectories)
            {
                string dirPath = Path.Combine(path, dir);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
            }

            // Ensure the Sounds/Stations directory structure
            string baseSoundPath = Path.Combine(path, "Sounds", "Stations");
            string[] languageFolders = new string[]
            {
        "ENGLISH", "HINDI", "ASSAMESE", "BANGLA", "DOGRI", "GUJARATI",
        "KANNADA", "KONKANI", "MALAYALAM", "MARATHI", "MANIPURI", "NEPALI",
        "ODIA", "PUNJABI", "SANSKRIT", "SINDHI", "TAMIL", "TELUGU", "URDU"
            };

            foreach (var folder in languageFolders)
            {
                string languagePath = Path.Combine(baseSoundPath, folder);
                if (!Directory.Exists(languagePath))
                {
                    Directory.CreateDirectory(languagePath);
                }
            }

            // Create the workspace metadata file if it doesn't exist
            string metadataFilePath = Path.Combine(path, "workspace.json");
            if (!File.Exists(metadataFilePath))
            {
                var metadata = new WorkspaceMetadata
                {
                    WorkspaceName = new DirectoryInfo(path).Name,
                    CreationDate = DateTime.Now,
                    Version = "1.0"
                };

                string metadataJson = JsonConvert.SerializeObject(metadata, Formatting.Indented);
                File.WriteAllText(metadataFilePath, metadataJson);
            }
        }

        //public static void EnsureWorkspaceDirectoriesExist(string path)
        //{
        //    string[] requiredDirectories = {
        //        "DB",
        //        "Recordings",
        //        "Audio",
        //        "Sounds",
        //        "Reports",
        //        "Alerts",
        //        "Media",
        //        "Renders",
        //        "Backup",
        //        "Fonts",
        //        "Internal"
        //    };

        //    foreach (string dir in requiredDirectories)
        //    {
        //        string dirPath = System.IO.Path.Combine(path, dir);
        //        if (!Directory.Exists(dirPath))
        //        {
        //            Directory.CreateDirectory(dirPath);
        //        }
        //    }

        //    // Create the workspace metadata file if it doesn't exist
        //    string metadataFilePath = Path.Combine(path, "workspace.json");
        //    if (!File.Exists(metadataFilePath))
        //    {
        //        var metadata = new WorkspaceMetadata
        //        {
        //            WorkspaceName = new DirectoryInfo(path).Name,
        //            CreationDate = DateTime.Now,
        //            Version = "1.0"
        //        };

        //        string metadataJson = JsonConvert.SerializeObject(metadata, Formatting.Indented);
        //        File.WriteAllText(metadataFilePath, metadataJson);
        //    }
        //}
    }
}
