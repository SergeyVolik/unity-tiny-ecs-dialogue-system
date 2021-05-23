using SV.DialogueGraph.Authoring.Tiny;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny.Text;
using Unity.Tiny.UI;
using Unity.Transforms;

public class DialogueUI : SystemBase
{
    ProcessUIEvents uiSys;
    protected override void OnCreate()
    {

        uiSys = World.GetExistingSystem<ProcessUIEvents>();
        base.OnCreate();

        UnityEngine.Debug.Log("DialogueUI created");
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
        //UnityEngine.Debug.Log("DialogueUI updated");
        var buttonEntity = uiSys.GetEntityByUIName(initDialogueButton1);
        var buttonState = GetComponent<UIState>(buttonEntity);
        if (buttonState.IsClicked)
        {

            //var dialogueSingleton = GetSingleton<CurrentDialogueSingleton>();
            if (TryGetSingleton<CurrentDialogueSingleton>(out var dialogueSingleton))
            {
                if (!dialogueSingleton.DialogueExist)
                {
                    UnityEngine.Debug.Log("Dialogue 1 started");
                    dialogueSingleton.DialogueExist = true;
                    EntityManager.AddComponent<DialogueInit>(buttonEntity);

                    dialogueSingleton.Entity = buttonEntity;
                    SetSingleton<CurrentDialogueSingleton>(dialogueSingleton);
                }
                else UnityEngine.Debug.LogWarning("Dialogue continue");
            }
            else
            {

                UnityEngine.Debug.LogError("CurrentDialogueSingleton not exist");
            }

        }

        buttonEntity = uiSys.GetEntityByUIName(initDialogueButton2);
        buttonState = GetComponent<UIState>(buttonEntity);
        if (buttonState.IsClicked)
        {
            if (TryGetSingleton<CurrentDialogueSingleton>(out var dialogueSingleton))
            {
                if (!dialogueSingleton.DialogueExist)
                {
                    UnityEngine.Debug.Log("Dialogue 2 started");
                    dialogueSingleton.DialogueExist = true;
                    EntityManager.AddComponent<DialogueContinue>(buttonEntity);

                    dialogueSingleton.Entity = buttonEntity;
                    SetSingleton<CurrentDialogueSingleton>(dialogueSingleton);
                }
                else UnityEngine.Debug.LogWarning("Dialogue continue");
            }
            else
            {

                UnityEngine.Debug.LogError("CurrentDialogueSingleton not exist");
            }

        }

    }
}
