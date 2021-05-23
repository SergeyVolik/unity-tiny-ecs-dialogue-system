using SV.DialogueGraph.Authoring.Tiny;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny.Text;
using Unity.Tiny.UI;
using Unity.Transforms;
[ConverterVersion("DialogueButtonClickEvent", 2)]
public class DialogueButtonClickEvent : SystemBase
{
    protected override void OnCreate()
    {

        base.OnCreate();
    }

    protected override void OnUpdate()
    {
      
        if (TryGetSingleton<CurrentDialogueSingleton>(out var dialogueSingleton))
        {
            var dialogueEntity = GetSingletonEntity<CurrentDialogueSingleton>();
            var ecsBuffer = new EntityCommandBuffer(Allocator.Temp);

            Entities.ForEach((ref UIState state, ref PlayerDialogueChoiceButton choice) =>
            {
                if (state.IsClicked && dialogueSingleton.DialogueExist)
                {
                    ecsBuffer.AddComponent<DialogueContinue>(dialogueEntity, new DialogueContinue() { SelectedNextNodeIndex = choice.DialogueNodeIndex });
                    //ecsBuffer.SetComponent(dialogueEntity, new DialogueContinue() { SelectedNextNodeIndex = choice.DialogueNodeIndex });

                }
            }).WithStructuralChanges().Run();

            ecsBuffer.Playback(EntityManager);
            ecsBuffer.Dispose();
        }
      

    }
}
