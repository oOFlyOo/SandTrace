using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    public Transform character;
    public GameObject player;
    private PlayerControl playerControl;

    // Settings
    public float cameraSmoothing = 0.01f;
    public float cameraPreview = 2.0f;

    // Private memeber data
    private Camera mainCamera;

    private Transform mainCameraTransform;
    private Vector3 cameraVelocity = Vector3.zero;
    private Vector3 initOffsetToPlayer;

    private Vector3 targetCamOffset;
    private Vector3 aimCamOffset;

    void Awake()
    {
        // Set main camera
        mainCamera = Camera.main;
        mainCameraTransform = mainCamera.transform;
        playerControl = player.GetComponent<PlayerControl>();

        // Ensure we have character set
        // Default to using the transform this component is on
        if (!character)
            character = transform;

        initOffsetToPlayer = mainCameraTransform.position - character.position;
        aimCamOffset = mainCameraTransform.position - new Vector3(character.position.x, character.position.y + 2.0f, character.position.z);
    }

    void Start()
    {
    }

    void Update()
    {
        Vector3 cameraAdjustmentVector = Vector3.zero;
        // HANDLE CAMERA ZOOM
        if (playerControl.isAiming())
        {
            
            targetCamOffset = aimCamOffset;
        }
        else
        {
            targetCamOffset = initOffsetToPlayer;
        }
        // Set the target position of the camera to point at the focus point
        Vector3 cameraTargetPosition = character.position + targetCamOffset + cameraAdjustmentVector * cameraPreview;
        // Apply some smoothing to the camera movement
        mainCameraTransform.position = Vector3.SmoothDamp(mainCameraTransform.position, cameraTargetPosition, ref cameraVelocity, cameraSmoothing);
    } 
}