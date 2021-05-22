using System.Collections;
using System.Collections.Generic;
//using Unity.Animation;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

public class CalcUtilityAiScoreSystem : ComponentSystem
{
    //EndSimulationEntityCommandBufferSystem EndSimulationEntityCommandBufferSystem;
    //EntityQuery m_EnergyQuery;
    //EntityQuery m_FoodQuery;
    //EntityQuery m_SleepyQuery;
    //protected override void OnCreate()
    //{
    //    EndSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

    //}

    protected override void OnUpdate()
    {
        ////EntityCommandBuffer.ParallelWriter cmdBuffer = EndSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        //Entities.WithNone<AiDestination>().ForEach((Entity e, ref AIEnergyData energy, ref AISleppyData sleepy, ref AIHungryData hungry, ref Translation trans) => {
        //    float score1 = AnimationCurveEvaluator.Evaluate(energy.Value, energy.BehaviourCurve);
        //    float score2 = AnimationCurveEvaluator.Evaluate(sleepy.Value, sleepy.BehaviourCurve);
        //    float score3 = AnimationCurveEvaluator.Evaluate(hungry.Value, hungry.BehaviourCurve);

        //    UnityEngine.Debug.Log(score1);
        //    UnityEngine.Debug.Log(score2);
        //    UnityEngine.Debug.Log(score3);
        //    var max = math.max(score1, math.max(score2, score3));

        //    float3 target = float3.zero;
        //    float currMindist = float.MaxValue;

        //    if (max == score1)
        //    {
        //        float3 pos = trans.Value;
        //        Entities.WithAll<Energy_Tag>().ForEach((ref Translation trans2) =>
        //        {
        //            UnityEngine.Debug.Log("Energy_Tag");
        //            float dist = math.distance(pos, trans2.Value);
        //            if (dist < currMindist)
        //            {
        //                currMindist = dist;
        //                target = trans2.Value;
        //                UnityEngine.Debug.Log(target);
        //            }
        //        });
        //    }
        //    else if (score2 == max)
        //    {
        //        float3 pos = trans.Value;
        //        Entities.WithAll<AISleepy_Tag>().ForEach((ref Translation trans2) => {
        //            float dist = math.distance(pos, trans2.Value);
        //            UnityEngine.Debug.Log("AISleepy_Tag");
        //            if (dist < currMindist)
        //            {
        //                currMindist = dist;
        //                target = trans2.Value;
        //                UnityEngine.Debug.Log(target);
        //            }
        //        });
        //    }
        //    else
        //    {
        //        float3 pos = trans.Value;
        //        Entities.WithAll<FoodComponent_Tag>().ForEach((ref Translation trans2) => {
        //            float dist = math.distance(pos, trans2.Value);
        //            UnityEngine.Debug.Log("FoodComponent_Tag");
        //            if (dist < currMindist)
        //            {
        //                currMindist = dist;
        //                target = trans2.Value;
        //                UnityEngine.Debug.Log(target);
        //            }
        //        });
        //    }

        //    UnityEngine.Debug.Log(target);
        //    EntityManager.AddComponent(e, typeof(AiDestination));
        //    EntityManager.SetComponentData(e, new AiDestination() { Position = target });
        //});
    }
}
