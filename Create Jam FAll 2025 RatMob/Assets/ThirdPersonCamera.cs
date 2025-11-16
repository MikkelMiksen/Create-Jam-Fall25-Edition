using UnityEngine;

/// <summary>
/// Third-person camera controller:
/// - Mouse orbit (yaw & pitch)
/// - Exposes playerYawEuler (Vector3) for the player to match yaw
/// - Smooth follow the target (position + rotation smoothing)
/// Attach to the Camera. Assign target (player Transform).
/// </summary>
[RequireComponent(typeof(Camera))]
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The player or object the camera follows")]
    public Transform target;

    [Header("Mouse")]
    public float mouseSensitivity = 6f;
    public bool invertY = false;

    [Header("Orbit (degrees)")]
    public float minPitch = -30f;
    public float maxPitch = 60f;
    public float startYaw = 0f;
    public float startPitch = 10f;

    [Header("Follow & smoothing")]
    [Tooltip("Local offset from target: e.g. (0, 2, -4)")]
    public Vector3 offset = new Vector3(0f, 1.8f, -4f);
    [Tooltip("Position smoothing time (smaller = snappier)")]
    public float positionSmoothTime = 0.12f;
    [Tooltip("Rotation smoothing speed (higher = snappier)")]
    public float rotationSmoothSpeed = 12f;

    [Header("Cursor")]
    public bool lockCursor = true;

    // Public read-only: the user requested "just give me a vector"
    // This vector contains the Euler angles you can apply to your player's rotation:
    // e.g. playerTransform.eulerAngles = cameraController.playerYawEuler;
    [HideInInspector] public Vector3 playerYawEuler;

    // internal
    float yaw;
    float pitch;
    Vector3 currentVelocity = Vector3.zero;

    void Start()
    {
        yaw = startYaw;
        pitch = startPitch;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (target == null)
        {
            Debug.LogWarning("ThirdPersonCamera: target not assigned. The camera will not follow anything.");
        }
    }

    void Update()
    {
        HandleMouseInput();
        // Update the vector that player script can read to rotate itself to the camera's yaw.
        // We only give yaw (y-axis) because that's usually what you want for player facing.
        playerYawEuler = new Vector3(0f, yaw, 0f);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired rotation from current yaw/pitch
        Quaternion desiredRotation = Quaternion.Euler(pitch, yaw, 0f);

        // Desired position is target position + rotated offset
        Vector3 desiredPosition = target.position + desiredRotation * offset;

        // Smooth position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, positionSmoothTime);

        // Smooth rotation towards the desired rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Mathf.Clamp01(rotationSmoothSpeed * Time.deltaTime));
    }

    void HandleMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // apply sensitivity and invert Y if desired
        yaw += mouseX * mouseSensitivity;
        pitch += (invertY ? 1 : -1) * mouseY * mouseSensitivity;

        // clamp pitch
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    // Optional: call this to re-center camera behind player instantly (useful on respawn)
    public void SnapBehindTarget(float yawOffset = 0f)
    {
        if (target == null) return;
        // Face the same forward as the target's current forward (optional)
        Vector3 flatForward = new Vector3(target.forward.x, 0f, target.forward.z).normalized;
        if (flatForward.sqrMagnitude > 0.001f)
        {
            yaw = Mathf.Atan2(flatForward.x, flatForward.z) * Mathf.Rad2Deg + yawOffset;
        }
        pitch = startPitch;
        Quaternion desiredRotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.rotation = desiredRotation;
        transform.position = target.position + desiredRotation * offset;
    }
}
