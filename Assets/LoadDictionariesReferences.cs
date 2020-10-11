using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadDictionariesReferences : MonoBehaviour
{
    public static LoadDictionariesReferences Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            return;
        }

        AnimationRoutineSystem.Init();
        UnitDatabase.Init();

        List<GameObject> allItems = new List<GameObject>();

        GameObject[] usableItems = Resources.LoadAll<GameObject>("ItemObjects");
        GameObject[] weapons = Resources.LoadAll<GameObject>("Weapons");
        GameObject[] feets = Resources.LoadAll<GameObject>("Feet");
        GameObject[] torso = Resources.LoadAll<GameObject>("Torso");
        GameObject[] waist = Resources.LoadAll<GameObject>("Waist");
        GameObject[] head = Resources.LoadAll<GameObject>("Head");
        GameObject[] accessories = Resources.LoadAll<GameObject>("Accessories");
        GameObject[] crafting = Resources.LoadAll<GameObject>("Mats");

        allItems.AddRange(usableItems);
        allItems.AddRange(weapons);       
        allItems.AddRange(feets);
        allItems.AddRange(torso);
        allItems.AddRange(waist);
        allItems.AddRange(head);
        allItems.AddRange(accessories);
        allItems.AddRange(crafting);

        RecipeContainer[] recipes = Resources.LoadAll<RecipeContainer>("Recipes");

        ExecutablesHandler.items = new Dictionary<string, GameObject>(100);
        RecipeDatabase.recipes = new Dictionary<string, Recipe>(100);

        foreach (var item in allItems)
        {
            if (!ExecutablesHandler.items.ContainsKey(item.name))
            {
                ItemsHandler.SetItemAmount(item, 50);
                ExecutablesHandler.items.Add(item.name, item);
            }
        }

        foreach (var recipe in recipes)
        {
            if (!RecipeDatabase.recipes.ContainsKey(recipe.name))
            {
                Recipe newRecipe = recipe.CreateRecipe();
                RecipeDatabase.recipes.Add(recipe.name, newRecipe);
            }
        }

        //Debug.Log(ExecutablesHandler.items.Count + " total items");
        //Debug.Log(RecipeDatabase.recipes.Count + " total recipes");
    }
}
