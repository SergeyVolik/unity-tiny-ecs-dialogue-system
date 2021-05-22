using Unity.Entities;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Jobs;

public class PickupSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem bufferSystem;
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;

    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        TriggerJob triggerJob = new TriggerJob
        {
            speedEntities = GetComponentDataFromEntity<SpeedData>(),
            entitiesToDelete = GetComponentDataFromEntity<DeleteTag>(),
            entitiesToAiDestination = GetComponentDataFromEntity<AiDestination>(),
            entitiesEnergy = GetComponentDataFromEntity<Energy_Tag>(),
            entitiesFood = GetComponentDataFromEntity<FoodComponent_Tag>(),
            entitiesSleepy = GetComponentDataFromEntity<AISleepy_Tag>(),
            entitiesEnergyData = GetComponentDataFromEntity<AIEnergyData>(),
            entitiesSleepyData = GetComponentDataFromEntity<AISleppyData>(),
            entitiesHungryData = GetComponentDataFromEntity<AIHungryData>(),
            commandBuffer = bufferSystem.CreateCommandBuffer()
        };

        JobHandle jobhandle = triggerJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

        bufferSystem.AddJobHandleForProducer(jobhandle);

        return jobhandle;
    }

    private struct TriggerJob : ITriggerEventsJob
    {
        public ComponentDataFromEntity<SpeedData> speedEntities;
        public ComponentDataFromEntity<DeleteTag> entitiesToDelete;
        public ComponentDataFromEntity<AiDestination> entitiesToAiDestination;
        public ComponentDataFromEntity<FoodComponent_Tag> entitiesFood;
        public ComponentDataFromEntity<AISleepy_Tag> entitiesSleepy;
        public ComponentDataFromEntity<Energy_Tag> entitiesEnergy;
        public ComponentDataFromEntity<AIEnergyData> entitiesEnergyData;
        public ComponentDataFromEntity<AISleppyData> entitiesSleepyData;
        public ComponentDataFromEntity<AIHungryData> entitiesHungryData;
        public EntityCommandBuffer commandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            TestEntityTrigger(triggerEvent.EntityA, triggerEvent.BodyIndexA, triggerEvent.EntityB, triggerEvent.BodyIndexB);
            TestEntityTrigger(triggerEvent.EntityB, triggerEvent.BodyIndexB, triggerEvent.EntityA, triggerEvent.BodyIndexA);
        }

        private void TestEntityTrigger(Entity entity1, int index1, Entity entity2, int index2)
        {
            if (speedEntities.HasComponent(entity1))
            {
                if (entitiesToDelete.HasComponent(entity2)) { return; }
                commandBuffer.AddComponent(entity2, new DeleteTag());
                if (entitiesToAiDestination.HasComponent(entity1))
                    commandBuffer.RemoveComponent<AiDestination>(entity1);

                if (entitiesFood.HasComponent(entity2))
                {
                    var hungryData = entitiesHungryData[entity1];
                    hungryData.Value -= 10;

                    if (hungryData.Value < 0)
                        hungryData.Value = 0;
                    commandBuffer.SetComponent(entity1, hungryData);
                }
                else if (entitiesSleepy.HasComponent(entity2))
                {
                    var sleepyData = entitiesSleepyData[entity1];
                    sleepyData.Value -= 100;

                    if (sleepyData.Value < 0)
                        sleepyData.Value = 0;
                    commandBuffer.SetComponent(entity1, sleepyData);
                }
                else if (entitiesEnergy.HasComponent(entity2))
                {
                    var energyData = entitiesEnergyData[entity1];
                    energyData.Value += 50;

                    if (energyData.Value > 100)
                        energyData.Value = 100;
                    commandBuffer.SetComponent(entity1, energyData);
                }
            }
        }
    }
}
