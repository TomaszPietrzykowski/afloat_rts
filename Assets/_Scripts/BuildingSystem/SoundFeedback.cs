using UnityEngine;

public class SoundFeedback : MonoBehaviour
{
    [SerializeField]
    private AudioClip clickSound, placeRaftSound, placeBuildingSound, removeSound, wrongPlacementSound;

    [SerializeField]
    private AudioSource audioSource;

    public void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Click:
                audioSource.PlayOneShot(clickSound);
                break;
            case SoundType.PlaceRaft:
                audioSource.PlayOneShot(placeRaftSound);
                break;
            case SoundType.PlaceBuilding:
                audioSource.PlayOneShot(placeBuildingSound);
                break;
            case SoundType.Remove:
                audioSource.PlayOneShot(removeSound);
                break;
            case SoundType.wrongPlacement:
                audioSource.PlayOneShot(wrongPlacementSound);
                break;
            default:
                break;
        }
    }
}

public enum SoundType
{
    Click,
    PlaceRaft,
    PlaceBuilding,
    Remove,
    wrongPlacement
}