using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateStoryTrigger : MonoBehaviour
{
    public int triggerSequenceID;
    public SequenceTiming timing = SequenceTiming.After;

    void Awake()
    {
        if (timing == SequenceTiming.After)
        {
            if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) >= triggerSequenceID)
            {
                this.gameObject.SetActive(false);
            }
        }
        else if (timing == SequenceTiming.Before)
        {
            if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) <= triggerSequenceID)
            {
                this.gameObject.SetActive(false);
            }
        }
        else if (timing == SequenceTiming.Equal)
        {
            if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) != triggerSequenceID)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}

public enum SequenceTiming
{
    Before,
    After,
    Equal
}
