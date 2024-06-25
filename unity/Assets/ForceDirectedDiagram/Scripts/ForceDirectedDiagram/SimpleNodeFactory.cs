using ForceDirectedDiagram.Scripts.Helpers;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class SimpleNodeFactory : NodeFactoryBase
    {
        [RequireComponentType(typeof(NodeBase))]
        [SerializeField] private GameObject defaultPrefab;

        public override NodeBase CreateInstance(NodeDto node, Transform nodesContainer, Vector3 position)
        {
            var prefab = defaultPrefab;

            var nodeGo = Instantiate(prefab, position, Quaternion.identity);
            nodeGo.transform.SetParent(nodesContainer);
            nodeGo.name = node.label;

            var nodeComponent = nodeGo.GetComponent<NodeBase>();

            nodeComponent.id = node.id;
            nodeComponent.label = node.label;
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