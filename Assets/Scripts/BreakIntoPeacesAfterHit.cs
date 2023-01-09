using System.Linq;
using TSUtils.Sounds;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BreakIntoPeacesAfterHit : MonoBehaviour
{
    public SoundAsset Sound;
    
    private void OnCollisionEnter(Collision collision)
    {
        var children = transform.Cast<Transform>().ToList();

        foreach (var child in children)
        {
            child.SetParent(null);
            child.gameObject.AddComponent<Rigidbody>();
            Destroy(child.gameObject, 1);
            
            var collisionSound = child.gameObject.AddComponent<PlaySoundOnCollision>();
            collisionSound.Sound = Sound;
        }

        enabled = false;
        Destroy(gameObject);
    }
}
