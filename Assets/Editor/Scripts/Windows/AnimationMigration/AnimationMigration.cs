using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimationMigration : EditorWindow
{
    public GameObject unitAnimations;
    public AnimationData unitAnimationData;

    [MenuItem("Window/Migrate Animations")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AnimationMigration));
    }

    void OnGUI()
    {
        unitAnimations = (GameObject)EditorGUILayout.ObjectField("Unit Animations Prefab", unitAnimations, typeof(GameObject), false);
        unitAnimationData = (AnimationData)EditorGUILayout.ObjectField("Unit Animations Scriptable Object", unitAnimationData, typeof(AnimationData), false);

        if (GUILayout.Button("Migrate"))
        {
            unitAnimationData.animations = unitAnimations.GetComponent<SimpleAnimator>().animations;
            EditorUtility.SetDirty(unitAnimationData);
        }
    }
}
