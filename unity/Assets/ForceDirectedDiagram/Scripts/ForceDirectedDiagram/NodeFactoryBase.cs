using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    public abstract class NodeFactoryBase : MonoBehaviour
    {
        public abstract NodeBase CreateInstance(NodeDto node, Transform nodesContainer, Vector3 position);
    }
}