using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace SV.DialogueGraph.Editor
{
    [EditorWindowTitle(title = k_ToolName)]
    public class DialogueGraphViewToolWindow : GraphViewToolWindow
    {
        const string k_ToolName = "Inspector";
        protected override string ToolName => k_ToolName;

        protected override void OnGraphViewChanged()
        {
            //throw new NotImplementedException();
        }

        protected override void OnGraphViewChanging()
        {
            //throw new NotImplementedException();
        }
    }
}
