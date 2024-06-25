using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class ColorNode : NodeBase
    {
        [SerializeField] private MeshRenderer meshRenderer;
    
        public void SetColor(Color newColor)
        {
            var tempMaterial = new Material(meshRenderer.sharedMaterial)
            {
                color = newColor
            };
            meshRenderer.sharedMaterial = tempMaterial;
        }
    }
}