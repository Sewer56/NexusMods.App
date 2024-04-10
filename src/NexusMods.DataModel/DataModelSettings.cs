﻿using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using NexusMods.Abstractions.Settings;
using NexusMods.Paths;

namespace NexusMods.DataModel;

/// <summary>
/// Settings for the data model.
/// </summary>
[PublicAPI]
public record DataModelSettings : ISettings
{
    /// <summary>
    /// If true, data model will be stored in memory only and the paths will be ignored.
    /// </summary>
    public bool UseInMemoryDataModel { get; set; }

    /// <summary>
    /// Path of the file which contains the backing data store or database.
    /// </summary>
    public ConfigurablePath DataStoreFilePath { get; set; }

    /// <summary>
    /// Path of the file which contains the backing data store or database
    /// used for inter process communication.
    /// </summary>
    public ConfigurablePath IpcDataStoreFilePath { get; set; }

    /// <summary>
    /// Preconfigured locations [full paths] where mod archives can/will be stored.
    /// </summary>
    public ConfigurablePath[] ArchiveLocations { get; set; } = [];

    /// <summary>
    /// Maximum number of simultaneous hashing jobs.
    /// Each job basically corresponds to CPU core.
    /// </summary>
    public int MaxHashingJobs { get; set; } = Environment.ProcessorCount;

    /// <summary>
    /// Maximum number of simultaneous loadout deployment jobs.
    /// Each job basically corresponds to CPU core.
    /// </summary>
    public int LoadoutDeploymentJobs { get; set; } = Environment.ProcessorCount;

    /// <summary>
    /// Maximum throughput for the hashing operations in bytes per second.
    /// Value of 0 represents no cap.
    /// </summary>
    public long MaxHashingThroughputBytesPerSecond { get; set; }

    /// <inheritdoc/>
    public static ISettingsBuilder Configure(ISettingsBuilder settingsBuilder)
    {
        // TODO: consider adding some properties to the UI?
        return settingsBuilder.ConfigureDefault(CreateDefault);
    }

    /// <summary>
    /// Create default value.
    /// </summary>
    public static DataModelSettings CreateDefault(IServiceProvider serviceProvider)
    {
        var os = serviceProvider.GetRequiredService<IOSInformation>();

        var baseKnownPath = os.MatchPlatform(
            onWindows: () => KnownPath.LocalApplicationDataDirectory,
            onLinux: () => KnownPath.XDG_DATA_HOME,
            onOSX: () => KnownPath.LocalApplicationDataDirectory
        );

        // NOTE: OSX ".App" is apparently special, using _ instead of . to prevent weirdness
        var baseDirectoryName = os.IsOSX ? "NexusMods_App/DataModel" : "NexusMods.App/DataModel";

        return new DataModelSettings
        {
            DataStoreFilePath = new ConfigurablePath(baseKnownPath, $"{baseDirectoryName}/DataModel.sqlite"),
            IpcDataStoreFilePath = new ConfigurablePath(baseKnownPath, $"{baseDirectoryName}/DataModel_IPC.sqlite"),
            ArchiveLocations = [
                new ConfigurablePath(baseKnownPath, $"{baseDirectoryName}/Archives"),
            ],
        };
    }
}
