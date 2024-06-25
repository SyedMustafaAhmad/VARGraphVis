using System.Collections.Generic;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    public class DiagramBuildContainer
    {
        public readonly NodeBase RootNode;
        public readonly Dictionary<NodeBase, List<LinkBase>> NodesToLink;

        public DiagramBuildContainer(NodeBase rootNode, Dictionary<NodeBase, List<LinkBase>> nodesToLink)
        {
            RootNode = rootNode;
            NodesToLink = nodesToLink;
        }
    }
}