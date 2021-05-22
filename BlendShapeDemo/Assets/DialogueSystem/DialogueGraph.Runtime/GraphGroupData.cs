using System.Collections.Generic;
using UnityEngine;

namespace SV.DialogueGraph.Runtime
{
    [System.Serializable]
    public class GraphGroupData
    {
        public List<string> childGuids = new List<string>();
        public string title;
        public Vector2 position;
    }
}
