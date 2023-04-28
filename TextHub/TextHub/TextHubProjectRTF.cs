using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace TextHub
{
    /// <summary>
    /// Represents the standard project made by TextHub app
    /// </summary>
    class TextHubProjectRTF : TextHubProject
    {
       
        /// <summary>
        /// Initializes the starting properties of project
        /// </summary>
        /// <param name="name">New project name</param>
        /// <param name="folderPath">The full path to the main project folder</param>
        /// <param name="currentVersionPath">The relative path to the main (and at the same time current) version of the project</param>
        public TextHubProjectRTF(string name, string folderPath, string currentVersionPath)
        {
            Title = name;
            Versions = new ObservableCollection<TextHubVersion>
            {
                new TextHubVersion(currentVersionPath, 0, true, this)
            };
            projectFolderPath = folderPath;
        }

        /// <summary>
        /// Initializes the starting properties of project. No versions are initialized
        /// </summary>
        /// <param name="name">New project name</param>
        /// <param name="folderPath">The full path to the main project folder</param>
        public TextHubProjectRTF(string name, string folderPath)
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
            string title = TextHubProject.MakeNewProject(newFolderPath, ".rtf");
            return new TextHubProjectRTF(title, newFolderPath, title + ".rtf");
        }

        /// <summary>
        /// Parses an .rtf file into a new project.
        /// </summary>
        /// <param name="filePath">The full path to the file to be parsed</param>
        /// <returns>A new project with a single (and at the same time current) version, which is a cope of the chosen file</returns>
        public new static TextHubProject ParseFile(string filePath)
        {
            string[] attrs = TextHubProject.ParseFile(filePath);
            return new TextHubProjectRTF(attrs[0], attrs[1], attrs[2]);
        }

        /// <summary>
        /// Parses a project with a standart TextHub project structure from the folder to a ready-to-work model
        /// </summary>
        /// <param name="folderPath">The path to the folder where the project is placed</param>
        /// <returns>The model of a project with all the versions parced into TextHub versions</returns>
        public new static TextHubProject ParseProject(string folderPath)
        {
            string projectName = GetName(folderPath);
            string pointerFullPath = GetPointerFullPath(folderPath);
            bool noPointer = TextHubProject.ParseProject(folderPath);
            if (!noPointer)
            {
                TextHubProject project = new TextHubProjectRTF(projectName, folderPath);
                try
                {
                    project.ParseVersionsFromFile(pointerFullPath, ".rtf");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(ex.Message);
                }
                return project;
            }
            else
            {
                TextHubProject.NoPointerToProject(folderPath, ".rtf");
                TextHubProject project = new TextHubProjectRTF(projectName, folderPath);
                project.ParseVersionsFromFile(pointerFullPath, ".rtf");
                return project;
            }
        }

        
        /// <summary>
        /// Adds a new version to the project chain of versions
        /// </summary>
        /// <param name="newVersionName">The title of the new created version</param>
        public override void SaveNewVersion(string newVersionName)
        {
            SaveNewVersion(newVersionName, ".rtf");
        }

        /// <summary>
        /// Creates a new subproject and adds it to the project chain of subversions
        /// </summary>
        /// <param name="textHubVersion">The version on which the new subproject is based</param>
        /// <param name="newName">New subproject name</param>
        /// <returns>A new TextHubProject instance aka the model of the newly created subproject</returns>
        public override TextHubProject MakeSubproject(TextHubVersion textHubVersion, string newName)
        {
            string newPath = MakeSubpojectDirs(textHubVersion, newName);
            TextHubProject project = new TextHubProjectRTF(newName, newPath);
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
    }
}
