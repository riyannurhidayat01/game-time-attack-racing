using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 100f;
    public Joystick joystick;

    void Update()
    {
        float move = joystick.Vertical;     // maju/mundur
        float turn = joystick.Horizontal;   // belok

        transform.Translate(Vector3.forward * move * speed * Time.deltaTime);
        transform.Rotate(Vector3.up * turn * turnSpeed * Time.deltaTime);
    }
}