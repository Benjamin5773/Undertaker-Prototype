using System.Collections;
using UnityEngine;

public class EnableOnDestroy : MonoBehaviour
{
    [Header("Object Settings")]
    [Tooltip("The object to monitor for destruction")]
    [SerializeField] private GameObject obj1;

    [Tooltip("The first object to enable when obj1 is destroyed")]
    [SerializeField] private GameObject obj2;

    [Tooltip("The second object to enable when obj1 is destroyed")]
    [SerializeField] private GameObject obj3;

    [Tooltip("Time in seconds before obj3 is hidden")]
    [SerializeField] private float hideObj3Delay = 3.0f;

    private bool hasTriggered = false; // 防止多次触发

    private void Update()
    {
        // 检查 obj1 是否被摧毁
        if (obj1 == null && !hasTriggered)
        {
            hasTriggered = true; // 确保只触发一次
            ActivateObjects();
        }
    }

    private void ActivateObjects()
    {
        if (obj2 != null)
        {
            obj2.SetActive(true);
            Debug.Log($"{obj2.name} has been enabled.");
        }
        else
        {
            Debug.LogWarning("Obj2 is not assigned.");
        }

        if (obj3 != null)
        {
            obj3.SetActive(true);
            Debug.Log($"{obj3.name} has been enabled.");
            StartCoroutine(HideObj3AfterDelay()); // 启动隐藏 obj3 的延迟逻辑
        }
        else
        {
            Debug.LogWarning("Obj3 is not assigned.");
        }
    }

    private IEnumerator HideObj3AfterDelay()
    {
        yield return new WaitForSeconds(hideObj3Delay);
        if (obj3 != null)
        {
            obj3.SetActive(false);
            Debug.Log($"{obj3.name} has been hidden after {hideObj3Delay} seconds.");
        }
    }
}
