using SV.DialogueGraph.Authoring.Tiny;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny.Text;
using Unity.Tiny.UI;
using Unity.Transforms;

[ConverterVersion("DialogueNextPart", 18)]
public class DialogueNextPart : SystemBase
{
    ProcessUIEvents uiSys;
    protected override void OnCreate()
    {

        uiSys = World.GetExistingSystem<ProcessUIEvents>();
        base.OnCreate();
    }


    private bool NextStep(CurrentDialogueSingleton dialogueSingleton, int newDialogueNodeIndex)
    {

        var dialogueGraphComp = GetComponent<Dialogue>(dialogueSingleton.Entity);
        ref var dialogueGraph = ref dialogueGraphComp.data.Value;
        ref var dialogueNode = ref dialogueGraph.Nodes[newDialogueNodeIndex];
        ref var npcText = ref dialogueNode.Text;
        var textEntity = uiSys.GetEntityByUIName("NpcText");

        TextLayout.SetEntityTextRendererString(EntityManager, textEntity, npcText.ToString());

        ref var list = ref dialogueNode.Links;
        NativeArray<int> indices = new NativeArray<int>(list.Length, Allocator.Temp);
        NativeArray<FixedString4096> texts = new NativeArray<FixedString4096>(list.Length, Allocator.Temp);
        var i = 0;

        for (i = 0; i < indices.Length; i++)
        {
            indices[i] = list[i].nodeIndex;
            ref var str = ref list[i].Text;
            texts[i] = new FixedString4096(str.ToString());
        }

        UnityEngine.Debug.Log(indices.Length);
        i = 0;
        Entities.ForEach((ref PlayerDialogueChoiceButton choiceButton, ref RectTransform trans) =>
        {

            if (indices.Length > i)
            {
                choiceButton.DialogueNodeIndex = indices[i];
                trans.Hidden = false;
            }
            else trans.Hidden = true;


            i++;
        }).WithStructuralChanges().Run();


        var ChoiceTextButton1 = uiSys.GetEntityByUIName("ChoiceTextButton1");
        var ChoiceTextButton2 = uiSys.GetEntityByUIName("ChoiceTextButton2");
        var ChoiceTextButton3 = uiSys.GetEntityByUIName("ChoiceTextButton3");
        var ChoiceTextButton4 = uiSys.GetEntityByUIName("ChoiceTextButton4");
        var ChoiceTextButton5 = uiSys.GetEntityByUIName("ChoiceTextButton5");

        var textEntity1 = uiSys.GetEntityByUIName("ChoiceText1");
        var textEntity2 = uiSys.GetEntityByUIName("ChoiceText2");
        var textEntity3 = uiSys.GetEntityByUIName("ChoiceText3");
        var textEntity4 = uiSys.GetEntityByUIName("ChoiceText4");
        var textEntity5 = uiSys.GetEntityByUIName("ChoiceText5");

      

        UpdateButtonUI(ChoiceTextButton1, textEntity1, 0, texts, indices);
        UpdateButtonUI(ChoiceTextButton2, textEntity2, 1, texts, indices);
        UpdateButtonUI(ChoiceTextButton3, textEntity3, 2, texts, indices);
        UpdateButtonUI(ChoiceTextButton4, textEntity4, 3, texts, indices);
        UpdateButtonUI(ChoiceTextButton5, textEntity5, 4, texts, indices);

        //if (indices.Length >= 2)
        //{
        //    var textEntity1 = uiSys.GetEntityByUIName("ChoiceText2");
        //    TextLayout.SetEntityTextRendererString(EntityManager, textEntity1, texts[1].ToString());
        //}
        //if (indices.Length >= 3)
        //{
        //    var textEntity1 = uiSys.GetEntityByUIName("ChoiceText3");
        //    TextLayout.SetEntityTextRendererString(EntityManager, textEntity1, texts[2].ToString());
        //}
        //if (indices.Length >= 4)
        //{
        //    var textEntity1 = uiSys.GetEntityByUIName("ChoiceText4");
        //    TextLayout.SetEntityTextRendererString(EntityManager, textEntity1, texts[3].ToString());
        //}
        //if (indices.Length >= 5)
        //{
        //    var textEntity1 = uiSys.GetEntityByUIName("ChoiceText5");
        //    TextLayout.SetEntityTextRendererString(EntityManager, textEntity1, texts[4].ToString());
        //}


        indices.Dispose();
        texts.Dispose();

        if (list.Length == 0)
            return false;

        return true;
    }

    void UpdateButtonUI(Entity choiceTextButton1, Entity textEntity1, int buttonIndex, NativeArray<FixedString4096> str, NativeArray<int> indices)
    {
        var rect = GetComponent<RectTransform>(choiceTextButton1);
        if (str.Length >= buttonIndex+1)
        {

            TextLayout.SetEntityTextRendererString(EntityManager, textEntity1, str[buttonIndex].ToString());

            var choiceButton = GetComponent<PlayerDialogueChoiceButton>(choiceTextButton1);
            choiceButton.DialogueNodeIndex = indices[buttonIndex];
            rect.Hidden = false;
            SetComponent<PlayerDialogueChoiceButton>(choiceTextButton1, choiceButton);
        }
        else
        {
            rect.Hidden = true;

        }

        SetComponent<RectTransform>(choiceTextButton1, rect);
    }
    protected override void OnUpdate()
    {
        //UnityEngine.Debug.Log("Dialogue Inited");
        if (TryGetSingleton<CurrentDialogueSingleton>(out var dialogueSingleton))
        {
            Entity dialogueEntity = Entity.Null;
            if (TryGetSingletonEntity<DialogueInit>(out dialogueEntity))
            {
                var dialogueGraphComp = GetComponent<Dialogue>(dialogueSingleton.Entity);
                ref var dialogueGraph = ref dialogueGraphComp.data.Value;
                UnityEngine.Debug.Log("Dialogue Inited");
                if (!NextStep(dialogueSingleton, dialogueGraph.entryPointIndex))
                {
                    dialogueSingleton.DialogueExist = false;
                   
                }
                else dialogueSingleton.DialogueExist = true;
                SetSingleton<CurrentDialogueSingleton>(dialogueSingleton);
                EntityManager.RemoveComponent<DialogueInit>(dialogueEntity);
            }
            else if(TryGetSingletonEntity<DialogueContinue>(out dialogueEntity))
            {
                var continueData = GetSingleton<DialogueContinue>();
                if (!NextStep(dialogueSingleton, continueData.SelectedNextNodeIndex))
                {
                    dialogueSingleton.DialogueExist = false;

                }
                else dialogueSingleton.DialogueExist = true;

                SetSingleton<CurrentDialogueSingleton>(dialogueSingleton);
                EntityManager.RemoveComponent<DialogueContinue>(dialogueEntity);
                UnityEngine.Debug.Log("Dialogue continue");
            }
        }
      
    }
}
