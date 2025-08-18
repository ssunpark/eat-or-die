using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TabInputNavigator : MonoBehaviour
{
    [Tooltip("탭 순서대로 나열하세요")]
    public List<Selectable> inputFields;
    private PlayerInputActions _inputActions;

    private void OnEnable()
    {
        _inputActions = InputReader.Instance?.InputActions;
        if (_inputActions == null)
        {
            return;
        }
        _inputActions.UI.Tab.performed += OnTabPerformed;
    }

    private void OnDisable()
    {
        _inputActions.UI.Tab.performed -= OnTabPerformed;
    }

    private void OnTabPerformed(InputAction.CallbackContext context)
    {
        Selectable current = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();
        if (current != null)
        {
            int index = inputFields.IndexOf(current);
            if (index != -1)
            {
                int nextIndex = (index + 1) % inputFields.Count;
                inputFields[nextIndex].Select();
            }
        }
    }
}
