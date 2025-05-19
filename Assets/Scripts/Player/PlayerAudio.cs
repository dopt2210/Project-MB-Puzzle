using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public PlayerMovement character;
    public PlayerCheck check;

    [Tooltip("Minimum velocity for moving audio to play")]
    /// <summary> "Minimum velocity for moving audio to play" </summary>
    private float velocityThreshold = .01f;
    private bool isMovingSoundPlaying = false;

    [SerializeField] private AudioSource moveAudioSource;
    private string runSound = "running", walkSound = "stepping";
    Vector2 lastCharacterPosition;
    Vector2 CurrentCharacterPosition => new Vector2(character.transform.position.x, character.transform.position.z);


    private void Reset()
    {
        moveAudioSource = GetComponentInChildren<AudioSource>();
        character = GetComponentInParent<PlayerMovement>();
        check = (transform.parent ?? transform).GetComponentInChildren<PlayerCheck>();
    }
    private void FixedUpdate()
    {
        PlayMovingAudio();
    }
    void PlayMovingAudio()
    {
        float velocity = Vector3.Distance(CurrentCharacterPosition, lastCharacterPosition);

        if (check && check.isGrounded && velocity > velocityThreshold)
        {
            string soundToPlay = character.IsRunning ? runSound : walkSound;
            AudioClip clip = SoundManager.Instance.GetFromLibrary(soundToPlay);

            if (!isMovingSoundPlaying || moveAudioSource.clip != clip)
            {
                moveAudioSource.clip = clip;
                moveAudioSource.Play();
                isMovingSoundPlaying = true;
            }
        }
        else
        {
            if (isMovingSoundPlaying)
            {
                moveAudioSource.Stop();
                isMovingSoundPlaying = false;
            }
        }

        lastCharacterPosition = CurrentCharacterPosition;
    }

}
