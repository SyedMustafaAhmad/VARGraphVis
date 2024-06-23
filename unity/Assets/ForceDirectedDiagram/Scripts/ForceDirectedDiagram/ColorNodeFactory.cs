using System.Collections.Generic;
using ForceDirectedDiagram.Scripts.Helpers;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class ColorNodeFactory : NodeFactoryBase
    {
        [RequireComponentType(typeof(ColorNode))]
        [SerializeField] private GameObject defaultPrefab;

        [SerializeField] private List<Color> nodeColorByGroup = new();

        public override NodeBase CreateInstance(NodeDto node, Transform nodesContainer, Vector3 position)
        {
            var nodeGo = Instantiate(defaultPrefab, position, Quaternion.identity);

            if (nodeGo.TryGetComponent<ColorNode>(out var nodeComponent))
            {
                if (node.group < nodeColorByGroup.Count)
                {
                    var colorIndex = node.group;
                    nodeComponent.SetColor(nodeColorByGroup[colorIndex]);
                }
                else
                {
                    Debug.Log($"Group index {node.group} was outside of the range of colors array.");
                }
            }
            else
            {
                Debug.LogError($"Could not find {nameof(ColorNode)} component on selected node.");
            }

            nodeGo.transform.SetParent(nodesContainer);
            nodeGo.name = node.id;

            nodeComponent.id = node.id;
            nodeComponent.label = node.id;
            nodeComponent.group = node.group;
            nodeComponent.subgroup = node.subgroup;
            nodeComponent.groupname = node.groupname;
            nodeComponent.isFixed = node.isFixed;

            if (node.isFixed)
            {
                nodeComponent.transform.position = node.fixedPosition;
            }
        
            return nodeComponent;
        }
    }
}