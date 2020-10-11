using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HypeAction 
{
    public Queue<float> hypeActionModify = new Queue<float>(5);

    public HypeAction(float[] hypeActionValues)
    {
        for (int i = 0; i < hypeActionValues.Length; i++)
        {
            hypeActionModify.Enqueue(hypeActionValues[i]);
        }
    }

    public void Execute()
    {
        /*float value;

        if (hypeActionModify.Count > 1)
            value = hypeActionModify.Dequeue();
        else
            value = hypeActionModify.Peek();

        if (value > 0)
        {
            BattleManager bm = FindObjectOfType<BattleManager>();
            if (bm.CurrentTurnUnit is PlayerBattleUnitHolder && bm.CurrentTurnUnit.CurrentHealthPercentage <= 0.1)
                value *= 2;
                
        }
        
         if (hypeActionModify.Count > 1)
            OverMeterHandler.Instance?.UpdateOverMeter(value, true);
        else
            OverMeterHandler.Instance?.UpdateOverMeter(value, true);
         */

        if (hypeActionModify.Count > 1)
            OverMeterHandler.Instance?.UpdateOverMeter(hypeActionModify.Dequeue(), true);
        else
            OverMeterHandler.Instance?.UpdateOverMeter(hypeActionModify.Peek(), true);
    }
}
