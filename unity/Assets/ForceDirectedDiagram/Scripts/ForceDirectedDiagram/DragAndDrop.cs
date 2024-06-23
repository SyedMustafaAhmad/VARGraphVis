using ForceDirectedDiagram.Scripts.Helpers;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class DragAndDrop : MonoBehaviour
    {
        private Camera _mainCamera;
        private CameraBase _cameraBase;

        private bool _isDragging;
        private Vector3 _offset;

        private NodeBase _nodeBase;

        private ForceDirectedDiagramManager _forceDirectedDiagramManager;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_mainCamera != null) _cameraBase = _mainCamera.gameObject.GetComponent<CameraBase>();
            _nodeBase = GetComponent<NodeBase>();
        }

        private void Start()
        {
            _forceDirectedDiagramManager = GetComponentInParent<ForceDirectedDiagramManager>();
        }
        
        private void OnMouseDown()
        {
            // Calculate the offset between the object's position and the mouse cursor position
            _offset = transform.position - GetMouseWorldPosition();
            _isDragging = true;
            _cameraBase.LockMovement(true);
        }

        private void OnMouseUp()
        {
            _isDragging = false;
            _cameraBase.LockMovement(false);
        }

        private void Update()
        {
            if (!_isDragging) return;
        
            // Update the position of the object to follow the mouse cursor
            transform.position = GetMouseWorldPosition() + _offset;

            UpdateNodeStruct(_nodeBase);
        }

        private void UpdateNodeStruct(NodeBase nodeBase)
        {
            _forceDirectedDiagramManager.UpdateNodeStructFor(nodeBase);
        }

        private Vector3 GetMouseWorldPosition()
        {
            // Get the mouse position in screen space
            var mousePosition = Input.mousePosition;

            var mainCameraTransform = _mainCamera.transform;
            var nodeToCam = transform.position - mainCameraTransform.position;

            var projectedDistanceBetweenMouseAndNode = Vector3.Dot(mainCameraTransform.forward, nodeToCam);

            mousePosition.z = projectedDistanceBetweenMouseAndNode; // Set the z-coordinate to zero to ensure the object stays in the 2D plane

            // Convert the mouse position from screen space to world space
            mousePosition = _mainCamera.ScreenToWorldPoint(mousePosition);
        
            return mousePosition;
        }

        public void UpdateCamera(Camera newCam)
        {
            _mainCamera = newCam;
            if (_mainCamera != null) _cameraBase = _mainCamera.gameObject.GetComponent<CameraBase>();
        }
    }
}