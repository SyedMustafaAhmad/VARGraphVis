using System.Collections.Generic;
using ForceDirectedDiagram.Scripts.Helpers;
using TMPro;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class TextNode : NodeBase
    {
        [SerializeField] internal List<FaceCamera> faceCamera;
        [SerializeField] private List<TextMeshPro> labelMesh;

        internal void RefreshLabelMesh()
        {
            foreach (var labelTMP in labelMesh)
            {
                labelTMP.text = label;
            }
        }

        public void ShowHideLabel(bool isVisible)
        {
            foreach (var labelTMP in labelMesh)
            {
                labelTMP.gameObject.SetActive(isVisible);
            }
        }

        public void UpdateCamera(Camera cam)
        {
            foreach (var faceCam in faceCamera)
            {
                faceCam.Cam = cam;
            }
        }
    }
}