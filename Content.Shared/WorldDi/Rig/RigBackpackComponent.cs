using Robust.Shared.Prototypes;
using Content.Shared.Actions;

namespace Content.Shared.WorldDi.Rig;

// Убрали атрибуты! Они здесь больше не нужны и только крашат сервер.
public sealed partial class ToggleRigActionEvent : InstantActionEvent { }

[RegisterComponent]
public sealed partial class RigBackpackComponent : Component
{
    [DataField("suitPrototype", required: true)]
    public EntProtoId SuitPrototype;

    [DataField("toggleAction")]
    public EntProtoId ToggleAction = "ActionToggleAegisRig";

    [ViewVariables]
    public EntityUid? ActionEntity;

    [ViewVariables]
    public EntityUid? SpawnedSuit;
}