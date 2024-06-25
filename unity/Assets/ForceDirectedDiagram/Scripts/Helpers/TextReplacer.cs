using TMPro;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.Helpers
{
    public class TextReplacer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;

        public void UpdateText(string text)
        {
            textMeshProUGUI.text = text;
        }
    }
}