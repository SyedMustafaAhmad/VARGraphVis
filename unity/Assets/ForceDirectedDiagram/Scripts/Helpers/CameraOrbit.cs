using System;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.Helpers
{
    internal sealed class CameraOrbit : CameraBase
    {
        [SerializeField] private float rotationInertia;
        [SerializeField] private Transform defaultParent;
        [SerializeField] private CameraRotationMode rotationMode;
        [SerializeField] private float zoomLerpSpeed = 0.1f;
        [SerializeField] private GameObject cameraOrbitRig;
        [SerializeField] private GameObject childCamera;
        [SerializeField] private float rotateSpeed = 0.08f;
        [SerializeField] private float minDistance = 10f;
        [SerializeField] private float maxDistance = 30f;
        [SerializeField] private float constantRotation;
        [SerializeField] private bool constantRotationEnabled;
        [SerializeField] private bool sinZoom;
        [SerializeField] private float zoomSpeed = 0.01f;
        [SerializeField] private float midRadius = 30f;
        [SerializeField] private float zoomRange = 10f;
        
        private Transform _parentTransform;
        private float _cameraTargetZoom;
        private bool _reposition;
        private float _currentZoomIncrement;
        private float _horizontalMovement;
        private float _verticalMovement;
        private float _horizontalMovementInertia;
        private float _verticalMovementInertia;
        private float _inertiaTimer;
        
        private enum CameraRotationMode
        {
            Turntable,
            Simple,
        }

        private void Start()
        {
            _cameraTargetZoom = childCamera.transform.localPosition.x;
            ResetParent();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (IsMouseMovementEnabled)
                {
                    _horizontalMovement = rotateSpeed * Input.GetAxis("Mouse X");
                    _verticalMovement = -rotateSpeed * Input.GetAxis("Mouse Y");

                    if (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0)
                    {
                        _inertiaTimer += Time.deltaTime;

                        if (_inertiaTimer > MaxInertiaWaitTime)
                        {
                            _horizontalMovementInertia = 0f;
                            _verticalMovementInertia = 0f;
                        }
                    }
                    else
                    {
                        _horizontalMovementInertia = _horizontalMovement;
                        _verticalMovementInertia = _verticalMovement;
                        _inertiaTimer = 0;
                    }

                    RotateCamera(_horizontalMovement, _verticalMovement);
                }
            }
            else
            {
                ApplyInertia();

                RotateCamera(_horizontalMovementInertia, _verticalMovementInertia);

                if (constantRotationEnabled)
                {
                    var eulerAngles = cameraOrbitRig.transform.eulerAngles;
                    var targetAngles = new Vector3(eulerAngles.x, eulerAngles.y + constantRotation, eulerAngles.z);
                    cameraOrbitRig.transform.eulerAngles = targetAngles;
                }
            }


            if (!sinZoom)
            {
                if (IsMouseMovementEnabled)
                {
                    var scrollFactor = Input.GetAxis("Mouse ScrollWheel");

                    if (scrollFactor != 0)
                    {
                        if (_cameraTargetZoom * (1f - scrollFactor) < maxDistance && _cameraTargetZoom * (1f - scrollFactor) > minDistance)
                        {
                            _cameraTargetZoom *= (1f - scrollFactor);
                        }
                    }

                    childCamera.transform.localPosition = Vector3.Lerp(
                        childCamera.transform.localPosition,
                        new Vector3(_cameraTargetZoom * (1f - scrollFactor), 0f, 0f), zoomLerpSpeed);
                }
            }
            else
            {
                _currentZoomIncrement += zoomSpeed;

                childCamera.transform.localPosition =
                    new Vector3(midRadius + zoomRange * Mathf.Sin(_currentZoomIncrement), 0f, 0f);
            }

            cameraOrbitRig.transform.position = _parentTransform.position;
        }

        private void ApplyInertia()
        {
            _horizontalMovementInertia *= Mathf.Max(0, 1 - rotationInertia * Time.deltaTime);
            _verticalMovementInertia *= Mathf.Max(0, 1 - rotationInertia * Time.deltaTime);
        }

        private void RotateCamera(float horizontalMovement, float verticalMovement)
        {
            switch (rotationMode)
            {
                case CameraRotationMode.Turntable:

                    RotateTurntable(horizontalMovement, verticalMovement);

                    break;
                case CameraRotationMode.Simple:

                    RotateSimple(horizontalMovement, verticalMovement);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RotateSimple(float horizontalMovement, float verticalMovement)
        {
            cameraOrbitRig.transform.Rotate(new Vector3(0f,horizontalMovement , verticalMovement));
        }

        private void RotateTurntable(float horizontalMovement, float verticalMovement)
        {
            if (cameraOrbitRig.transform.eulerAngles.x - verticalMovement <= 70f || cameraOrbitRig.transform.eulerAngles.x - verticalMovement >= 290f)
            {
                var eulerAngles = cameraOrbitRig.transform.eulerAngles;
                var targetAngles = new Vector3(eulerAngles.x, eulerAngles.y + horizontalMovement, eulerAngles.z + verticalMovement);
                cameraOrbitRig.transform.eulerAngles = targetAngles;
            }
        }

        private void OnValidate()
        {
            _reposition = true;
        }

        private void LateUpdate()
        {
            if (_reposition)
            {
                cameraOrbitRig.transform.parent = _parentTransform;
                _reposition = false;
            }
        }

        public void SetParent(Component component)
        {
            _parentTransform = component.transform;
        }

        private void ResetParent()
        {
            if (defaultParent != null)
            {
                _parentTransform = defaultParent;
            }
            else
            {
                Debug.Log("Can not find default parent.");
            }
        }
    }
}