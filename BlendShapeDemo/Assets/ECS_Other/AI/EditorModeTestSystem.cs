
using Unity.Entities;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.Editor)]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class EditorModeTestSystem : SystemBase
{
    bool wasMouseDown = false;
    protected override void OnUpdate()
    {
        
    }
}

