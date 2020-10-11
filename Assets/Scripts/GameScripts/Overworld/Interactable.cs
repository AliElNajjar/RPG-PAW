using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool keepNamesConstant = false;
    public string receiverName;
    public string senderName = "Muchacho Man";

    public UnityEvent onInteractDone;

    public InteractableLineInfo[] interactableMessage;

    public Action onInteractionDone;

    public GameObject[] markAsObjectiveOnDone; //optional list of objects to mark as objectives after interact

    public GameObject Sender
    {
        get;
        set;
    }

    public GameObject Receiver
    {
        get;
        set;
    }

    void OnInteractionCompleted()
    {
        StartCoroutine(ToggleSources(true));

        onInteractDone?.Invoke();

        if (onInteractionDone != null)
        {
            onInteractionDone.Invoke();
        }

        foreach (var objective in markAsObjectiveOnDone)
        {
            ObjectiveManager.AddObjective(objective);
        }
    }

    public void ActivateMessage()
    {
        GetComponent<UnitOverworldMovement>().DisableMovement();
        StartCoroutine(ToggleSources(false));
        ChangeNames();
        
        MessagesManager.Instance.BuildMessageBox(interactableMessage, 16, 4, -1, OnInteractionCompleted, Sender, Receiver);
        MessagesManager.Instance.CurrentMessageSetActive(true);

        ObjectiveManager.RemoveObjective(gameObject); //If this object is an objective, remove it because it has been interacted with
    }

    private void ChangeNames()
    {
        foreach (var message in interactableMessage)
        {
            if (keepNamesConstant)
            {
                if (message.source == InteractableMessageSource.Receiver) message.speakerName = receiverName;
                else if (message.source == InteractableMessageSource.Sender) message.speakerName = senderName;
            }
        }        
    }

    private IEnumerator ToggleSources(bool enabled)
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        if (audioSources.Length > 0)
        {
            foreach (var audio in audioSources)
            {
                if (audio.gameObject.name == "MusicManager" || audio.gameObject.name == "SFXManager")
                    continue;

                switch (enabled)
                {
                    case true:
                        audio.Play();
                        break;
                    case false:
                        audio.Stop();
                        break;
                }

                yield return null;
                yield return null;
                yield return null;
                yield return null;
                yield return null;
            }
        }
    }
}

[System.Serializable]
public class InteractableLineInfo
{
    public string speakerName;
    [TextArea] public string message;
    public InteractableMessageSource source;
    
    //
    //regular font size 3.31

    public string FinalMessage
    {
        //Previously used colors and size
        // get { return string.Format("<size=130%><color=\"red\">{0}: <size=100%><color=\"black\">{1}", speakerName.ToUpper(), message); }
        get { return string.Format("<size=100%><color=\"black\">{0}{2}<size=100%><color=\"black\">{1}", speakerName.ToUpper(), message, string.IsNullOrEmpty(speakerName) ? "" : ": "); }
    }



    public InteractableLineInfo()
    {
        message = "";
        speakerName = "";
    }
}

public enum InteractableMessageSource
{
    Receiver,
    Sender
}