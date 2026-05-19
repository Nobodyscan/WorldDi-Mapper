using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Aegis;

[RegisterComponent] // Убрали NetworkedComponent, теперь ошибки CS0246 исчезнут
public sealed partial class AegisWeaponCaseComponent : Component
{
    [DataField] public string BulldogPrototype = "WeaponShotgunBulldog";
    [DataField] public string BulldogMagPrototype = "MagazineShotgun";

    [DataField] public string C20rPrototype = "WeaponSubMachineGunC20r";
    [DataField] public string C20rMagPrototype = "MagazinePistolSubMachineGun";
}

[Serializable, NetSerializable]
public enum AegisWeaponCaseUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class AegisWeaponCaseSelectedMessage : BoundUserInterfaceMessage
{
    public string SelectedWeapon { get; }

    public AegisWeaponCaseSelectedMessage(string selectedWeapon)
    {
        SelectedWeapon = selectedWeapon;
    }
}
