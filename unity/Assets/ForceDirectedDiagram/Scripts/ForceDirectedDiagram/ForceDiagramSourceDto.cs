using System;
using System.Collections.Generic;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    [Serializable]
    public class ForceDiagramSourceDto
    {
        public List<NodeDto> nodes = new();
        public List<LinkDto> links = new();
    }
}