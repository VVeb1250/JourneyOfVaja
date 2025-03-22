using UnityEngine;

public class SoundManager : MonoBehaviour {
    public int mode = 1; // [0] = BGM, [1] = EFFECT
    public static int[] settingSound = {5, 5}; // [0] = BGM, [1] = EFFECT

    // bgm
    void Start()
    {
        AudioSource audioSource = FindAnyObjectByType<AudioSource>();
        if (mode == 0) { AdjustBGMSound(audioSource); }
        if (mode == 1) { AdjustEffectSound(audioSource); }
    }
    public static void AdjustBGMSound(AudioSource audioSource) {
        audioSource.volume = 0.1f * settingSound[0];
    }
    public static void AdjustEffectSound(AudioSource audioSource) {
        audioSource.volume = 0.1f * settingSound[1];
    }
}