using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RingCosmetics/Playable", fileName = "CosmeticPlayable"), System.Serializable]
public class CosmeticPlayable : ScriptableObject
{
    public string cosmeticName = "";
    [TextArea] public string cosmeticDescription = "";
    public CosmeticCategory category;
    public GameObject playerParticles;
    public GameObject generalParticles;
    public List<GameObject> extraEffects;
    public string animationToPlay = "";
    public AudioClip soundFX;

    [Range(0f, 1f)] public float cosmeticTime = 1f;
    public bool isThemeMusic = false;

    [Header("Inventory Settings")]
    public bool isNew = true;
    public bool isEquipped = false;
    public bool isUnlocked = false;

    public IEnumerator PlayCosmetic(GameObject player = null, AudioSource audioSource = null)
    {
        if (extraEffects.Count > 0)
        {
            foreach (var effect in extraEffects)
            {
                Instantiate(effect, Vector3.zero, Quaternion.identity);
            }
        }

        if (playerParticles)
        {
            if (player)
            {
                GameObject vfx = Instantiate(playerParticles, player.transform);
                vfx.transform.position = player.transform.position;
            }
        }

        if (generalParticles)
        {
            Instantiate(generalParticles, Vector3.zero, generalParticles.transform.rotation);
        }

        if (!string.IsNullOrEmpty(animationToPlay) && player)
        {
            player.GetComponent<SimpleAnimator>().Play(animationToPlay);
        }

        if (audioSource && soundFX)
        {
            Debug.Log("Playing soundFX");
            audioSource.PlayOneShot(soundFX);
        }
        else
        {
            if (audioSource == null)
                Debug.Log("Audio source null");
            if (soundFX == null)
                Debug.Log("Sound FX null");
        }

        yield return new WaitForSeconds(cosmeticTime);

        StopCosmeticEffects();
    }

    public void StopCosmeticEffects()
    {
        foreach (var effect in extraEffects)
        {
            effect.OnComponent<CameraFlashEffect>(flashEffect =>
            {
                flashEffect.executing = false;
                flashEffect.StopAllCoroutines();
            }
            );
        }
    }
}

public enum CosmeticCategory
{
    Intro,
    Intermediate,
    Outro,
    Passive
}

public enum IntermediateCategory
{
    XBtn,
    YBtn,
    BBtn,
    ABtn
}
