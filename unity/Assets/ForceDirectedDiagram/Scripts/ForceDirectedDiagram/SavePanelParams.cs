using SFB;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class SavePanelParams
    {
        public string Title { get; set; }
        public string Directory { get; set; }
        public string FileName { get; set; }
        public ExtensionFilter[] ExtensionList { get; set; }
    }
}