using UnityEngine;
using UnityEngine.Events;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class NodeRaycast : MonoBehaviour
    {
        private Camera _mainCamera;

        [SerializeField] private UnityEvent<NodeBase> objectDetected;
        [SerializeField] private UnityEvent noObjectDetected;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            // Detect objects on mouse movement
            DetectObjectsOnMouseMovement();
        }

        private void DetectObjectsOnMouseMovement()
        {
            // Cast a ray from the mouse position
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            // Perform the raycast
            if (Physics.Raycast(ray, out var hit))
            {
                // Check if the raycast hits any object with a collider
                var nodeComponent = hit.collider.GetComponent<NodeBase>();
            
                if (nodeComponent != null)
                {
                    objectDetected?.Invoke(nodeComponent);
                }
            }
            else
            {
                noObjectDetected?.Invoke();
            }
        }
    }
}