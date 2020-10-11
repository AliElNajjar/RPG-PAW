using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyBattleUnitHolder))]
public class EnemyBattleUnitHolderEditor : Editor
{
    EnemyBattleUnitHolder _target;

    SerializedProperty hurtSFX;
    SerializedProperty damageTakenNumberPrefab;
    SerializedProperty unitPersistentData;
    SerializedProperty unitPersistentDataPlayableUnit;
    SerializedProperty unitData;

    private void OnEnable()
    {
        _target = (EnemyBattleUnitHolder)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        hurtSFX = serializedObject.FindProperty("hurtSFX");
        damageTakenNumberPrefab = serializedObject.FindProperty("damageTakenNumberPrefab");
        unitPersistentData = serializedObject.FindProperty("unitPersistentData");
        unitPersistentDataPlayableUnit = serializedObject.FindProperty("unitPersistentData").FindPropertyRelative("playableUnit");
        unitData = serializedObject.FindProperty("enemyUnitData");

        EditorGUILayout.PropertyField(hurtSFX, true);
        EditorGUILayout.PropertyField(damageTakenNumberPrefab);
        EditorGUILayout.PropertyField(unitPersistentData);

        if (unitPersistentData.objectReferenceValue != null)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(unitData, true);
            EditorGUI.EndDisabledGroup();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
