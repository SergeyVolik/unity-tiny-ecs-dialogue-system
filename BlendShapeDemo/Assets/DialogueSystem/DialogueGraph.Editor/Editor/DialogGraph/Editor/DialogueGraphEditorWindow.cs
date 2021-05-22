using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using SV.DialogueGraph.Runtime;
using UnityEditor.Callbacks;
using System.Text.RegularExpressions;

namespace SV.DialogueGraph.Editor
{
    public static class DialogueGraphManifest
    {
        public const string StartPortName = "start";
    }

    public class Language
    {
        public string Name { get; private set; }

        public Language(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }


    public class Languages
    {
        public int current = 0;

        public List<Language> popupFieldValues;
        public Languages()
        {
            popupFieldValues = new List<Language>
            {
                new Language("English"),
                new Language("Russion"),
                new Language("Ukraine"),
            };
        }
    }

    public class DialogueGraphEditorWindow : EditorWindow
    {
        private Languages localization;
        private LanguagesSO localizationSO;
        private DialogueGraphView m_graphView;

        private PopupField<Language> languageDropDown;
        private MiniMap minimap;
        private Blackboard blackBoard;
        private string assetName;
        //private DialogueGraphViewToolWindow tools;

        [OnOpenAsset(1)]
        public static bool ShowWindow(int _instanceId, int line)
        {
            UnityEngine.Object item = EditorUtility.InstanceIDToObject(_instanceId);

            if (item is DialogueConteinerSO)
            {

                var window = GetWindow<DialogueGraphEditorWindow>();

                window.Init(item as DialogueConteinerSO);


                return true;
            }

            return false;
        }

        private void Init(DialogueConteinerSO item)
        {
            assetName = item.name.ToString();
            Texture2D myIcon = EditorGUIUtility.Load("Assets/ECS/DialogGraph/DialogueIcon.png") as Texture2D;
           
            titleContent = new GUIContent(assetName, myIcon, "Dialogue graph tool");
          
            minSize = new Vector2(500, 250);

            rootVisualElement.Clear();

            CreateGraphView(item);
            rootVisualElement.Add(m_graphView);
            CreateToolbar();
            CreateMinimap();
            CreateBlackBoard();
           
            LoadGraph();

        }
        private void OnDisable()
        {
            rootVisualElement.Clear();
        }


        private void LoadGraph()
        {
            m_graphView.Load();
        }

        void CreateLocalizationAssset()
        {
            bool needRefresh = false;
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                needRefresh = true;
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            localizationSO = AssetDatabase.LoadAssetAtPath<LanguagesSO>("Assets/Resources/Languages.asset");
            if (localizationSO == null)
            {
                needRefresh = true;
                localizationSO = CreateInstance<LanguagesSO>();
                localization = new Languages();
                localization.popupFieldValues.ForEach(e =>
                {
                    localizationSO.Languages.Add(e.Name);
                });


                AssetDatabase.CreateAsset(localizationSO, "Assets/Resources/Languages.asset");
                AssetDatabase.SaveAssets();
            }
            else
            {
                localization = new Languages();
                localization.popupFieldValues.Clear();
                localizationSO.Languages.ForEach(e => localization.popupFieldValues.Add(new Language(e)));
            }

            if (needRefresh)
                AssetDatabase.Refresh();
        }

        public void ShowUIElementOnGrapth(VisualElement elem)
        {
            if (!m_graphView.Contains(elem))
                m_graphView.Add(elem);
        }

        public void HideUIElementOnGrapth(VisualElement elem)
        {
            if (m_graphView.Contains(elem))
                m_graphView.Remove(elem);
        }

        private void CreateBlackBoard()
        {
           
            blackBoard = new Blackboard(m_graphView);
            blackBoard.Add(new BlackboardSection { title = "Exposed Properties" });
            blackBoard.addItemRequested = _blackBoard =>
            {
                m_graphView.AddProperyToBlackBoard(new ExposedProperty());
            };
            blackBoard.editTextRequested = (_blackBoard, element, newValue) =>
            {
                var oldPropertyName = ((BlackboardField)element).text;

                if (m_graphView.ExposedProperties.Any(x => x.PropertyName == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "This property name alredy exist, please enter another one!", "OK");
                    return;
                }

                var propertyIndex = m_graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
                m_graphView.ExposedProperties[propertyIndex].PropertyName = newValue;

                (element as BlackboardField).text = newValue;
            };
            blackBoard.SetPosition(new Rect(10, 30, 200, 300));


            m_graphView.Blackboard = blackBoard;
        }

        private void CreateMinimap()
        {
            minimap = new MiniMap {};
            minimap.SetPosition(new Rect(10, 30, 200, 140));
        }
      

        private void CreateGraphView(DialogueConteinerSO saveConteiner)
        {
            m_graphView = new DialogueGraphView(this, saveConteiner)
            {
                name = "Dialog Graph"
            };
        
        }

        private void CreateToolbar()
        {
            CreateLocalizationAssset();
            var toolbar = new Toolbar();
            toolbar.styleSheets.Add(Resources.Load<StyleSheet>("ToolBar"));
            var conteinerLeft = new VisualElement();

            
            var saveToggle = new ToolbarToggle { text = "Save Asset" };

            saveToggle.RegisterValueChangedCallback((evt) => {
                if (!evt.newValue)
                    return;

                saveToggle.value = false;
                m_graphView.Save();
                
            });
            var saveAsToggle = new ToolbarToggle { text = "Save As..." };

            saveAsToggle.RegisterValueChangedCallback((evt) => {
                if (!evt.newValue)
                    return;

                var path = EditorUtility.SaveFilePanel(
                "Save dialogue graph",
               "",
                "save" + ".asset",
                "asset");

                if (path.Length != 0)
                {
                    var newAsset = ScriptableObject.CreateInstance<DialogueConteinerSO>();


                    var resultString = Regex.Replace(path,
                    Application.dataPath, "", RegexOptions.IgnorePatternWhitespace);
                    Debug.Log(resultString);
                    Debug.Log(Application.dataPath);
                    m_graphView.currentDialogueSO = newAsset;
                    m_graphView.Save();
                    AssetDatabase.CreateAsset(newAsset, $"Assets{resultString}");
                }

                saveAsToggle.value = false;

            });

            var showInProjectToggle = new ToolbarToggle { text = "Show In Project" };

            showInProjectToggle.RegisterValueChangedCallback((evt) => {
                if (!evt.newValue)
                    return;
                EditorGUIUtility.PingObject(m_graphView.currentDialogueSO);
                showInProjectToggle.value = false;

            });

            m_graphView.SaveEvent += () => {

                titleContent.text = assetName;
            };
            m_graphView.LoadEvent += () => {
                titleContent.text = assetName;
            };
            m_graphView.RevertEvent += () => {
                titleContent.text = assetName;
            };
            m_graphView.GraphChangedEvent += () => {
                titleContent.text = assetName+"*";
            };


            var revertToggle = new ToolbarToggle { text = "Revert Changes" };
            revertToggle.RegisterValueChangedCallback((evt) => {
                if (!evt.newValue)
                    return;
                revertToggle.value = false;
                m_graphView.Load();
            });



            languageDropDown = new PopupField<Language>(localization.popupFieldValues, localization.popupFieldValues[0]);
            languageDropDown.RegisterValueChangedCallback((value) => Debug.Log(value.newValue.Name));


          
            var languageToggel = new ToolbarToggle { text = "Edit Languages" };
            languageToggel.RegisterValueChangedCallback((evt) => {
                if (!evt.newValue)
                    return;


                var window = GetWindow<EditLanguagesWindow>();
                window.Init(localization, localizationSO, languageDropDown);


                languageToggel.value = false;
            });

            conteinerLeft.Add(saveToggle);
            conteinerLeft.Add(saveAsToggle);
            conteinerLeft.Add(revertToggle);
            conteinerLeft.Add(showInProjectToggle);
          

            var conteinerRight = new VisualElement();

            var miniMapToggle = new ToolbarToggle { text = "Minmap" };
            miniMapToggle.RegisterValueChangedCallback((evt) => {
                if (evt.newValue)
                {
                    ShowUIElementOnGrapth(minimap);
                }
                else
                {
                    HideUIElementOnGrapth(minimap);
                }
            });

            var blackBoardToggle = new ToolbarToggle { text = "Blackboard" };
            blackBoardToggle.RegisterValueChangedCallback((evt) => {
                if (evt.newValue)
                {
                    ShowUIElementOnGrapth(blackBoard);
                }
                else
                {
                    HideUIElementOnGrapth(blackBoard);
                }
            });

            var InspectorToggle = new ToolbarToggle { text = "Graph Inspector" };
            blackBoardToggle.RegisterValueChangedCallback((evt) => {
                if (evt.newValue)
                {
                    //ShowUIElementOnGrapth(insperctorWindow);
                }
                else
                {
                    //HideUIElementOnGrapth(blackBoard);
                }
            });

            conteinerRight.Add(miniMapToggle);
            conteinerRight.Add(blackBoardToggle);
            conteinerRight.Add(languageDropDown);
            conteinerRight.Add(languageToggel);

            conteinerLeft.AddToClassList("toggleLeft");
            conteinerLeft.name = "toggleLeft";
            toolbar.Add(conteinerLeft);

            conteinerRight.AddToClassList("toggleRight");
            conteinerLeft.name = "toggleRight";
            toolbar.Add(conteinerRight);


            rootVisualElement.Add(toolbar);
        }
    }
}
