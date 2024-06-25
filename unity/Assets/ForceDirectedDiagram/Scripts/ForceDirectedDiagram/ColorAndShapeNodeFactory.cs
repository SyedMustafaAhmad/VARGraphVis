using System.Collections.Generic;
using ForceDirectedDiagram.Scripts.Helpers;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class ColorAndShapeNodeFactory : NodeFactoryBase
    {
        [RequireComponentType(typeof(ColorAndShapeNode))]
        [SerializeField] private GameObject defaultPrefab;

        [SerializeField] private List<Color> nodeColorByGroup = new();
        [SerializeField] private List<Mesh> nodeShapeBySubgroup = new();

        public override NodeBase CreateInstance(NodeDto node, Transform nodesContainer, Vector3 position)
        {
            var nodeGo = Instantiate(defaultPrefab, position, Quaternion.identity);

            if (nodeGo.TryGetComponent<ColorAndShapeNode>(out var nodeComponent))
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

                if (node.subgroup < nodeShapeBySubgroup.Count)
                {
                    var meshIndex = node.subgroup;
                    nodeComponent.SetMesh(nodeShapeBySubgroup[meshIndex]);
                }
                else
                {
                    Debug.Log($"Subgroup index {node.subgroup}  was outside of the range of meshes array.");
                }
            }
            else
            {
                Debug.LogError($"Could not find {nameof(ColorAndShapeNode)} component on selected node.");
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