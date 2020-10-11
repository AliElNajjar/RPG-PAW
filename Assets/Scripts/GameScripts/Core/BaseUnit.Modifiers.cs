using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Modifier
{
    public float value;
    public ModType modType;
    public int order;
    public object source;

    public Modifier()
    {
        this.value = 0;
        this.modType = ModType.Flat;
        this.order = 0;
        this.source = null;
    }

    public Modifier(float value, ModType modType, int order, object source)
    {
        this.value = value;
        this.modType = modType;
        this.order = order;
        this.source = source;
    }

    public Modifier(float value, ModType modType) : this(value, modType, (int)modType, null) { }
    public Modifier(float value, ModType modType, int order) : this(value, modType, order, null) { }
    public Modifier(float value, ModType modType, object source) : this(value, modType, (int)modType, source) { }
}

public enum ModType
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300
}
