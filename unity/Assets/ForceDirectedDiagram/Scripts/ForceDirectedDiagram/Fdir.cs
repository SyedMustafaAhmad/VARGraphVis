using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    public class Fdir : MonoBehaviour
    {
        [Serializable]
        public class NodeFdir
        {
            [SerializeField] internal NodeBase node;
            [SerializeField] internal SelectableForceDirectDiagramObject selectComponent;
            [SerializeField] internal NodeFdir parent;
            [SerializeField] internal LinkBase linkFromParent;
            [SerializeField] internal SelectableForceDirectDiagramObject linkFromParentSelectComponent;
            [SerializeField] internal List<NodeFdir> children;
            [SerializeField] internal int depth;
        }

        [SerializeField] private NodeFdir fdirDiagram;
        [SerializeField] private UnityEvent<string> failureChanged;
       
        private int _currentDepth;
        private string _failureText;
        private List<NodeBase> _failNodes = new();
        private readonly HashSet<LinkBase> _linkDiscovered = new();

        public void BuildFdirDiagram(DiagramBuildContainer diagramSource)
        {
            fdirDiagram = AddNodeAndChildren(diagramSource, diagramSource.RootNode, null, null);
        }

        private NodeFdir AddNodeAndChildren(DiagramBuildContainer diagramBuildContainer, NodeBase node, NodeFdir parent, LinkBase linkFromParent)
        {
            var newNodeFdir = new NodeFdir()
            {
                node = node,
                parent = parent,
                depth = _currentDepth,
                linkFromParent = linkFromParent
            };

            var selectComponent = node.GetComponent<SelectableForceDirectDiagramObject>();

            if (selectComponent != null)
            {
                newNodeFdir.selectComponent = selectComponent;
            }

            if (linkFromParent != null)
            {
                var linkFromParentSelectComponent = linkFromParent.GetComponent<SelectableForceDirectDiagramObject>();

                if (linkFromParentSelectComponent != null)
                {
                    newNodeFdir.linkFromParentSelectComponent = linkFromParentSelectComponent;
                }
            }

            newNodeFdir.children = GetNodeChildren(diagramBuildContainer, newNodeFdir);

            return newNodeFdir;
        }

        private List<NodeFdir> GetNodeChildren(DiagramBuildContainer diagramBuildContainer, NodeFdir newNodeFdir)
        {
            _currentDepth++;

            var children = new List<NodeFdir>();

            foreach (var link in diagramBuildContainer.NodesToLink[newNodeFdir.node])
            {
                if (!_linkDiscovered.Contains(link))
                {
                    _linkDiscovered.Add(link);

                    if (link.sourceNode != newNodeFdir.node)
                    {
                        children.Add(AddNodeAndChildren(diagramBuildContainer, link.sourceNode, newNodeFdir, link));
                    }
                    else if (link.targetNode != newNodeFdir.node)
                    {
                        children.Add(AddNodeAndChildren(diagramBuildContainer, link.targetNode, newNodeFdir, link));
                    }
                }
            }

            _currentDepth--;

            return children;
        }

        public void GenerateRandomFailure()
        {
            _failNodes = new List<NodeBase>();
            _failureText = string.Empty;

            ResetSelection(fdirDiagram);

            GenerateRandomFailureAt(fdirDiagram);

            _failureText = UpdateFailureText(_failNodes);

            failureChanged?.Invoke(_failureText);
        }

        private static string UpdateFailureText(IEnumerable<NodeBase> failNodes)
        {
            return string.Join(" > ", failNodes.Select(o => o.label));
        }

        private static void ResetSelection(NodeFdir fdirDiagramNode)
        {
            var selectComponent = fdirDiagramNode.selectComponent;

            if (selectComponent != null)
            {
                selectComponent.UpdateSelectedState(false);
            }

            var linkFromParentSelect = fdirDiagramNode.linkFromParentSelectComponent;

            if (linkFromParentSelect != null)
            {
                linkFromParentSelect.UpdateSelectedState(false);
            }

            foreach (var child in fdirDiagramNode.children)
            {
                ResetSelection(child);
            }
        }

        private void GenerateRandomFailureAt(NodeFdir fdirDiagramNode)
        {
            _failNodes.Add(fdirDiagramNode.node);

            var selectComponent = fdirDiagramNode.selectComponent;

            if (selectComponent != null)
            {
                selectComponent.UpdateSelectedState(true);
            }

            var linkFromParentSelectComponent = fdirDiagramNode.linkFromParentSelectComponent;

            if (linkFromParentSelectComponent != null)
            {
                linkFromParentSelectComponent.UpdateSelectedState(true);
            }

            if (fdirDiagramNode.children.Count > 0)
            {
                var randomChildrenIndex = Random.Range(0, fdirDiagramNode.children.Count);

                GenerateRandomFailureAt(fdirDiagramNode.children[randomChildrenIndex]);
            }
        }
    }
}