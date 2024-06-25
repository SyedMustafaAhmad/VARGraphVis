using System.Collections.Generic;
using ForceDirectedDiagram.Scripts.Helpers;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class CustomLinkFactory : LinkFactoryBase
    {
        [RequireComponentType(typeof(LinkBase))]
        [SerializeField] private GameObject defaultPrefab;

        [SerializeField] private List<GameObject> linkPrefabByValue = new();

        [SerializeField] private float defaultDistanceBetweenNodes;

        private Dictionary<string, NodeBase> _idToNodeDictionary;

        private GameObject GetLinkPrefab(LinkDto linkDto)
        {
            var group = _idToNodeDictionary[linkDto.target].group;

            if (group >= 0 && group < linkPrefabByValue.Count)
            {
                return linkPrefabByValue[group] ?? defaultPrefab;
            }

            return defaultPrefab;
        }

        public override LinkBase CreateInstance(LinkDto link, Transform linksContainer, Dictionary<string, NodeBase> idToNode)
        {
            _idToNodeDictionary = idToNode;

            var linkPrefab = GetLinkPrefab(link);

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