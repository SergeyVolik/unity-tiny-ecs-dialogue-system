//using SV.DialogueGraph.Runtime;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using SV.DialogueGraph.Runtime;
using SV.DialogueGraph.Authoring.Tiny;


namespace SV.DialogueGraph.Authoring
{
    public static class BlobAssetStoreExtention
    {
        public static BlobAssetReference<DlgGraph> GetDialogue(this BlobAssetStore blobAssetStore, DialogueConteinerSO dialogue)
        {
            BlobAssetReference<DlgGraph> dialogueBlobAssetRef;

            //TODO: fix DialogueGraph convertion
            BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);

            ref DlgGraph graphBlob = ref blobBuilder.ConstructRoot<DlgGraph>();
            //ref DialogueNode dialogueNode = ref blobBuilder.ConstructRoot<DialogueNode>();
            BlobBuilderArray<DlgNode> nodes = blobBuilder.Allocate(ref graphBlob.Nodes, dialogue.DialogNodeData.Count);


            for (var i = 0; i < dialogue.DialogNodeData.Count; i++)
            {

                if (dialogue.StartNodeGuid == dialogue.DialogNodeData[i].GUID)
                    graphBlob.entryPointIndex = i;

                ref var node = ref nodes[i];

                blobBuilder.AllocateString(ref node.Text, dialogue.DialogNodeData[i].DialogText);


                var links = dialogue.NodeLinks.FindAll(e => e.BaseNodeGuid == dialogue.DialogNodeData[i].GUID);

                if (links != null)
                {
                    BlobBuilderArray<DlgChoiceData> choices = blobBuilder.Allocate(ref node.Links, links.Count);

                    for (var j = 0; j < links.Count; j++)
                    {
                        ref var choice = ref choices[j];

                        blobBuilder.AllocateString(ref choice.Text, links[j].PortName);


                        choice.nodeIndex = dialogue.DialogNodeData.IndexOf(dialogue.DialogNodeData.Find(e => e.GUID == links[j].TargetNodeGuid));

                    }

                }


            }

            dialogueBlobAssetRef = blobBuilder.CreateBlobAssetReference<DlgGraph>(Allocator.Persistent);
            blobBuilder.Dispose();

            return dialogueBlobAssetRef;
        }
    }
}




