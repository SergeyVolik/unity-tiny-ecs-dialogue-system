using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace SV.DialogueGraph.Editor
{
    public class DialogueNode : Node
    {
        public string GUID;
        public string DialogeText;
        public bool EntryPoint;

        public DialogueNode()
        {
            //capabilities |= Capabilities.Resizable | Capabilities.Renamable;
        }
    }
}
