using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PipeMoveSystemV2 : JobComponentSystem
{
    private DOTSEvents_NextFrame<EventComponent> dotsEvents;
    protected override void OnCreate()
    {
        dotsEvents = new DOTSEvents_NextFrame<EventComponent>(World);
        //entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    public delegate void DialogueShowReplic(EventComponent eventData);

    public event DialogueShowReplic OnEventTrigger;
    public struct EventComponent : IComponentData { public Entity from;  }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        double elapsedTime = Time.ElapsedTime;

        DOTSEvents_NextFrame<EventComponent>.EventTrigger eventTrigger = dotsEvents.GetEventTrigger();

        JobHandle jobHandle = Entities.ForEach((int entityInQueryIndex, Entity e, ref PostEventComponent testEventComp) => {
            eventTrigger.TriggerEvent(entityInQueryIndex, new EventComponent {  });
        }).Schedule(inputDeps);


        dotsEvents.CaptureEvents(jobHandle, (EventComponent basicEvent) => {

            OnEventTrigger?.Invoke(basicEvent);
        });
        return inputDeps;
    }
}
