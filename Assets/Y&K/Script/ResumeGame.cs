using UnityEngine;

public class ResumeGame : MonoBehaviour
{
    public void OnResumeButtonClick()
    {
        // ª÷∏¥”Œœ∑
        Time.timeScale = 1f;
        Debug.Log("Game Resumed.");
    }
}
