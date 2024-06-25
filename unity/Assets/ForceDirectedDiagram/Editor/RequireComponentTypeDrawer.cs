using ForceDirectedDiagram.Scripts.Helpers;
using UnityEditor;
using UnityEngine;

namespace ForceDirectedDiagram.Editor
{
    [CustomPropertyDrawer(typeof(RequireComponentTypeAttribute))]
    public class RequireComponentTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PropertyField(new Rect(
                position.min.x,
                position.min.y,
                position.size.x,
                EditorGUIUtility.singleLineHeight
            ), property, label);

            var requiredComponentAttribute = attribute as RequireComponentTypeAttribute;

            if (property.objectReferenceValue == null)
            {
                DisplayWarning(position, requiredComponentAttribute);

                EditorGUI.EndProperty();
                return;
            }

            var selectedGameObject = property.objectReferenceValue as GameObject;

            if (requiredComponentAttribute != null && selectedGameObject != null && requiredComponentAttribute.RequiredType != null)
            {
                // Check if the selected GameObject prefab has the required component type.
                if (!selectedGameObject.TryGetComponent(requiredComponentAttribute.RequiredType, out _))
                {
                    DisplayWarning(position, requiredComponentAttribute);
                }
            }

            EditorGUI.EndProperty();
        }

        private static void DisplayWarning(Rect position, RequireComponentTypeAttribute requiredComponentAttribute)
        {
            EditorGUI.HelpBox(new Rect(position.min.x,
                position.min.y + EditorGUIUtility.singleLineHeight + +EditorGUIUtility.standardVerticalSpacing,
                position.size.x,
                EditorGUIUtility.singleLineHeight
            ), "The selected prefab should have the required component type: " + requiredComponentAttribute.RequiredType.Name, MessageType.Warning);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
            {
                return 2 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            var selectedGameObject = property.objectReferenceValue as GameObject;

            if (attribute is RequireComponentTypeAttribute requiredComponentAttribute && selectedGameObject != null && requiredComponentAttribute.RequiredType != null)
            {
                // Check if the selected GameObject prefab has the required component type.
                if (!selectedGameObject.TryGetComponent(requiredComponentAttribute.RequiredType, out _))
                {
                    return 2 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return EditorGUIUtility.singleLineHeight;
        }
    }
}