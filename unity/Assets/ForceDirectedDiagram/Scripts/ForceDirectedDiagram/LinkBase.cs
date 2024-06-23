using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    public abstract class LinkBase : MonoBehaviour
    {
        public NodeBase sourceNode;
        public NodeBase targetNode;
        public int length;
        public int group;
        public string description;

        [SerializeField] public float distanceBetweenNodes;
    
    }
}