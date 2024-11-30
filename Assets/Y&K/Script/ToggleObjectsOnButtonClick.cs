using UnityEngine;

public class ToggleObjectsOnButtonClick : MonoBehaviour
{
    [Header("Objects to Enable")]
    [Tooltip("The first object to enable")]
    [SerializeField] private GameObject objectToEnable1;

    [Tooltip("The second object to enable")]
    [SerializeField] private GameObject objectToEnable2;

    [Header("Objects to Disable")]
    [Tooltip("The first object to disable")]
    [SerializeField] private GameObject objectToDisable1;

    [Tooltip("The second object to disable")]
    [SerializeField] private GameObject objectToDisable2;

    public void OnButtonClick()
    {
        // 启用前两个对象
        if (objectToEnable1 != null)
        {
            objectToEnable1.SetActive(true);
            Debug.Log($"{objectToEnable1.name} has been enabled.");
        }
        if (objectToEnable2 != null)
        {
            objectToEnable2.SetActive(true);
            Debug.Log($"{objectToEnable2.name} has been enabled.");
        }

        // 关闭后两个对象
        if (objectToDisable1 != null)
        {
            objectToDisable1.SetActive(false);
            Debug.Log($"{objectToDisable1.name} has been disabled.");
        }
        if (objectToDisable2 != null)
        {
            objectToDisable2.SetActive(false);
            Debug.Log($"{objectToDisable2.name} has been disabled.");
        }
    }
}
