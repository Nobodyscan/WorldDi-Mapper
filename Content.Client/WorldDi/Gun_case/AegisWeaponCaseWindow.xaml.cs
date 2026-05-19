using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using System;

namespace Content.Client.WorldDi.Gun_case;

public sealed partial class AegisWeaponCaseWindow : DefaultWindow
{
    public event Action<string>? OnWeaponSelected;

    public AegisWeaponCaseWindow()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        var bulldogButton = FindControl<Button>("BulldogButton");
        var c20rButton = FindControl<Button>("C20rButton");

        bulldogButton.OnPressed += _ => OnWeaponSelected?.Invoke("Bulldog");
        c20rButton.OnPressed += _ => OnWeaponSelected?.Invoke("C20r");
    }
}
