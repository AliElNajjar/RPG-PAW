using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoopCarAnmController : MonoBehaviour
{
    public SpriteRenderer[] sprites;
    public float SpeedOfCar;
    public bool CarAnimation;
    public static bool CinematicComplete=false;
    // Start is called before the first frame update
    void Start()
    {
         StartCoroutine(CarSprtiesChanger());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator CarSprtiesChanger()
    {
        yield return new WaitForSeconds(SpeedOfCar);
        sprites[0].enabled=true;
        yield return new WaitForSeconds(SpeedOfCar);
        sprites[0].enabled=false;
        sprites[1].enabled=true;
        yield return new WaitForSeconds(SpeedOfCar);
        sprites[1].enabled=false;
        sprites[2].enabled=true;
        yield return new WaitForSeconds(SpeedOfCar);
        sprites[2].enabled=false;
        sprites[3].enabled=true;
        yield return new WaitForSeconds(SpeedOfCar);
        sprites[3].enabled=false;
        sprites[4].enabled=true;
        yield return new WaitForSeconds(SpeedOfCar);
        sprites[4].enabled=false;
        sprites[5].enabled=true;
        yield return new WaitForSeconds(SpeedOfCar);
        sprites[5].enabled=false;
        sprites[6].enabled=true;
        yield return new WaitForSeconds(SpeedOfCar);
        sprites[6].enabled=false;
        sprites[7].enabled=true;
        yield return new WaitForSeconds(SpeedOfCar);
        sprites[7].enabled=false;
        sprites[8].enabled=true;
        yield return new WaitForSeconds(SpeedOfCar);
        sprites[8].enabled=false;
        sprites[9].enabled=true;
                yield return new WaitForSeconds(SpeedOfCar);
        sprites[9].enabled=false;
        sprites[10].enabled=true;
                yield return new WaitForSeconds(SpeedOfCar);
        sprites[10].enabled=false;
        sprites[11].enabled=true;
                yield return new WaitForSeconds(SpeedOfCar);
        sprites[11].enabled=false;
        sprites[12].enabled=true;
                yield return new WaitForSeconds(SpeedOfCar);
        sprites[12].enabled=false;
        sprites[13].enabled=true;
                yield return new WaitForSeconds(SpeedOfCar);
        sprites[13].enabled=false;
        sprites[14].enabled=true;
                yield return new WaitForSeconds(SpeedOfCar);
        sprites[14].enabled=false;
        CinematicComplete=true;
        
/*             for(int i=0;i<10;i++)
            {
                yield return new WaitForSeconds(SpeedOfCar);
                sprites[i].enabled=true;
                yield return new WaitForSeconds(SpeedOfCar);
                sprites[i].enabled=false;
            } */
        

        yield return null;


    }
}
