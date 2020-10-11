using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCrowdLoading : MonoBehaviour
{
    public List<Sprite> allSingleCrowdSp = new List<Sprite>();
    public GameObject singleCrowdPrefab;
    public bool load = false;

    private void OnValidate()
    {
        if (load)
        {
            load = false;
            SimpleAnimator sa = null;
            for (int i = 0; i < allSingleCrowdSp.Count; i++)
            {
                if(i%6 == 0)
                {
                    sa = Instantiate(singleCrowdPrefab, transform).GetComponent<SimpleAnimator>();
                    sa.GetComponent<SpriteRenderer>().sprite = allSingleCrowdSp[i];
                    sa.animations.Add(new SimpleAnimator.Anim());
                    sa.animations[0].frames = new Sprite[6];
                }

                sa.animations[0].frames[i % 6] = allSingleCrowdSp[i];
            }
        }
    }
}
