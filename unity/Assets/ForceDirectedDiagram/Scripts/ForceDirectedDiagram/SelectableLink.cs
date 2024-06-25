using UnityEngine;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class SelectableLink : MonoBehaviour
    {
        private bool _isSelected;

        [SerializeField] private GameObject selectedState;
        [SerializeField] private GameObject unselectedState;

        private void Start()
        {
            UpdateSelectedState(_isSelected);
        }

        public void UpdateSelectedState(bool isSelected)
        {
            _isSelected = isSelected;
            selectedState.SetActive(_isSelected);
            unselectedState.SetActive(!_isSelected);
        }
    }
}