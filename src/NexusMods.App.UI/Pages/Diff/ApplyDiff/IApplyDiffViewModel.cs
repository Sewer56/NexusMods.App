using NexusMods.Abstractions.Loadouts;
using NexusMods.App.UI.Controls.Trees;
using NexusMods.App.UI.WorkspaceSystem;

namespace NexusMods.App.UI.Pages.Diff.ApplyDiff;

public interface IApplyDiffViewModel : IPageViewModelInterface
{
    public LoadoutId LoadoutId { get; set; }
    
    public IFileTreeViewModel? FileTreeViewModel { get; }
}
