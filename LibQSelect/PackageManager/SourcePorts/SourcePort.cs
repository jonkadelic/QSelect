﻿using LibPackageManager.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LibQSelect.PackageManager.SourcePorts
{
    public class SourcePort : IRepositoryItem, INotifyPropertyChanged
    {
        #region Enums
        public enum OperatingSystem
        {
            Unknown,
            Win32,
            Win64,
            Linux32,
            Linux64,
            MacOS
        }
        #endregion

        #region Properties
        /// <summary>
        /// The source port's unique identifier.
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// The name of the source port.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The author(s) of the source port.
        /// </summary>
        public string Author { get; }
        /// <summary>
        /// The path of the executable relative to the base folder of the source port.
        /// </summary>
        public string Executable { get; }
        /// <summary>
        /// The URL at which the source port can be downloaded.
        /// </summary>
        public string DownloadUrl { get; }
        /// <summary>
        /// The OS the source port runs on.
        /// </summary>
        public OperatingSystem SupportedOS { get; }

        /// <summary>
        /// True if source port has been downloaded, false otherwise.
        /// </summary>
        public bool IsDownloaded => InstallPath != null;


        /// <summary>
        /// Directory in which the source port is installed.
        /// </summary>
        private string installPath = null; public string InstallPath
        {
            get => installPath;
            set
            {
                installPath = value;
                PropertyChanged?.Invoke(this, new(nameof(InstallPath)));
                PropertyChanged?.Invoke(this, new(nameof(IsDownloaded)));
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        public SourcePort(string id, string name = null, string author = null, string executable = null, string downloadUrl = null, OperatingSystem os = OperatingSystem.Unknown)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name;
            Author = author;
            Executable = executable;
            DownloadUrl = downloadUrl;
            SupportedOS = os;
        }
        #endregion
    }
}
