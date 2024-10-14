using UnityEngine;

public class ExampleScript : MonoBehaviour {
    [SerializeField] private GameObject overshoulderCamera;
    [SerializeField] private GameObject overheadCamera;

    // Call this function to disable FPS camera,
    // and enable overhead camera.
    private void Awake()
    {
        overshoulderCamera.SetActive(false);
        overheadCamera.SetActive(true);
    }
    public void ShowOverheadView() {
        overshoulderCamera.SetActive(false);
        overheadCamera.SetActive(true);
    }
    
    // Call this function to enable FPS camera,
    // and disable overhead camera.
    public void ShowOvershoulderView() {
        overshoulderCamera.SetActive(true);
        overheadCamera.SetActive(false);
    }
}