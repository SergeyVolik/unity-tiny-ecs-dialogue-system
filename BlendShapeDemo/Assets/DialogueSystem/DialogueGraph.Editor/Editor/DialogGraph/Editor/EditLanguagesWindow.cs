using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using SV.DialogueGraph.Runtime;
namespace SV.DialogueGraph.Editor
{
    class EditLanguagesWindow : EditorWindow
    {
        private ListView lastListView;
        public void Init(Languages localization, LanguagesSO localizationSO, UnityEditor.UIElements.PopupField<Language> languageDropDown)
        {
            titleContent = new GUIContent($"Edit Languages");

            List<Language> lang = new List<Language>(localization.popupFieldValues);

            var label = new Label("Languages");
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            var conteiner = new VisualElement();

            RecreateListView(conteiner, lang);


            var buttonOk = new Button(() => {
                localization.popupFieldValues.Clear();
                localization.popupFieldValues.AddRange(lang);
                localizationSO.Languages.Clear();
                lang.ForEach(e => localizationSO.Languages.Add(e.Name));

                Close();
            })
            { text = "OK" };

            var buttonAdd = new Button(() => {
                var Language = new Language("New Language");
                lang.Add(Language);

                RecreateListView(conteiner, lang, lastListView);
            })
            { text = "Add" };

            conteiner.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            conteiner.style.height = new StyleLength(new Length(100, LengthUnit.Pixel));
            rootVisualElement.Add(label);
            rootVisualElement.Add(conteiner);
            rootVisualElement.Add(buttonAdd);
            rootVisualElement.Add(buttonOk);
            //this.ShowModal();
        }

        private void RecreateListView(VisualElement conteiner, List<Language> langs, ListView old = null)
        {
            if (old != null)
                conteiner.Remove(old);

            Func<VisualElement> makeItem = () => new VisualElement();

            // As the user scrolls through the list, the ListView object
            // will recycle elements created by the "makeItem"
            // and invoke the "bindItem" callback to associate
            // the element with the matching data item (specified as an index in the list)
            Action<VisualElement, int> bindItem = (e, i) => {
                e.style.flexDirection = FlexDirection.Row;
                var textFiled = new TextField { value = langs[i].Name };
                var elem = langs[i];
                var button = new Button(() =>
                {
                    e.parent.Remove(e);
                    langs.Remove(elem);
                })
                { text = "X" };

                e.Add(button);
                e.Add(textFiled);
            };

            // Provide the list view with an explict height for every row
            // so it can calculate how many items to actually display
            const int itemHeight = 16;

            lastListView = new ListView(langs, itemHeight, makeItem, bindItem);

            lastListView.selectionType = SelectionType.Multiple;

            //listView.onItemChosen += obj => Debug.Log(obj);
            //listView.onSelectionChanged += objects => Debug.Log(objects);


            lastListView.style.flexGrow = 1.0f;
            conteiner.Add(lastListView);
        }
    }
}
