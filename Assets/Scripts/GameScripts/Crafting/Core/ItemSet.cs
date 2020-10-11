using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class ItemSet
{
    Dictionary<string, GameObject> bundles = new Dictionary<string, GameObject>();

    public IEnumerable<GameObject> Bundles { get { return bundles.Values; } }

    public void AddItem(GameObject item)
    {
        ChangeAmount(item.name, ItemsHandler.GetItemAmount(item));
    }

    public void ChangeAmount(string item, int amount)
    {
        if (amount == 0)
            return;

        GameObject bundle = null;
        if (!bundles.TryGetValue(item, out bundle))
            CreateBundle(item, amount);
        else
        {
            ItemsHandler.SetItemAmount(bundle, ItemsHandler.GetItemAmount(bundle) + amount);
            Prune(bundle);
        }
    }

    public int GetAmount(string item)
    {
        GameObject bundle = null;
        if (!bundles.TryGetValue(item, out bundle))
            return 0;
        else
            return ItemsHandler.GetItemAmount(bundle);
    }

    public void SetAmount(string item, int amount)
    {
        if (amount == 0)
        {
            Remove(item);
            return;
        }

        GameObject bundle = null;
        if (!bundles.TryGetValue(item, out bundle))
            bundle = CreateBundle(item, amount);
        else
            ItemsHandler.SetItemAmount(bundle, amount);

        Prune(bundle);
    }

    public bool Contains(ItemSet items)
    {
        return items.Bundles.All(item => GetAmount(item.name) >= ItemsHandler.GetItemAmount(item));
    }

    public void Remove(string item)
    {
        bundles.Remove(item);
    }

    public bool Remove(ItemSet items)
    {
        if (!Contains(items))
            return false;

        foreach (var item in items.Bundles)
            ChangeAmount(ItemsHandler.GetItemName(item), -1);

        return true;
    }

    private void Prune(GameObject bundle)
    {
        if (ItemsHandler.GetItemAmount(bundle) <= 0)
            Remove(bundle.name);
    }

    private GameObject CreateBundle(string item, int amount)
    {
        GameObject bundle = ExecutablesHandler.items[item];
        bundles.Add(item, bundle);
        return bundle;
    }

    public override string ToString()
    {
        return string.Join(", ", Bundles.Select(bundle => bundle.ToString()).ToArray());
    }
}
