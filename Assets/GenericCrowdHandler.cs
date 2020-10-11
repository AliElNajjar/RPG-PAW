using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GenericCrowdHandler : MonoBehaviour
{
    public Texture2D spriteSheet;
    public GameObject audienceMemberPrefab;
    public int maxAudienceMembers = 20;
    public int spriteSheetMultipleOf = 6;

    private Bounds bounds;
    private Sprite[] sprites;
    private float currentYPos = 0;
    private int currentMember = 0;

    void Start()
    {
        sprites = Resources.LoadAll<Sprite>(spriteSheet.name);
        bounds = GetComponent<Collider2D>().bounds;

        InstantiateAudienceMembers();
    }

    private void InstantiateAudienceMembers()
    {
        for (int i = 0; i < maxAudienceMembers; i++)
        {            
            GameObject NPC = Instantiate(audienceMemberPrefab, RandomPosInsideCollider(), Quaternion.identity, this.gameObject.transform);

            Sprite[] animSprites = GetAudienceMemberSprites();

            NPC.GetComponent<SimpleAnimator>().animations[0].frames = animSprites;

            NPC.GetComponent<SimpleAnimator>().Play("Cheer");

            NPC.GetComponent<SpriteRenderer>().sortingOrder = -currentMember;

            currentMember++;
        }
    }

    private Vector3 RandomPosInsideCollider()
    {
        Vector3 finalPos = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            CalculateYPos()
            );

        return finalPos;
    }
    private float CalculateYPos()
    {
        currentYPos = (float)((float)currentMember / (float)maxAudienceMembers);

        return Mathf.Lerp(bounds.min.y, bounds.max.y, currentYPos);
    }

    private Sprite[] GetAudienceMemberSprites()
    {
        if (sprites.Length == 0)
            return new Sprite[1];

        int numberOfPossibleAnimations = sprites.Length / spriteSheetMultipleOf;

        int[] multiplesOfSix = new int[numberOfPossibleAnimations]; //Spritesheet has a new animation every 5 frames

        int counter = 0;

        for (int i = 0; i < multiplesOfSix.Length; i++)
        {
            multiplesOfSix[i] = counter;

            counter += 6;
        }

        int randomIndex = Random.Range(0, multiplesOfSix.Length);

        Sprite[] anim = new Sprite[6]
        {
            sprites[multiplesOfSix[randomIndex]],
            sprites[multiplesOfSix[randomIndex] + 1],
            sprites[multiplesOfSix[randomIndex] + 2],
            sprites[multiplesOfSix[randomIndex] + 3],
            sprites[multiplesOfSix[randomIndex] + 4],
            sprites[multiplesOfSix[randomIndex] + 5],
        };

        return anim;
    }
}
