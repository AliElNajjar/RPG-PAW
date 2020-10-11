using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLockedCollider : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.doorLocked));
        }
    }
}
