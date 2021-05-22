using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class IncreaseSleepySystem : JobComponentSystem
{
    float time = 0;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        time += Time.DeltaTime;

        bool increase;
        if (time > 1)
        {
            time = 0;
            increase = true;
        }
        else increase = false;

        JobHandle handle = Entities.ForEach((ref AISleppyData hungryData) =>
        {
            if (increase)
            {
                hungryData.Value += 1;
            }
        }).Schedule(inputDeps);

        return handle;
    }
}
