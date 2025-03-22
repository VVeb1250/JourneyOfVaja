using UnityEngine;

public class SoundEnemy : MonoBehaviour
{
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        SoundManager.AdjustEffectSound(audioSource);
    }

    private void PlaySound(AudioClip Sound)
    {
        audioSource.PlayOneShot(Sound);
    }
}
