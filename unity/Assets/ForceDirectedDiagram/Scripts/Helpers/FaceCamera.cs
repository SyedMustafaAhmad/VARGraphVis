using UnityEngine;

namespace ForceDirectedDiagram.Scripts.Helpers
{
    public class FaceCamera : MonoBehaviour
    {
        internal Camera Cam;

        private void Update()
        {
            transform.rotation = Cam.transform.rotation;
        }
    }
}