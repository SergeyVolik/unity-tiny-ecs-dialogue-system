using SV.DialogueGraph.Authoring.Tiny;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny.Text;
using Unity.Tiny.UI;
using Unity.Transforms;

public class DialogueButtonClickEvent : SystemBase
{
    EndSimulationEntityCommandBufferSystem ecbSys;
    protected override void OnCreate()
    {
        ecbSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var dialogueEntity = GetSingletonEntity<CurrentDialogueSingleton>();
        var ecsBuffer = ecbSys.CreateCommandBuffer();
        if (TryGetSingleton<CurrentDialogueSingleton>(out var dialogueSingleton))
        {
            Entities.ForEach((ref UIState state, ref PlayerDialogueChoiceButton choice) =>
            {
                if (state.IsClicked && dialogueSingleton.DialogueExist)
                {
                    
                    ecsBuffer.AddComponent<DialogueContinue>(dialogueEntity);
                    ecsBuffer.SetComponent(dialogueEntity, new DialogueContinue() { SelectedNextNodeIndex = choice.DialogueNodeIndex });

                }
            }).Run();
        }
        ecsBuffer.Playback(EntityManager);

    }
}
