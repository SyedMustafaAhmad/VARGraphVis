using SFB;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class OpenPanelParams
    {
        public string Title { get; set; }
        public string Directory { get; set; }
        public ExtensionFilter[] Extensions { get; set; }
        public bool Multiselect { get; set; }
    }
}