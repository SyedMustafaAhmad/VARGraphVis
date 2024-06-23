using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class Click : MonoBehaviour
    {
        private Camera _mainCamera;

        private bool _isDragging;
        private Vector3 _offset;

        private NodeBase _nodeBase;

        [SerializeField] private UnityEvent<NodeBase> nodeClicked;
        [SerializeField] private UnityEvent noNodeClicked;

        [SerializeField] private float detectionRadius;
        [SerializeField] private float detectionTimeInMilliseconds;
    
        private EventSystem _eventSystem;

        private Vector3 _mouseDownPosition;
        private DateTime _mouseDownTime;

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
            if (Input.GetMouseButtonDown(0))
            {
                // verify pointer is not on top of GUI; if it is, return
                if (_eventSystem.IsPointerOverGameObject()) return;
            
                MouseDown();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                // verify pointer is not on top of GUI; if it is, return
                if (_eventSystem.IsPointerOverGameObject()) return;
            
                MouseUp();
            }
        }

        private void MouseDown()
        {
            // Calculate the offset between the object's position and the mouse cursor position
            _mouseDownPosition = Input.mousePosition;
            _mouseDownTime = DateTime.Now;
        }

        private void MouseUp()
        {
            var mouseUpPosition = Input.mousePosition;

            if (GetDistance(mouseUpPosition, _mouseDownPosition) < detectionRadius && (DateTime.Now-_mouseDownTime).TotalMilliseconds < detectionTimeInMilliseconds)
            {
                // Cast a ray from the mouse position
                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

                // Perform the raycast
                if (Physics.Raycast(ray, out var hit))
                {
                    // Check if the raycast hits any object with a collider
                    var hitObject = hit.collider.gameObject;

                    var nodeComponent = hitObject.GetComponent<NodeBase>();
                    if (nodeComponent != null)
                    {
                        nodeClicked?.Invoke(nodeComponent);
                        return;
                    }
                }
            
                noNodeClicked?.Invoke();
            }

        }

        private static double GetDistance(Vector3 mouseUpPosition, Vector3 mouseDownPosition)
        {
            return (mouseDownPosition - mouseUpPosition).magnitude;
        }
    }
}