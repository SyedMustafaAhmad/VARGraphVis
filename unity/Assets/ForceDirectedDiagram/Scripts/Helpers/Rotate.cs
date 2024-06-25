using UnityEngine;

namespace ForceDirectedDiagram.Scripts.Helpers
{
    public class Rotate : MonoBehaviour
    {
        public float rotationSpeed = 10f;

        private void Update()
        {
            // Rotate the object around the Z-axis
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }
    }
}