using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Recipe
{
    private readonly GameObject output;
    private readonly Vector2 craftingGridRequirement;
    private ItemSet ingredients;

    public string Name { get { return output.name; } }
    public int Amount { get { return ItemsHandler.GetItemAmount(output); } }

    public Recipe(string item, int amount, Vector2 gridPos)
    {
        output = ExecutablesHandler.items[item];
        ItemsHandler.SetItemAmount(output, amount);
        ingredients = new ItemSet();
        craftingGridRequirement = gridPos;
    }

    public void SetIngredients(ItemSet ingredients)
    {
        this.ingredients = ingredients;
    }

    public static Recipe For(string item, Vector2 gridPos, int amount = 1)
    {
        return new Recipe(item, amount, gridPos);
    }

    public Recipe Require(string item, int amount = 1)
    {
        ingredients.ChangeAmount(item, amount);
        return this;
    }

    public bool CanCraft(ItemSet availableItems)
    {
        return availableItems.Contains(ingredients);
    }

    public GameObject Craft(ItemSet availableItems)
    {
        GameObject clone = new GameObject();
        clone = output;

        bool removed = availableItems.Remove(ingredients);

        Debug.Log("Removed Items = " + removed);

        return clone;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(output);
        sb.Append(" = ");
        sb.Append(ingredients);
        return sb.ToString();
    }
}
