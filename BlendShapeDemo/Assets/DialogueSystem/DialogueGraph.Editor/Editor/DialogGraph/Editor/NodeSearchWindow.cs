using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SV.DialogueGraph.Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueGraphView m_dialogGraph;
        private EditorWindow m_window;
        private Texture2D m_indentationaIcon;

        public void Init(EditorWindow window, DialogueGraphView graphView)
        {
            m_dialogGraph = graphView;
            m_window = window;

            m_indentationaIcon = new Texture2D(1, 1);
            m_indentationaIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            m_indentationaIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create element"), 0),
                new SearchTreeGroupEntry(new GUIContent("Dialog Node"), 1),
                new SearchTreeEntry(new GUIContent("Dialog Node", m_indentationaIcon))
                {
                    userData = new DialogueNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("Comment", m_indentationaIcon))
                {
                    userData = new StickyNote(), level = 2
                },
                new SearchTreeEntry(new GUIContent("Stack", m_indentationaIcon))
                {
                    userData = new StackNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("Scope", m_indentationaIcon))
                {
                    userData = new GroupCustomNode(), level = 2
                },
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var worldMousePosition = m_window.rootVisualElement.ChangeCoordinatesTo(
                m_window.rootVisualElement.parent,
                context.screenMousePosition - m_window.position.position
            );

            var localMousePosition = m_dialogGraph.contentViewContainer.WorldToLocal(worldMousePosition);
            switch (SearchTreeEntry.userData)
            {
                case DialogueNode dialogNode:
                    m_dialogGraph.CreateNode("Dialog Node", localMousePosition);
                    return true;
                case StickyNote note:
                    //m_dialogGraph.CreateNode("Dialog Node", localMousePosition);
                    m_dialogGraph.CreateComment();
                    return true;
                case StackNode stack:
                    //m_dialogGraph.CreateNode("Dialog Node", localMousePosition);
                    m_dialogGraph.CreateStack();
                    return true;
                case GroupCustomNode scope:
                    //m_dialogGraph.CreateNode("Dialog Node", localMousePosition);
                    m_dialogGraph.CreateGroupScope();
                    return true;
                default:
                    return false;
            }

            //throw new System.NotImplementedException();
        }
    }
}
