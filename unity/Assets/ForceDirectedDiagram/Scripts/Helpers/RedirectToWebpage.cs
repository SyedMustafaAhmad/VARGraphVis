using UnityEngine;

namespace ForceDirectedDiagram.Scripts.Helpers
{
    public class RedirectToWebpage : MonoBehaviour
    {
        [SerializeField] private string url;

        public void OpenWebpage()
        {
            Application.OpenURL(url);
        }
    }
}
