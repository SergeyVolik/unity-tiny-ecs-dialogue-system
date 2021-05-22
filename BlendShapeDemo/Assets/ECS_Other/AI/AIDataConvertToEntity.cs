using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
//using Unity.Animation.Hybrid;
//using Unity.Animation;


[AddComponentMenu("DOTS/Utility AI/AIDataHealth")]
public class AIDataConvertToEntity : MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEngine.AnimationCurve SleepyCurveBehaviour;
    public float SleepyData;

    public UnityEngine.AnimationCurve HungryCurveBehaviour;
    public float HungryData;

    public UnityEngine.AnimationCurve EnergyCurveBehaviour;
    public float EnergyData;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //dstManager.AddComponentData(entity, new AIHungryData
        //{
        //    BehaviourCurve = conversionSystem.BlobAssetStore.GetAnimationCurve(HungryCurveBehaviour),
        //    Value = HungryData
        //});

        //dstManager.AddComponentData(entity, new AISleppyData
        //{
        //    BehaviourCurve = conversionSystem.BlobAssetStore.GetAnimationCurve(SleepyCurveBehaviour),
        //    Value = SleepyData
        //});

        //dstManager.AddComponentData(entity, new AIEnergyData
        //{
        //    BehaviourCurve = conversionSystem.BlobAssetStore.GetAnimationCurve(EnergyCurveBehaviour),
        //    Value = EnergyData
        //});
    }
}

//public class AnimationCurveBlobTranslationSystem : SystemBase
//{
//    protected override void OnUpdate()
//    {
//        var time = Mathf.Repeat((float)Time.ElapsedTime, 1.0F);
//        Dependency = Entities.ForEach((ref Translation position, in AnimateTranslation translation) =>
//        {
//            float value = AnimationCurveEvaluator.Evaluate(time, translation.TranslationCurve);
//            position.Value.y = value;
//        }).Schedule(Dependency);
//    }
//}
