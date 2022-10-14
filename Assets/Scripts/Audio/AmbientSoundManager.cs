using System.Collections;
using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    [Header("2D Audio")]
    [SerializeField] private AudioSource ambientSource2D;
    [Header("3D Audio")]
    [SerializeField] private AudioSource ambientSource3D;

    private IEnumerator Start() {
        // TODO: maybe replace with start on event

        ambientSource2D.Play();
        ambientSource3D.Play();
        while (true) {
            if (!ambientSource2D.isPlaying) {
                ambientSource2D.Play();
            }
            if (!ambientSource3D.isPlaying) {
                ambientSource3D.Play();
            }

            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }
}
