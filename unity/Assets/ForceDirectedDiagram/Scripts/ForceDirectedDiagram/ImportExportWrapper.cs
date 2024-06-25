using SFB;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal static class ImportExportWrapper
    {
        public static string SaveFilePanel(SavePanelParams savePanelParams)
        {
            return StandaloneFileBrowser.SaveFilePanel(
                savePanelParams.Title,
                savePanelParams.Directory,
                savePanelParams.FileName,
                savePanelParams.ExtensionList);
        }

        public static string[] OpenFilePanel(OpenPanelParams openPanelParams)
        {
            return StandaloneFileBrowser.OpenFilePanel(
                openPanelParams.Title,
                openPanelParams.Directory,
                openPanelParams.Extensions,
                openPanelParams.Multiselect);
        }
    }
}