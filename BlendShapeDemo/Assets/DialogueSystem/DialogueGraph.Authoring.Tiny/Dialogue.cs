
using Unity.Entities;
namespace SV.DialogueGraph.Authoring.Tiny
{
    public struct Dialogue : IComponentData
    {
        public BlobAssetReference<DlgGraph> data;
        public int replicIndex;
    }
    public struct DlgGraph
    {
        public BlobArray<DlgNode> Nodes;
        public int entryPointIndex;
    }

    public struct DlgChoiceData
    {
        public int nodeIndex;
        public BlobString Text;
        public bool finish;
    }

    public struct DlgNode
    {
        public BlobArray<DlgChoiceData> Links;
        public BlobString Text;
    }


    public struct DialogueChoice : IComponentData
    {
        public int Value;
    }
}