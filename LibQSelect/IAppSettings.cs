﻿using Config.Net;
using System.Collections.Generic;

namespace LibQSelect
{
    public interface IAppSettings
    {
        [Option(Alias = "Paths.BinariesDirectory", DefaultValue = "Binaries")]
        string BinariesDirectory { get; set; }

        [Option(Alias = "Paths.ModsDirectory", DefaultValue = "Mods")]
        string ModsDirectory { get; set; }

        [Option(Alias = "Paths.SyncQuakeConfig", DefaultValue = true)]
        bool SyncQuakeConfig { get; set; }
    }
}
