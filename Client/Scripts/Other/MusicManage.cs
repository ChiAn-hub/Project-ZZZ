using UnityEngine;

public class MusicManage : MonoBehaviour
{
    private AudioSource audioSource;
    public static MusicManage Instance { get; private set; }
    public AudioSource source => audioSource;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        audioSource = gameObject.GetComponent<AudioSource>();
        AudioClip music = Resources.Load<AudioClip>("Audio/Start");
        MusicChange(music);
    }
    public void MusicSize(float size)
    {
        if (size < 0.0f || size > 1.0f)
        {
            return;
        }
        audioSource.volume = size;
    }

    public void MusicChange(AudioClip audioClip)
    {
        if (audioSource == null || audioClip == null)
        {
            return;
        }

        if (audioSource.clip != audioClip)
        {
            audioSource.clip = audioClip;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
