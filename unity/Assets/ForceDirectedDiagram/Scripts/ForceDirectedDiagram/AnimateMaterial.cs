using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    public class AnimateMaterial : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Vector2 scrollSpeed;

        private Vector2 _offset;
        
        private void Update()
        {
            lineRenderer.material.SetTextureOffset("_MainTex", _offset);

            _offset += scrollSpeed * Time.deltaTime;

            if (_offset.x > 1)
            {
                _offset = new Vector2(0, _offset.y);
            }

            if (_offset.y > 1)
            {
                _offset = new Vector2(_offset.x, 0f);
            }
        }
    }
}