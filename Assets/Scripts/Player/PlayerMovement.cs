using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public AudioSource boing;

    // Player Parameters
    private bool canControl = true;
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float rotationSpeed = 100f;
    public float jumpForce = 5f;
    [Range(0f, 2f)]
    public float airMultiplier = .5f;
    private bool isGrounded;
    public float groundDrag;

    private float Offset = 0.5f;

    public bool isFalling = false;
    private float fallTime = 0f;
    public float maxFallTime = 3f;

    public float maxSlowTime = 2.0f;
    public float slowTimeSpeed = 0.5f;

    private bool isMenuVisible = false;

    private Rigidbody rb;
    public GameObject radialMenu;
    private GameObject vine;
    private TimerController timerController;
    private PlayerRespawn playerRespawn;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        timerController = FindObjectOfType<TimerController>();
        playerRespawn = GetComponent<PlayerRespawn>();
    }

    void Update()
    {
        // Only handle input if the object is active
        
        if (gameObject.activeSelf)
        {
            // Check if the countdown timer has reached zero
            if (timerController != null && timerController.elapsedTime <= 0f)
            {
                playerRespawn.GameOver();
            }
            if (canControl)
            {
                HandleInput();
            }
            // Check if the M key is pressed
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (!isMenuVisible)
                {
                    // Enable the cursor when the radial menu is visiable
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    // Disable the cursor when the radial menu is not visiable
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }

                // Toggle the visibility of the radial menu
                isMenuVisible = !isMenuVisible;

                // Set the visibility of the radial menu canvas
                SetMenuVisibility(radialMenu, isMenuVisible);
            }
            if(Input.GetKeyDown(KeyCode.G))
            {
                slowTime();
                StartCoroutine(TimeCap());
            }
        }
    }

    void FixedUpdate()
    {
        // Check for falling in FixedUpdate to ensure consistent physics updates
        CheckFalling();
    }

    void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Check if the shift key is held down to sprint and if the player is grounded
        bool isSprinting = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));// && isGrounded;

        // Choose the speed based on whether the player is sprinting
        float currentMoveSpeed = isSprinting ? sprintSpeed : moveSpeed;

        // Get the player's local forward and right directions
        // Prevents keys from flipping when player's orientation changes.
        Vector3 forward = transform.forward * vertical;
        Vector3 right = transform.right * horizontal;

        // Calculate the movement direction based on the player's local space
        Vector3 movement = (forward + right).normalized;
        if (isFalling) movement.y = movement.y - .4f;
        // Apply movement to the player with the current speed
        //rb.velocity = new Vector3(movement.x * currentMoveSpeed, rb.velocity.y, movement.z * currentMoveSpeed);
        rb.AddForce(movement * currentMoveSpeed * (!isGrounded ? airMultiplier : 1));

        // Jumping with space key
        CheckGrounded();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            boing.Play();
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        float rayLength = 1.25f;
        float secondrayLength = 1.75f;
        //No Shift
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            isGrounded = true;
        }
        else if (Physics.Raycast(transform.position + Vector3.left * Offset + Vector3.up * 0.5f, Vector3.down, out hit, secondrayLength))
        {
            isGrounded = true;
        }
        else if (Physics.Raycast(transform.position + Vector3.right * Offset + Vector3.up * 0.5f, Vector3.down, out hit, secondrayLength))
        {
            isGrounded = true;
        }
        else if (Physics.Raycast(transform.position + Vector3.forward * Offset + Vector3.up * 0.5f, Vector3.down, out hit, secondrayLength))
        {
            isGrounded = true;
        } 
        else if (Physics.Raycast(transform.position - Vector3.forward * Offset + Vector3.up * 0.5f, Vector3.down, out hit, secondrayLength))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    void CheckFalling()
    {
        if (!isGrounded && !IsTouchingVine())
        {
            isFalling = true;
            fallTime += Time.fixedDeltaTime;

            // Check if the player has been falling for too long
            if (fallTime >= maxFallTime)
            {
                // Notify PlayerRespawn script about falling
                playerRespawn.FallDetected();
                // Reset falling variables
                isFalling = false;
                fallTime = 0f;
            }
        }
        else
        {
            // Reset falling variables if the player is grounded or touching the vine
            isFalling = false;
            fallTime = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ColorManager togglePlatformScript = FindObjectOfType<ColorManager>();
        // Check if the collided object has the "RedLens" tag
        if (other.CompareTag("RedLens"))
        {
            if (togglePlatformScript != null)
            {
                togglePlatformScript.collectedRedLens();
            }
            // Destroy the RedLens 
            Destroy(other.gameObject);
        }

        // Check if the collided object has the "BlueLens" tag
        if (other.CompareTag("BlueLens"))
        {
            if (togglePlatformScript != null)
            {
                togglePlatformScript.collectedBlueLens();
            }
            // Destroy the BlueLens 
            Destroy(other.gameObject);
        }

        // Check if the collided object has the "GreenLens" tag
        if (other.CompareTag("GreenLens"))
        {
            if (togglePlatformScript != null)
            {
                togglePlatformScript.collectedGreenLens();
            }
            // Destroy the BlueLens 
            Destroy(other.gameObject);
        }
        // Check if the collided object has the "Vine" tag
        if (other.CompareTag("Vine"))
        {
            vine = other.gameObject;
        }
    }

    bool IsTouchingVine()
    {
        // Check if the player is in contact with the vine
        return vine != null && vine.GetComponent<Collider>().bounds.Intersects(GetComponent<Collider>().bounds);
    }

    // Needed to pause player movement in pause script
    public void SetPlayerControl(bool enableControl)
    {
        canControl = enableControl;
    }

    // Function to set the visibility of the radial menu
    private void SetMenuVisibility(GameObject menu, bool isVisible)
    {
        if (menu != null)
        {
            menu.gameObject.SetActive(isVisible);
        }
    }

    private void slowTime()
    {
        if(Time.timeScale == 1.0f){
            Time.timeScale = slowTimeSpeed;
        } else {
            Time.timeScale = 1.0f;
        }
    }

    private IEnumerator TimeCap()
    {
        yield return new WaitForSeconds(maxSlowTime);
        Time.timeScale = 1.0f;
    }
}
