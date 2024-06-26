﻿using System.Windows.Input;

namespace NexusMods.App.UI.Overlays.Login;

public class NexusLoginOverlayDesignerViewModel : AOverlayViewModel<INexusLoginOverlayViewModel>, INexusLoginOverlayViewModel
{
    public ICommand Cancel { get; } = Initializers.ICommand;
    public Uri Uri { get; } = new("https://www.nexusmods.com/some/login?name=John&key=1234567890");
}
