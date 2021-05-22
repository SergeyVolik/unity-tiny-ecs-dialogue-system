using SV.DialogueGraph.Authoring.Tiny;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny.Text;
using Unity.Tiny.UI;
using Unity.Transforms;

public class DialogueNextPart : SystemBase
{
    ProcessUIEvents uiSys;
    protected override void OnCreate()
    {

        uiSys = World.GetExistingSystem<ProcessUIEvents>();
        base.OnCreate();
    }


    //unity editor ui elements names
    const string initDialogueButton1 = "InitDialogueButton1";
    const string initDialogueButton2 = "InitDialogueButton2";

    const string choiceButton1 = "ChoiceTextButton1";
    const string choiceButton2 = "ChoiceTextButton2";
    const string choiceButton3 = "ChoiceTextButton3";
    const string choiceButton4 = "ChoiceTextButton4";
    const string choiceButton5 = "ChoiceTextButton5";

    const string choiceButtonText1 = "ChoiceText1";
    const string choiceButtonText2 = "ChoiceText2";
    const string choiceButtonText3 = "ChoiceText3";
    const string choiceButtonText4 = "ChoiceText4";
    const string choiceButtonText5 = "ChoiceText5";

    const string npcReplic = "NPC_TEXT ";


    protected override void OnUpdate()
    {
        if(TryGetSingleton<CurrentDialogueSingleton>(out var dialogueSingleton))
        {
            Entity dialogueEntity = Entity.Null;
            if (TryGetSingletonEntity<DialogueInit>(out dialogueEntity))
            {
                var dialogueGraphComp = GetComponent<Dialogue>(dialogueSingleton.Entity);
                ref var dialogueGraph = ref dialogueGraphComp.data.Value;
                ref var dialogueNode = ref dialogueGraph.Nodes[dialogueGraph.entryPointIndex];

                var textEntity = uiSys.GetEntityByUIName("NPC_TEXT");
                TextLayout.SetEntityTextRendererString(EntityManager, textEntity, dialogueNode.Text.ToString());

                ref var list = ref dialogueNode.Links;
                NativeArray<int> indices = new NativeArray<int>(list.Length, Allocator.Temp);
                NativeArray<FixedString128> texts = new NativeArray<FixedString128>(list.Length, Allocator.Temp);
                var i = 0;

                for (i = 0; i < indices.Length; i++)
                {
                    indices[i] = list[i].nodeIndex;
                    ref var str = ref list[i].Text;
                    texts[i] = new FixedString128(str.ToString());
                }


                i = 0;
                Entities.ForEach((ref PlayerDialogueChoiceButton choiceButton, ref RectTransform trans) => {

                    if (indices.Length > i)
                    {
                        choiceButton.DialogueNodeIndex = indices[i];
                        trans.Hidden = false;
                    }
                    else trans.Hidden = true;


                    i++;
                }).Run();
                i = 0;

                var entityManager = EntityManager;
                Entities.WithAll<PlayerChoiceText>().ForEach((Entity e) => {
                    if (texts.Length > i)
                    {
                        TextLayout.SetEntityTextRendererString(entityManager, e, texts[i].ToString());
                        //choiceButton.DialogueNodeIndex = indices[i];
                    }
                    i++;
                }).WithoutBurst().Run();

                indices.Dispose();
                texts.Dispose();
                EntityManager.RemoveComponent<DialogueInit>(dialogueEntity);
            }
            else if(TryGetSingletonEntity<DialogueContinue>(out dialogueEntity))
            {
                
            }
        }
      
        
            //UnityEngine.Debug.Log("DialogueUI updated");
        //    var buttonEntity = uiSys.GetEntityByUIName(initDialogueButton1);
        //var buttonState = GetComponent<UIState>(buttonEntity);
        //if (buttonState.IsClicked)
        //{

        //    //var dialogueSingleton = GetSingleton<CurrentDialogueSingleton>();
        //    if (TryGetSingleton<CurrentDialogueSingleton>(out var dialogueSingleton))
        //    {
        //        if (!dialogueSingleton.DialogueExist)
        //        {
        //            UnityEngine.Debug.Log("Dialogue 1 started");
        //            dialogueSingleton.DialogueExist = true;
        //            EntityManager.AddComponent<DialogueContinue>(buttonEntity);

        //            dialogueSingleton.Entity = buttonEntity;
        //            SetSingleton<CurrentDialogueSingleton>(dialogueSingleton);
        //        }
        //        else UnityEngine.Debug.LogWarning("Dialogue continue");
        //    }
        //    else
        //    {

        //        UnityEngine.Debug.LogError("CurrentDialogueSingleton not exist");
        //    }

        //}

    }
}
