using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovment : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundMask;

    public AudioClip footStepSFX;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector2 movementInput;
    private PlayerInput playerInput;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = new PlayerInput();

        StartCoroutine(PlayFootStep());

    }


    void Update()
    {
        
    }

    void FixedUpdate()
    {
        CheckGrounded();
        MovePlayer();
        rb.angularVelocity = Vector3.zero;
    }
    void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
    }

    void OnMovment(InputValue value) 
    {
        movementInput = value.Get<Vector2>();
    }
    void MovePlayer()
    {
        Vector3 direction = transform.right * movementInput.x + transform.forward * movementInput.y;
        direction.Normalize();
        rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);
    }

    IEnumerator PlayFootStep()
    { 
        while (true)
        {
            if (rb.linearVelocity.magnitude > 0.1f && isGrounded)
            {
                AudioManager.Instance.PlaySFX(footStepSFX);
            }
            yield return new WaitForSeconds(0.5f);

        }
    }
}
