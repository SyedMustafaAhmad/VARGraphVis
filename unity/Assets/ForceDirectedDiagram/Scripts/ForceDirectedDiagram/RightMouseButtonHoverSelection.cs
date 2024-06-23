using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class RightMouseButtonHoverSelection : MonoBehaviour
    {
        [SerializeField] private UnityEvent<NodeBase> nodeRightMouseButtonHover;

        private Camera _mainCamera;
        private EventSystem _eventSystem;

        private void Awake()
        {
            _eventSystem = EventSystem.current;
            _mainCamera = Camera.main;
        }

        public void UpdateCamera(Camera newCam)
        {
            _mainCamera = newCam;
        }
        
        private void Update()
        {
            if (!Input.GetMouseButton(1)) return;
        
            // verify pointer is not on top of GUI; if it is, return
            if (_eventSystem.IsPointerOverGameObject()) return;
        
            // Cast a ray from the mouse position
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            // Perform the raycast
            if (!Physics.Raycast(ray, out var hit)) return;
            
            // Check if the raycast hits any object with a collider
            var hitObject = hit.collider.gameObject;

            var nodeComponent = hitObject.GetComponent<NodeBase>();
        
            if (nodeComponent == null) return;
        
            nodeRightMouseButtonHover?.Invoke(nodeComponent);
        }
    }
}