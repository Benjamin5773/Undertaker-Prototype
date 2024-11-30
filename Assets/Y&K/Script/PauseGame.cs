using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public void OnPauseButtonClick()
    {
        // ‘›Õ£”Œœ∑
        Time.timeScale = 0f;
        Debug.Log("Game Paused.");
    }
}
