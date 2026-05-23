using Content.Shared.Aegis;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using System;

namespace Content.Client.WorldDi.Gun_case;

[UsedImplicitly]
public sealed class AegisWeaponCaseBoundUserInterface : BoundUserInterface
{
    private AegisWeaponCaseWindow? _window;

    public AegisWeaponCaseBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey) {}

    protected override void Open()
    {
        base.Open();
        _window = new AegisWeaponCaseWindow();
        
        _window.OnClose += Close;
        
        _window.OnWeaponSelected += weapon =>
        {
            SendMessage(new AegisWeaponCaseSelectedMessage(weapon));
            _window.Close();
        };
        _window.OpenCentered();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _window?.Dispose();
        }
    }
}
