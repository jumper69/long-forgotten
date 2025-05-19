using UnityEngine;

public class DeathSound : MonoBehaviour
{
    public AudioClip deathClip;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDeathSound()
    {
        if (deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
        }
    }
}