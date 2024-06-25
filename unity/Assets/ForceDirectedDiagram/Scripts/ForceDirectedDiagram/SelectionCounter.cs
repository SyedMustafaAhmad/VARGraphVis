using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class SelectionCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI selectionCountText;

        public void UpdateSelectionCount(List<NodeBase> nodes)
        {
            selectionCountText.text = nodes.Count.ToString();
        }
    }
}
