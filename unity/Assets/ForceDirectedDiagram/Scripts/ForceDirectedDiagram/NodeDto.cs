using System;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    [Serializable]
    public class NodeDto
    {
        public string label;
        public string description;
        public string id;
        public int group;
        public int subgroup;
        public string groupname;
        public int depth;
        public bool isFixed;
        public Vector3 fixedPosition;
    }
}