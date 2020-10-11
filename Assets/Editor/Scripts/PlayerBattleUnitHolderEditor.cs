using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerBattleUnitHolder))]
public class PlayerBattleUnitHolderEditor : Editor
{
    PlayerBattleUnitHolder _target;

    SerializedProperty hurtSFX;
    SerializedProperty damageTakenNumberPrefab;
    SerializedProperty unitPersistentData;
    SerializedProperty unitPersistentDataPlayableUnit;
    SerializedProperty unitData;
    SerializedProperty equipment;

    private void OnEnable()
    {
        _target = (PlayerBattleUnitHolder)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        hurtSFX = serializedObject.FindProperty("hurtSFX");
        damageTakenNumberPrefab = serializedObject.FindProperty("damageTakenNumberPrefab");
        unitPersistentData = serializedObject.FindProperty("unitPersistentData");
        unitPersistentDataPlayableUnit = serializedObject.FindProperty("unitPersistentData").FindPropertyRelative("playableUnit");
        unitData = serializedObject.FindProperty("_playableUnitData");
        equipment = serializedObject.FindProperty("equipment");

        EditorGUILayout.PropertyField(hurtSFX, true);
        EditorGUILayout.PropertyField(damageTakenNumberPrefab);
        EditorGUILayout.PropertyField(unitPersistentData);

        if (unitPersistentData.objectReferenceValue != null)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(unitData, true);
            EditorGUI.EndDisabledGroup();
        }

        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.PropertyField(equipment, true);
        //EditorGUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }
}
