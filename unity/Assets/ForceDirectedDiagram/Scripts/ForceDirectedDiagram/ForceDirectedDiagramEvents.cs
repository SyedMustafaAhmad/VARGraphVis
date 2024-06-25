using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    [Serializable]
    internal sealed class ForceDirectedDiagramEvents
    {
        [SerializeField] internal UnityEvent<NodeBase> nodeRemoved;
        [SerializeField] internal UnityEvent<DiagramBuildContainer> diagramGenerated;
        [SerializeField] internal UnityEvent<string> onMessageSent;
        [SerializeField] internal UnityEvent<List<NodeBase>> selectEvent;
        [SerializeField] internal UnityEvent<List<NodeBase>> deselectEvent;
        [SerializeField] internal UnityEvent nodesLabelUpdated;
    }
}