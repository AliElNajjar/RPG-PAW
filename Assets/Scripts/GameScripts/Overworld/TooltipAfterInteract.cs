using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipAfterInteract : MonoBehaviour
{
    Interactable interactable;

    public string[] tooltips;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.onInteractionDone = OnDialogueDone;
    }

    void OnDialogueDone()
    {
        StartCoroutine(DelayedDialogue());
    }

    IEnumerator DelayedDialogue()
    {
        yield return new WaitForSeconds(0.1f);
        MessagesManager.Instance.BuildMessageBox(tooltips, 16, 7, -1, null);
    }
}
