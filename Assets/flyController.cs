using UnityEngine;

public class FlyController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float moveSpeed = 5f;
    public float flySpeed = 8f;
    public float fastFlySpeed = 16f;
    public float rotationSpeed = 100f;
    public float maxFloatHeight = 10f;
    public float minFloatHeight = 0f; 

    private float currentHeight; 
    private bool isFlying = false; 
    private bool isGrounded = true; 
    public float fallSpeed = 2f; 
    public float forwardFallReduction = 2f;
    private Animator anim;
    private Rigidbody rb;
    private Camera mainCamera;
    private CameraMovement cameraMovement;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        cameraMovement = mainCamera.GetComponent<CameraMovement>();

        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleFlyToggle();
        ClampHeight();
        RotateCharacter();
        HandleFalling();
    }

    private void HandleMovement()
    {
        if (!isFlying)
        {
            if (!isGrounded)
            {
                // Play falling animation
                anim.SetBool("isFalling", true);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                RunOrWalk();
            }
            else
            {
                ResetMovement();
            }
        }
        else
        {
            Fly();
        }
    }

    private void RunOrWalk()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("isRunning", true);
            anim.SetBool("isWalking", false);
            MoveForward(moveSpeed);
        }
        else
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isRunning", false);
            MoveForward(walkSpeed);
        }
    }

    // private void Fly()
    // {
    //     isGrounded = false;
    //     anim.SetBool("isFlying", true);
    //     anim.SetBool("isWalking", false);
    //     anim.SetBool("isRunning", false);
    //     anim.SetBool("isFalling", false);

    //     Vector3 forward = mainCamera.transform.forward * flySpeed * Time.deltaTime;
    //     transform.position += forward;

    // }

    private void Fly()
    {
        anim.SetBool("isFlying", true);
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isFalling", false);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            cameraMovement.SetTargetDistance(25f); // Notify camera
            Vector3 forward = mainCamera.transform.forward * fastFlySpeed * Time.deltaTime;
            transform.position += forward;
        }
        else
        {
            cameraMovement.SetTargetDistance(15f); // Notify camera
            Vector3 forward = mainCamera.transform.forward * flySpeed * Time.deltaTime;
            transform.position += forward;
        }
    }

    private void MoveForward(float speed)
    {
        Vector3 forward = mainCamera.transform.forward * speed * Time.deltaTime;
        transform.position += new Vector3(forward.x, 0, forward.z);
    }

    private void RotateCharacter()
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        if (!isFlying)
        {
            cameraForward.y = 0;
        }
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void HandleFalling()
    {
        if (!isFlying && !isGrounded)
        {
            anim.SetBool("isFlying", false);
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", false);

            Vector3 forward = mainCamera.transform.forward * (walkSpeed / forwardFallReduction) * Time.deltaTime;
            transform.position += new Vector3(forward.x, 0, forward.z); // Reduced forward movement
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }
    }

    private void ResetMovement()
    {
        anim.SetBool("isFlying", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isWalking", false);
    }

    private void HandleFlyToggle()
    {
        if (Input.GetKey(KeyCode.F))
        {
            isFlying = true;
            rb.linearVelocity = Vector3.zero; // Reset velocity
        }
        else
        {
            isFlying = false;
        }
    }

    private void ClampHeight()
    {
        if (isFlying)
        {
            currentHeight = Mathf.Clamp(transform.position.y, minFloatHeight, maxFloatHeight);
            transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isFalling", false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}