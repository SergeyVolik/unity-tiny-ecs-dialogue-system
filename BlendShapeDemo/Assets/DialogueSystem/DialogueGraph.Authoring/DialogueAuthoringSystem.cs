using Unity.Entities;
using SV.DialogueGraph.Runtime;
using SV.DialogueGraph.Authoring.Tiny;

namespace SV.DialogueGraph.Authoring
{


    [WorldSystemFilter(WorldSystemFilterFlags.DotsRuntimeGameObjectConversion)]
    public class DialogueAuthoringSystem : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
           
            Entities.ForEach((DialogueAuthoring dialogueAuth) =>
            {
                //if (dialogueAuth.dialogue == null)
                //    throw new ArgumentException($"No dialogue found in the DialogueAuthoringSystem authoring component of the gameobject: {dialogueAuth.name}. Please assign one");

                var entity = GetPrimaryEntity(dialogueAuth);
                ////DstEntityManager.AddComponentData(entity, new Tiny.Rendering.AutoMovingDirectionalLight()
                ////{
                ////    autoBounds = dialogueAuth.autoBounds,
                ////    clipToCamera = entityCamera
                ////});
                ////BlobAssetStore

                DstEntityManager.AddComponentData(entity, new Dialogue
                {
                    data = BlobAssetStore.GetDialogue(dialogueAuth.dialogue),
                    replicIndex = 0
                });

                //DeclareDependency(dialogueAuth.gameObject, dialogueAuth.mainCamera);
            });
        }
    }

    //public class DialogueAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    //{
    //    public DialogueConteinerSO dialogue;

    //    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    //    {


    //    }
    //}
}