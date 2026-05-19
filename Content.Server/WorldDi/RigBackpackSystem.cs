using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Content.Shared.WorldDi.Rig;
using Content.Server.Popups;
using Content.Shared.Actions;
using Robust.Shared.GameObjects;

namespace Content.Server.WorldDi.Rig;

public sealed class RigBackpackSystem : EntitySystem
{
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;

    public override void Initialize()
    {
        base.Initialize();
        
        SubscribeLocalEvent<RigBackpackComponent, ToggleRigActionEvent>(OnToggleRig);
        SubscribeLocalEvent<RigBackpackComponent, GotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<RigBackpackComponent, GotUnequippedEvent>(OnUnequipped);
    }

    private void OnEquipped(EntityUid uid, RigBackpackComponent component, GotEquippedEvent args)
    {
        if (args.Slot == "back")
        {
            _actions.AddAction(args.Equipee, ref component.ActionEntity, component.ToggleAction, uid);
        }
    }

    private void OnUnequipped(EntityUid uid, RigBackpackComponent component, GotUnequippedEvent args)
    {
        if (component.SpawnedSuit != null)
        {
            // Передаем владельца рюкзака (args.Equipee), чтобы система знала у кого снимать скафандр
            RetractRig(uid, component, args.Equipee);
        }

        if (component.ActionEntity != null)
        {
            _actions.RemoveAction(args.Equipee, component.ActionEntity);
        }
    }

    private void OnToggleRig(EntityUid uid, RigBackpackComponent component, ToggleRigActionEvent args)
    {
        args.Handled = true;
        var user = args.Performer;

        if (component.SpawnedSuit != null)
        {
            RetractRig(uid, component, user);
            return;
        }

        if (!_inventory.TryGetContainingSlot(uid, out var slot) || slot.Name != "back")
        {
            _popup.PopupEntity("РИГ должен быть надет на спину!", user, user);
            return;
        }

        if (_inventory.TryGetSlotEntity(user, "outerClothing", out _))
        {
            _popup.PopupEntity("Сначала снимите верхнюю одежду!", user, user);
            return;
        }

        var suit = Spawn(component.SuitPrototype, Transform(user).Coordinates);
        
        if (_inventory.TryEquip(user, suit, "outerClothing", force: true))
        {
            component.SpawnedSuit = suit;
        }
        else
        {
            QueueDel(suit);
        }
    }

    // Добавили аргумент user, чтобы корректно взаимодействовать с его инвентарем
    private void RetractRig(EntityUid uid, RigBackpackComponent component, EntityUid user)
    {
        if (component.SpawnedSuit != null)
        {
            // КРИТИЧЕСКИЙ ФИКС: Сначала принудительно вытаскиваем скафандр из слота.
            // Это заставит движок чисто убрать дебафф скорости с игрока.
            _inventory.TryUnequip(user, "outerClothing", force: true);

            // Теперь, когда скафандра на игроке физически нет, его можно спокойно удалять
            QueueDel(component.SpawnedSuit.Value);
            component.SpawnedSuit = null;
        }
    }
}