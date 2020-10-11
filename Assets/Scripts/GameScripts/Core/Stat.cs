using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;

[System.Serializable]
public class Stat
{
    public float baseValue;
    public StatGrowth growthPercent = new StatGrowth();
    public readonly ReadOnlyCollection<Modifier> Modifiers;
    public List<Modifier> modifiers;

    protected bool _isDirty = true;
    [SerializeField] protected float _value;
    protected float _lastBaseValue = float.MinValue;

    public virtual float Value
    {
        get
        {
            if (_isDirty || _lastBaseValue != baseValue)
            {
                _lastBaseValue = baseValue;
                _value = CalculateFinalValue();
                _isDirty = false;
            }
            return _value;
        }
    }

    public Stat(float baseValue)
    {
        this.baseValue = baseValue;
        modifiers = new List<Modifier>();
        Modifiers = modifiers.AsReadOnly();
    }

    public Stat()
    {
        modifiers = new List<Modifier>();
        Modifiers = modifiers.AsReadOnly();
    }

    //public Stat(float baseValue)
    //{
    //    this.baseValue = baseValue;
    //}

    public virtual void AddModifier(Modifier mod)
    {
        _isDirty = true;
        modifiers.Add(mod);
        modifiers.Sort(CompareModifierOrder);
    }

    public virtual bool RemoveModifier(Modifier mod)
    {
        if (modifiers.Remove(mod))
        {
            _isDirty = true;
            return true;
        }

        return false;
    }

    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;

        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            if (modifiers[i].source == source)
            {
                _isDirty = true;
                didRemove = true;
                modifiers.RemoveAt(i);
            }
        }
        return didRemove;
    }

    public virtual void RemoveAllModifiers()
    {
        if (modifiers.Count == 0)
            return;

        Debug.Log($"</color>Removing all modifiers. Modifiers count: {modifiers.Count}</color>");
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            _isDirty = true;
            modifiers.RemoveAt(i);
        }
        Debug.Log($"</color>All modifiers removed. Modifiers count: {modifiers.Count}</color>");
    }

    protected virtual float CalculateFinalValue()
    {
        float finalValue = baseValue;
        float sumPercentAdd = 0;        

        for (int i = 0; i < modifiers.Count; i++)
        {
            Modifier mod = modifiers[i];

            if (mod.modType == ModType.Flat)
            {
                finalValue += mod.value;
            }
            else if (mod.modType ==  ModType.PercentAdd)
            {
                sumPercentAdd += mod.value; // Start adding together all modifiers of this type

                // If we're at the end of the list OR the next modifer isn't of this type
                if (i + 1 >= modifiers.Count || modifiers[i + 1].modType != ModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd; // Multiply the sum with the "finalValue", like we do for "PercentMult" modifiers
                    sumPercentAdd = 0; // Reset the sum back to 0
                }
            }
            else if (mod.modType == ModType.PercentMult)
            {
                finalValue *= 1 + mod.value;
            }
        }
        // Rounding gets around dumb float calculation errors (like getting 12.0001f, instead of 12f)
        // 4 significant digits is usually precise enough, but feel free to change this to fit your needs
        return (float)Math.Round(finalValue, 6);
    }

    protected virtual int CompareModifierOrder(Modifier a, Modifier b)
    {
        if (a.order < b.order)
            return -1;
        else if (a.order > b.order)
            return 1;
        return 0; // if (a.Order == b.Order)
    }

}

[System.Serializable]
public class StatGrowth
{
    [Range(1, 100)] public int growthAddMax = 1;

    public int GetFinalGrowth()
    {
        return Mathf.RoundToInt(growthAddMax * UnityEngine.Random.Range(0, 1));
    }
}
