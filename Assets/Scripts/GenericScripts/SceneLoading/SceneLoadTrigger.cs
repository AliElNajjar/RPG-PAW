using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionCinematic
{
    None,
    Walking
}

public class SceneLoadTrigger : MonoBehaviour
{
    public TransitionCinematic playerAnimation;
    [SerializeField] private string _sceneName;
    public int loadPosition = 0;
    public bool active = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (active && playerAnimation == TransitionCinematic.Walking)
            {
                Camera.main.GetComponent<CameraFollowPlayer>().target = null;
                collision.GetComponent<UnitOverworldMovement>().isNPC = true;
                UnitOverworldMovement.loadPosition = loadPosition;
        
                StartCoroutine(FadeAndLoad());
                
            }
            else if (active && playerAnimation == TransitionCinematic.None)
            {               
                GetComponent<EventOnSceneLoadTriggered>()?.onSceneLoadTriggered?.Invoke();

                collision.GetComponent<UnitOverworldMovement>().DisableMovement();
                UnitOverworldMovement.loadPosition = loadPosition;

                StartCoroutine(FadeAndLoad());
            }
        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<AudioSource>().volume = 0f;

            if (playerAnimation == TransitionCinematic.Walking)
            {
                UnitOverworldMovement unitMovement = other.GetComponent<UnitOverworldMovement>();
                unitMovement.Translate(unitMovement.GetDir(unitMovement.LookingDir));
            }
        }
    }

    private IEnumerator FadeAndLoad()
    {
        Camera.main.GetComponent<FadeCamera>().FadeOut(0.5f);

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(_sceneName);
    }

    public void SetStorySequenceIndex(int index)
    {
        SaveSystem.SetInt(SaveSystemConstants.storyProgressString, index);
        //CutSceneManager.CutSceneSequesnceCompleted();
    }
}
