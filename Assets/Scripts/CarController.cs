using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 100f;
    public Joystick joystick;

    [Header("NOS Settings")]
    public float nosMultiplier = 2f;
    public float nosDuration = 3f;
    public float nosCooldown = 5f;

    private float currentSpeed;
    private bool isNosActive;
    private float nosTimer;
    private float cooldownTimer;

    void Start()
    {
        currentSpeed = speed;
    }

    void Update()
    {
        if (isNosActive)
        {
            nosTimer -= Time.deltaTime;
            if (nosTimer <= 0f)
            {
                isNosActive = false;
                currentSpeed = speed;
                cooldownTimer = nosCooldown;
            }
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        float move = joystick.Vertical;
        float turn = joystick.Horizontal;

        transform.Translate(Vector3.forward * move * currentSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * turn * turnSpeed * Time.deltaTime);
    }

    public bool IsNosReady()
    {
        return !isNosActive && cooldownTimer <= 0f;
    }

    public bool IsNosActive()
    {
        return isNosActive;
    }

    public float GetCooldownRemaining()
    {
        return cooldownTimer;
    }

    public void ActivateNos()
    {
        if (!IsNosReady()) return;

        isNosActive = true;
        nosTimer = nosDuration;
        currentSpeed = speed * nosMultiplier;
    }
}