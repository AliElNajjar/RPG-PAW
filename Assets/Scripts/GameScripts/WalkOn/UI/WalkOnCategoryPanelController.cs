using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired.Integration.UnityUI;
using UnityEngine.EventSystems;

public class WalkOnCategoryPanelController : MonoBehaviour
{
    public GameObject selector;

    public RewiredStandaloneInputModule rewiredInputModule;

    [SerializeField] private Vector3 selectorSeparation;

    private WalkOnUIController uiController;

    [SerializeField] private GameObject[] elements;
    private int _currentSelected;
    
    public bool Reading
    {
        get; set;
    } = true;


    private void Awake()
    {
        uiController = FindObjectOfType<WalkOnUIController>();
    }    

    void Update()
    {
        if (Reading)
        {
            PlaceSelector();

            if (RewiredInputHandler.Instance.player.GetButtonDown("Up"))
            {
                Navigate(-1);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Down"))
            {
                Navigate(1);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Unequip"))
            {
                EventSystem.current.currentSelectedGameObject.GetComponent<CategoryReference>().UnequipCategory();
                uiController.SavePreset();
            }
        }
    }

    protected virtual void Navigate(int itemPosition)
    {
        if (_currentSelected + itemPosition >= 0 && _currentSelected + itemPosition < elements.Length)
        {
            _currentSelected += itemPosition;
            PlaceSelector();
        }
        else if (itemPosition == 1337)
        {
            _currentSelected = 0;
            PlaceSelector();
        }
    }

    private void PlaceSelector()
    {
        selector.transform.position = EventSystem.current.currentSelectedGameObject.transform.position + selectorSeparation;
    }

    public void SelectCurrent()
    {
        EventSystem.current.SetSelectedGameObject(elements[_currentSelected]);
        PlaceSelector();
    }
}
