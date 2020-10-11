using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectiveManager : MonoBehaviour
{
    static public ObjectiveManager instance;
    public GameObject exclamationMarkPrefab;

    static List<ObjectiveInstance> objectiveInstances = new List<ObjectiveInstance>();

    public class ObjectiveInstance
    {
        public GameObject exclamationMark;
        public GameObject objectiveObject;

        public string objectiveObjName;

        public ObjectiveInstance(GameObject exclamationMark, GameObject objectiveObject)
        {
            this.exclamationMark = exclamationMark;
            this.objectiveObject = objectiveObject;
            this.objectiveObjName = objectiveObject.name;
        }
    }

    public static void AddObjective(GameObject objective)
    {
        //If the objective is already added as an objective, return
        if (objectiveInstances.Find((x) => x.objectiveObject == objective) != null)
            return;

        var exclamationMarkInstance = MakeAndAddExclamationMark(objective);

        objectiveInstances.Add(new ObjectiveInstance(exclamationMarkInstance, objective));
    }

    public static GameObject MakeAndAddExclamationMark(GameObject target)
    {
        var exclamationMarkInstance = Instantiate(instance.exclamationMarkPrefab);
        exclamationMarkInstance.transform.SetParent(target.transform);
        exclamationMarkInstance.transform.localPosition = Vector3.zero + new Vector3(0.0f, 0.5f, 0.0f);

        return exclamationMarkInstance;
    }

    public static void RemoveObjective(GameObject objective)
    {
        var toRemove = new List<ObjectiveInstance>();

        foreach (var objectiveInst in objectiveInstances)
        {
            if (objectiveInst.objectiveObject == objective)
            {
                Destroy(objectiveInst.exclamationMark);
                toRemove.Add(objectiveInst);
            }
        }

        foreach (var remove in toRemove)
        {
            objectiveInstances.Remove(remove);
        }
    }

    private void Awake()
    {
        instance = this;

        foreach (var objectiveInst in objectiveInstances)
        {
            if (objectiveInst.exclamationMark == null)
            {
                var objectiveObj = GameObject.Find(objectiveInst.objectiveObjName); //This is not a good way of doing it, but this is a placeholder objective system

                if (objectiveObj != null)
                {
                    var excl = MakeAndAddExclamationMark(objectiveObj);
                    objectiveInst.exclamationMark = excl;
                    objectiveInst.objectiveObject = objectiveObj;
                }
            }
        }
    }
}
