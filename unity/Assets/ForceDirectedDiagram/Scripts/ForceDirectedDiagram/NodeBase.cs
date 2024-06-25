using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    public abstract class NodeBase : MonoBehaviour
    {
        public string id;
        public string label;
        public bool isFixed;
        public int group;
        public int subgroup;
        public string groupname;
    }
}