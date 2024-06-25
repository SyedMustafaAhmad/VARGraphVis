using ForceDirectedDiagram.Scripts.Helpers;
using TMPro;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class TextLink : LinkBase
    {
        [SerializeField] internal FaceCamera faceCamera;
        [SerializeField] private TextMeshPro labelMesh;

        internal void UpdateLabelMesh()
        {
            labelMesh.text = description;
        }

        private void Update()
        {
            labelMesh.gameObject.transform.position = (sourceNode.transform.position + targetNode.transform.position) / 2f;
        }
    }
}