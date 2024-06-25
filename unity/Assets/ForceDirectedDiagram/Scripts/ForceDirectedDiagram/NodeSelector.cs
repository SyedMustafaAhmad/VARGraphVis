using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class NodeSelector : MonoBehaviour
    {
        private readonly List<SelectableForceDirectDiagramObject> _selectedSelectableNodes = new();
        private HashSet<NodeBase> _selectedNodes = new();

        [SerializeField] private UnityEvent<List<NodeBase>> nodeSelected;
        [SerializeField] private UnityEvent<StringAndNodes> nodesLabelChanged;

        public void UnselectNodes()
        {
            if (!(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                ResetSelectedNodes();
            
                RefreshNodesSelected();
                nodeSelected?.Invoke(_selectedNodes.ToList());
            }
        }

        public sealed class StringAndNodes
        {
            public List<NodeBase> Nodes;
            public string NewLabel;

            public StringAndNodes(List<NodeBase> nodes, string newLabel)
            {
                Nodes = nodes;
                NewLabel = newLabel;
            }
        }
    
        public void UpdateNodesLabel(string newLabel)
        {
            nodesLabelChanged?.Invoke(new StringAndNodes(_selectedNodes.ToList(), newLabel));
        }

        public void RightHover(NodeBase node)
        {
            var selectableNode = node.GetComponent<SelectableForceDirectDiagramObject>();

            if (selectableNode == null) return;

            if (!_selectedSelectableNodes.Contains(selectableNode))
            {
                _selectedSelectableNodes.Add(selectableNode);
                selectableNode.UpdateSelectedState(true);
        
                RefreshNodesSelected();
                nodeSelected?.Invoke(_selectedNodes.ToList());
            }
        }
    
        public void SelectNode(NodeBase node)
        {
            var selectableNode = node.GetComponent<SelectableForceDirectDiagramObject>();

            if (selectableNode != null)
            {
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    if (_selectedSelectableNodes.Contains(selectableNode))
                    {
                        _selectedSelectableNodes.Remove(selectableNode);
                        selectableNode.UpdateSelectedState(false);
                    }
                    else
                    {
                        _selectedSelectableNodes.Add(selectableNode);
                        selectableNode.UpdateSelectedState(true);
                    }
                }
                else
                {
                    ResetSelectedNodes();

                    _selectedSelectableNodes.Add(selectableNode);
                    selectableNode.UpdateSelectedState(true);
                }
            }

            RefreshNodesSelected();
            nodeSelected?.Invoke(_selectedNodes.ToList());
        }

        private void RefreshNodesSelected()
        {
            _selectedNodes = _selectedSelectableNodes.Select(o => o.GetComponent<NodeBase>()).ToHashSet();
        }

        public void ResetSelectedNodes()
        {
            foreach (var node in _selectedSelectableNodes)
            {
                node.UpdateSelectedState(false);
            }

            _selectedSelectableNodes.Clear();
        }

        public void NodeRemoved(NodeBase node)
        {
            if (_selectedNodes.Contains(node))
            {
                _selectedNodes.Remove(node);
                _selectedSelectableNodes.Remove(node.GetComponent<SelectableForceDirectDiagramObject>());

                nodeSelected?.Invoke(_selectedNodes.ToList());
            }
        }

        public void Select(List<NodeBase> nodesToSelect)
        {
            _selectedSelectableNodes.Clear();
            _selectedNodes.Clear();

            foreach (var node in nodesToSelect)
            {
                var selectableNode = node.GetComponent<SelectableForceDirectDiagramObject>();
                _selectedSelectableNodes.Add(selectableNode);
                selectableNode.UpdateSelectedState(true);
            }

            RefreshNodesSelected();
            nodeSelected?.Invoke(_selectedNodes.ToList());
        }
    
        public void Deselect(List<NodeBase> nodesToDeselect)
        {
            foreach (var node in nodesToDeselect)
            {
                var selectableNode = node.GetComponent<SelectableForceDirectDiagramObject>();

                if (_selectedSelectableNodes.Contains(selectableNode))
                {
                    _selectedSelectableNodes.Remove(selectableNode);
                }
            
                selectableNode.UpdateSelectedState(false);
            }

            RefreshNodesSelected();
            nodeSelected?.Invoke(_selectedNodes.ToList());
        }
    }
}