using UnityEngine;

namespace ForceDirectedDiagram.Scripts.Helpers
{
    internal sealed class CameraPan : CameraBase
    {
        [SerializeField] private float zoomSpeed;
        [SerializeField] private float minDistance = 10f;
        [SerializeField] private float maxDistance = 30f;
        [SerializeField] private float panSpeed = 1f;
        [SerializeField] private float panInertia;
        [SerializeField] private float zoomLerpSpeed = 0.1f;

        private bool _reposition;
        private float _currentZ;
        private float _targetZ;
        private float _horizontalMovementInertia;
        private float _verticalMovementInertia;
        private float _inertiaTimer;
        private float _horizontalMovement;
        private float _verticalMovement;
        
        private void Start()
        {
            var position = transform.position;
            _currentZ = position.z;
            _targetZ = position.z;
        }
        
        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (IsMouseMovementEnabled)
                {
                    var moveSpeed = ProcessMoveSpeed();

                    _horizontalMovement = -moveSpeed * Input.GetAxis("Mouse X");
                    _verticalMovement = -moveSpeed * Input.GetAxis("Mouse Y");

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

                    PanCamera(_horizontalMovement, _verticalMovement);
                }
            }
            else
            {
                ApplyInertia();
                PanCamera(_horizontalMovementInertia, _verticalMovementInertia);
            }

            if (IsMouseMovementEnabled)
            {
                var scrollFactor = Input.GetAxis("Mouse ScrollWheel");

                if (scrollFactor != 0)
                {
                    _currentZ = transform.position.z;

                    var newZPosition = _currentZ + zoomSpeed * scrollFactor;

                    if (newZPosition > 0)
                    {
                        _targetZ = -minDistance;
                    }
                    else if (Mathf.Abs(newZPosition) < minDistance)
                    {
                        _targetZ = -minDistance;
                    }
                    else if (Mathf.Abs(newZPosition) > maxDistance)
                    {
                        _targetZ = -maxDistance;
                    }
                    else
                    {
                        _targetZ = newZPosition;
                    }
                }
            }

            var position = transform.position;
            var targetPosition = new Vector3(position.x, position.y, _targetZ);
            transform.position = Vector3.Lerp(
                position,
                targetPosition, zoomLerpSpeed);
        }

        private void PanCamera(float horizontalMovement, float verticalMovement)
        {
            transform.position += new Vector3(horizontalMovement, verticalMovement, 0f);
        }

        private void ApplyInertia()
        {
            _horizontalMovementInertia *= Mathf.Max(0, 1 - panInertia * Time.deltaTime);
            _verticalMovementInertia *= Mathf.Max(0, 1 - panInertia * Time.deltaTime);
        }

        private float ProcessMoveSpeed()
        {
            _currentZ = transform.position.z;

            var processMoveSpeed = panSpeed * Mathf.Abs(_currentZ);

            return processMoveSpeed;
        }
    }
}