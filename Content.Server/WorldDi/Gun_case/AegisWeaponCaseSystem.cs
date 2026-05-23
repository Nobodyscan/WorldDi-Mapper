using Content.Server.Storage.EntitySystems;
using Content.Shared.Aegis;
using Content.Shared.Inventory;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Server.Aegis;

public sealed class AegisWeaponCaseSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly StorageSystem _storage = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();
        
        // Подписываемся на событие выбора оружия в UI
        Subs.BuiEvents<AegisWeaponCaseComponent>(AegisWeaponCaseUiKey.Key, subs => {
            subs.Event<AegisWeaponCaseSelectedMessage>(OnWeaponSelected);
        });
    }

    private void OnWeaponSelected(EntityUid uid, AegisWeaponCaseComponent component, AegisWeaponCaseSelectedMessage args)
    {
        var player = args.Actor;

        // Определяем, какие прототипы спавнить
        string weaponProto;
        string magProto;

        if (args.SelectedWeapon == "Bulldog")
        {
            weaponProto = component.BulldogPrototype;
            magProto = component.BulldogMagPrototype;
        }
        else if (args.SelectedWeapon == "C20r")
        {
            weaponProto = component.C20rPrototype;
            magProto = component.C20rMagPrototype;
        }
        else return;

        if (!_prototype.HasIndex<EntityPrototype>(weaponProto) ||
            !_prototype.HasIndex<EntityPrototype>(magProto))
        {
            return;
        }

        var coords = Transform(player).Coordinates;

        // Ищем рюкзак у игрока в слоте "back"
        EntityUid? backpackUid = null;
        if (_inventory.TryGetSlotEntity(player, "back", out var backItem))
        {
            // Проверяем, можно ли в этот предмет складывать вещи
            if (HasComp<StorageComponent>(backItem))
                backpackUid = backItem;
        }

        // Спавним и пытаемся упаковать пушку
        var weapon = Spawn(weaponProto, coords);
        TryPutInBackpackOrDrop(player, backpackUid, weapon);

        // Спавним и пытаемся упаковать 4 магазина
        for (int i = 0; i < 4; i++)
        {
            var mag = Spawn(magProto, coords);
            TryPutInBackpackOrDrop(player, backpackUid, mag);
        }

        // Удаляем использованный кейс из рук/мира
        QueueDel(uid);
    }

    private void TryPutInBackpackOrDrop(EntityUid player, EntityUid? backpack, EntityUid item)
    {
        if (backpack != null && _storage.Insert(backpack.Value, item, out _, user: player, playSound: false))
        {
            return;
        }

        _transform.DropNextTo(item, player);
    }
}
