using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    public class UpdateLineRenderer : MonoBehaviour
    {
        [SerializeField] private LinkBase relatedLink;
    
        [SerializeField] protected LineRenderer lineRenderer;
    
        private void Update()
        {
            lineRenderer.SetPosition(0, relatedLink.sourceNode.transform.position);
            lineRenderer.SetPosition(1, relatedLink.targetNode.transform.position);
        }
    }
}
