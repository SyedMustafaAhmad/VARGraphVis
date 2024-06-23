using TMPro;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.Helpers
{
    public class FadingConsole : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private float fadeDuration = 3f;
        [SerializeField] private float displayDuration = 5f;

        private float _startTime;
        private Color _originalColor;

        private void Start()
        {
            _startTime = Time.time;
            _originalColor = messageText.color;
        }

        public void DisplayMessage(string message)
        {
            _startTime = Time.time;

            messageText.text = message;
            messageText.enabled = true;
            messageText.color = _originalColor;

            enabled = true;
        }
    
        private void Update()
        {
            var elapsedTime = Time.time - _startTime;

            // Check if it's time to fade out the message
            if (!(elapsedTime >= displayDuration)) return;
        
            var fadeElapsedTime = elapsedTime - displayDuration;
            var fadeProgress = fadeElapsedTime / fadeDuration;

            // Calculate the new alpha value based on the fade progress
            var newAlpha = Mathf.Lerp(1f, 0f, fadeProgress);
            var newColor = new Color(_originalColor.r, _originalColor.g, _originalColor.b, newAlpha);

            // Apply the new color to the message text
            messageText.color = newColor;

            // Check if the fading is complete
            if (!(fadeElapsedTime >= fadeDuration)) return;
            
            // Disable the text component to hide the message
            messageText.enabled = false;

            // Disable this script to stop the fading process
            enabled = false;
        }
    }
}
