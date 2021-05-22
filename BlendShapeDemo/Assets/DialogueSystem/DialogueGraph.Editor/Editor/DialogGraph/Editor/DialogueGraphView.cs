using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using SV.DialogueGraph.Runtime;
namespace SV.DialogueGraph.Editor
{
    //public enum DialogueGraphLastAction { Copy, Past, Save, Load, Cut, Duplicate, Delete }
    public class DialogueGraphView : GraphView
    {
        public readonly Vector2 defaultNodeSize = new Vector2(150, 200);


        private NodeSearchWindow m_searchWindow;
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
        public Blackboard Blackboard;

        public List<StickyNote> StickyNotes = new List<StickyNote>();
        public List<GroupCustomNode> Groups = new List<GroupCustomNode>();
        public DialogueConteinerSO currentDialogueSO;

        //graph events
        public delegate void SaveNotify();
        public event SaveNotify SaveEvent;

        public delegate void LoadNotify();
        public event LoadNotify LoadEvent;

        public delegate void RevertNotify();
        public event RevertNotify RevertEvent;

        public delegate void GraphChangedNotify();
        public event GraphChangedNotify GraphChangedEvent;


        public DialogueGraphView(EditorWindow editorWindow, DialogueConteinerSO item)
        {
           
            currentDialogueSO = item;
            this.isReframable = true;

            this.StretchToParentSize();
            styleSheets.Add(Resources.Load<StyleSheet>("DialogGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());


            var grid = new GridBackground();
            Insert(0, grid);

            var nodeStart = GenerateEntryPointNode();
            AddElement(nodeStart);

            AddSearchWindow(editorWindow);
            RegisterGraphHotKeys();
            RegisterGraphCallbacks();
        }

        public static void CollectElements2(IEnumerable<GraphElement> elements, HashSet<GraphElement> collectedElementSet, Func<GraphElement, bool> conditionFunc)
        {
            foreach (var element in elements.Where(e => e != null && !collectedElementSet.Contains(e) && conditionFunc(e)))
            {
                var collectibleElement = element as ICollectibleElement;
                collectibleElement?.CollectElements(collectedElementSet, conditionFunc);
                collectedElementSet.Add(element);
            }
        }

        private void AddSearchWindow(EditorWindow editorWindow)
        {
            m_searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            m_searchWindow.Init(editorWindow, this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), m_searchWindow);
        }

        #region Graph callbacks

       

        void RegisterGraphCallbacks()
        {
            serializeGraphElements = (elements) =>
            {
                var saveUtility = DialogueGraphSaveUtility.GetInstance(this);
                var json = saveUtility.Copy(elements);


                return json;
            };

            unserializeAndPaste = (oper, data) =>
            {


                Debug.Log(data);

                var saveUtility = DialogueGraphSaveUtility.GetInstance(this);
                saveUtility.Past(data);

            };

            graphViewChanged = (changes) =>
            {
                GraphChangedEvent.Invoke();
                return changes;
            };

            deleteSelection = (operationName, askUser) =>
            {
                var elementsToRemoveSet = new HashSet<GraphElement>();

                selection.OfType<GroupCustomNode>().ToList().ForEach(e =>
                {
                    //e.remove
                    elementsToRemoveSet.Add(e);
                    //e.capabilities |= Capabilities.Groupable;
                });


                foreach (var group in elementsToRemoveSet)
                {
                    selection.Remove(group);
                }

                CollectElements2(selection.OfType<GraphElement>(), elementsToRemoveSet, e => (e.capabilities & Capabilities.Deletable) == Capabilities.Deletable);


                var previouslyCollapsedElements = new HashSet<GraphElement>();
                // For each collapsed placemat, expand and keep list of elements over the expanded placemat
                foreach (var placemat in elementsToRemoveSet.OfType<Placemat>().Where(p => p.Collapsed))
                {
                    previouslyCollapsedElements.UnionWith(placemat.CollapsedElements);
                    placemat.Collapsed = false;
                }

                Groups.RemoveAll(e => elementsToRemoveSet.Contains(e));
                DeleteElements(elementsToRemoveSet);

                selection.Clear();

                // Add "expanded placemats" elements to selection now.
                foreach (var ge in previouslyCollapsedElements)
                    AddToSelection(ge);
            };
            elementsRemovedFromGroup = (group, graphElements) =>
            {
                Debug.Log("elementsRemovedFromGroup");
                //graphElements.ToList().ForEach(e => AddElement(e));
            };
            elementsInsertedToStackNode = (stackNode, index, elements) =>
            {
                Debug.Log("elementsInsertedToStackNode");
            };
            elementsAddedToGroup = (Group, elements) =>
            {
                Debug.Log("elementsAddedToGroup");
            };
            elementsRemovedFromGroup = (Group, elements) =>
            {
                Debug.Log("elementsRemovedFromGroup");
            };
        }

        #endregion
        #region Hot keys
        public void Load()
        {
            RequestDataOperation(false);
            LoadEvent.Invoke();
        }
        public void Revert()
        {
            RequestDataOperation(false);
            RevertEvent.Invoke();
        }

        public void Save()
        {
            RequestDataOperation(true);
            SaveEvent.Invoke();
           
        }

        public void RequestDataOperation(bool save)
        {
            if (!currentDialogueSO)
            {
                EditorUtility.DisplayDialog("Save File Deleted", "Please enter a valid file name", "OK");
                return;
            }

            var saveUtility = DialogueGraphSaveUtility.GetInstance(this);

            if (save)
                saveUtility.SaveGraph(currentDialogueSO);
            else saveUtility.LoadGraph(currentDialogueSO);
        }

        void RegisterGraphHotKeys()
        {
            SelectAllHotKey();
            SaveHotKey();
            RevertHotKey();
        }

        void SaveHotKey()//CTRL+S
        {
            RegisterCallback<KeyDownEvent>((evt) =>
            {
                if (evt.keyCode == KeyCode.S && evt.actionKey)
                {
                    Save();

                }
            });
        }

        void SelectAllHotKey() //CTRL+A
        {
            RegisterCallback<KeyDownEvent>((evt) =>
            {
                if (evt.keyCode == KeyCode.A && evt.actionKey)
                {
                    ClearSelection();
                    FrameAll();
                    graphElements.ForEach(e => AddToSelection(e));
                }
            });
        }

        void RevertHotKey()
        {
            RegisterCallback<KeyDownEvent>((evt) =>
            {
                if (evt.keyCode == KeyCode.R && evt.actionKey)
                {
                    Load();

                }
            });
        }

        #endregion
        #region Blackboard
        public void AddProperyToBlackBoard(ExposedProperty exposedProperty)
        {
            var localPropertyName = exposedProperty.PropertyName;
            var localPropertyValue = exposedProperty.PropertyValue;

            while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
                localPropertyName = $"{localPropertyName}(1)";

            var property = new ExposedProperty();
            property.PropertyName = localPropertyName;
            property.PropertyValue = localPropertyValue;
            ExposedProperties.Add(property);

            var conteiner = new VisualElement();
            var blackBoardField = new BlackboardField
            {
                text = exposedProperty.PropertyName,
                typeText = "string property"
            };

            conteiner.Add(blackBoardField);

            var propertyValueTextField = new TextField("Value:")
            {
                value = localPropertyValue
            };
            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                var changingPropertyIndex = ExposedProperties.FindIndex(x => x.PropertyName == property.PropertyName);
                ExposedProperties[changingPropertyIndex].PropertyValue = evt.newValue;
            });

            var blackBoardValueRow = new BlackboardRow(blackBoardField, propertyValueTextField);
            conteiner.Add(blackBoardValueRow);

            Blackboard.Add(conteiner);
        }

        public void ClearBlackBoardAndExposedProperties()
        {
            ExposedProperties.Clear();
            Blackboard.Clear();
        }

        #endregion
        #region Nodes

        private DialogueNode GenerateEntryPointNode()
        {
            var node = new DialogueNode
            {
                title = DialogueGraphManifest.StartPortName,
                GUID = Guid.NewGuid().ToString(),
                DialogeText = "Hello world",
                EntryPoint = true
            };

            var genPort = GeneratePort(node, Direction.Output);
            genPort.portName = DialogueGraphManifest.StartPortName;
            node.outputContainer.Add(genPort);
            node.SetPosition(new Rect(100, 200, 100, 150));

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;
            node.capabilities &= ~Capabilities.Selectable;

            node.RefreshExpandedState();
            node.RefreshPorts();


            return node;
        }

        public void CreateNode(string nodeName, Vector2 position)
        {
            AddElement(CreateDialogNode(nodeName, position));
        }

        public void CreateComment()
        {
            var note = new StickyNote()
            {
                title = "Name",
                contents = "Comment"
            };
            StickyNotes.Add(note);
            note.capabilities |= Capabilities.Groupable;
            note.capabilities |= Capabilities.Collapsible;
            AddElement(note);
        }

        public void CreateStack()
        {
            var note = new StackNode()
            {
                title = "Stack",
            };
            //StickyNotes.Add(note);
            AddElement(note);
        }

        public void CreateGroupScope()
        {
            var group = new GroupCustomNode();
            group.title = "Group";
            Groups.Add(group);
            AddElement(group);
        }

        public DialogueNode CreateDialogNode(string nodeName, Vector2 position)
        {
            var dialogNode = new DialogueNode
            {
                title = nodeName,
                DialogeText = nodeName,
                GUID = Guid.NewGuid().ToString()
            };

            dialogNode.capabilities |= Capabilities.Copiable | Capabilities.Groupable | Capabilities.Ascendable | Capabilities.Collapsible;

            dialogNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            var inputPort = GeneratePort(dialogNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "input";


            var button = new Button(() => AddChoicePort(dialogNode));
            button.text = "new choice";
            dialogNode.titleContainer.Add(button);

            var textField = new TextField(string.Empty);
            textField.RegisterValueChangedCallback(evt =>
            {
                dialogNode.DialogeText = evt.newValue;
                dialogNode.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(dialogNode.title);
            dialogNode.mainContainer.Add(textField);

            dialogNode.inputContainer.Add(inputPort);


            dialogNode.RefreshPorts();
            dialogNode.RefreshExpandedState();
            dialogNode.SetPosition(new Rect(position, defaultNodeSize));

            return dialogNode;
        }

        #endregion
        #region Ports
        private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                var portView = port;
                if (startPort != port && startPort.node != port.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        public void AddChoicePort(DialogueNode dialogeNode, string overridePortName = "")
        {
            var generatedPort = GeneratePort(dialogeNode, Direction.Output);

            var oldLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(oldLabel);
            var outputPortCount = dialogeNode.outputContainer.Query("connector").ToList().Count;

            var choicePortName = string.IsNullOrEmpty(overridePortName) ? $"Choice {outputPortCount + 1}" : overridePortName;

            var textField = new TextField
            {
                name = string.Empty,
                value = choicePortName
            };
            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            generatedPort.contentContainer.Add(new Label(" "));
            generatedPort.contentContainer.Add(textField);
            var deleteButton = new Button(() => RemovePort(dialogeNode, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);
            generatedPort.portName = choicePortName;
            dialogeNode.outputContainer.Add(generatedPort);
            dialogeNode.RefreshExpandedState();
            dialogeNode.RefreshPorts();
        }

        private void RemovePort(DialogueNode dialogeNode, Port generatedPort)
        {
            var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

            if (!targetEdge.Any()) return;

            var edge = targetEdge.First();

            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());

            dialogeNode.outputContainer.Remove(generatedPort);
            dialogeNode.RefreshPorts();
            dialogeNode.RefreshExpandedState();
        }

        #endregion
    }
}
