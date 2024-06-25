using System;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    [Serializable]
    public class LinkDto
    {
        public string source;
        public string target;
        public int length;
        public int group;
        public string description;
    }
}