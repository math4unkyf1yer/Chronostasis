using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("References")]
    public Transform orientation; // Assign an empty GameObject for movement direction

    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    public float runSpeedMultiplier = 1.7f; // Speed multiplier when running
    public float maxStamina = 100f; // Max stamina value
    public float staminaDrainRate = 17f; // How much stamina is drained per second while running
    public float staminaRegenRate = 3.5f; // How much stamina regenerates per second when not running
    public float staminaCooldownTimer = 3f; // Cooldown time before stamina can regenerate
    private float currentStamina; // Current stamina value
    private Vector3 moveDirection;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float playerHeight = 2f;
    public LayerMask groundMask;
    public LayerMask moveObjectMask;
    public Transform groundCheck;

    private bool grounded;

    private Slider staminaSlider;

    public Selecting selectingScript;
    private PlatformMove platScript;

    public bool running;

    void Start()
    {
        //   staminaSlider = GameObject.Find("StaminaSlider").GetComponent<Slider>();
        //   staminaSlider.maxValue = maxStamina;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevents unwanted rotation
        currentStamina = maxStamina; // Initialize stamina
        staminaCooldownTimer = 0f; // Initialize the cooldown timer

    }


    void Update()
    {
        GetInput();
        Jump();
     //   UpdateStaminaSlider();
      //  HandleStamina();
    }

    void FixedUpdate()
    {
            MovePlayer();  
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        running = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton8)) && currentStamina > 0f;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        moveDirection.y = 0; // Prevent vertical movement

        float currentSpeed = running ? moveSpeed * runSpeedMultiplier : moveSpeed;


        // Apply movement with controller sensitivity
        rb.velocity = moveDirection.normalized * currentSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded() && selectingScript.isGrabbing == false)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // reset vertical velocity
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                Debug.Log("Jump");
            }
            else
            {
                Debug.Log("Not grounded");
            }
        }
    }

    private bool isGrounded()
    {
        return Physics.Raycast(groundCheck.position, Vector3.down, 0.3f, groundMask);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
        {
            Debug.Log("asdsds");
            selectingScript.cannotGrabble = true;
            if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "PlatformG")
            {
                platScript = collision.gameObject.GetComponent<PlatformMove>();
                platScript.PlayerOnObject();
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
        {
            Debug.Log("Exit");
            selectingScript.cannotGrabble = false;
            if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "PlatformG")
            {
                platScript = collision.gameObject.GetComponent<PlatformMove>();
                platScript.PlayerOffObject();
            }
        }
    }

    /* private void HandleStamina()
     {
         // If stamina is above 0, drain stamina when running
         if (running)
         {
             currentStamina -= staminaDrainRate * Time.deltaTime;
             if (currentStamina < 0)
             {
                 currentStamina = 0;
                 staminaCooldownTimer = 3; // Start cooldown when stamina reaches zero
                 moveSpeed = 2;
             }
         }
         else
         {
             // If the cooldown timer is running, do not regenerate stamina
             if (staminaCooldownTimer > 0f)
             {

                 staminaCooldownTimer -= Time.deltaTime;
             }
             else
             {

                 if (moveSpeed != 5) moveSpeed = 5;
                 // Regenerate stamina when not running and cooldown has passed
                 currentStamina += staminaRegenRate * Time.deltaTime;
                 if (currentStamina > maxStamina) currentStamina = maxStamina;
             }
         }
     }
     private void UpdateStaminaSlider()
     {
         if (staminaSlider != null)
         {
             // Update the stamina slider to reflect current stamina value
             staminaSlider.value = currentStamina;
         }
     }*/
}


