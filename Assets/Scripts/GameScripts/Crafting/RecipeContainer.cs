using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipes", fileName = "DefualtRecipe"), System.Serializable]
public class RecipeContainer : ScriptableObject
{
    public GameObject output;
    public RecipeIngredients[] ingredients;
    public Vector2 gridPos;

    public Recipe CreateRecipe()
    {
        ItemSet requiredIngredients = new ItemSet();

        for (int i = 0; i < ingredients.Length; i++)
        {
            requiredIngredients.AddItem(ingredients[i].ingredient);
        }

        Recipe newRecipe = new Recipe(output.name, 1, gridPos);

        newRecipe.SetIngredients(requiredIngredients);

        for (int i = 0; i < ingredients.Length; i++)
        {
            newRecipe.Require(ingredients[i].ingredient.name, ingredients[i].amount);
        }

        return newRecipe;
    }
}

[System.Serializable]
public class RecipeIngredients
{
    public GameObject ingredient;
    public int amount;
}