using System.Collections.Generic;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class TradingSelector : MonoBehaviour
    {
        [SerializeField] private ForceDirectedDiagramManager forceDirectedDiagramManager;

        private readonly List<SelectableForceDirectDiagramObject> _selectedNodes = new();
        private readonly List<SelectableForceDirectDiagramObject> _selectedLinks = new();

        public void NodeHovered(NodeBase node)
        {
            var selectable = node.GetComponent<SelectableForceDirectDiagramObject>();

            if (selectable != null)
            {
                selectable.UpdateSelectedState(true);
                _selectedNodes.Add(selectable);
            }

            if (forceDirectedDiagramManager.GetNodesToLinks().TryGetValue(node, out var links))
            {
                foreach (var link in links)
                {
                    if (link.TryGetComponent<SelectableForceDirectDiagramObject>(out var linkSelectable))
                    {
                        linkSelectable.UpdateSelectedState(true);
                        _selectedLinks.Add(linkSelectable);
                    }

                    if (link.sourceNode != node)
                    {
                        var selectableLinkNode = link.sourceNode.GetComponent<SelectableForceDirectDiagramObject>();

                        if (selectableLinkNode != null)
                        {
                            selectableLinkNode.UpdateSelectedState(true);
                            _selectedNodes.Add(selectableLinkNode);
                        }
                    }
                    else if (link.targetNode != node)
                    {
                        var selectableLinkNode = link.targetNode.GetComponent<SelectableForceDirectDiagramObject>();

                        if (selectableLinkNode != null)
                        {
                            selectableLinkNode.UpdateSelectedState(true);
                            _selectedNodes.Add(selectableLinkNode);
                        }
                    }
                }
            }
        }

        public void ResetNodes()
        {
            foreach (var selectedNode in _selectedNodes)
            {
                selectedNode.UpdateSelectedState(false);
            }

            foreach (var selectedLink in _selectedLinks)
            {
                selectedLink.UpdateSelectedState(false);
            }

            _selectedLinks.Clear();
            _selectedNodes.Clear();
        }
    }
}