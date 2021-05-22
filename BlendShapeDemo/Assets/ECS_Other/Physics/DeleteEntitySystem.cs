using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;

[AlwaysSynchronizeSystem]
[UpdateAfter(typeof(PickupSystem))]
public class DeleteEntitySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .WithAll<DeleteTag>()
            //.WithoutBurst()
            .ForEach((Entity entity) =>
            {
                //GameManager.instance.IncreaseScore();
                commandBuffer.DestroyEntity(entity);
            }).Run();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();

        return inputDeps;
    }
}
