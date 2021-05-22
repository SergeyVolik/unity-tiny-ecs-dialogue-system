using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

using System;
using SV.DialogueGraph.Runtime;

namespace SV.DialogueGraph.Editor
{
    public class DialogueGraphSaveUtility
    {
        private DialogueGraphView m_targetGraphView;
        private DialogueConteinerSO m_conteinerCache;
        private List<Edge> Edges => m_targetGraphView.edges.ToList();
        private List<DialogueNode> Nodes => m_targetGraphView.nodes.ToList().OfType<DialogueNode>().ToList();
        public static DialogueGraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
        {
            return new DialogueGraphSaveUtility
            {
                m_targetGraphView = targetGraphView
            };
        }

        public void SaveGraph(DialogueConteinerSO dialogConteiner)
        {
            if (!Edges.Exists(e => e.output.portName == DialogueGraphManifest.StartPortName))
            {
                EditorUtility.DisplayDialog("Error", "Not found input connection, please add connection to start port", "OK");
                return;
            }
            dialogConteiner.ClearData();

          
            dialogConteiner.NodeLinks = SaveDialogueLinks(Edges).ToList();
            dialogConteiner.DialogNodeData = SaveDialogueNodes(Nodes.OfType<DialogueNode>().ToList()).ToList();
            SaveExposedProperties(dialogConteiner);


            dialogConteiner.Comments.AddRange(SaveComments(m_targetGraphView.StickyNotes));
            dialogConteiner.GraphGroups.AddRange(SaveGroups(m_targetGraphView.Groups));

            //save entry point node guid
            dialogConteiner.StartNodeGuid = dialogConteiner.NodeLinks.FirstOrDefault(e => e.BaseNodeGuid == Nodes.Find(e1 => e1.EntryPoint).GUID).TargetNodeGuid;

            AssetDatabase.Refresh();
            EditorUtility.SetDirty(dialogConteiner);
            AssetDatabase.SaveAssets();
        }

        private IEnumerable<GraphGroupData> SaveGroups(IEnumerable<GroupCustomNode> groups)
        {
            List<GraphGroupData> groupsData = new List<GraphGroupData>();

            var list = groups.ToList();

            list.RemoveAll(e => e == null);
            list.ForEach(e =>
            {
                GraphGroupData data = new GraphGroupData();
                data.title = e.title;
                data.position = e.GetPosition().position;
                HashSet<GraphElement> ge = new HashSet<GraphElement>();
                e.CollectElements(ge, (e1) => e1 is DialogueNode);

                ge.Where((e2) => e2 is DialogueNode).Cast<DialogueNode>().ToList().ForEach(e1 =>
                {
                    data.childGuids.Add(e1.GUID);
                });

                groupsData.Add(data);
            });

            return groupsData;
        }

        private IEnumerable<GraphCommentData> SaveComments(IEnumerable<StickyNote> comments)
        {
            List<GraphCommentData> data = new List<GraphCommentData>();
            comments.ToList().RemoveAll(e => e == null);

            comments.Where(e => e is StickyNote)
                .ToList()
                .ForEach(e =>
                {
                    data.Add(new GraphCommentData
                    {
                        Content = e.contents,
                        Rect = e.GetPosition(),
                        Title = e.title
                    });
                });

            return data;
        }

        private void LoadComments(IEnumerable<GraphCommentData> data, Vector2 positionOffset)
        {
            data.ToList().ForEach(e =>
            {
                var note = new StickyNote()
                {
                    title = e.Title,
                    contents = e.Content,
                };
                note.SetPosition(new Rect(e.Rect.position + positionOffset, e.Rect.size));

                m_targetGraphView
                    .StickyNotes.Add(note);

                m_targetGraphView.AddElement(note);
            });
        }

        private IEnumerable<GroupCustomNode> LoadGroups(IEnumerable<GraphGroupData> data, Vector2 offset)
        {
            var groups = new List<GroupCustomNode>();
            data.ToList().ForEach(e =>
            {
                var group = new GroupCustomNode()
                {
                    title = e.title
                };
                group.SetPosition(new Rect(e.position + offset, new Vector2(100, 100)));

                group.AddElements(m_targetGraphView.nodes.ToList().Where(e1 => e1 is DialogueNode && e.childGuids.Exists((GUID) => ((DialogueNode)e1).GUID == GUID)));

                m_targetGraphView
                    .Groups.Add(group);

                m_targetGraphView.AddElement(group);

                groups.Add(group);
            });

            return groups;
        }

        private void SaveExposedProperties(DialogueConteinerSO dialogConteiner)
        {
            dialogConteiner.ExposedProperties.AddRange(m_targetGraphView.ExposedProperties);
        }

        public void LoadGraph(DialogueConteinerSO dialogConteiner)
        {
            m_conteinerCache = dialogConteiner;
            ClearGraph();
            CreateNodes(dialogConteiner.DialogNodeData, dialogConteiner.NodeLinks, Vector2.zero);
            ConnectNodes(dialogConteiner.NodeLinks);
            CreateExposedProperties();
            LoadComments(dialogConteiner.Comments, Vector2.zero);
            LoadGroups(dialogConteiner.GraphGroups, Vector2.zero);
        }

        private void CreateExposedProperties()
        {
            m_targetGraphView.ClearBlackBoardAndExposedProperties();
            foreach (var exposedProperty in m_conteinerCache.ExposedProperties)
            {
                m_targetGraphView.AddProperyToBlackBoard(exposedProperty);
            }
        }

        private IEnumerable<Edge> ConnectNodes(IEnumerable<DialogueLinkNodeData> NodeLinks)
        {
            var links = new List<Edge>();
            for (var i = 0; i < Nodes.Count; i++)
            {
                var connections = NodeLinks.Where(x => x.BaseNodeGuid == Nodes[i].GUID).ToList();

                for (var j = 0; j < connections.Count; j++)
                {
                    var targeNodeGuid = connections[j].TargetNodeGuid;
                    var targetNode = Nodes.First(x => targeNodeGuid == x.GUID);
                    links.Add(LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]));
                   
                }
            }

            return links;
        }

        private Edge LinkNodes(Port output, Port input)
        {
            var tempEdge = new Edge
            {
                output = output,
                input = input
            };

            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            m_targetGraphView.Add(tempEdge);

            return tempEdge;
        }

        private IEnumerable<DialogueNode> CreateNodes(IEnumerable<DialogueNodeData> DialogNodeData, IEnumerable<DialogueLinkNodeData> NodeLinks, Vector2 offset)
        {
            var nodes = new List<DialogueNode>();

            foreach (var nodeData in DialogNodeData)
            {
                var tempNode = m_targetGraphView.CreateDialogNode(nodeData.DialogText, Vector2.zero);
                tempNode.GUID = nodeData.GUID;
                m_targetGraphView.AddElement(tempNode);
                tempNode.SetPosition(new Rect(
                    nodeData.Position+ offset,
                    m_targetGraphView.defaultNodeSize
                ));

               NodeLinks?
                    .Where(x => x.BaseNodeGuid == nodeData.GUID)
                    .ToList()
                    .ForEach(x => m_targetGraphView.AddChoicePort(tempNode, x.PortName));

                nodes.Add(tempNode);
            }

            return nodes;
        }

        IEnumerable<DialogueNodeData> SaveDialogueNodes(List<DialogueNode> Nodes)
        {
            var DialogNodeData = new List<DialogueNodeData>();
            var entryNode = Nodes.Find(node => node.EntryPoint);
            foreach (var dialogNode in Nodes.Where(node => !node.EntryPoint))
            {
                DialogNodeData.Add(new DialogueNodeData
                {
                    GUID = dialogNode.GUID,
                    DialogText = dialogNode.DialogeText,
                    Position = dialogNode.GetPosition().position
                });
            }
            return DialogNodeData;
        }
        IEnumerable<DialogueLinkNodeData> SaveDialogueLinks(List<Edge> Nodes)
        {
            var connectedPorts = Nodes.Where(x => x.input.node != null).ToArray();
       
            var NodeLinks = new List<DialogueLinkNodeData>();

            for (var i = 0; i < connectedPorts.Length; i++)
            {
                var outsideNode = connectedPorts[i].output.node as DialogueNode;
                var inputNode = connectedPorts[i].input.node as DialogueNode;

                NodeLinks.Add(new DialogueLinkNodeData
                {
                    BaseNodeGuid = outsideNode.GUID,
                    PortName = connectedPorts[i].output.portName,
                    TargetNodeGuid = inputNode.GUID
                });
            }
            return NodeLinks;
        }

        private void ClearGraph()
        {
            var startNode = Nodes.Find(x => x.EntryPoint);
            if (m_conteinerCache.NodeLinks.Count == 0)
                startNode.GUID = Guid.NewGuid().ToString();
            else startNode.GUID = m_conteinerCache.NodeLinks[0].BaseNodeGuid;

            foreach (var node in Nodes)
            {
                if (node.EntryPoint) continue;
                Edges
                    .Where(x => x.input.node == node)
                    .ToList()
                    .ForEach(edge => m_targetGraphView.RemoveElement(edge));

                m_targetGraphView.RemoveElement(node);
            }

            List<GraphElement> elemenst = new List<GraphElement>();
            elemenst.AddRange(m_targetGraphView.StickyNotes);
            elemenst.AddRange(m_targetGraphView.Groups);
            elemenst.ForEach(e =>
            {
                if (e != null)
                    m_targetGraphView.RemoveElement(e);
            });
            m_targetGraphView.StickyNotes.Clear();
            m_targetGraphView.Groups.Clear();
        }

        #region Copy Past
        public string Copy(IEnumerable<GraphElement> elements)
        {
            var comments = elements.Where(e => e != null).OfType<StickyNote>();
            var dialogueNodes = elements.Where(e => e != null).OfType<DialogueNode>();
            var groupsNodes = elements.Where(e => e != null).OfType<GroupCustomNode>();
            var edges = elements.Where(e => e != null &&  e is Edge && ((Edge)e)?.output.portName != DialogueGraphManifest.StartPortName).Cast<Edge>();
            
         
            var groups = SaveGroups(groupsNodes);
            var nodes = SaveDialogueNodes(dialogueNodes.ToList());
            var links = SaveDialogueLinks(edges.ToList());

            
           

            var CopyPastSerializationData = new CopyPastSerializationData
            {
                comments = SaveComments(comments).ToList(),
                groups = groups.ToList(),
                nodes = nodes.ToList(),
                links = links.ToList()
            };

            return CopyPastSerializationData.ToJson();
        }

        public void Past(string data)
        {
            var CopyPastSerializationData = new CopyPastSerializationData();

            
            CopyPastSerializationData.LoadFromJson(data);


            LoadComments(CopyPastSerializationData.comments, new Vector2(5, 5));
           
            //m_targetGraphView.ClearSelection();

            var toSelect = new List<GraphElement>();
            toSelect.AddRange(CreateNodes(CopyPastSerializationData.nodes, CopyPastSerializationData.links, new Vector2(5, 5)));
            
            toSelect.AddRange(LoadGroups(CopyPastSerializationData.groups, new Vector2(5, 5)));
            toSelect.AddRange(ConnectNodes(CopyPastSerializationData.links));

            toSelect.ForEach(e =>
            {

                m_targetGraphView.AddToSelection(e);
            });

        }

        [System.Serializable]
        public class CopyPastSerializationData
        {
            public List<GraphCommentData> comments = new List<GraphCommentData>();
            public List<GraphGroupData> groups = new List<GraphGroupData>();
            public List<DialogueNodeData> nodes = new List<DialogueNodeData>();
            public List<DialogueLinkNodeData> links = new List<DialogueLinkNodeData>();

            public string ToJson()
            {
                return JsonUtility.ToJson(this);
            }

            public void LoadFromJson(string json)
            {
                JsonUtility.FromJsonOverwrite(json, this);
                FixGuids();
            }

            private void FixGuids()
            {
                Dictionary<string, string> oldnodeGuidToNew = new Dictionary<string, string>();
                nodes.ToList().ForEach(e =>
                {
                    var newGuid = Guid.NewGuid().ToString();

                    oldnodeGuidToNew.Add(e.GUID, newGuid);

                    e.GUID = newGuid;
                });

                groups.ToList().ForEach(e =>
                {
                    for (var i = 0; i < e.childGuids.Count; i++)
                    {

                        if (oldnodeGuidToNew.TryGetValue(e.childGuids[i], out var newGuid))
                            e.childGuids[i] = newGuid;
                    }

                });

                links.ToList().ForEach(e =>
                {
                    if (oldnodeGuidToNew.TryGetValue(e.BaseNodeGuid, out var result1))
                    {
                        e.BaseNodeGuid = result1;
                    }

                    if (oldnodeGuidToNew.TryGetValue(e.TargetNodeGuid, out var result2))
                    {
                        e.TargetNodeGuid = result2;
                    }
                });
            }
        }
        #endregion
    }
}
