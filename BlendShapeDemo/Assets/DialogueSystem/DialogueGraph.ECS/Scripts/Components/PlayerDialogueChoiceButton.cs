using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerDialogueChoiceButton : IComponentData
{
    public int DialogueNodeIndex;
}

