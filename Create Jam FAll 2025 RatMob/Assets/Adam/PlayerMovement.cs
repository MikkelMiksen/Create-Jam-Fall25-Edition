using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float velocityLimit;

    private Vector3 moveDir;
    
    public InputAction moveInput;

    private void Awake()
    {
        moveInput.Enable();
    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        moveDir = new Vector3(moveInput.ReadValue<Vector2>().x, 0, moveInput.ReadValue<Vector2>().y).normalized;
        print(moveDir);
    }

    public void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        rb.linearVelocity = moveDir * movementSpeed;
       print("linveloc: " + rb.linearVelocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            TriggerBox triggerBox = other.gameObject.GetComponent<TriggerBox>();
            if (triggerBox.triggerType == TriggerType.LevelChangeTrigger)
            {
                SceneHandler.instance.LoadScene(triggerBox.sceneIndexToLoad);
            }
        }
    }
}












// unused struct
public struct PlayerData
{
    public PlayerData(float moveSpeed, float jumpForce)
    {
        this.Movespeed = moveSpeed;
        this.JumpForce = jumpForce;
    }
    
    public float Movespeed { get; private set; }
    public float JumpForce { get; private set; }
}

