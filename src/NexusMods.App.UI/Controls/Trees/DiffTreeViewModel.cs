using System.Collections.ObjectModel;
using Avalonia.Controls;
using DynamicData;
using NexusMods.Abstractions.GameLocators;
using NexusMods.Abstractions.Games.Trees;
using NexusMods.Abstractions.Loadouts;
using NexusMods.App.UI.Controls.Trees.Files;
using NexusMods.App.UI.Helpers.TreeDataGrid;
using NexusMods.Paths.Trees.Traits;

namespace NexusMods.App.UI.Controls.Trees;

public class DiffTreeViewModel : AViewModel<IFileTreeViewModel>, IFileTreeViewModel
{
    private LoadoutId _loadoutId;
    private IApplyService _applyService;
    private ILoadoutRegistry _loadoutRegistry;
    private readonly SourceCache<IFileTreeNodeViewModel, GamePath> _treeSourceCache;
    private ReadOnlyObservableCollection<IFileTreeNodeViewModel> _items;
    
    private uint _loadoutFileCount;
    private ulong _loadoutFileSize;
    private uint _diskFileCount;
    private ulong _diskFileSize;
    private uint _addedFileCount;
    private uint _modifiedFileCount;
    private uint _deletedFileCount;
    private ulong _operationSize;
    
    private readonly ReadOnlyObservableCollection<string> _statusBarStrings;
    private SourceList<string> _statusBarSourceList;
    
    public ITreeDataGridSource<IFileTreeNodeViewModel> TreeSource { get; }
    public ReadOnlyObservableCollection<string> StatusBarStrings => _statusBarStrings;
    
    
    public DiffTreeViewModel(LoadoutId loadoutId, IApplyService applyService, ILoadoutRegistry loadoutRegistry)
    {
        _loadoutId = loadoutId;
        _applyService = applyService;
        _loadoutRegistry = loadoutRegistry;
        
        _treeSourceCache = new SourceCache<IFileTreeNodeViewModel, GamePath>(_ => throw new NotImplementedException());
        _statusBarSourceList = new SourceList<string>();
        
        _treeSourceCache.Connect()
            .TransformToTree(model => model.ParentKey)
            .Transform(node => node.Item.Initialize(node))
            .Bind(out _items)
            .Subscribe();
        
        _statusBarSourceList.Connect()
            .Bind(out _statusBarStrings)
            .Subscribe();

        TreeSource = ModFileTreeViewModel.CreateTreeSource(_items);
    }

    private async void Refresh()
    {
        var loadout = _loadoutRegistry.Get(_loadoutId);
        if (loadout is null)
        {
            throw new KeyNotFoundException($"Loadout with ID {_loadoutId} not found.");
        }
        var locationsRegister = loadout.Installation.LocationsRegister;

        var diffTree = await _applyService.GetApplyDiffTree(_loadoutId);
        
        List<IFileTreeNodeViewModel> fileViewModelNodes = [];
        
        // Convert the diff tree to a list of FileTreeNodeViewModels
        foreach (var diffEntry in diffTree.GetAllDescendents())
        {
            var fullPath = diffEntry.GamePath();
            var parentPath = diffEntry.Parent()?.GamePath() ?? IFileTreeViewModel.RootParentGamePath;
            var isFile = diffEntry.IsFile();
            var fileSize = diffEntry.IsFile() 
                ? diffEntry.Item.Value.Size.Value 
                : diffEntry.GetFiles()
                    .Aggregate(0ul,  (sum,file) =>
                        {
                            return sum += file.Item.Value.Size.Value;
                        }
                    );
            var numChildFiles = diffEntry.IsFile() ? 0 : diffEntry.CountFiles();
            var model = new FileTreeNodeViewModel(fullPath, parentPath, isFile, fileSize, (uint)numChildFiles);
            fileViewModelNodes.Add(model);
        }
        
        _treeSourceCache.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddOrUpdate(fileViewModelNodes);
        });
        
    }

    
}
