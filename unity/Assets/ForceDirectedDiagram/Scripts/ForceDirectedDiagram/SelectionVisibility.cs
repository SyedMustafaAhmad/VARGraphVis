using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class SelectionVisibility : MonoBehaviour
    {
        [SerializeField] private GameObject panel;

        public void UpdateSelection(List<NodeBase> nodes)
        {
            panel.SetActive(nodes.Any());
        }
    }
}