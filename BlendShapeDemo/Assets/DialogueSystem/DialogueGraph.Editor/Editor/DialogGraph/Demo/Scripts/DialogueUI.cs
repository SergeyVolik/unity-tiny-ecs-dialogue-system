//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;
//using System.Linq;
//using Unity.Entities;
//using SV.DialogueGraph.Authoring;

//public class DialogueUI : MonoBehaviour
//{
//    private struct DialogueUIChoice
//    {
//        public TMP_Text choiceText;
//        public Button button;
//    }
//    [SerializeField]
//    Button[] PlayerChoices;
//    [SerializeField]
//    TMP_Text NPM_Replic;

//    List<DialogueUIChoice> availableChoices = new List<DialogueUIChoice>();
//    EntityManager manager;

//    // Start is called before the first frame update
//    void Awake()
//    {
//        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
//        PlayerChoices.ToList().ForEach(e =>
//        {
//            availableChoices.Add(new DialogueUIChoice {
//                button = e,
//                choiceText = e.GetComponentInChildren<TMP_Text>()
//            });
//        });

//        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<DialogueNextStepSystem>().OnEventTrigger += (eventData) =>
//        {
//            if (manager.HasComponent<Dialogue>(eventData.from))
//            {
//                var dialogue = manager.GetComponentData<Dialogue>(eventData.from);
//                ref var replic = ref dialogue.data.Value.Nodes[eventData.dialogueNode];

//                if (!gameObject.activeSelf)
//                    gameObject.SetActive(true);

//                availableChoices.ForEach(e => e.button.gameObject.SetActive(false));

//                NPM_Replic.text = replic.Text.ToString();

//                for (var i = 0; i < replic.Links.Length; i++)
//                {
//                    var choice = availableChoices[i];
//                    choice.button.gameObject.SetActive(true);
//                    choice.button.onClick.RemoveAllListeners();
//                    choice.choiceText.text = replic.Links[i].Text.ToString();
//                    var index = replic.Links[i].nodeIndex;
//                    choice.button.onClick.AddListener(() =>
//                    {
                        
//                        manager.AddComponent<DialogueChoice>(eventData.from);
//                        manager.SetComponentData(eventData.from, new DialogueChoice { Value = index });

//                    });
//                }
               
//            }

            

//        };

//        gameObject.SetActive(false);
//    }


//}
