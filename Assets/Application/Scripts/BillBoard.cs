using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private bool upwardDirection = false;

    void Start()
    {
        // Cache the main camera at the start for performance reasons
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (upwardDirection)
        {
            // Rotate the tooltip to face the camera, adjusting for upward direction
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
        else
        {
            // Rotate the tooltip to face the camera without adjusting upward direction
            Vector3 cameraForward = mainCamera.transform.rotation * Vector3.forward;
            cameraForward.y = 0; // Lock the Y-axis to prevent tilting
            transform.rotation = Quaternion.LookRotation(cameraForward);
        }
    }
}
