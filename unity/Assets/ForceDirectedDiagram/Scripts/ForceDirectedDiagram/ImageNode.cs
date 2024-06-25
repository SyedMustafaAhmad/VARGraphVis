using System.Collections.Generic;
using ForceDirectedDiagram.Scripts.Helpers;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class ImageNode : NodeBase
    {
        [SerializeField] internal FaceCamera faceCamera;

        [SerializeField] private List<Material> imageMaterials;
        [SerializeField] private Material defaultMaterial;

        [SerializeField] private MeshRenderer meshRenderer;

        internal void UpdateImage(int matIndex)
        {
            if (matIndex < imageMaterials.Count)
            {
                meshRenderer.material = imageMaterials[matIndex];
            }
            else
            {
                meshRenderer.material = defaultMaterial;
            }
        }
    }
}