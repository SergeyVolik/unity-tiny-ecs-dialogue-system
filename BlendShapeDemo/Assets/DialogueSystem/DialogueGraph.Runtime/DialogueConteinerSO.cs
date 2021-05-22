using System.Collections.Generic;
using UnityEngine;


namespace SV.DialogueGraph.Runtime
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DialogueData", menuName = "SV/Dialogue", order = 1)]
    public class DialogueConteinerSO : ScriptableObject
    {
        public string StartNodeGuid;
        public List<DialogueLinkNodeData> NodeLinks = new List<DialogueLinkNodeData>();
        public List<DialogueNodeData> DialogNodeData = new List<DialogueNodeData>();
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
        public List<GraphCommentData> Comments = new List<GraphCommentData>();
        public List<GraphGroupData> GraphGroups = new List<GraphGroupData>();
        public void ClearData()
        {
            GraphGroups.Clear();
            DialogNodeData.Clear();
            ExposedProperties.Clear();
            NodeLinks.Clear();
            Comments.Clear();
        }


    }
}
