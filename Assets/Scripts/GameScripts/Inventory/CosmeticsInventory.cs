using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CosmeticsInventory : MonoBehaviour
{
    public List<CosmeticPlayable> cosmetics;

    public void Sort()
    {
        cosmetics.OrderBy(o => o.category).ToList();
    }
}
