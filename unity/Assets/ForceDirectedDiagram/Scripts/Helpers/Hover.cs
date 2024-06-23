using UnityEngine;
using UnityEngine.Events;

namespace ForceDirectedDiagram.Scripts.Helpers
{
    public class Hover : MonoBehaviour
    {
        [SerializeField] private UnityEvent onHover;
        [SerializeField] private UnityEvent onExit;
        
        private void OnMouseEnter()
        {
            onHover?.Invoke();
        }

        private void OnMouseExit()
        {
            onExit?.Invoke();
        }
    }
}