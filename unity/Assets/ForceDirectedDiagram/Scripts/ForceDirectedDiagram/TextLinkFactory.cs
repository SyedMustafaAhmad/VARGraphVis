using System.Collections.Generic;
using ForceDirectedDiagram.Scripts.Helpers;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class TextLinkFactory : LinkFactoryBase
    {
        [RequireComponentType(typeof(TextLink))]
        [SerializeField] private GameObject defaultPrefab;
    
        [SerializeField] private float baseDistanceBetweenNodes;
        
        [SerializeField] private Camera defaultCamera;

        public override LinkBase CreateInstance(LinkDto link, Transform linksContainer, Dictionary<string, NodeBase> idToNode)
        {
            var linkPrefab = defaultPrefab;

            var sourceNode = idToNode[link.source];
            var targetNode = idToNode[link.target];

            var linkGo = Instantiate(linkPrefab, linksContainer, true);
            linkGo.name = $"Link from {sourceNode.label} to {targetNode.label}";
            var linkComponent = linkGo.GetComponent<TextLink>();

            linkComponent.sourceNode = sourceNode;
            linkComponent.targetNode = targetNode;
            linkComponent.group = link.group;
            linkComponent.length = link.length;
            linkComponent.description = link.description;
            linkComponent.faceCamera.Cam = defaultCamera;
            linkComponent.UpdateLabelMesh();
        
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