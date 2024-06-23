using UnityEngine;

namespace ForceDirectedDiagram.Scripts.Helpers
{
    public abstract class CameraBase : MonoBehaviour
    {
        protected const float MaxInertiaWaitTime = 0.05f;
        protected bool IsMouseMovementEnabled = true;
        
        public void LockMovement(bool isLocked)
        {
            IsMouseMovementEnabled = !isLocked;
        }
    }
}