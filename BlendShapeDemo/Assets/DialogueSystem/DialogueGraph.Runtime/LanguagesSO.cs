using System.Collections.Generic;
using UnityEngine;


namespace SV.DialogueGraph.Runtime
{
    [System.Serializable]
    public class LanguagesSO : ScriptableObject
    {
        public List<string> Languages = new List<string>();
    }
}
