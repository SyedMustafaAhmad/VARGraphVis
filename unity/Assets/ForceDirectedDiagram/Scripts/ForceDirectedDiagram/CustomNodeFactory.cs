using System.Collections.Generic;
using ForceDirectedDiagram.Scripts.Helpers;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class CustomNodeFactory : NodeFactoryBase
    {
        [RequireComponentType(typeof(NodeBase))]
        [SerializeField] private GameObject defaultPrefab;

        [SerializeField] private List<GameObject> nodePrefabByGroup = new();

        private GameObject GetNodePrefab(NodeDto nodeDto)
        {
            if (nodeDto.group >= 0 && nodeDto.group < nodePrefabByGroup.Count)
            {
                return nodePrefabByGroup[nodeDto.group];
            }

            return defaultPrefab;
        }

        public override NodeBase CreateInstance(NodeDto node, Transform nodesContainer, Vector3 position)
        {
            var prefab = GetNodePrefab(node);

            var nodeGo = Instantiate(prefab, position, Quaternion.identity);
            nodeGo.transform.SetParent(nodesContainer);
            nodeGo.name = node.id;
            
            var nodeComponent = nodeGo.GetComponent<NodeBase>();

            nodeComponent.id = node.id;
            nodeComponent.label = node.id;
            nodeComponent.group = node.group;
            nodeComponent.isFixed = node.isFixed;
            nodeComponent.subgroup = node.subgroup;
            nodeComponent.groupname = node.groupname;
            
            if (node.isFixed)
            {
                nodeComponent.transform.position = node.fixedPosition;
            }
            
            return nodeComponent;
        }
    }
}