using Unity.Entities;

[GenerateAuthoringComponent]
public struct DialogueContinue : IComponentData
{
    public int SelectedNextNodeIndex;
}

