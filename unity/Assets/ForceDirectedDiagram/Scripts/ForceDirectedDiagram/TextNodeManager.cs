using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class TextNodeManager : MonoBehaviour
    {
        [SerializeField] private ForceDirectedDiagramManager forceDirectedDiagramManager;
        [SerializeField] private TextNodeFactory textNodeFactory;

        public void ShowHideTextNodesLabels(bool isVisible)
        {
            var textNodes = GetTextNodes();

            foreach (var textNode in textNodes)
            {
                textNode.ShowHideLabel(isVisible);
            }

            textNodeFactory.showLabelsByDefault = isVisible;
        }

        public void UpdateCameras(Camera newCam)
        {
            var textNodes = GetTextNodes();

            foreach (var textNode in textNodes)
            {
                textNode.UpdateCamera(newCam);
            }

            var dragAndDrops = GetDragAndDrops();
        
            foreach (var dragAndDrop in dragAndDrops)
            {
                dragAndDrop.UpdateCamera(newCam);
            }
        }

        public void RefreshLabels()
        {
            var textNodes = GetTextNodes();

            foreach (var textNode in textNodes)
            {
                textNode.RefreshLabelMesh();
            }
        }

        private IEnumerable<TextNode> GetTextNodes()
        {
            return forceDirectedDiagramManager.GetNodesToLinks().Keys.Select(o => o.GetComponent<TextNode>()).Where(o => o != null);
        }
    
        private IEnumerable<DragAndDrop> GetDragAndDrops()
        {
            return forceDirectedDiagramManager.GetNodesToLinks().Keys.Select(o => o.GetComponent<DragAndDrop>()).Where(o => o != null);
        }
    }
}