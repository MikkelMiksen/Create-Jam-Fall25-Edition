using TMPro;
using UnityEngine;

public class MJ_PlayerController : MonoBehaviour
{
    public static MJ_PlayerController Instance; void Awake() => Instance = this;

    [SerializeField]
    private float _speed = 5f, interactionDistance = 5f;

    float horizontal, vertical, promptTime = 3f, elapsedTime, distToNearestInteractable;

    [SerializeField]
    private TextMeshProUGUI promptDisplay;

    Rigidbody rb;
    private Camera _cam;

    private Vector3 camForward, camRight;
    private bool promptIsShowing = false;

    private string prompt;
    private Iinteractable nearestIinteractable = null;
    private Collider nearestHit;

    public Vector3 ExposedMoveDir;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _cam = Camera.main;
        promptIsShowing = false;
        prompt = null;
        promptDisplay.text = "";
    }

    void Update()
    {
        PlayerYawToCamAlign();
        Vector3 dir = UpdateInputAndDirection();
        ExposedMoveDir = dir;

        // Apply constant horizontal velocity
        rb.linearVelocity = new Vector3(dir.x * _speed, rb.linearVelocity.y, dir.z * _speed);

        if (!promptIsShowing)
        {
            //Locate nearest interactable
            PromptNotShowing();
        }
        else
        {
            //show nearest interactable prompt
            ShowPrompt();
        }
    }


    void PromptNotShowing()
    {
        elapsedTime = 0f;
        distToNearestInteractable = float.MaxValue;
        nearestIinteractable = null;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance);
        if (hitColliders.Length <= 0)
            return;

        foreach (var hit in hitColliders)
        {
            Iinteractable interactable = hit.GetComponent<Iinteractable>();
            if (interactable != null)
            {
                if (Vector3.Distance(transform.position, hit.transform.position) < distToNearestInteractable)
                {
                    distToNearestInteractable = Vector3.Distance(transform.position, hit.transform.position);
                    nearestIinteractable = interactable;
                    nearestHit = hit;
                }
            }
        }

        if (nearestIinteractable != null)
        {
            prompt = nearestIinteractable.GetPrompt();
            promptDisplay.text = prompt;
            promptIsShowing = true;
        }
        else
        {
            promptDisplay.text = "";
            promptIsShowing = false;
        }
    }

    void ShowPrompt()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime <= promptTime && nearestIinteractable != null && Vector3.Distance(transform.position,nearestHit.transform.position) < interactionDistance)
        {
            promptDisplay.text = prompt;
            if (Input.GetKeyDown(KeyCode.E))
            {
                nearestIinteractable.Interact();
                elapsedTime = 0f;
                promptIsShowing = false;
                promptDisplay.text = "";
            }
        }
        else
        {
            elapsedTime = 0f;
            promptDisplay.text = "";
            promptIsShowing = false;
        }
    }

    void OnDrawGizmos()
    {
        if (promptIsShowing)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }



    void PlayerYawToCamAlign()
    {
        // Align player yaw with camera yaw
        transform.rotation = Quaternion.Euler(0f, _cam.transform.eulerAngles.y, 0f);
    }

    Vector3 UpdateInputAndDirection()
    {
        // Get input
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Build movement direction relative to camera
        camForward = _cam.transform.forward;
        camRight = _cam.transform.right;

        // Flatten to horizontal plane
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 dir = (camForward * vertical + camRight * horizontal).normalized;
        return dir;
    }
}