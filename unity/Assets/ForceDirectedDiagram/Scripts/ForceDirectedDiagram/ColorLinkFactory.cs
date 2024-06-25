using System.Collections.Generic;
using ForceDirectedDiagram.Scripts.Helpers;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class ColorLinkFactory : LinkFactoryBase
    {
        [RequireComponentType(typeof(ColorLink))]
        [SerializeField] private GameObject defaultPrefab;

        [SerializeField] private List<Color> linkColorByValue = new();

        [SerializeField] private float defaultDistanceBetweenNodes;

        public override LinkBase CreateInstance(LinkDto link, Transform linksContainer, Dictionary<string, NodeBase> idToNode)
        {
            var sourceNode = idToNode[link.source];
            var targetNode = idToNode[link.target];

            var linkGo = Instantiate(defaultPrefab, linksContainer, true);
            linkGo.name = $"Link from {sourceNode.label} to {targetNode.label}";

            var linkComponent = linkGo.GetComponent<ColorLink>();

            linkComponent.sourceNode = sourceNode;
            linkComponent.targetNode = targetNode;
            linkComponent.group = sourceNode.group;
            linkComponent.length = link.length;
            linkComponent.description = link.description;

            // Assign colors
            if (linkComponent != null)
            {
                if (linkComponent.group < linkColorByValue.Count)
                {
                    var colorIndex = linkComponent.group;
                    linkComponent.SetColor(linkColorByValue[colorIndex]);
                }
                else
                {
                    Debug.Log("Value index was outside of the range of colors array.");
                }
            }
            else
            {
                Debug.Log("Could not find ColorLink component on selected node.");
            }

            if (link.length > 0)
            {
                linkComponent.distanceBetweenNodes = link.length;
            }
            else
            {
                linkComponent.distanceBetweenNodes = defaultDistanceBetweenNodes;
            }

            return linkComponent;
        }
    }
}