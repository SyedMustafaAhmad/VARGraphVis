using UnityEngine;
using UnityEngine.Events;

namespace ForceDirectedDiagram.Scripts.Helpers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Camera> _2DActivated;
        [SerializeField] private UnityEvent<Camera> _3DActivated;

        [SerializeField] private Camera _2DCamera;
        [SerializeField] private Camera _3DCamera;
    
        public void SwitchCamera(bool is2D)
        {
            if (is2D)
            {
                _2DActivated?.Invoke(_2DCamera);
            }
            else
            {
                _3DActivated?.Invoke(_3DCamera);
            }
        }
    }
}