using UnityEngine;

public class WheelRotate : MonoBehaviour
{
    public float rotationSpeed = 500f;

    void Update()
    {
        float move = Input.GetAxis("Vertical");

        if (Mathf.Abs(move) > 0.1f)
        {
            transform.Rotate(rotationSpeed * move * Time.deltaTime, 0, 0);
        }
    }
}