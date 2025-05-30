using System.Collections;
using UnityEngine;
 
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
 
    [SerializeField]
    private MusicLibrary musicLibrary;
    [SerializeField]
    private AudioSource musicSource;
 
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }
    private void Reset()
    {
        musicLibrary = Resources.Load<MusicLibrary>("Prefab/Audio/MusicLibrary");
        musicSource = transform.GetChild(1).GetComponent<AudioSource>();
    }

    public void PlayMusic(string trackName, float fadeDuration = 0.5f)
    {
        StartCoroutine(AnimateMusicCrossfade(musicLibrary.GetClipFromName(trackName), fadeDuration));
    }
 
    IEnumerator AnimateMusicCrossfade(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(1f, 0, percent);
            yield return null;
        }
 
        musicSource.clip = nextTrack;
        musicSource.Play();
 
        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, 1f, percent);
            yield return null;
        }
    }
}