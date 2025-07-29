using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;


    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private bool readyToJump;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool grounded;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private RaycastHit slopeHit;
    private bool exitingSlope;
    
    [SerializeField] private Transform orientation; 
    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDir;

    private Rigidbody rb;
    
    [SerializeField] private MovementState state;

    public enum MovementState {
        walking,
        sprinting,
        crouching,
        air
    }

    [Header("Other Scripts")]
    [SerializeField] private PlayerGunSelector gunSelector;

    void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;  // prevents rb from falling over

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight*0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        if (grounded) {
            rb.drag = groundDrag;
        } else {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate() {
        MovePlayer();
    }

    private void MyInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Shoot input 
        if (gunSelector.activeGun != null) {
            gunSelector.activeGun.Tick(Input.GetMouseButton(0));
        }

        // Jump input
        if (Input.GetKey(jumpKey) && readyToJump && grounded) {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Crouch input down
        if (Input.GetKeyDown(crouchKey)) {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down *5f, ForceMode.Impulse);
        }

        // Crouch input up
        if (Input.GetKeyUp(crouchKey)) {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }


    }

    private void StateHandler() {
        // Crouching
        if (Input.GetKey(crouchKey)) {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Sprinting
        if (grounded && Input.GetKey(sprintKey)) {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        // Walking
        else if (grounded) {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        // Air
        else {
            state = MovementState.air;
        }
    }

    private void MovePlayer() {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Handle slope
        if (OnSlope() && !exitingSlope) {
            // Add a force in direction up/down slope 
            rb.AddForce(GetSlopeMoveDir() * moveSpeed * 10f , ForceMode.Force);
            rb.useGravity = false;
            if (rb.velocity.y > 0) {
                rb.AddForce(Vector3.down*40f, ForceMode.Force);
            }
        }
        // Grounded
        else if (grounded) {
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
            rb.useGravity = true;
        } 
        // Air
        else if (!grounded) {
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            rb.useGravity = true;
        }
        
    }

    private void SpeedControl() {
        // on slope speed limit
        if (OnSlope() && !exitingSlope) {
            if (rb.velocity.magnitude > moveSpeed) {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        } else {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Limit velocity to move speed
            if (flatVel.magnitude > moveSpeed) {
                Vector3 limitedVel = flatVel.normalized*moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump() {
        exitingSlope = true;
        // reset y velocity to get the same height jump
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight*0.5f+0.4f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return (angle < maxSlopeAngle && angle != 0);
        }
        return false;
    }

    private Vector3 GetSlopeMoveDir() {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }
}
