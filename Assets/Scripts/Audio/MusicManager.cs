using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] songs;
    [SerializeField] private float silenceBetweenSongs;

    [Header("Events")]
    [SerializeField] private VoidGameEvent musicStartEvent;
    [SerializeField] private IntGameEvent musicChangeEvent;
    [SerializeField] private VoidGameEvent allMusicPlayedEvent;

    private bool started;

    private void Start() {
        musicStartEvent.Call();
    }

    private void OnEnable() {
        musicStartEvent.AddCallback(OnMusicStart);
    }

    private void OnMusicStart() {
        if (!started) {
            started = true;
            StartCoroutine(MusicCoroutine());
        }
    }

    private IEnumerator MusicCoroutine() {
        for (int i = 0; i < songs.Length; i++) {
            musicSource.clip = songs[i];
            musicSource.Play();
            musicChangeEvent.Call(i);

            while (musicSource.isPlaying) {
                yield return new WaitForSeconds(0.5f);
            }

            if (i != (songs.Length - 1)) {
                yield return new WaitForSeconds(silenceBetweenSongs);
            }
        }

        allMusicPlayedEvent.Call();
    }

    private void OnDisable() {
        musicStartEvent.RemoveCallback(OnMusicStart);
    }
}
