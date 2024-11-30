using UnityEngine;
using Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    private CinemachineBrain cinemachineBrain;
    [SerializeField] private GameObject overshoulderCamera;
    [SerializeField] private GameObject overheadCamera;
    [SerializeField] private GameObject narrativeCamera;

    // Call this function to disable FPS camera,
    // and enable overhead camera and narrative camera.
    private void Awake()
    {
        overshoulderCamera.SetActive(false);
        overheadCamera.SetActive(false);
        narrativeCamera.SetActive(true);
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
    }
    public void ShowOverheadView()
    {
        overshoulderCamera.SetActive(false);
        narrativeCamera.SetActive(false);
        overheadCamera.SetActive(true);
    }

    // Call this function to enable FPS camera,
    // and disable overhead camera and narrative camera.
    public void ShowOvershoulderView()
    {
        SetDefaultBlend(CinemachineBlendDefinition.Style.Cut, 0f);
        overshoulderCamera.SetActive(true);
        overheadCamera.SetActive(false);
        narrativeCamera.SetActive(false);
    }

    // Call this function to enable FPS camera,
    // and disable overhead camera and narrative camera.
    public void ShowNarrativeView()
    {
        SetDefaultBlend(CinemachineBlendDefinition.Style.HardOut, 1.0f);
        narrativeCamera.SetActive(true);
        overheadCamera.SetActive(false);
        overshoulderCamera.SetActive(false);
    }

        public void SetDefaultBlend(CinemachineBlendDefinition.Style blendStyle, float blendTime)
    {
        
        CinemachineBlendDefinition newBlend = new CinemachineBlendDefinition
        {
            m_Style = blendStyle,       // Set the blend style 
            m_Time = blendTime         
        };

        // Apply the new blend as the default blend
        cinemachineBrain.m_DefaultBlend = newBlend;
    }
}