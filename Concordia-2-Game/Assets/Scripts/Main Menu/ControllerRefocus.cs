using UnityEngine;
using UnityEngine.EventSystems;

// Taken from http://answers.unity.com/answers/1101363/view.html

//// If there is no selected item, set the selected item to the event system's first selected item
//public class ControllerRefocus : MonoBehaviour
//{
//    void Update()
//    {
//        if (EventSystem.current.currentSelectedGameObject == null)
//        {
//            Debug.Log("Reselecting first input");
//            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
//        }
//    }
//}


// Taken from http://answers.unity.com/answers/1512939/view.html
public class ControllerRefocus : MonoBehaviour
{
    protected GameObject lastSelectedGameObject;
    protected GameObject currentSelectedGameObject_Recent;

    void Update()
    {
        GetLastGameObjectSelected();

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            Debug.Log("Reselecting first input");
            EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
        }
    }

    private void GetLastGameObjectSelected()
    {
        if (EventSystem.current.currentSelectedGameObject != currentSelectedGameObject_Recent)
        {
            lastSelectedGameObject = currentSelectedGameObject_Recent;
            currentSelectedGameObject_Recent = EventSystem.current.currentSelectedGameObject;
        }
    }
}
