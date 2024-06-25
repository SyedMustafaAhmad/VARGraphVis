using ForceDirectedDiagram.Scripts.Helpers;
using TMPro;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class ColorShapeTextNode : NodeBase
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;

        [SerializeField] internal FaceCamera faceCamera;
        [SerializeField] private TextMeshPro labelMesh;
    
        internal void UpdateLabelMesh()
        {
            labelMesh.text = label;
        }
    
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