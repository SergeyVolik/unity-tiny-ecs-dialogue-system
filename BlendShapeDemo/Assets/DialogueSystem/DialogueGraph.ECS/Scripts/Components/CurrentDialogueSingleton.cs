using Unity.Entities;

[GenerateAuthoringComponent]
public struct CurrentDialogueSingleton : IComponentData
{
    public Entity Entity;
    public int DialogueNodeIndex;
    public bool DialogueExist;
}

