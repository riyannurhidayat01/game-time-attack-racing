using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 15f;
    public float turnSpeed = 80f;
    public float nitroSpeed = 30f;

    private Rigidbody rb;
    private AudioSource engineAudio;

    private float moveInput;
    private float turnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        engineAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        if (engineAudio != null)
        {
            if (Mathf.Abs(moveInput) > 0.1f && !engineAudio.isPlaying)
                engineAudio.Play();

            if (Mathf.Abs(moveInput) <= 0.1f && engineAudio.isPlaying)
                engineAudio.Stop();
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? nitroSpeed : speed;

        Vector3 moveVelocity = transform.forward * moveInput * currentSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);

        if (Mathf.Abs(moveInput) > 0.1f)
        {
            Quaternion turnRotation = Quaternion.Euler(0, turnInput * turnSpeed * Time.fixedDeltaTime, 0);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}