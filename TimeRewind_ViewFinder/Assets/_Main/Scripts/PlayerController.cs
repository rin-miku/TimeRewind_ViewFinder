using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;
    public float rotationSpeed = 1000f;

    private CharacterController characterController;
    private Animator animator;
    private Vector3 velocity;

    private bool isGrounded;
    private bool isRunning;
    private Vector3 inputDirection;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        inputDirection = new Vector3(horizontal, 0f, vertical);

        if (inputDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            characterController.Move(transform.forward * moveSpeed * Time.deltaTime);
        }

        if (isGrounded)
        {
            isRunning = horizontal != 0 || vertical != 0;
            animator.SetBool("toRun", isRunning);
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            animator.SetTrigger("toJump");
        }

        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = -2f;
        }

        characterController.Move(velocity * Time.deltaTime);
    }
}
