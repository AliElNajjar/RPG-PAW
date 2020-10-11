using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitLevel
{
    public int currentLevel;
    public float currentExperience;

    public delegate void UnitExperienceGainEvent();
    public event UnitExperienceGainEvent OnLevelUp;
    public event UnitExperienceGainEvent OnExperienceGained;

    public UnitLevel()
    {
        currentLevel = 1;
        currentExperience = 0;
    }

    public void ApplyXP(float addXP)
    {
        currentExperience += addXP;

        OnExperienceGained?.Invoke();

        if (currentExperience > ToNextLevel(currentLevel))
            LevelUp();
    }

    public void LevelUp()
    {
        currentLevel++;
        currentExperience = 0;

        Debug.Log("Leveled up to " + currentLevel);
        OnLevelUp?.Invoke();
    }

    public static float ToNextLevel(float level)
    {
        float exponent = 1.5f;
        float baseXP = 100;
        float result = Mathf.Floor(baseXP * (Mathf.Pow(level, exponent)));
        return Mathf.RoundToInt(result);
    }
}

