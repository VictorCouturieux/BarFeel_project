using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    [SerializeField] private PlayerSoundGameEvent playerSoundEvent;
    [SerializeField] private AudioClip[] clean;
    [SerializeField] private AudioClip[] glassFill;
    [SerializeField] private AudioClip[] glassGrab;
    [SerializeField] private AudioClip[] glassLaunch;
    [SerializeField] private AudioClip[] glassSlip;
    [SerializeField] private AudioClip[] glassWipe;

    private void OnEnable() {
        playerSoundEvent.AddCallback(OnPlayerSound);
    }

    private void OnPlayerSound(PlayerSoundType type, Vector3 position) {
        AudioClip clip = GetRandomClipForType(type);
        if (clip != null) {
            AudioSource.PlayClipAtPoint(clip, position, 2.0f);
        }
    }

    private AudioClip GetRandomClipForType(PlayerSoundType type) {
        switch (type) {
            case PlayerSoundType.CLEAN:
                return clean[Random.Range(0, clean.Length)];
            case PlayerSoundType.GLASS_FILL:
                return glassFill[Random.Range(0, glassFill.Length)];
            case PlayerSoundType.GLASS_GRAB:
                return glassGrab[Random.Range(0, glassGrab.Length)];
            case PlayerSoundType.GLASS_LAUNCH:
                return glassLaunch[Random.Range(0, glassLaunch.Length)];
            case PlayerSoundType.GLASS_SLIP:
                return glassSlip[Random.Range(0, glassSlip.Length)];
            case PlayerSoundType.GLASS_WIPE:
                return glassWipe[Random.Range(0, glassWipe.Length)];
        }

        return null;
    }

    private void OnDisable() {
        playerSoundEvent.RemoveCallback(OnPlayerSound);
    }
}

[System.Serializable]
public enum PlayerSoundType {
    CLEAN,
    GLASS_FILL,
    GLASS_GRAB,
    GLASS_LAUNCH,
    GLASS_SLIP,
    GLASS_WIPE
}
