using System.Collections.Generic;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    public abstract class LinkFactoryBase : MonoBehaviour
    {
        public abstract LinkBase CreateInstance(LinkDto link, Transform linksContainer, Dictionary<string, NodeBase> idToNode);
    }
}