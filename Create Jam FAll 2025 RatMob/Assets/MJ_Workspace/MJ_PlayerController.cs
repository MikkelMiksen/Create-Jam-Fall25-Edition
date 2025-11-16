using TMPro;
using UnityEngine;

public class MJ_PlayerController : MonoBehaviour
{
    public static MJ_PlayerController Instance; 
    void Awake() => Instance = this;

    [SerializeField]
    private float _speed = 5f, interactionDistance = 5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float groundCheckDistance = 0.25f;
    [SerializeField] private LayerMask groundMask;

    float horizontal, vertical, promptTime = 3f, elapsedTime, distToNearestInteractable;

    [SerializeField]
    private TextMeshProUGUI promptDisplay;

    Rigidbody rb;
    private Camera _cam;

    private Vector3 camForward, camRight;
    private bool promptIsShowing = false;
    private bool isGrounded;

    private string prompt;
    private Iinteractable nearestIinteractable = null;
    private Collider nearestHit;

    public Animator animator;



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

        // Apply constant horizontal velocity
        rb.linearVelocity = new Vector3(dir.x * _speed, rb.linearVelocity.y, dir.z * _speed);

        // --- Ground check + Jump ---
        CheckGrounded();
        HandleJump();

        // Interaction logic
        if (!promptIsShowing)
        {
            PromptNotShowing();
        }
        else
        {
            ShowPrompt();
        }

        // Animator movement speed
        if (animator != null)
            animator.SetFloat("Speed", new Vector3(dir.x, 0, dir.z).magnitude);
    }


    // ---------------- JUMP SYSTEM ----------------

    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f,
                                    Vector3.down,
                                    groundCheckDistance,
                                    groundMask);
    }

    void HandleJump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);

            if (animator != null)
                animator.SetTrigger("Jump");
        }
    }



    // ---------------- INTERACTION SYSTEM ----------------

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
        if (elapsedTime <= promptTime && nearestIinteractable != null && 
            Vector3.Distance(transform.position, nearestHit.transform.position) < interactionDistance)
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
        Gizmos.color = promptIsShowing ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        // Ground check visualization
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.1f,
                        transform.position + Vector3.up * 0.1f + Vector3.down * groundCheckDistance);
    }


    // ---------------- MOVEMENT SYSTEM ----------------

    void PlayerYawToCamAlign()
    {
        transform.rotation = Quaternion.Euler(0f, _cam.transform.eulerAngles.y, 0f);
    }

    Vector3 UpdateInputAndDirection()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        camForward = _cam.transform.forward;
        camRight = _cam.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 dir = (camForward * vertical + camRight * horizontal).normalized;
        return dir;
    }
}
