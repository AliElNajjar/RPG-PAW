using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierDatabase
{
    public Dictionary<int, Modifier> modifiers = new Dictionary<int, Modifier>(100);

    public void InitializeModList()
    {
        //modifiers.Add(
        //    0,
        //    new Modifier()
        //    {
        //        modName = "Power Fists",
        //        AddMod = () =>
        //        {
        //            //strength += 5;
        //        }
        //    }
        //);
    }
}
