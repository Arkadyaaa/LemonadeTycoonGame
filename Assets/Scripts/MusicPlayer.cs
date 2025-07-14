using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    public string musicFolder = "Audio/Music";
    public float minDelay = 10f;
    public float maxDelay = 30f;
    public float fadeDuration = 5f;
    public float maxVolume = 0.2f;

    private AudioClip[] musicClips;
    private AudioSource audioSource;
    private int lastPlayedIndex = -1;

    private void Start()
    {
        musicClips = Resources.LoadAll<AudioClip>(musicFolder);
        audioSource = gameObject.GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.volume = maxVolume;

        StartCoroutine(PlayRandomMusicWithDelay());
    }

    private IEnumerator PlayRandomMusicWithDelay()
    {
        while (true)
        {
            if (!audioSource.isPlaying)
            {
                int newIndex;
                do
                {
                    newIndex = Random.Range(0, musicClips.Length);
                }
                while (musicClips.Length > 1 && newIndex == lastPlayedIndex);

                lastPlayedIndex = newIndex;
                AudioClip newClip = musicClips[newIndex];

                if (audioSource.clip != null)
                    yield return StartCoroutine(FadeOut(fadeDuration));

                audioSource.clip = newClip;
                audioSource.Play();
                yield return StartCoroutine(FadeIn(fadeDuration));

                float waitTime = Random.Range(minDelay, maxDelay) + newClip.length;
                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator FadeOut(float duration)
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    private IEnumerator FadeIn(float duration)
    {
        float time = 0f;
        audioSource.volume = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, maxVolume, time / duration);
            yield return null;
        }

        audioSource.volume = maxVolume;
    }
}