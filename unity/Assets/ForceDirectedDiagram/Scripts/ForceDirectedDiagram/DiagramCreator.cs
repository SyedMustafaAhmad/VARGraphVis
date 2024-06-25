using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ForceDirectedDiagram.Scripts.ForceDirectedDiagram
{
    internal sealed class DiagramCreator : MonoBehaviour
    {
        [SerializeField] private Button linkNodesButton;
        [SerializeField] private Button linkNodesInOrderButton;
        [SerializeField] private Button linkNodesInOrderAndCloseButton;
        [SerializeField] private Button unlinkNodesButton;
        [SerializeField] private Button deleteNodesButton;
        [SerializeField] private Button createLinkedNodesButton;
        [SerializeField] private Button createLinkedNodeAndSelectButton;
        [SerializeField] private Button createIndependantNodeButton;

        [SerializeField] private UnityEvent<List<NodeBase>> unlinkNodes;
        [SerializeField] private UnityEvent<List<NodeBase>> linkNodes;
        [SerializeField] private UnityEvent<List<NodeBase>> linkNodesInOrder;
        [SerializeField] private UnityEvent<List<NodeBase>> linkNodesInOrderAndClose;
        [SerializeField] private UnityEvent<List<NodeBase>> deleteNodes;
        [SerializeField] private UnityEvent<List<NodeBase>> createLinkedNode;
        [SerializeField] private UnityEvent<List<NodeBase>> createLinkedNodeAndSelect;
        [SerializeField] private UnityEvent createIndependantNode;
        [SerializeField] private UnityEvent selectAllNodes;
        [SerializeField] private UnityEvent newFilePrompt;
        [SerializeField] private UnityEvent newFilePromptClosed;
        [SerializeField] private UnityEvent parametersOpen;
        [SerializeField] private UnityEvent parametersClosed;
        [SerializeField] private UnityEvent newFile;
        [SerializeField] private UnityEvent exportDiagram;

        private List<NodeBase> _selectedNodes = new();

        private bool _shortcutsEnabled = true;

        private bool _isNewFilePromptOpen;
        private bool _isParametersOpen;

        public void EnableShortcuts(bool value)
        {
            _shortcutsEnabled = value;
        }

        private void Start()
        {
            RefreshButtons(_selectedNodes);
        }

        private void Update()
        {
            if (_shortcutsEnabled && !_isNewFilePromptOpen && !_isParametersOpen)
            {
                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.A))
                {
                    SelectAllNodes();
                    return;
                }
                
                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.S))
                {
                    ExportDiagram();
                    return;
                }

                if (Input.GetKeyDown(KeyCode.N))
                {
                    OpenNewFilePrompt();

                    return;
                }

                if (Input.GetKeyDown(KeyCode.P))
                {
                    if (linkNodesInOrderAndCloseButton.interactable)
                    {
                        LinkNodesInOrderAndClose();
                    }

                    return;
                }

                if (Input.GetKeyDown(KeyCode.O))
                {
                    if (linkNodesInOrderButton.interactable)
                    {
                        LinkNodesInOrder();
                    }

                    return;
                }

                if (Input.GetKeyDown(KeyCode.L))
                {
                    if (linkNodesButton.interactable)
                    {
                        LinkNodes();
                    }

                    return;
                }

                if (Input.GetKeyDown(KeyCode.U))
                {
                    if (unlinkNodesButton.interactable)
                    {
                        UnlinkNodes();
                    }

                    return;
                }

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    if (deleteNodesButton.interactable)
                    {
                        DeleteNodes();
                    }

                    return;
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (createIndependantNodeButton.interactable)
                    {
                        CreateIndependantNode();
                    }

                    return;
                }

                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.Space))
                {
                    if (createLinkedNodeAndSelectButton.interactable)
                    {
                        CreateLinkedNodeAndSelect();
                    }

                    return;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (createLinkedNodesButton.interactable)
                    {
                        CreateLinkedNode();
                    }
                }
            }
        }

        public void RefreshButtons(List<NodeBase> selectedNodes)
        {
            _selectedNodes = selectedNodes;

            switch (selectedNodes.Count)
            {
                case 0:
                    deleteNodesButton.interactable = false;
                    linkNodesButton.interactable = false;
                    linkNodesInOrderButton.interactable = false;
                    linkNodesInOrderAndCloseButton.interactable = false;
                    unlinkNodesButton.interactable = false;
                    createLinkedNodesButton.interactable = false;
                    createLinkedNodeAndSelectButton.interactable = false;
                    createIndependantNodeButton.interactable = true;
                    break;
                case 1:
                    deleteNodesButton.interactable = true;
                    linkNodesButton.interactable = false;
                    linkNodesInOrderButton.interactable = false;
                    linkNodesInOrderAndCloseButton.interactable = false;
                    unlinkNodesButton.interactable = false;
                    createLinkedNodesButton.interactable = true;
                    createLinkedNodeAndSelectButton.interactable = true;
                    createIndependantNodeButton.interactable = true;
                    break;
                case 2:
                    deleteNodesButton.interactable = true;
                    linkNodesButton.interactable = true;
                    linkNodesInOrderButton.interactable = true;
                    linkNodesInOrderAndCloseButton.interactable = true;
                    unlinkNodesButton.interactable = true;
                    createLinkedNodesButton.interactable = true;
                    createLinkedNodeAndSelectButton.interactable = true;
                    createIndependantNodeButton.interactable = true;
                    break;
                default:
                    deleteNodesButton.interactable = true;
                    linkNodesButton.interactable = true;
                    linkNodesInOrderButton.interactable = true;
                    linkNodesInOrderAndCloseButton.interactable = true;
                    unlinkNodesButton.interactable = true;
                    createLinkedNodesButton.interactable = true;
                    createLinkedNodeAndSelectButton.interactable = true;
                    createIndependantNodeButton.interactable = true;
                    break;
            }
        }

        public void OpenNewFilePrompt()
        {
            _isNewFilePromptOpen = true;
            newFilePrompt?.Invoke();
        }
        
        public void CloseNewFilePrompt()
        {
            _isNewFilePromptOpen = false;
            newFilePromptClosed?.Invoke();
        }
        
        public void OpenParameters()
        {
            _isParametersOpen = true;
            parametersOpen?.Invoke();
        }
        
        public void CloseParameters()
        {
            _isParametersOpen = false;
            parametersClosed?.Invoke();
        }
        
        public void OpenNewFile()
        {
            newFile?.Invoke();
        }

        public void ExportDiagram()
        {
            exportDiagram?.Invoke();
        }

        public void SelectAllNodes()
        {
            selectAllNodes?.Invoke();
        }

        public void LinkNodes()
        {
            linkNodes?.Invoke(_selectedNodes);
        }

        public void LinkNodesInOrder()
        {
            linkNodesInOrder?.Invoke(_selectedNodes);
        }

        public void LinkNodesInOrderAndClose()
        {
            linkNodesInOrderAndClose?.Invoke(_selectedNodes);
        }

        public void UnlinkNodes()
        {
            unlinkNodes?.Invoke(_selectedNodes);
        }

        public void DeleteNodes()
        {
            deleteNodes?.Invoke(_selectedNodes);
        }

        public void CreateLinkedNode()
        {
            createLinkedNode?.Invoke(_selectedNodes);
        }

        public void CreateLinkedNodeAndSelect()
        {
            createLinkedNodeAndSelect?.Invoke(_selectedNodes);
        }

        public void CreateIndependantNode()
        {
            createIndependantNode?.Invoke();
        }
    }
}