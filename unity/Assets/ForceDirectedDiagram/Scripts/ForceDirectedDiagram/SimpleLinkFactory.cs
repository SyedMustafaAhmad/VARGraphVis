using System.Collections.Generic;
using ForceDirectedDiagram.Scripts.Helpers;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class SimpleLinkFactory : LinkFactoryBase
    {
        [RequireComponentType(typeof(LinkBase))]
        [SerializeField] private GameObject defaultPrefab;
    
        [SerializeField] private float baseDistanceBetweenNodes;

        public override LinkBase CreateInstance(LinkDto link, Transform linksContainer, Dictionary<string, NodeBase> idToNode)
        {
            var linkPrefab = defaultPrefab;

            var sourceNode = idToNode[link.source];
            var targetNode = idToNode[link.target];

            var linkGo = Instantiate(linkPrefab, linksContainer, true);
            linkGo.name = $"Link from {sourceNode.label} to {targetNode.label}";
            var linkComponent = linkGo.GetComponent<LinkBase>();

            linkComponent.sourceNode = sourceNode;
            linkComponent.targetNode = targetNode;
            linkComponent.group = link.group;
            linkComponent.length = link.length;
            linkComponent.description = link.description;

            if (link.group > 0)
            {
                linkComponent.distanceBetweenNodes = baseDistanceBetweenNodes / link.group;
            }
            else
            {
                linkComponent.distanceBetweenNodes = baseDistanceBetweenNodes;
            }

            return linkComponent;
        }
    }
}