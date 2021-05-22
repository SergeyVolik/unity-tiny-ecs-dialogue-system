
//using SV.DialogueGraph.Authoring;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Unity.Entities;
//using Unity.Jobs;

//public class DialogueNextStepSystem : JobComponentSystem
//{
//    private DOTSEvents_NextFrame<EventComponent> dotsEvents;
//    EndSimulationEntityCommandBufferSystem entityCommandBufferSystem;
//    EntityManager manager;
//    protected override void OnCreate()
//    {
//        dotsEvents = new DOTSEvents_NextFrame<EventComponent>(World);
//        entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//        manager = World.EntityManager;
//    }

//    public delegate void DialogueShowReplic(EventComponent eventData);

//    public event DialogueShowReplic OnEventTrigger;
//    public struct EventComponent : IComponentData { public Entity from; public int dialogueNode; }
//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        DOTSEvents_NextFrame<EventComponent>.EventTrigger eventTrigger = dotsEvents.GetEventTrigger();
//        var buffer = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

//        JobHandle jobHandle = Entities.ForEach((int entityInQueryIndex, Entity e, ref DialogueChoice testEventComp, in Dialogue dialogue) => {

//            buffer.RemoveComponent<DialogueChoice>(entityInQueryIndex, e);
//            eventTrigger.TriggerEvent(entityInQueryIndex, new EventComponent { from = e, dialogueNode = testEventComp.Value });
//        }).Schedule(inputDeps);

//        entityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

//        dotsEvents.CaptureEvents(jobHandle, (EventComponent basicEvent) => {

//            OnEventTrigger?.Invoke(basicEvent);

           
           
//        });
//        return inputDeps;
//    }
//}

