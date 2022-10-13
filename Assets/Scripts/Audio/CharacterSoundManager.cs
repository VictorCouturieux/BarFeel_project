using UnityEngine;

public class CharacterSoundManager : MonoBehaviour
{
    [SerializeField] private CharacterSoundGameEvent characterSoundEvent;
    [SerializeField] private CharacterSoundList[] characterSoundLists;
    
    private void OnEnable() {
        characterSoundEvent.AddCallback(OnCharacterSound);
    }

    private void OnCharacterSound(CharacterType charType, CharacterSoundType soundType, Vector3 position) {
        for (int i = 0; i < characterSoundLists.Length; i++) {
            if (characterSoundLists[i].characterType == charType) {
                AudioSource.PlayClipAtPoint(characterSoundLists[i].GetRandomClip(soundType), position);
                break;
            }
        }
    }

    private void OnDisable() {
        characterSoundEvent.RemoveCallback(OnCharacterSound);
    }
}

[System.Serializable]
public struct CharacterSoundList {
    public CharacterType characterType;
    public AudioClip[] drinking;
    public AudioClip[] ordering;
    public AudioClip[] randomNoises;
    public AudioClip[] satisfied;
    public AudioClip[] sigh;

    public AudioClip GetRandomClip(CharacterSoundType type) {
        switch (type) {
            case CharacterSoundType.DRINKING:
                return drinking[Random.Range(0, drinking.Length)];
            case CharacterSoundType.ORDERING:
                return ordering[Random.Range(0, ordering.Length)];
            case CharacterSoundType.SATISFIED:
                return satisfied[Random.Range(0, satisfied.Length)];
            case CharacterSoundType.SIGH:
                return sigh[Random.Range(0, sigh.Length)];
            default:
                return randomNoises[Random.Range(0, randomNoises.Length)];
        }
    }
}

[System.Serializable]
public enum CharacterSoundType {
    DRINKING,
    ORDERING,
    RANDOM_NOISES,
    SATISFIED,
    SIGH
}
