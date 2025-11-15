using System;
using TMPro;
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
 
    void Start()
    {
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
        HandleActionInput();
        Jump();
        
        PlayerYawToCamAlign();
        direction = UpdateInputAndDirection();
        

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

    private void FixedUpdate()
    {
        MovePlayer();
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


    private void MovePlayer()
    {
        if (isSneak)
        {
            rb.linearVelocity = new Vector3(direction.x * _speed * sneakSpeedMultiplier, rb.linearVelocity.y, direction.z * _speed *sneakSpeedMultiplier);    
        }
        else
        {
            rb.linearVelocity = new Vector3(direction.x * _speed, rb.linearVelocity.y, direction.z * _speed);
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