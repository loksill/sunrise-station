using Robust.Shared.Serialization;

namespace Content.Shared._Sunrise.AI.LawSelector;

[RegisterComponent]
public sealed partial class AiLawSelectorComponent : Component
{
    /// <summary>
    /// Action that will be open law-seletor window
    /// </summary>
    [DataField("actionId")]
    public string ActionId = "ActionOpenAiLawSelector";

    [DataField("actionEntity")]
    public EntityUid? ActionEntity;
}

[Serializable, NetSerializable]
public enum AiLawSelectorUiKey : byte
{
    Key
}

/// <summary>
/// Law-set structure
/// </summary>
[Serializable, NetSerializable]
public sealed class AiLawSet
{
    public string Name { get; set; } = string.Empty;
    public List<string> Laws { get; set; } = new();
}

/// <summary>
/// A sended server to client UI state
/// </summary>
[Serializable, NetSerializable]
public sealed class AiLawSelectorBuiState : BoundUserInterfaceState
{
    public List<AiLawSet> LawSets { get; }
    public AiLawSelectorBuiState(List<AiLawSet> lawSets) => LawSets = lawSets;
}

/// <summary>
/// A message from client to server about of a selected law-set
/// </summary>
[Serializable, NetSerializable]
public sealed class AiLawSelectorSelectMessage : BoundUserInterfaceMessage
{
    public string SelectedSetName { get; }
    public AiLawSelectorSelectMessage(string selectedSetName) => SelectedSetName = selectedSetName;
}
