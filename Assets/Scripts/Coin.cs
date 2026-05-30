using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioClip coinSound;

    void Update()
    {
        transform.Rotate(0, 0, 100 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject tempAudio = new GameObject("CoinSound");

            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();

            audioSource.clip = coinSound;
            audioSource.Play();

            Destroy(tempAudio, coinSound.length);

            GameManager.instance.AddScore(1);

            Destroy(gameObject);
        }
    }
}