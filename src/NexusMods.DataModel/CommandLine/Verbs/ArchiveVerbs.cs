using Microsoft.Extensions.DependencyInjection;
using NexusMods.Abstractions.Cli;
using NexusMods.Abstractions.FileStore;
using NexusMods.Abstractions.FileStore.ArchiveMetadata;
using NexusMods.Abstractions.Games.Loadouts;
using NexusMods.Paths;
using NexusMods.ProxyConsole.Abstractions;
using NexusMods.ProxyConsole.Abstractions.VerbDefinitions;

namespace NexusMods.DataModel.CommandLine.Verbs;

internal static class ArchiveVerbs
{
    internal static IServiceCollection AddArchiveVerbs(this IServiceCollection services) =>
        services.AddVerb(() => AnalyzeArchive);

    [Verb("analyze-archive", "Analyze the given archive")]
    private static async Task<int> AnalyzeArchive([Injected] IRenderer renderer,
        [Injected] IFileOriginRegistry fileOriginRegistry,
        [Option("i", "input", "The archive to analyze")] AbsolutePath inputFile,
        [Injected] CancellationToken token)
    {
        var results = await renderer.WithProgress(token, async () =>
        {
            var downloadId = await fileOriginRegistry.RegisterDownload(inputFile,
                (tx, id) =>
                {
                    tx.Add(id, FilePathMetadata.OriginalName, inputFile.Name);
                }, token);
            var metadata = fileOriginRegistry.Get(downloadId);
            return metadata.Contents.Select(kv =>
            {
                return new object[]
                {
                    kv.Path, kv.Size, kv.Hash,
                };
            });
        });

        await renderer.Table(new[] { "Path", "Size", "Hash"},
            results.OrderBy(e => (RelativePath)e[0]));
        return 0;
    }

}
