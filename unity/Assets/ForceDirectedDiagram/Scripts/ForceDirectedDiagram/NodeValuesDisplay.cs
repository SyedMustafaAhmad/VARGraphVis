using TMPro;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class NodeValuesDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nodeName;
        [SerializeField] private TextMeshProUGUI nodeGroup;
        [SerializeField] private GameObject displayPanel;

        public void HideDisplay()
        {
            displayPanel.SetActive(false);
        }

        private void ShowDisplay()
        {
            displayPanel.SetActive(true);
        }

        public void UpdateNodeDisplay(NodeBase node)
        {
            ShowDisplay();

            nodeName.text = node.name;
            nodeGroup.text = node.group.ToString();
        }
    }
}