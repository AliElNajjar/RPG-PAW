using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatuesPadManager : MonoBehaviour
{
    public static StatuesPadManager Instance;

    [Header("Transforms")]
    public Transform[] statuePad;
    public Transform machachoManTransform, flamingoTrans, jaguarTrans, zebraTrans;

    [Header("Transforms")]
    public PolygonCollider2D nonWalkableCollider;
    public PolygonCollider2D walkableCollider;

    CameraFollowPlayer playerCam;
    CameraFilterPack_Drawing_Manga_Flash cameraVfx;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //get refrences
        playerCam = Camera.main.gameObject.GetComponent<CameraFollowPlayer>();
        cameraVfx = Camera.main.transform.GetComponent<CameraFilterPack_Drawing_Manga_Flash>();
        
        //check for unlocked statues
        for (int i = 0; i < SaveSystem.GetInt("StatuePad"); i++)
        {
            statuePad[i].gameObject.SetActive(true);
        }
        //enalbe walkable area on water
        if (SaveSystem.GetInt("StatuePad") > 0)
        {
            nonWalkableCollider.enabled = false;
            walkableCollider.enabled = true;
        }
        if (SaveSystem.GetBool("JaguarPad") == true)
        {
            SaveSystem.SetBool("JaguarPad", false);
            JaugarStatue();
        }
    }

    public void JaugarStatue()
    {
        StartCoroutine(JaguarStatueRise());
    }

    public void FlamingoStatue()
    {
        StartCoroutine(FlamingoStatueRise());
    }

    public void ZebraoStatue()
    {
        StartCoroutine(ZebraStatueRise());
    }

    IEnumerator FlamingoStatueRise()
    {
        playerCam.target = flamingoTrans;

        nonWalkableCollider.enabled = false;
        walkableCollider.enabled = true;
        yield return new WaitForSeconds(1.5f);

        if(cameraVfx)
            cameraVfx.enabled = true;
        playerCam.ShakeGameObject(1.5f, 0.5f);
        flamingoTrans.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        
        if (cameraVfx)
            cameraVfx.enabled = false;
        playerCam.target = machachoManTransform;

        //set the value in save system
        SaveSystem.SetInt("StatuePad", 1);

    }
    IEnumerator JaguarStatueRise()
    {
        //machachoManTransform.gameObject.SetActive(false);
       // yield return new WaitForSeconds(1.5f);
        playerCam.target = jaguarTrans;

        nonWalkableCollider.enabled = false;
        walkableCollider.enabled = true;
        yield return new WaitForSeconds(1.5f);

        if (cameraVfx)
            cameraVfx.enabled = true;
        playerCam.ShakeGameObject(1.5f, 0.5f);
        jaguarTrans.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        if (cameraVfx)
            cameraVfx.enabled = false;

        //disable box collider of previous statue pad
       // zebraTrans.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
        //load back teh scene
        StartCoroutine(FadeAndLoadBack());

        // StatuesPadManager.Instance.JaugarStatue();
        SaveSystem.SetInt("StatuePad", 2);
    }
    IEnumerator ZebraStatueRise()
    {
        playerCam.target = zebraTrans;
        nonWalkableCollider.enabled = false;
        walkableCollider.enabled = true;
        yield return new WaitForSeconds(1.5f);

        if (cameraVfx)
            cameraVfx.enabled = true;
        playerCam.ShakeGameObject(1.5f, 0.5f);
        zebraTrans.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        if (cameraVfx)
            cameraVfx.enabled = false;

        playerCam.target = machachoManTransform;
        flamingoTrans.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
        SaveSystem.SetInt("StatuePad", 3);
    }


    //load back scene
    private IEnumerator FadeAndLoadBack()
    {
        Camera.main.GetComponent<FadeCamera>().FadeOut(0.5f);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Junglaji_MazeC");
    }
}
