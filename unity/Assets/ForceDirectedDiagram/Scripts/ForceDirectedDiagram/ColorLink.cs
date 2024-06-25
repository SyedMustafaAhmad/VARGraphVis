using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    public class ColorLink : LinkBase
    {
        [SerializeField] protected LineRenderer lineRenderer;

        private void Update()
        {
            lineRenderer.SetPosition(0, sourceNode.transform.position);
            lineRenderer.SetPosition(1, targetNode.transform.position);
        }

        public void SetColor(Color newColor)
        {
            SetColorStart(newColor);
            SetColorEnd(newColor);
        }

        public void SetColorStart(Color newColor)
        {
            lineRenderer.startColor = newColor;
        }

        public void SetColorEnd(Color newColor)
        {
            lineRenderer.endColor = newColor;
        }
    }
}