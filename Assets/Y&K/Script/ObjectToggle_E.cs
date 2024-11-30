using UnityEngine;

public class ObjectToggle_E : MonoBehaviour
{
    [Header("Objects to Control")]
    [Tooltip("The object to hide when E is pressed")]
    [SerializeField] private GameObject objectToHide;
    [SerializeField] private GameObject objectToHide1;
    [SerializeField] private GameObject objectToHide2;

    [Tooltip("The first object to show when E is pressed")]
    [SerializeField] private GameObject objectToShow1;

    [Tooltip("The second object to show when E is pressed")]
    [SerializeField] private GameObject objectToShow2;

    [Header("Input Settings")]
    [Tooltip("The key to toggle the objects")]
    [SerializeField] private KeyCode toggleKey = KeyCode.E;

    private bool isToggled = false; // 用于跟踪切换状态（可选）

    private void Update()
    {
        // 检测按键输入
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleObjects();
        }
    }

    private void ToggleObjects()
    {
        if (objectToHide != null)
        {
            objectToHide.SetActive(false); // 隐藏指定对象
             objectToHide1.SetActive(false); // 隐藏指定对象
             objectToHide2.SetActive(false); // 隐藏指定对象
        }

        if (objectToShow1 != null)
        {
            objectToShow1.SetActive(true); // 启用第一个对象
        }

        if (objectToShow2 != null)
        {
            objectToShow2.SetActive(true); // 启用第二个对象
        }

        Debug.Log("Objects toggled successfully.");
    }
}
