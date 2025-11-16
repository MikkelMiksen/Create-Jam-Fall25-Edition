using System;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class MJ_PlayerController : MonoBehaviour
{
    public static MJ_PlayerController Instance; void Awake() => Instance = this;

    [Header("Player Data, feel free to change")] 
    [SerializeField]private float _speed = 5f;
    [SerializeField]private float interactionDistance = 5f;
    [SerializeField]private float jumpForce = 5f;

    [SerializeField]
    [Range(0f, 1f)] private float sneakSpeedMultiplier = 0.5f;

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
    //animator stuff

    public Animator animController;
    public Transform ModelTransform;
    
    // movement stuff
    private bool isSneak;
    private bool isGrounded;
    Vector3 direction;
    
    //jump stuff
    private float _jumpTickdown = 0.2f;
    private float _jumpTimerReset = 0.2f;
    private bool _canJump;
    private bool _pressedJump;
    
    //input actions
    [Header("Input Actions")]
    private InputAction jumpAction;
    public InputAction SneakAction;
    private float baseModelOffset;
 
    // cam
    
    public Camera Camera;
    
    void Start()
    {
        baseModelOffset = ModelTransform.position.y;
        jumpAction = InputSystem.actions.FindAction("Jump");   
        SneakAction.Enable();
        rb = GetComponent<Rigidbody>();
        _cam = Camera.main;
        promptIsShowing = false;
        prompt = null;
        promptDisplay.text = "";
    }

    void Update()
    {
        direction = UpdateInputAndDirection();
        Jump();
        HandleActionInput();
        PlayerYawToCamAlign();
        MovePlayer();
        

        // Apply constant horizontal velocity
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

    private void HandleActionInput()
    {
        if (jumpAction.triggered)
        {
            _pressedJump = true;
        }
        else
        {
            _pressedJump = false;
        }
        if (Input.GetKey(KeyCode.LeftShift)){isSneak = true;} else {isSneak = false;}
    }

    
    
    private void Jump()
    {
        _jumpTickdown -= Time.deltaTime;
        if(_jumpTickdown <= 0){_canJump = true;}
        
        if (isGrounded && _canJump && _pressedJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _pressedJump = false;
            _jumpTickdown = _jumpTimerReset;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    [SerializeField] private float modelOffsetvalue;
    private void MovePlayer()
    {
        if (direction.magnitude < 0.1f)
        {
            animController.SetBool("IsMoving", false);
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        animController.SetBool("IsMoving", true);

        // Movement: use direction vector directly (world space), NOT ModelTransform.TransformDirection
        Vector3 move = direction * _speed;
        if (isSneak) move *= sneakSpeedMultiplier;

        // Keep Y velocity (gravity / jump)
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        // Smoothly rotate model to face movement direction (not camera)
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        ModelTransform.rotation = Quaternion.Slerp(ModelTransform.rotation, targetRotation, 12f * Time.deltaTime);
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
        float camYaw = Camera.GetComponent<ThirdPersonCamera>().playerYawEuler.y;

        
        Quaternion targetRotation = Quaternion.Euler(0f, camYaw, 0f);
        ModelTransform.rotation = targetRotation;

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