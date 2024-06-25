using System.Collections.Generic;
using ForceDirectedDiagram.Scripts.Helpers;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class ColorShapeTextNodeFactory : NodeFactoryBase
    {
        [RequireComponentType(typeof(ColorShapeTextNode))]
        [SerializeField] private GameObject defaultPrefab;

        [SerializeField] private List<Color> nodeColorByGroup = new();
        [SerializeField] private List<Mesh> nodeShapeBySubgroup = new();
        [SerializeField] private Camera defaultCamera;

        public override NodeBase CreateInstance(NodeDto node, Transform nodesContainer, Vector3 position)
        {
            var nodeGo = Instantiate(defaultPrefab, position, Quaternion.identity);

            if (nodeGo.TryGetComponent<ColorShapeTextNode>(out var nodeComponent))
            {
                if (node.group < nodeColorByGroup.Count)
                {
                    var colorIndex = node.group;
                    nodeComponent.SetColor(nodeColorByGroup[colorIndex]);
                }
                else
                {
                    Debug.LogError($"Group index {node.group} was outside of the range of colors array.");
                }

                if (node.subgroup < nodeShapeBySubgroup.Count)
                {
                    var meshIndex = node.subgroup;
                    nodeComponent.SetMesh(nodeShapeBySubgroup[meshIndex]);
                }
                else
                {
                    Debug.LogError($"Subgroup index {node.subgroup}  was outside of the range of meshes array.");
                }
            }
            else
            {
                Debug.LogError("Could not find ColorAndShapeNode component on selected node.");
            }
            
            nodeGo.transform.SetParent(nodesContainer);
            nodeGo.name = node.label;

            nodeComponent.id = node.id;
            nodeComponent.label = node.label;
            nodeComponent.group = node.group;
            nodeComponent.subgroup = node.subgroup;
            nodeComponent.groupname = node.groupname;
            nodeComponent.isFixed = node.isFixed;

            nodeComponent.faceCamera.Cam = defaultCamera;
            nodeComponent.UpdateLabelMesh();

            if (node.isFixed)
            {
                nodeComponent.transform.position = node.fixedPosition;
            }
            
            return nodeComponent;
        }
    }
}