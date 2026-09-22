using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public CapsuleCollider capsule;

    //the camera to move relative to (assign the Cinemachine FreeLook's actual render camera, i.e. Main Camera)
    public Transform cameraTransform;

    public float speed = 12f;
    public float jumpHeight = 3f;

    //multiplies Physics.gravity for this player only, so we can tweak feel without affecting other rigidbodies
    public float gravityScale = 1f;

    //how long (in seconds) it takes the player to turn to face the movement direction
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float dashSpeed = 24f;
    public float dashDuration = 0.2f;

    Rigidbody rb;
    bool isGrounded;
    bool jumpRequested;

    bool dashRequested;
    bool isDashing;
    float dashTimer;
    Vector3 dashDirection;

    float inputX;
    float inputZ;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //physics shouldn't tumble the character - we control facing ourselves
        rb.freezeRotation = true;
        //we apply our own scaled gravity in FixedUpdate instead
        rb.useGravity = false;
    }

    void Start()
    {
        //hide and lock the cursor to the center of the screen while playing
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame - read input here so we never miss a button press between physics steps
    void Update()
    {
        //checking if we're on the ground so we can jump
        //(uses TransformPoint + lossyScale instead of raw local values so this still lines up
        //correctly if the player's scale changes, e.g. during the grow power-up)
        Vector3 capsuleCenterWorld = transform.TransformPoint(capsule.center);
        float halfHeight = capsule.height * transform.lossyScale.y / 2f;
        Vector3 feetPosition = capsuleCenterWorld - Vector3.up * halfHeight;
        isGrounded = Physics.CheckSphere(feetPosition, groundDistance, groundMask);

        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        //also require near-zero vertical speed so we can't re-trigger a jump while still rising
        //(isGrounded can stay true for a frame or two after a jump starts, since the CheckSphere
        //still overlaps the ground until we've risen past groundDistance)
        if (Input.GetButtonDown("Jump") && isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            jumpRequested = true;
        }

        if (Input.GetMouseButtonDown(1) && !isDashing)
        {
            dashRequested = true;
        }
    }

    // FixedUpdate is where all rigidbody/transform changes happen so interpolation stays smooth
    void FixedUpdate()
    {
        Vector3 input = new Vector3(inputX, 0f, inputZ);
        Vector3 moveDirection = Vector3.zero;

        if (input.magnitude >= 0.1f)
        {
            //turn to face the input direction relative to where the camera is looking
            float targetAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(rb.rotation.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        if (dashRequested)
        {
            //dash in whatever direction we're currently moving, or where we're facing if standing still
            dashDirection = moveDirection.magnitude >= 0.1f ? moveDirection.normalized : transform.forward;
            isDashing = true;
            dashTimer = dashDuration;
            dashRequested = false;
        }

        if (isDashing)
        {
            //override normal movement for the duration of the dash - flat, no vertical drift since gravity is off
            rb.linearVelocity = new Vector3(dashDirection.x * dashSpeed, 0f, dashDirection.z * dashSpeed);

            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }
        else
        {
            Vector3 velocity = moveDirection.normalized * speed;
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        }

        if (!isDashing)
        {
            //apply our own gravity since rb.useGravity is off, so gravityScale actually affects falling
            //(skipped while dashing so the dash stays flat instead of arcing/falling)
            rb.linearVelocity += Physics.gravity * (gravityScale * Time.fixedDeltaTime);
        }

        if (jumpRequested)
        {
            //the equation for jumping, using our scaled gravity so it matches how the Rigidbody actually falls
            float scaledGravityY = Physics.gravity.y * gravityScale;
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * scaledGravityY);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
            jumpRequested = false;
        }
    }
}
