using ForceDirectedDiagram.Scripts.Helpers;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class TextNodeFactory : NodeFactoryBase
    {
        [RequireComponentType(typeof(TextNode))]
        [SerializeField] private GameObject defaultPrefab;
        [SerializeField] public bool showLabelsByDefault = true;
        
        [SerializeField] private Camera defaultCamera;

        public override NodeBase CreateInstance(NodeDto node, Transform nodesContainer, Vector3 position)
        {
            var prefab = defaultPrefab;

            var nodeGo = Instantiate(prefab, position, Quaternion.identity);
            nodeGo.transform.SetParent(nodesContainer);
            nodeGo.name = node.label;

            var nodeComponent = nodeGo.GetComponent<TextNode>();

            nodeComponent.id = node.id;
            nodeComponent.label = node.label;
            nodeComponent.group = node.group;
            nodeComponent.isFixed = node.isFixed;

            foreach (var faceCam in nodeComponent.faceCamera)
            {
                faceCam.Cam = defaultCamera;
            }

            nodeComponent.RefreshLabelMesh();
            nodeComponent.ShowHideLabel(showLabelsByDefault);
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