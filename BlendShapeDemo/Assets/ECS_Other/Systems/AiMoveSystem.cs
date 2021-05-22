using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public class AiMoveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;


        return Entities.ForEach((ref Translation translation, in AiDestination dest, in SpeedData speed) =>
        {
            var direction = dest.Position - translation.Value;
            translation.Value += math.normalize(direction) * speed.speed * deltaTime;
        }).Schedule(inputDeps);
    }
}
