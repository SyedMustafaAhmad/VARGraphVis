using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ForceDirectedDiagram.Scripts.Helpers;
using SFB;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class ForceDirectedDiagramManager : MonoBehaviour
    {
        private int _nodesCount;
        private int _linksCount;

        private NodeStruct[] _allNodeStructs;
        private ForceStruct[] _forces;
        private NodeBase[] _allNodes;
        private readonly Dictionary<NodeBase, int> _nodesToInt = new();
        private LinkStruct[] _allLinkStructs;
        private LinkBase[] _allLinks;

        private readonly Dictionary<Tuple<NodeBase, NodeBase>, LinkBase> _linksSet = new(new SymmetricTupleComparer<NodeBase>());
        private readonly Dictionary<string, NodeBase> _idToNode = new();
        private readonly Dictionary<LinkBase, int> _indexToLink = new();
        private readonly Dictionary<NodeBase, List<LinkBase>> _nodeToLinks = new();

        private Transform _nodesContainer;
        private Transform _linksContainer;

        private int _currentNodeIndex;
        private int _currentLinkIndex;
        private int _previousNodeIndex;

        private DirectoryInfo _previousDirectory = new(@"C:\");

        [SerializeField] private ForceDiagramSourceDto diagramSource;

        [Header("Factories")] [Space(10)] 
        [RequireComponentType(typeof(NodeFactoryBase))] [SerializeField]
        private NodeFactoryBase nodeFactory;

        [RequireComponentType(typeof(LinkFactoryBase))] [SerializeField]
        private LinkFactoryBase linkFactory;
        
        [Header("Physics parameters")] [Space(10)] [SerializeField]
        private bool toggle2d;

        [SerializeField] [Range(0f, 1000f)] private float maxForcesMagnitude = 50f;
        [SerializeField] [Range(0f, 1000f)] private float repulsionStrength = 100f;
        [SerializeField] [Range(0f, 100f)] private float gravityStrength = 1f;
        [SerializeField] [Range(0f, 10f)] private float springStrength = 1f;

        [Header("Settings")] [Space(10)] 
        [RequireComponentType(typeof(ComputeShader))] 
        [SerializeField]
        private ComputeShader computationShader;

        [SerializeField] [Range(0f, 1000f)] private float nodesMaxDistance = 1000f;

        [Space(10)] [SerializeField] private ForceDirectedDiagramEvents diagramEvents;

        [Serializable]
        public struct NodeStruct
        {
            public Vector3 position;
            public int isFixed; // 0 if not, 1 if isFixed, we use this int workaround because bool is not blittable
        }

        [Serializable]
        public struct LinkStruct
        {
            public int sourceIndex;
            public int targetIndex;
            public float distanceBetweenNodes;
        }

        [Serializable]
        public struct ForceStruct
        {
            public Vector3 value;
        }

        private void Start()
        {
            ReloadDiagram();
            RefreshComputationShaderSettings();
        }

        private void Update()
        {
            var hasLinks = _allLinkStructs.Length > 0;
            var hasNodes = _allNodeStructs.Length > 0;

            if (!hasNodes) return;

            ProcessForces(hasLinks);
        }

        private void ProcessForces(bool useLinks)
        {
            const int kernelIndex = 0;

            ComputeBuffer linkBuffer = null;

            // Set up the compute shader buffers
            var nodeBuffer = new ComputeBuffer(_allNodeStructs.Length, sizeof(float) * 3 + sizeof(int));
            var forceBuffer = new ComputeBuffer(_allNodeStructs.Length, sizeof(float) * 3);
            if (useLinks) linkBuffer = new ComputeBuffer(_allLinkStructs.Length, sizeof(int) * 2 + sizeof(float));

            // Set the data in the compute shader buffers
            nodeBuffer.SetData(_allNodeStructs);
            forceBuffer.SetData(_forces);
            if (useLinks) linkBuffer.SetData(_allLinkStructs);

            // Set the compute shader buffers
            computationShader.SetBuffer(kernelIndex, "nodes", nodeBuffer);
            computationShader.SetBuffer(kernelIndex, "forces", forceBuffer);
            if (useLinks) computationShader.SetBuffer(kernelIndex, "links", linkBuffer);

            // Dispatch the compute shader
            computationShader.Dispatch(kernelIndex, Mathf.CeilToInt(_allNodeStructs.Length / 64f), 1, 1);

            // Read the forces from the compute shader buffer
            forceBuffer.GetData(_forces);

            for (var i = 0; i < _allNodeStructs.Length; i++)
            {
                var node = _allNodeStructs[i];

                SetNewPosition(ref node, _forces[i].value);

                _allNodeStructs[i] = node;
            }

            // Release the compute shader buffers
            nodeBuffer.Release();
            forceBuffer.Release();
            if (useLinks) linkBuffer.Release();

            // Apply the forces to the nodes
            UpdateNodes();
        }


        private void SetNewPosition(ref NodeStruct node, Vector3 newForce)
        {
            if (float.IsNaN(newForce.x) || float.IsNaN(newForce.y) || float.IsNaN(newForce.z))
            {
                newForce = Vector3.zero;
                node.position = Random.insideUnitSphere;
            }

            newForce = Vector3.ClampMagnitude(newForce, maxForcesMagnitude);

            var newPos = node.position + newForce * Time.deltaTime;

            var newClampedPos = toggle2d
                ? new Vector3(Mathf.Clamp(newPos.x, -nodesMaxDistance, nodesMaxDistance), Mathf.Clamp(newPos.y, -nodesMaxDistance, nodesMaxDistance), 0f)
                : Vector3.ClampMagnitude(newPos, nodesMaxDistance);

            node.position = newClampedPos;
        }

        private void UpdateNodes()
        {
            for (var i = 0; i < _allNodes.Length; i++)
            {
                var node = _allNodes[i];

                if (node != null)
                {
                    node.transform.position = _allNodeStructs[i].position;
                }
            }
        }

        [ContextMenu("ForceDirectedDiagram/Export nodes to json...")]
        public void ExportToJson()
        {
            var json = JsonUtility.ToJson(diagramSource, true);

            var savePanelParams = new SavePanelParams
            {
                Title = "Save nodes as .json",
                Directory = _previousDirectory.FullName,
                FileName = "nodes.json",
                ExtensionList = new[] { new ExtensionFilter("JSON files", "json") }
            };

            var path = ImportExportWrapper.SaveFilePanel(savePanelParams);

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            File.WriteAllText(path, json);
            diagramEvents.onMessageSent?.Invoke($"[{DateTime.Now:HH:mm:ss}] Diagram exported successfully");
        }

        [ContextMenu("ForceDirectedDiagram/Import diagram...")]
        public void ImportFromJson()
        {
            var openPanelParams = new OpenPanelParams
            {
                Title = "Import diagram",
                Directory = _previousDirectory.FullName,
                Extensions = new[] { new ExtensionFilter("JSON files", "json") },
                Multiselect = false
            };

            var path = ImportExportWrapper.OpenFilePanel(openPanelParams);

            if (path.Length <= 0) return;

            var directory = path[0];

            try
            {
                _previousDirectory = new DirectoryInfo(directory);

                var fileContent = File.ReadAllText(directory);

                var importedDiagram = JsonUtility.FromJson<ForceDiagramSourceDto>(fileContent);

                diagramSource = importedDiagram;
                ReloadDiagram();

                diagramEvents.onMessageSent?.Invoke($"[{DateTime.Now:HH:mm:ss}] Diagram imported successfully");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
        }

        [ContextMenu("ForceDirectedDiagram/Reload diagram")]
        public void ReloadDiagram()
        {
            ResetIndices();
            ResetDictionaries();
            CleanHierarchy();
            ResetArrays();
            CreateNodes();
            CreateLinks();
            NotifyDiagramGenerated();
        }

        private void ResetIndices()
        {
            _nodesCount = GetNodesCount();
            _linksCount = GetLinksCount();

            _currentNodeIndex = 0;
            _currentLinkIndex = 0;
        }

        private void ResetDictionaries()
        {
            _linksSet.Clear();
            _indexToLink.Clear();
            _idToNode.Clear();
            _nodeToLinks.Clear();
        }

        private void CleanHierarchy()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            _nodesContainer = CreateFolder("Nodes");
            _linksContainer = CreateFolder("Links");
        }

        private Transform CreateFolder(string title)
        {
            var folder = new GameObject(title);
            folder.transform.SetParent(transform);
            return folder.transform;
        }

        private void ResetArrays()
        {
            _allNodeStructs = new NodeStruct[_nodesCount];
            _allLinkStructs = new LinkStruct[_linksCount];
            _allNodes = new NodeBase[_nodesCount];
            _allLinks = new LinkBase[_linksCount];
            _forces = new ForceStruct[_nodesCount];
        }

        private void NotifyDiagramGenerated()
        {
            var diagramBuilt = new DiagramBuildContainer(_allNodes.Length > 0 ? _allNodes[0] : null, _nodeToLinks);
            diagramEvents.diagramGenerated?.Invoke(diagramBuilt);
        }

        private int GetLinksCount()
        {
            return diagramSource.links.Count;
        }

        private int GetNodesCount()
        {
            return diagramSource.nodes.Count;
        }

        private void CreateNodes()
        {
            foreach (var node in diagramSource.nodes)
            {
                AddNode(node, Random.insideUnitSphere);
            }
        }

        private void CreateLinks()
        {
            foreach (var link in diagramSource.links)
            {
                AddLink(link);
            }
        }

        private void AddLink(LinkDto link)
        {
            var linkComponent = linkFactory.CreateInstance(link, _linksContainer, _idToNode);

            var sourceNode = linkComponent.sourceNode;
            var targetNode = linkComponent.targetNode;

            _allLinkStructs[_currentLinkIndex] = new LinkStruct
            {
                sourceIndex = _nodesToInt[sourceNode],
                targetIndex = _nodesToInt[targetNode],
                distanceBetweenNodes = linkComponent.distanceBetweenNodes
            };

            _allLinks[_currentLinkIndex] = linkComponent;

            AddToNodeLinksDictionary(sourceNode, linkComponent);
            AddToNodeLinksDictionary(targetNode, linkComponent);

            _indexToLink.Add(linkComponent, _currentLinkIndex);

            if (!_linksSet.TryAdd(new Tuple<NodeBase, NodeBase>(sourceNode, targetNode), linkComponent))
            {
                Debug.Log($"Link with source id {sourceNode.id} and target id {targetNode.id} has already been added.");
            }

            _currentLinkIndex++;
        }

        private void AddToNodeLinksDictionary(NodeBase sourceNode, LinkBase linkComponent)
        {
            if (_nodeToLinks.TryGetValue(sourceNode, out var listLinksSource))
            {
                listLinksSource.Add(linkComponent);
            }
            else
            {
                _nodeToLinks.Add(sourceNode, new List<LinkBase> { linkComponent });
            }
        }

        private NodeBase AddNode(NodeDto node, Vector3 position)
        {
            var nodeComponent = nodeFactory.CreateInstance(node, _nodesContainer, position);

            _allNodeStructs[_currentNodeIndex] = new NodeStruct
            {
                position = nodeComponent.transform.position,
                isFixed = nodeComponent.isFixed.ToInt()
            };

            _allNodes[_currentNodeIndex] = nodeComponent;
            _nodesToInt.Add(nodeComponent, _currentNodeIndex);

            _currentNodeIndex++;

            _idToNode.Add(node.id, nodeComponent);

            _nodeToLinks.TryAdd(nodeComponent, new List<LinkBase>());

            return nodeComponent;
        }

        public void UpdateNodeStructFor(NodeBase nodeBase)
        {
            if (_nodesToInt.TryGetValue(nodeBase, out var index))
            {
                _allNodeStructs[index].position = nodeBase.transform.position;
            }
            else
            {
                Debug.Log("Could not parse ");
            }
        }

        public void UnlinkNodes(List<NodeBase> nodesToUnlink)
        {
            foreach (var nodeSource in nodesToUnlink)
            {
                foreach (var nodeTarget in nodesToUnlink.Except(new[] { nodeSource }))
                {
                    var linkTuple = new Tuple<NodeBase, NodeBase>(nodeSource, nodeTarget);

                    if (_linksSet.TryGetValue(linkTuple, out var linkToRemove))
                    {
                        RemoveLink(linkToRemove);
                        _linksSet.Remove(linkTuple);
                    }
                }
            }
        }

        private void RemoveLink(LinkBase linkToRemove)
        {
            RemoveLinkFromNodeLinks(linkToRemove.sourceNode, linkToRemove);
            RemoveLinkFromNodeLinks(linkToRemove.targetNode, linkToRemove);

            var indexOfLink = _indexToLink[linkToRemove];

            var newAllLinks = new List<LinkBase>(_allLinks);
            var newAllLinksStruct = new List<LinkStruct>(_allLinkStructs);

            newAllLinks.RemoveAt(indexOfLink);
            newAllLinksStruct.RemoveAt(indexOfLink);

            _allLinks = newAllLinks.ToArray();
            _allLinkStructs = newAllLinksStruct.ToArray();

            foreach (var link in _allLinks)
            {
                if (_indexToLink.TryGetValue(link, out var index))
                {
                    if (index >= indexOfLink)
                    {
                        _indexToLink[link] = index - 1;
                    }
                }
            }

            diagramSource.links.RemoveAll(o => o.source == linkToRemove.sourceNode.id && o.target == linkToRemove.targetNode.id);
            _indexToLink.Remove(linkToRemove);

            _currentLinkIndex--;
            Destroy(linkToRemove.gameObject);
        }

        private void RemoveLinkFromNodeLinks(NodeBase node, LinkBase linkToRemove)
        {
            if (_nodeToLinks.TryGetValue(node, out var links))
            {
                links.Remove(linkToRemove);
            }
        }

        public void NewDiagram()
        {
            diagramEvents.onMessageSent?.Invoke($"[{DateTime.Now:HH:mm:ss}] New diagram created");

            diagramSource = new ForceDiagramSourceDto();

            ReloadDiagram();
        }

        public void LinkNodes(List<NodeBase> nodesToLink)
        {
            var linksCount = nodesToLink.Count * (nodesToLink.Count - 1);
            if (linksCount > 10000)
            {
                var errorMessage = $"Can not create more than 10 000 links ({linksCount})";
                Debug.Log(errorMessage);
                diagramEvents.onMessageSent?.Invoke($"[{DateTime.Now:HH:mm:ss}] {errorMessage}");
                return;
            }

            foreach (var nodeSource in nodesToLink)
            {
                foreach (var nodeTarget in nodesToLink.Except(new[] { nodeSource }))
                {
                    if (!_linksSet.ContainsKey(new Tuple<NodeBase, NodeBase>(nodeSource, nodeTarget)))
                    {
                        CreateLinkBetween(nodeSource, nodeTarget);
                    }
                }
            }
        }

        public void LinkNodesInOrder(List<NodeBase> nodesToLink)
        {
            for (var i = 0; i < nodesToLink.Count - 1; i++)
            {
                var nodeSource = nodesToLink[i];
                var nodeTarget = nodesToLink[i + 1];

                if (!_linksSet.ContainsKey(new Tuple<NodeBase, NodeBase>(nodeSource, nodeTarget)))
                {
                    CreateLinkBetween(nodeSource, nodeTarget);
                }
            }
        }

        public void LinkNodesInOrderAndClose(List<NodeBase> nodesToLink)
        {
            for (var i = 0; i < nodesToLink.Count - 1; i++)
            {
                var nodeSource = nodesToLink[i];
                var nodeTarget = nodesToLink[i + 1];

                if (!_linksSet.ContainsKey(new Tuple<NodeBase, NodeBase>(nodeSource, nodeTarget)))
                {
                    CreateLinkBetween(nodeSource, nodeTarget);
                }
            }

            // Close

            var lastNode = nodesToLink.Last();
            var firstNode = nodesToLink.First();

            if (!_linksSet.ContainsKey(new Tuple<NodeBase, NodeBase>(lastNode, firstNode)))
            {
                CreateLinkBetween(lastNode, firstNode);
            }
        }

        public void DeleteNodes(List<NodeBase> nodesToDelete)
        {
            for (var i = nodesToDelete.Count - 1; i >= 0; i--)
            {
                var nodeToDelete = nodesToDelete[i];

                // DELETE LINKS
                if (_nodeToLinks.TryGetValue(nodeToDelete, out var linksToRemove))
                {
                    for (var j = linksToRemove.Count - 1; j >= 0; j--)
                    {
                        var linkToRemove = linksToRemove[j];
                        RemoveLink(linkToRemove);

                        _linksSet.Remove(new Tuple<NodeBase, NodeBase>(linkToRemove.sourceNode, linkToRemove.targetNode));
                    }
                }

                // DELETE NODE
                RemoveNode(nodeToDelete);
            }
        }

        private void RemoveNode(NodeBase nodeToDelete)
        {
            Array.Resize(ref _forces, _forces.Length - 1);

            var indexOfNode = _nodesToInt[nodeToDelete];

            var newAllNodes = new NodeBase[_allNodes.Length - 1];
            var newAllNodesStruct = new NodeStruct[_allNodeStructs.Length - 1];

            for (var i = 0; i < newAllNodes.Length; i++)
            {
                if (i < indexOfNode)
                {
                    newAllNodes[i] = _allNodes[i];
                    newAllNodesStruct[i] = _allNodeStructs[i];
                }
                else
                {
                    newAllNodes[i] = _allNodes[i + 1];
                    newAllNodesStruct[i] = _allNodeStructs[i + 1];
                }
            }

            _allNodes = newAllNodes;

            _allNodeStructs = newAllNodesStruct;

            _idToNode.Remove(nodeToDelete.id);

            _nodeToLinks.Remove(nodeToDelete);

            foreach (var node in newAllNodes)
            {
                if (_nodesToInt.TryGetValue(node, out var index))
                {
                    int newIndex;

                    if (index < indexOfNode)
                    {
                        newIndex = index;
                    }
                    else
                    {
                        newIndex = index - 1;
                        RefreshLinkStructsIndexes(index, newIndex);
                    }

                    _nodesToInt[node] = newIndex;
                }
                else
                {
                    Debug.Log("shouldnt happen");
                }
            }

            diagramSource.nodes.RemoveAll(o => o.id == nodeToDelete.id);
            _nodesToInt.Remove(nodeToDelete);

            _currentNodeIndex--;

            diagramEvents.nodeRemoved?.Invoke(nodeToDelete);

            Destroy(nodeToDelete.gameObject);
        }

        private void RefreshLinkStructsIndexes(int oldValue, int newValue)
        {
            for (var i = 0; i < _allLinkStructs.Length; i++)
            {
                var linkStruct = _allLinkStructs[i];

                if (linkStruct.sourceIndex == oldValue)
                {
                    linkStruct.sourceIndex = newValue;
                }

                if (linkStruct.targetIndex == oldValue)
                {
                    linkStruct.targetIndex = newValue;
                }

                _allLinkStructs[i] = linkStruct;
            }
        }

        public void CreateLinkedNode(List<NodeBase> nodesToLinkTo)
        {
            foreach (var targetNodes in nodesToLinkTo)
            {
                var sourceNode = GetIndependantNodeAtPosition(targetNodes.transform.position + Random.onUnitSphere);
                CreateLinkBetween(sourceNode, targetNodes);
            }
        }

        public void CreateLinkedNodesAndSelect(List<NodeBase> nodesToLinkTo)
        {
            var listNodesToSelect = new List<NodeBase>();
            var listNodesToDeselect = new List<NodeBase>(nodesToLinkTo);

            foreach (var targetNodes in nodesToLinkTo)
            {
                var sourceNode = GetIndependantNodeAtPosition(targetNodes.transform.position + Random.onUnitSphere);
                CreateLinkBetween(sourceNode, targetNodes);
                listNodesToSelect.Add(sourceNode);
            }

            diagramEvents.selectEvent?.Invoke(listNodesToSelect);
            diagramEvents.deselectEvent?.Invoke(listNodesToDeselect);
        }

        private void CreateLinkBetween(NodeBase sourceNode, NodeBase targetNode)
        {
            Array.Resize(ref _allLinkStructs, _allLinkStructs.Length + 1);
            Array.Resize(ref _allLinks, _allLinks.Length + 1);

            var linkDto = new LinkDto
            {
                source = sourceNode.id,
                target = targetNode.id,
                group = sourceNode.group,
                length = 1
            };

            diagramSource.links.Add(linkDto);

            AddLink(linkDto);
        }

        public void CreateIndependantNode()
        {
            GetIndependantNode();
        }

        private void GetIndependantNode()
        {
            Array.Resize(ref _allNodeStructs, _allNodeStructs.Length + 1);
            Array.Resize(ref _allNodes, _allNodes.Length + 1);
            Array.Resize(ref _forces, _forces.Length + 1);

            var nodeId = GetNewNodeId();

            var node = new NodeDto()
            {
                id = nodeId,
                group = 0,
                label = nodeId
            };

            diagramSource.nodes.Add(node);

            AddNode(node, Random.insideUnitSphere);
        }

        private NodeBase GetIndependantNodeAtPosition(Vector3 position)
        {
            Array.Resize(ref _allNodeStructs, _allNodeStructs.Length + 1);
            Array.Resize(ref _allNodes, _allNodes.Length + 1);
            Array.Resize(ref _forces, _forces.Length + 1);

            var nodeId = GetNewNodeId();

            var node = new NodeDto()
            {
                id = nodeId,
                group = 0,
                label = nodeId
            };

            diagramSource.nodes.Add(node);

            return AddNode(node, position);
        }

        private string GetNewNodeId()
        {
            var existingIds = new HashSet<string>(diagramSource.nodes.Select(node => node.id));
            string newName;

            do
            {
                newName = $"New node {_previousNodeIndex}";
                _previousNodeIndex++;
            } while (existingIds.Contains(newName));

            return newName;
        }

        public void RefreshPreviousNodeIndex()
        {
            _previousNodeIndex = 0;

            var allIds = diagramSource.nodes.Select(o => o.id).ToHashSet();

            string newName;
            do
            {
                newName = $"New node {_previousNodeIndex}";
                _previousNodeIndex++;
            } while (allIds.Contains(newName));
        }

        public void SelectAllNodes()
        {
            diagramEvents.selectEvent?.Invoke(_allNodes.ToList());
        }

        public Dictionary<NodeBase, List<LinkBase>> GetNodesToLinks()
        {
            return _nodeToLinks;
        }

        private void OnValidate()
        {
            RefreshComputationShaderSettings();
        }

        private void RefreshComputationShaderSettings()
        {
            if (computationShader != null)
            {
                computationShader.SetFloat("_springStrength", springStrength);
                computationShader.SetFloat("_repulsionStrength", repulsionStrength);
                computationShader.SetFloat("_gravityStrength", gravityStrength);
            }
        }

        public void UpdateNodesLabel(NodeSelector.StringAndNodes nodesAndNewLabel)
        {
            foreach (var nodeBase in nodesAndNewLabel.Nodes)
            {
                UpdateNodeLabel(nodeBase, nodesAndNewLabel.NewLabel);
            }

            diagramEvents.nodesLabelUpdated?.Invoke();
        }

        private void UpdateNodeLabel(NodeBase nodeBase, string newLabel)
        {
            nodeBase.label = newLabel;
            diagramSource.nodes.ElementAt(_nodesToInt[nodeBase]).label = newLabel;
        }

        public void Toggle2D(bool is2d)
        {
            toggle2d = is2d;
        }

        public void SetRepulsion(float repulsion)
        {
            repulsionStrength = repulsion;
            computationShader.SetFloat("_repulsionStrength", repulsionStrength);
        }

        public void SetGravity(float gravity)
        {
            gravityStrength = gravity;
            computationShader.SetFloat("_gravityStrength", gravityStrength);
        }

        public void SetSpringForce(float springForce)
        {
            springStrength = springForce;
            computationShader.SetFloat("_springStrength", springStrength);
        }
    }
}