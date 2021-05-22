using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SV.DialogueGraph.Editor
{
    public class GroupCustomNode : Group
    {
        public GroupCustomNode()
        {
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
        }

        void DisconnectAll(DropdownMenuAction a)
        {
            //HashSet<GraphElement> toDelete = new HashSet<GraphElement>();

            //AddConnectionsToDeleteSet(inputContainer, ref toDelete);
            //AddConnectionsToDeleteSet(outputContainer, ref toDelete);
            //toDelete.Remove(null);

            //if (graphView != null)
            //{
            //    graphView.DeleteElements(toDelete);
            //}
            //else
            //{
            //    Debug.Log("Disconnecting nodes that are not in a GraphView will not work.");
            //}
        }

        DropdownMenuAction.Status DisconnectAllStatus(DropdownMenuAction a)
        {
            //VisualElement[] containers =
            //{
            //    inputContainer, outputContainer
            //};

            //foreach (var container in containers)
            //{
            //    var currentInputs = container.Query<Port>().ToList();
            //    foreach (var elem in currentInputs)
            //    {
            //        if (elem.connected)
            //        {
            //            return DropdownMenuAction.Status.Normal;
            //        }
            //    }
            //}

            return DropdownMenuAction.Status.Disabled;
        }

        public virtual void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is GroupCustomNode)
            {
                evt.menu.AppendAction("Delete only scope", DisconnectAll, DisconnectAllStatus);
                evt.menu.AppendSeparator();
            }
        }
    }
}
