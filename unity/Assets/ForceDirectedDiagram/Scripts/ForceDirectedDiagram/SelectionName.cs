using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class SelectionName : MonoBehaviour
    {
        [SerializeField] private TMP_InputField selectionNameText;

        [SerializeField] private UnityEvent<bool> inputFieldFocused;

        private void Start()
        {
            selectionNameText.onSelect.AddListener(OnInputFieldFocus);
            selectionNameText.onDeselect.AddListener(OnInputFieldUnfocus);
        }

        private void OnInputFieldFocus(string value)
        {
            inputFieldFocused?.Invoke(false);
        }
    
        private void OnInputFieldUnfocus(string value)
        {
            inputFieldFocused?.Invoke(true);
        }
    
        public void UpdateSelectionName(List<NodeBase> nodes)
        {
            if (!nodes.Any()) return;

            if (nodes.Count == 1)
            {
                selectionNameText.text = nodes[0].label;
            }
            else
            {
                selectionNameText.text = nodes.GroupBy(o => o.label).Count() > 1 ? "--" : nodes[0].label;
            }
        
            selectionNameText.ForceLabelUpdate();
        }
    }
}