﻿using NexusMods.DataModel.JsonConverters;
using NexusMods.Hashing.xxHash64;
using NexusMods.Interfaces;
using NexusMods.Paths;

namespace NexusMods.DataModel.ModLists.ModFiles;

[JsonName("NexusMods.DataModel.ModFiles.GameFile")]
public record GameFile : AStaticModFile
{
    public required GameInstallation Installation { get; init; }
}