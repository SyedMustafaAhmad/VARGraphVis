using UnityEngine;

// Custom attribute to ensure the required component type on a GameObject prefab.
namespace ForceDirectedDiagram.Scripts.Helpers
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class RequireComponentTypeAttribute : PropertyAttribute
    {
        public readonly System.Type RequiredType;

        public RequireComponentTypeAttribute(System.Type type)
        {
            RequiredType = type;
        }
    }
}