using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private Animator animator;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isRunning;
    private Vector3 inputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        if (inputDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        }

        if (isGrounded)
        {
            isRunning = inputDirection.magnitude > 0;
            animator.SetBool("toRun", isRunning);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
                animator.SetTrigger("toJump");
            }
        }
    }

    private void FixedUpdate()
    {
        if (inputDirection != Vector3.zero)
        {
            Vector3 move = inputDirection * moveSpeed;
            rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag.Equals("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag.Equals("Ground"))
        {
            isGrounded = false;
        }
    }
}
