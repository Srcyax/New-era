using Mirror;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Player components")]
    [SerializeField] PlayerComponents components;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] walkSounds;
    [SerializeField] AudioClip[] runSounds;

    private void Start() {
        audioSource.volume = components.localPlayer ? 0.05f : 0.8f;
    }

    private void Update() {
        if ( !audioSource.isPlaying && audioSource.clip )
            audioSource.clip = null;
    }

    public void WalkFootStep() {
        if ( audioSource.isPlaying )
            return;

        int i = Random.Range(0, walkSounds.Length);

        audioSource.clip = walkSounds[ i ];
        audioSource.Play();
    }

    public void RunFootStep() {
        int i = Random.Range(0, runSounds.Length);

        audioSource.clip = runSounds[ i ];
        audioSource.PlayOneShot(audioSource.clip);
    }
}
