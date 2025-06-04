using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSmoothTime = 0.1f;
    public float jumpForce = 7f;
    public Transform cameraTransform;
    public Animator playerAnimator;

    private Rigidbody rb;
    private float turnSmoothVelocity;
    private bool isGrounded;
    private bool isDamaged = false;

    private float idleTimer = 0f;
    private float idleThreshold = 10f;
    private bool wasIdle = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();

    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerAnimator.SetBool("isJumping", true);
            Jump();
        }

        if (playerAnimator != null)
            playerAnimator.SetBool("isDamaged", isDamaged);

        bool isIdle = rb.velocity.magnitude < 0.1f && isGrounded;
        bool isWalking = false;
        if (playerAnimator != null)
            isWalking = playerAnimator.GetBool("isWalking");

        if (isIdle && !isWalking)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > idleThreshold && !wasIdle)
            {
                wasIdle = true;
                if (playerAnimator != null)
                {
                    int idleIndex = Random.Range(1, 3);
                    switch(idleIndex)
                    {
                        case 1:
                            playerAnimator.SetBool("idle1", true);
                            playerAnimator.SetBool("idle2", false);
                            StartCoroutine(ResetIdleAfterRotation("idle1"));
                            break;
                        case 2:
                            playerAnimator.SetBool("idle1", false);
                            playerAnimator.SetBool("idle2", true);
                            StartCoroutine(ResetIdleAfterRotation("idle2"));
                            break;
                    }
                }
            }
        }
        else
        {
            idleTimer = 0f;
            wasIdle = false;
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("idle1", false);
                playerAnimator.SetBool("idle2", false);
            }
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        bool isRunning = direction.magnitude >= 0.1f && isGrounded;

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isWalking", isRunning);
        }

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 velocity = moveDir.normalized * moveSpeed;
            velocity.y = rb.velocity.y;
            rb.velocity = velocity;
        }
        else if (isGrounded)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void Jump()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        isGrounded = false;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
            if (playerAnimator != null)
                playerAnimator.SetBool("isJumping", false);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
        if (playerAnimator != null)
            playerAnimator.SetBool("isJumping", true);

        // Unfreeze Y when not grounded (falling)
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void TakeDamage()
    {
        isDamaged = true;
        if (playerAnimator != null)
            playerAnimator.SetBool("isDamaged", true);
    }

    public void ResetDamage()
    {
        isDamaged = false;
        if (playerAnimator != null)
            playerAnimator.SetBool("isDamaged", false);
    }

    private System.Collections.IEnumerator ResetIdleAfterRotation(string idleBool)
    {
        float idleAnimDuration = 2.5f;
        yield return new WaitForSeconds(idleAnimDuration);

    }
}
