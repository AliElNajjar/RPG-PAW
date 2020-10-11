using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipeDatabase
{
    public static Dictionary<string, Recipe> recipes = new Dictionary<string, Recipe>();

    static RecipeDatabase()
    {

    }

    public static void Add(Recipe Recipe)
    {
        recipes.Add(Recipe.Name, Recipe);
    }

    public static Recipe Create(string item, Vector2 gridPos, int amount = 1)
    {
        Recipe Recipe = Recipe.For(item, gridPos, amount);
        Add(Recipe);
        return Recipe;
    }

    public static Recipe Get(string Recipe)
    {
        return recipes[Recipe];
    }

    public static IEnumerable<Recipe> GetCraftableRecipes(ItemSet availableItems)
    {
        foreach (var Recipe in recipes.Values)
            if (Recipe.CanCraft(availableItems))
                yield return Recipe;
    }

    public static Recipe GetFirstCraftableRecipe(ItemSet availableItems)
    {
        foreach (var Recipe in recipes.Values)
            if (Recipe.CanCraft(availableItems))
                return Recipe;

        return null;
    }

    public static void Craft(string Recipe, ItemSet inventory)
    {
        inventory.AddItem(Get(Recipe).Craft(inventory));
        ItemsHandler.AddItemAmount(ExecutablesHandler.items[Get(Recipe).Name], 1);
    }

    public static bool CanCraft(string Recipe, ItemSet inventory)
    {
        return Get(Recipe).CanCraft(inventory);
    }

    public static bool CanCraftAny(ItemSet inventory)
    {
        if (GetFirstCraftableRecipe(inventory) != null)
        {
            return true;
        }

        return false;
    }
}
