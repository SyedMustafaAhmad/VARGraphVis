using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    public class LinkLine : LinkBase
    {
        [SerializeField] protected LineRenderer lineRenderer;
    
        private void Update()
        {
            lineRenderer.SetPosition(0, sourceNode.transform.position);
            lineRenderer.SetPosition(1, targetNode.transform.position);
        }
    }
}