using TSUtils.Sounds;
using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    public SoundAsset Sound;

    private void OnCollisionEnter(Collision other)
    {
        SoundManager.Instance.Play(Sound);
    }
}
