using TMPro;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class NodeTooltip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tooltipText;
        [SerializeField] private float offset = 20f;

        private void Update()
        {
            UpdateTooltipPosition();
        }

        private void UpdateTooltipPosition()
        {
            // Update the tooltip position to follow the mouse cursor
            var tooltipPosition = Input.mousePosition;

            tooltipPosition += new Vector3(offset, offset, 0f);
            transform.position = tooltipPosition;
        }

        public void ShowTooltip(NodeBase node)
        {
            if (!Input.GetMouseButton(0))
            {
                UpdateTooltipPosition();
                
                // Set the tooltip text and activate the tooltip
                tooltipText.text = node.label;
                gameObject.SetActive(true);
            }
        }

        public void HideTooltip()
        {
            if (!Input.GetMouseButton(0))
            {
                // Deactivate the tooltip
                gameObject.SetActive(false);
            }
        }
    }
}