using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextHub
{
    /// <summary>
    /// Represents a version of the project made by TextHub app
    /// </summary>
    class TextHubVersion
    {
        /// <summary>
        /// Path to the version, relative to the main folder of the project
        /// </summary>
        public string RelativeFilePath { get; set; }
        /// <summary>
        /// Indicates, if the version is prohibited to edit or not
        /// </summary>
        public bool Changeable { get; set; }
        /// <summary>
        /// Nuber of the version in the list of versions of the project
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// The title (name) of the version
        /// </summary>
        public string Title { get; }
        /// <summary>
        /// The project to which the version is attached
        /// </summary>
        public TextHubProject Project { get; }
        /// <summary>
        /// Initialises version's properties
        /// </summary>
        /// <param name="path">Path to the version, relative to the main folder of the project</param>
        /// <param name="number">Nuber of the version in the list of versions of the project</param>
        /// <param name="changeable">Indicates, if the version is prohibited to edit or not</param>
        /// <param name="project">The project to which the version is being attached</param>
        public TextHubVersion(string path, int number, bool changeable, TextHubProject project)
        {
            RelativeFilePath = path;
            Number = number;
            Changeable = changeable;
            Title = GetName(path);
            Project = project;
        }
        /// <summary>
        /// Gets the title of the version from its relative path
        /// </summary>
        /// <param name="filePath">The relative path of the version</param>
        /// <returns></returns>
        private static string GetName(string filePath)
        {
            string[] path = filePath.Split(Path.DirectorySeparatorChar);
            return path[path.Length - 1].Split('.')[0];
        }
    }
}
