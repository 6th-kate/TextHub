using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace TextHub
{
    /// <summary>
    /// Represents the standard project made by TextHub app
    /// </summary>
    class TextHubProject
    {
        // The standard name of the directory where all the versions are contained
        private const string versionsDirectoryName = "versions";
        // The standard name of the directory where all the subprojects are contained
        private const string subversionsDirectoryName = "subversions";

        /// <summary>
        /// The full path to the main project folder
        /// </summary>
        private string projectFolderPath;
        /// <summary>
        /// The list of versions of the project
        /// </summary>
        public ObservableCollection<TextHubVersion> Versions { get; set; }
        /// <summary>
        /// The title of the project
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Initializes the starting properties of project
        /// </summary>
        /// <param name="name">New project name</param>
        /// <param name="folderPath">The full path to the main project folder</param>
        /// <param name="currentVersionPath">The relative path to the main (and at the same time current) version of the project</param>
        private TextHubProject(string name, string folderPath, string currentVersionPath)
        {
            Title = name;
            Versions = new ObservableCollection<TextHubVersion>();
            Versions.Add(new TextHubVersion(currentVersionPath, 0, true, this));
            projectFolderPath = folderPath;
        }
        /// <summary>
        /// Initializes the starting properties of project. No versions are initialized
        /// </summary>
        /// <param name="name">New project name</param>
        /// <param name="folderPath">The full path to the main project folder</param>
        private TextHubProject(string name, string folderPath)
        {
            Title = name;
            Versions = new ObservableCollection<TextHubVersion>();
            projectFolderPath = folderPath;
        }

        /// <summary>
        /// Creates new instance of a project
        /// </summary>
        /// <param name="newFolderPath">The full path to the new (and still absent) project folder</param>
        /// <returns>New project with the chosen name and a single (and at the same time current) version, which is void</returns>
        public static TextHubProject MakeNewProject(string newFolderPath)
        {
            string title = GetName(newFolderPath);
            if (title.Equals(OpeningDialogViewModel.NewFileStandardPlaceHolder))
            {
                throw new ArgumentException("Необходимо выбрать имя для папки проекта");
            }
            if (NotAllowed(title) || newFolderPath.EndsWith(Path.DirectorySeparatorChar))
            {
                throw new ArgumentException("Недопустимое имя проекта");
            }
            if (Directory.Exists(newFolderPath))
            {
                throw new IOException("Директория с таким именем уже существует");
            }
            Directory.CreateDirectory(newFolderPath);
            File.Create(newFolderPath + Path.DirectorySeparatorChar + title + ".rtf").Close();
            File.Create(GetPointerFullPath(newFolderPath)).Close();
            File.WriteAllLines(GetPointerFullPath(newFolderPath), new string[] { title + ".rtf" });
            File.SetAttributes(GetPointerFullPath(newFolderPath), File.GetAttributes(GetPointerFullPath(newFolderPath)) | FileAttributes.Hidden);
            Directory.CreateDirectory(GetVersionsDirectoryFullPath(newFolderPath));
            Directory.CreateDirectory(GetSubversionsDirectoryFullPath(newFolderPath));
            return new TextHubProject(title, newFolderPath, title + ".rtf");
        }

        /// <summary>
        /// Parses an .rtf file into a new project.
        /// </summary>
        /// <param name="filePath">The full path to the file to be parsed</param>
        /// <returns>A new project with a single (and at the same time current) version, which is a cope of the chosen file</returns>
        public static TextHubProject ParseFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("Файл не существует");
            }
            if (!filePath.EndsWith(".rtf"))
            {
                throw new ArgumentException("Файл должен быть в формате .rtf");
            }
            string shortPath = GetName(filePath);
            string title = shortPath.Split('.')[0];
            string parentDirectory = GetDirectory(filePath);
            string folderPath = parentDirectory + Path.DirectorySeparatorChar + title;
            if (Directory.Exists(folderPath))
            {
                throw new IOException("Проект не может быть создан, так как директория с таким именем уже существует");
            }
            Directory.CreateDirectory(folderPath);
            File.Copy(filePath, folderPath + Path.DirectorySeparatorChar + shortPath);
            File.Create(GetPointerFullPath(folderPath)).Close();
            File.WriteAllLines(GetPointerFullPath(folderPath), new string[] { shortPath });
            File.SetAttributes(GetPointerFullPath(folderPath), File.GetAttributes(GetPointerFullPath(folderPath)) | FileAttributes.Hidden);
            Directory.CreateDirectory(GetVersionsDirectoryFullPath(folderPath));
            Directory.CreateDirectory(GetSubversionsDirectoryFullPath(folderPath));
            return new TextHubProject(title, folderPath, shortPath);
        }

        /// <summary>
        /// Parses a project with a standart TextHub project structure from the folder to a ready-to-work model
        /// </summary>
        /// <param name="folderPath">The path to the folder where the project is placed</param>
        /// <returns>The model of a project with all the versions parced into TextHub versions</returns>
        public static TextHubProject ParseProject(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new ArgumentException("Папка не существует");
            }
            string projectName = GetName(folderPath);
            string pointerFullPath = GetPointerFullPath(folderPath);
            // If the project folder does not contain the version pointer file
            if (!File.Exists(pointerFullPath))
            {
                return NoPointerToProject(folderPath);
            }
            if (!Directory.Exists(GetVersionsDirectoryFullPath(folderPath)))
            {
                Directory.CreateDirectory(GetVersionsDirectoryFullPath(folderPath));
            }
            if (!Directory.Exists(GetSubversionsDirectoryFullPath(folderPath)))
            {
                Directory.CreateDirectory(GetSubversionsDirectoryFullPath(folderPath));
            }
            TextHubProject project = new TextHubProject(projectName, folderPath);
            try
            {
                // Parces the version list from the pointer file 
                project.ParseVersionsFromFile(pointerFullPath);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return project;
        }

        /// <summary>
        /// Parses a project with a standart TextHub project structure but without the version poiner file from the folder to a ready-to-work model
        /// </summary>
        /// <param name="folderPath">The path to the folder where the project is placed</param>
        /// <returns>The model of a project with all the versions parced into TextHub versions</returns>
        private static TextHubProject NoPointerToProject(string folderPath)
        {
            string projectName = GetName(folderPath);
            string pointerFullPath = GetPointerFullPath(folderPath);
            File.Create(pointerFullPath).Close();
            string[] probableMainFiles = Directory.GetFiles(folderPath, "*.rtf");
            if (!Directory.Exists(GetVersionsDirectoryFullPath(folderPath)))
            {
                Directory.CreateDirectory(GetVersionsDirectoryFullPath(folderPath));
            }
            if (!Directory.Exists(GetSubversionsDirectoryFullPath(folderPath)))
            {
                Directory.CreateDirectory(GetSubversionsDirectoryFullPath(folderPath));
            }
            if (probableMainFiles.Length == 0 && Directory.GetFiles(GetVersionsDirectoryFullPath(folderPath)).Length == 0)
            {
                File.Create(GetVersionsDirectoryFullPath(folderPath) + Path.DirectorySeparatorChar + projectName + "ver0.rtf");
            }
            // Moves all the .rtf files from the main directory into the version directory to sort them out
            foreach (string probablyMainFile in probableMainFiles)
            {
                string newFilePath = folderPath + Path.DirectorySeparatorChar +
                    versionsDirectoryName + Path.DirectorySeparatorChar +
                    GetName(probablyMainFile);
                if (File.Exists(newFilePath))
                {
                    int counter = 0;
                    newFilePath = folderPath + Path.DirectorySeparatorChar +
                        versionsDirectoryName + Path.DirectorySeparatorChar +
                        GetFileTitle(probablyMainFile) + counter + ".rtf";
                    while (File.Exists(newFilePath))
                    {
                        ++counter;
                        newFilePath = folderPath + Path.DirectorySeparatorChar +
                            versionsDirectoryName + Path.DirectorySeparatorChar +
                            GetFileTitle(probablyMainFile) + counter + ".rtf";
                    }
                }
                File.Move(probablyMainFile, newFilePath);
            }
            // Fills the pointer file with versions' paths
            FillPointer(pointerFullPath, GetVersionsDirectoryFullPath(folderPath));
            TextHubProject project = new TextHubProject(projectName, folderPath);
            // Parces the version list from the pointer file 
            project.ParseVersionsFromFile(pointerFullPath);
            File.SetAttributes(pointerFullPath, File.GetAttributes(pointerFullPath) | FileAttributes.Hidden);
            return project;
        }

        /// <summary>
        /// Fills the pointer file with versions' paths by sorting them
        /// </summary>
        /// <param name="pointerPath">The full path to the version pointer file</param>
        /// <param name="versionsdirectoryFullPath">The full path to the directory where all the versions are contained</param>
        private static void FillPointer(string pointerPath, string versionsdirectoryFullPath)
        {
            string[] fileNames = Directory.GetFiles(versionsdirectoryFullPath, "*.rtf");
            var files = Array.ConvertAll(fileNames, x => new FileInfo(x));
            Array.Sort(files, (x, y) => ((x.CreationTime - y.CreationTime).TotalMinutes > 0 ? 1 : -1));
            fileNames = Array.ConvertAll(files, x => versionsDirectoryName + Path.DirectorySeparatorChar + x.Name);
            File.SetAttributes(pointerPath, File.GetAttributes(pointerPath) & ~FileAttributes.Hidden);
            File.WriteAllLines(pointerPath, fileNames);
            File.SetAttributes(pointerPath, File.GetAttributes(pointerPath) | FileAttributes.Hidden);
        }

        /// <summary>
        ///  Parces the version list from the pointer file 
        /// </summary>
        /// <param name="pointerPath">The full path to the version pointer file</param>
        private void ParseVersionsFromFile(string pointerPath)
        {
            // Reads all file paths from the pointer
            File.SetAttributes(pointerPath, File.GetAttributes(pointerPath) & ~FileAttributes.Hidden);
            List<string> versionPaths = new List<string>(File.ReadAllLines(pointerPath));
            File.SetAttributes(pointerPath, File.GetAttributes(pointerPath) | FileAttributes.Hidden);
            // An array to indicate which versions are not present
            bool[] versionsExisting = new bool[versionPaths.Count];
            for (int i = 0; i < versionPaths.Count; ++i)
            {
                versionsExisting[i] = true;
            }

            for (int i = 0; i < versionPaths.Count; ++i)
            {
                if (!File.Exists(projectFolderPath + Path.DirectorySeparatorChar + versionPaths[i]))
                {
                    versionsExisting[i] = false;
                }
            }
            // Removes versions which are not present from the paths list
            for (int i = versionPaths.Count - 1; i >= 0; --i)
            {
                if (!versionsExisting[i])
                {
                    versionPaths.RemoveAt(i);
                }
            }
            // If there are no versions present, returns a message
            if (versionPaths.Count == 0)
            {
                throw new ArgumentException("Нет ни одной существующей версии, указанной в файле-указателе");
            }
            // Foreach version present adds it to the project versions list
            for (int i = 0; i < versionPaths.Count - 1; ++i)
            {
                Versions.Add(new TextHubVersion(versionPaths[i], i, false, this));
            }
            // If the last version is not the main folder, moves it
            if (!versionPaths[versionPaths.Count - 1].Equals(GetName(versionPaths[versionPaths.Count - 1])))
            {
                string newFilePath = projectFolderPath + Path.DirectorySeparatorChar + GetName(versionPaths[versionPaths.Count - 1]);
                string newShortPath = GetName(versionPaths[versionPaths.Count - 1]);
                if (File.Exists(newFilePath))
                {
                    int counter = 0;
                    newFilePath = projectFolderPath + Path.DirectorySeparatorChar +
                        GetFileTitle(versionPaths[versionPaths.Count - 1]) + counter + ".rtf";
                    newShortPath = GetFileTitle(versionPaths[versionPaths.Count - 1]) + counter + ".rtf";
                    while (File.Exists(newFilePath))
                    {
                        ++counter;
                        newFilePath = projectFolderPath + Path.DirectorySeparatorChar +
                        GetFileTitle(versionPaths[versionPaths.Count - 1]) + counter + ".rtf";
                        newShortPath = GetFileTitle(versionPaths[versionPaths.Count - 1]) + counter + ".rtf";
                    }
                }
                File.Move(projectFolderPath + Path.DirectorySeparatorChar + versionPaths[versionPaths.Count - 1], newFilePath);
                versionPaths[versionPaths.Count - 1] = newShortPath;
            }
            // Adds the last version to the project
            Versions.Add(new TextHubVersion(versionPaths[versionPaths.Count - 1], versionPaths.Count - 1, true, this));
            // Refills the pointer vith versions' paths
            File.SetAttributes(pointerPath, File.GetAttributes(pointerPath) & ~FileAttributes.Hidden);
            File.WriteAllLines(pointerPath, versionPaths);
            File.SetAttributes(pointerPath, File.GetAttributes(pointerPath) | FileAttributes.Hidden);
        }

        /// <summary>
        /// Adds a new version to the project chain of versions
        /// </summary>
        /// <param name="newVersionName">The title of the new created version</param>
        public void SaveNewVersion(string newVersionName)
        {
            if (newVersionName.Equals(ChooseNameViewModel.NamePlaceHolder))
            {
                throw new ArgumentException("Необходимо выбрать имя версии");
            }
            if (NotAllowed(newVersionName))
            {
                throw new ArgumentException("Некорректное имя версии.");
            }
            TextHubVersion lastVersion = Versions[Versions.Count - 1];
            string newVersionNamePath = versionsDirectoryName + Path.DirectorySeparatorChar + newVersionName + ".rtf";
            if (File.Exists(projectFolderPath + Path.DirectorySeparatorChar + newVersionNamePath))
            {
                throw new ArgumentException("Версия с таким именем уже существует");
            }
            string pointerPath = GetPointerFullPath(projectFolderPath);
            if (!File.Exists(pointerPath))
            {
                throw new ArgumentException("Структура проекта была нарушена, рекомендуем закрыть и заново открыть проект для восстановления");
            }
            File.Copy(projectFolderPath + Path.DirectorySeparatorChar + lastVersion.RelativeFilePath,
                projectFolderPath + Path.DirectorySeparatorChar + newVersionNamePath);
            FileInfo currentVersion = new FileInfo(projectFolderPath + Path.DirectorySeparatorChar + lastVersion.RelativeFilePath);
            currentVersion.CreationTime = DateTime.Now;
            Versions.Add(new TextHubVersion(newVersionNamePath, lastVersion.Number, false, this));
            Versions.Add(new TextHubVersion(lastVersion.RelativeFilePath, lastVersion.Number + 1, true, this));
            Versions.Remove(lastVersion);
            File.SetAttributes(pointerPath, File.GetAttributes(pointerPath) & ~FileAttributes.Hidden);
            List<string> lines = new List<string>(File.ReadAllLines(pointerPath));
            lines.RemoveAt(lines.Count - 1);
            lines.Add(newVersionNamePath);
            lines.Add(lastVersion.RelativeFilePath);
            File.WriteAllLines(pointerPath, lines);
            File.SetAttributes(pointerPath, File.GetAttributes(pointerPath) | FileAttributes.Hidden);
        }

        /// <summary>
        /// Checks if the name of the file is prohibited
        /// </summary>
        /// <param name="filename">The short name to check</param>
        /// <returns>True, if the name of the file is prohibited tu use, false otherwise</returns>
        private static bool NotAllowed(string filename)
        {
            return filename.Contains(@"\") || filename.Contains(@"/") || filename.Contains(":") ||
                filename.Contains("*") || filename.Contains("?") || filename.Contains('"') ||
                filename.Contains("<") || filename.Contains(">") || filename.Contains("|") ||
                filename.Contains("+") || filename.EndsWith(' ') || filename.EndsWith('.') ||
                filename.Contains("!") || filename.Contains("%") || filename.Contains("@");
        }

        /// <summary>
        /// Creates a new subproject and adds it to the project chain of subversions
        /// </summary>
        /// <param name="textHubVersion">The version on which the new subproject is based</param>
        /// <param name="newName">New subproject name</param>
        /// <returns>A new TextHubProject instance aka the model of the newly created subproject</returns>
        public TextHubProject MakeSubproject(TextHubVersion textHubVersion, string newName)
        {
            if (newName.Equals(ChooseNameViewModel.NamePlaceHolder))
            {
                throw new ArgumentException("Необходимо выбрать имя версии");
            }
            if (NotAllowed(newName))
            {
                throw new ArgumentException("Некорректное имя подпроекта");
            }
            string newPath = GetSubversionsDirectoryFullPath(projectFolderPath) + Path.DirectorySeparatorChar + newName;
            if (Directory.Exists(newPath))
            {
                throw new ArgumentException("Подпроект с таким именем уже существует");
            }
            if (!Directory.Exists(GetSubversionsDirectoryFullPath(projectFolderPath)))
            {
                Directory.CreateDirectory(GetSubversionsDirectoryFullPath(projectFolderPath));
            }
            if (!File.Exists(projectFolderPath + Path.DirectorySeparatorChar + textHubVersion.RelativeFilePath))
            {
                throw new ArgumentException("Что-то случилось с версией! Рекомендуем закрыть и заново открыть проект для восстановления структуры");
            }

            Directory.CreateDirectory(newPath);
            TextHubProject project = new TextHubProject(newName, newPath);
            Directory.CreateDirectory(GetVersionsDirectoryFullPath(newPath));
            Directory.CreateDirectory(GetSubversionsDirectoryFullPath(newPath));
            // Copies the full version chain before the base version
            List<string> versionPaths = new List<string>();
            int counter = 0;
            for (int i = 0; i < Versions.Count && Versions[i] != textHubVersion; ++i)
            {
                if (File.Exists(projectFolderPath + Path.DirectorySeparatorChar + Versions[i].RelativeFilePath))
                {
                    File.Copy(projectFolderPath + Path.DirectorySeparatorChar + Versions[i].RelativeFilePath,
                        newPath + Path.DirectorySeparatorChar + versionsDirectoryName + Path.DirectorySeparatorChar + Versions[i].Title + ".rtf");
                    versionPaths.Add(versionsDirectoryName + Path.DirectorySeparatorChar + Versions[i].Title + ".rtf");
                    project.Versions.Add(new TextHubVersion(versionsDirectoryName + Path.DirectorySeparatorChar + Versions[i].Title + ".rtf", counter++, false, project));
                }
            }

            File.Copy(projectFolderPath + Path.DirectorySeparatorChar + textHubVersion.RelativeFilePath,
                newPath + Path.DirectorySeparatorChar + newName + ".rtf");
            versionPaths.Add(newName + ".rtf");
            project.Versions.Add(new TextHubVersion(newName + ".rtf", counter, true, project));

            File.Create(GetPointerFullPath(newPath)).Close();
            File.WriteAllLines(GetPointerFullPath(newPath), versionPaths);
            File.SetAttributes(GetPointerFullPath(newPath), File.GetAttributes(GetPointerFullPath(newPath)) | FileAttributes.Hidden);
            return project;
        }

        /// <summary>
        /// Deletez the chosen version from the project
        /// </summary>
        /// <param name="textHubVersion">The version to delete</param>
        public void DeleteVersion(TextHubVersion textHubVersion)
        {
            string pointer = GetPointerFullPath(projectFolderPath);
            if (!File.Exists(pointer))
            {
                throw new ArgumentException("Структура проекта была нарушена. Попробуйте закрыть проект и открыть заново");
            }
            File.SetAttributes(pointer, File.GetAttributes(pointer) & ~FileAttributes.Hidden);
            List<string> paths = new List<string>(File.ReadAllLines(pointer));
            bool notFound = true;
            int counter = 0;
            while (notFound)
            {
                if (paths[counter].Equals(textHubVersion.RelativeFilePath))
                {
                    File.Delete(projectFolderPath + Path.DirectorySeparatorChar + textHubVersion.RelativeFilePath);
                    notFound = false;
                    paths.Remove(paths[counter]);
                }
                ++counter;
            }
            File.WriteAllLines(pointer, paths);
            File.SetAttributes(pointer, File.GetAttributes(pointer) | FileAttributes.Hidden);
            Versions.Remove(textHubVersion);
            if (textHubVersion.Changeable)
            {
                Versions[Versions.Count - 1].Changeable = true;
            }
        }

        /// <summary>
        /// Saves the text to the current version of the project
        /// </summary>
        /// <param name="text">The text to save</param>
        public void Save(string text)
        {
            File.WriteAllText(projectFolderPath + Path.DirectorySeparatorChar + Versions[Versions.Count - 1].RelativeFilePath, text);
        }

        /// <summary>
        /// Gets the text of the chosen version
        /// </summary>
        /// <param name="version">The chosen version</param>
        /// <returns>The text of the version. If the version cannot be opened, throws an Exception</returns>
        public string GetText(TextHubVersion version)
        {
            if (!File.Exists(projectFolderPath + Path.DirectorySeparatorChar + version.RelativeFilePath))
            {
                throw new ArgumentException("Файл не обнаружен. Попробуйте закрыть и открыть проект заново");
            }
            return File.ReadAllText(projectFolderPath + Path.DirectorySeparatorChar + version.RelativeFilePath);
        }

        /// <summary>
        /// Gets the short name of the folder
        /// </summary>
        /// <param name="folderPath">Full folder path</param>
        /// <returns>The short name of the folder</returns>
        private static string GetName(string folderPath)
        {
            string[] path = folderPath.Split(Path.DirectorySeparatorChar);
            return path[path.Length - 1];
        }

        /// <summary>
        /// Gets the title of the file
        /// </summary>
        /// <param name="folderPath">Full file path</param>
        /// <returns>The title of the file</returns>
        private static string GetFileTitle(string folderPath)
        {
            string[] path = folderPath.Split(Path.DirectorySeparatorChar);
            return path[path.Length - 1].Split('.')[0];
        }

        /// <summary>
        /// Gets the parent directory of a folder
        /// </summary>
        /// <param name="folderPath">Full folder path</param>
        /// <returns>The parent directory of a folder</returns>
        private static string GetDirectory(string folderPath)
        {
            string[] path = folderPath.Split(Path.DirectorySeparatorChar);
            string folder = path[0];
            for (int i = 1; i < path.Length - 1; ++i)
            {
                folder += Path.DirectorySeparatorChar + path[i];
            }
            return folder;
        }

        /// <summary>
        /// Gets the full path of the directory of TextHub versions corresponding to the chosen folder
        /// </summary>
        /// <param name="folderPath">The full folder path</param>
        /// <returns>The full path of the directory of TextHub versions corresponding to the chosen folder</returns>
        private static string GetVersionsDirectoryFullPath(string folderPath)
        {
            return folderPath + Path.DirectorySeparatorChar + versionsDirectoryName;
        }

        /// <summary>
        /// Gets the full path of the directory of TextHub subversions corresponding to the chosen folder
        /// </summary>
        /// <param name="folderPath">The full folder path</param>
        /// <returns>The full path of the directory of TextHub subversions corresponding to the chosen folder</returns>
        private static string GetSubversionsDirectoryFullPath(string folderPath)
        {
            return folderPath + Path.DirectorySeparatorChar + subversionsDirectoryName;
        }

        /// <summary>
        /// Gets the full path of the versions pointer corresponding to the chosen folder
        /// </summary>
        /// <param name="folderPath">The full folder path</param>
        /// <returns>The full path of the versions pointer corresponding to the chosen folder</returns>
        private static string GetPointerFullPath(string folderPath)
        {
            return folderPath + Path.DirectorySeparatorChar + GetPointerName(GetName(folderPath));
        }

        /// <summary>
        /// Gets the specific versions pointer name corresponding to the chosen project
        /// </summary>
        /// <param name="projectName">Paroject title</param>
        /// <returns>The specific versions pointer name corresponding to the chosen project</returns>
        private static string GetPointerName(string projectName)
        {
            return "VersionsPointer" + projectName + "Project.txt";
        }
    }
}
