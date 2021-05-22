using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class DecreaseEnergySystem : JobComponentSystem
{
    float time = 0;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        time += Time.DeltaTime;

        bool increase;
        if (time > 0.3f)
        {
            time = 0;
            increase = true;
        }
        else increase = false;

        JobHandle handle = Entities.ForEach((ref AIEnergyData hungryData) =>
        {
            if (increase)
            {
                hungryData.Value -= 1;
            }
        }).Schedule(inputDeps);

        return handle;
    }
}
