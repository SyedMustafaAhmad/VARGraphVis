using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class ColorAndShapeNode : NodeBase
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;

        public void SetColor(Color newColor)
        {
            var tempMaterial = new Material(meshRenderer.sharedMaterial)
            {
                color = newColor
            };
            meshRenderer.sharedMaterial = tempMaterial;
        }

        public void SetMesh(Mesh newMesh)
        {
            meshFilter.mesh = newMesh;
        }
    }
}