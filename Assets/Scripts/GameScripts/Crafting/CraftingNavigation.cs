using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CraftingNavigation : MonoBehaviour
{
    public CraftingGrid gridObj;
    public GameObject selector;

    public Sprite itemSprite;
    public List<GameObject> committedItems;
    public static GameObject[] craftableEuipments;

    [SerializeField] private PauseMenuCraftingComponents craftingSelect;
    private ItemSet inventory = new ItemSet();
    private ItemSet currentlyCommitted = new ItemSet();
    private Vector2 craftedItemSpriteSize = new Vector2(0.055f, 0.055f);
    private Vector2 navigationSpriteSize = new Vector2(0.12f, 0.12f);
    private Vector3 _parentVerticalSpacing = new Vector3(0, 0.08f, 0);
    private bool isCrafted = false;
    private bool isSelected = true;
    public bool Reading
    {
        get;
        set;
    }

    private void Awake()
    {
        committedItems = new List<GameObject>();
        CreateRecipes();
        PopulateInventory();
        selector.SetActive(false);
        craftableEuipments = Resources.LoadAll<GameObject>("Craftables");
        
    }
    private void CreateRecipes()
    {
        Debug.Log(RecipeDatabase.recipes.Count + " total recipes");
    }

    private void Craft(string item)
    {
        Debug.Log(item);
        RecipeDatabase.Craft(item, inventory);
    }

    private void PopulateInventory()
    {
        foreach (KeyValuePair<string, GameObject> entry in ExecutablesHandler.items)
        {
            inventory.AddItem(entry.Value);
        }
    }

    private void Start()
    {
        gridObj.gridPositions = new Transform[3, 3];

        gridObj.gridPositions[0, 0] = gridObj.gridObjects[0].transform;
        gridObj.gridPositions[1, 0] = gridObj.gridObjects[1].transform;
        gridObj.gridPositions[2, 0] = gridObj.gridObjects[2].transform;

        gridObj.gridPositions[0, 1] = gridObj.gridObjects[3].transform;
        gridObj.gridPositions[1, 1] = gridObj.gridObjects[4].transform;
        gridObj.gridPositions[2, 1] = gridObj.gridObjects[5].transform;

        gridObj.gridPositions[0, 2] = gridObj.gridObjects[6].transform;
        gridObj.gridPositions[1, 2] = gridObj.gridObjects[7].transform;
        gridObj.gridPositions[2, 2] = gridObj.gridObjects[8].transform;

        gridObj.currentIndex = Vector2.zero;
        PlaceSelector();
    }

    void Update()
    {
        CheckRecipe();
        if (Reading)
        {
            Inputs();
            selector.SetActive(true);
        }
        else
        {
            selector.SetActive(false);
        }
    }

    void Inputs()
    {
        //Debug.Log(craftingSelect._currentItem);
        if (RewiredInputHandler.Instance.player.GetButtonDown("Down"))
        {
            gridObj.Navigate(Vector2.down);
            PlaceSelector();
        }
        else if (RewiredInputHandler.Instance.player.GetButtonDown("Up"))
        {
            gridObj.Navigate(Vector2.up);
            PlaceSelector();
        }
        else if (RewiredInputHandler.Instance.player.GetButtonDown("Right"))
        {
            gridObj.Navigate(Vector2.right);
            PlaceSelector();
        }
        else if (RewiredInputHandler.Instance.player.GetButtonDown("Left"))
        {
            gridObj.Navigate(Vector2.left);
            PlaceSelector();
        }
        else if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
        {
            if (!committedItems.Contains(craftingSelect._currentItem))
            {
                committedItems.Add(craftingSelect._currentItem);
                SetItemSprite();
                this.Reading = false;
                craftingSelect.Reading = true;
                isCrafted = false;
            }
            for (int i = 0; i < committedItems.Count; i++)
            {
                if (currentlyCommitted.GetAmount(committedItems[i].name) == 0)
                {
                    currentlyCommitted.AddItem(committedItems[i]);
                    currentlyCommitted.SetAmount(committedItems[i].name, ItemsHandler.GetItemAmount(committedItems[i])); 
                }
            }
        }
        else if (RewiredInputHandler.Instance.player.GetButtonUp("Cancel"))
        { 
            this.Reading = false;
            craftingSelect.Reading = true;           
        }
        else if (RewiredInputHandler.Instance.player.GetButtonUp("Craft"))
        {
            ItemSet initalCommitted = new ItemSet();
            if (RecipeDatabase.GetFirstCraftableRecipe(currentlyCommitted) != null && !isCrafted)
            {
                SpriteRenderer craftedItemSpriteRenderer = GameObject.Find("CraftedItemSprite").GetComponent<SpriteRenderer>();
                Recipe craftingRecipe = RecipeDatabase.GetFirstCraftableRecipe(currentlyCommitted);
                Craft(craftingRecipe.Name);
                craftingSelect.RefreshItemData();
                isCrafted = true;
                craftingSelect.craftedText.text = "You made a " + craftingRecipe.Name + "!";
                SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.craftSuccess);
                currentlyCommitted = initalCommitted;
                this.Reading = false;
                craftingSelect.Reading = true;
            }
            else
            {
                craftingSelect.craftedText.text = "Unknown Recipe";
                craftingSelect.craftedText.text = "You don't know the recipe";
                return;
            }
            CleanUp();
        }
        else if (RewiredInputHandler.Instance.player.GetButtonUp("CleanCrafting"))
        {
            //CleanUp();
            var item = CheckSlot();
            if(item != null)
            {
                committedItems.Remove(item);
                ClearItemSprite();
            }                    
        }
    }
    private void CheckRecipe()
    {
        Recipe craftingRecipe = RecipeDatabase.GetFirstCraftableRecipe(currentlyCommitted);
        if (RecipeDatabase.GetFirstCraftableRecipe(currentlyCommitted) != null)
        {
            SetCraftedItemSprite();
            craftingSelect.craftedText.text = "You can make a " + craftingRecipe.Name + "!";
            craftingSelect.craftingText.text = "Crafting: " + craftingRecipe.Name;
        }
        else
        {
            craftingSelect.craftingText.text = "Crafting: Unknown";
        }

    }
    public void CleanUp()
    {
        foreach (var sprite in gridObj.gridObjects)
        {
            sprite.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>().sprite = null;
        }
        committedItems.Clear();
        RemoveCraftedItemSprite();
    }
    private void PlaceSelector()
    {
        selector.transform.position = gridObj.gridPositions[(int)gridObj.currentIndex.x, (int)gridObj.currentIndex.y].position;
    }
    public void SetItemSprite()
    {
        SpriteRenderer curentSlotSpriteRenderer = gridObj.gridPositions[(int)gridObj.currentIndex.x, (int)gridObj.currentIndex.y].transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
        curentSlotSpriteRenderer.sprite = GetCurrentItemSprite();
        curentSlotSpriteRenderer.size = navigationSpriteSize;
    }
    public void ClearItemSprite()
    {
        SpriteRenderer curentSlotSpriteRenderer = gridObj.gridPositions[(int)gridObj.currentIndex.x, (int)gridObj.currentIndex.y].transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
        curentSlotSpriteRenderer.sprite = null;
    }
    public GameObject CheckSlot()
    {
        SpriteRenderer curentSlotSpriteRenderer = gridObj.gridPositions[(int)gridObj.currentIndex.x, (int)gridObj.currentIndex.y].transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();  
        foreach(GameObject item in committedItems)
        {
            if (item.GetComponent<ItemInfo>())
            {
                if (item.GetComponent<ItemInfo>().menusSprite.name == curentSlotSpriteRenderer.sprite.name)
                {
                    return item;
                }
            }
            else if (item.GetComponent<EquipmentInfo>())
            {
                if (item.GetComponent<EquipmentInfo>().menusSprite.name == curentSlotSpriteRenderer.sprite.name)
                {
                    return item;
                }
            }
        }
        return null;
    }
        
    private Sprite GetCurrentItemSprite()
    {
        Sprite newSprite = itemSprite;

        if (craftingSelect._currentItem.GetComponent<ItemInfo>())
        {
            newSprite = craftingSelect._currentItem.GetComponent<ItemInfo>().menusSprite;
        }
        else if (craftingSelect._currentItem.GetComponent<EquipmentInfo>())
        {
            newSprite = craftingSelect._currentItem.GetComponent<EquipmentInfo>().menusSprite;
        }

        return newSprite;
    }
   
    private Sprite GetCraftedItemSprite()
    {
        Sprite newSprite = itemSprite;
        Recipe craftedItemRecipe = RecipeDatabase.GetFirstCraftableRecipe(currentlyCommitted);
        for (int i = 0; i < craftableEuipments.Length; i++)
        {
            if (craftableEuipments[i].name == craftedItemRecipe.Name)
            {
                newSprite = craftableEuipments[i].GetComponent<EquipmentInfo>().menusSprite;
            }
        }
        return newSprite;
    }
    public void SetCraftedItemSprite()
    {
        SpriteRenderer craftedItemSpriteRenderer = GameObject.Find("CraftedItemSprite").GetComponent<SpriteRenderer>();
        craftedItemSpriteRenderer.sprite = GetCraftedItemSprite();
        craftedItemSpriteRenderer.size = craftedItemSpriteSize;
    }
    public void RemoveCraftedItemSprite()
    {
        SpriteRenderer craftedItemSpriteRenderer = GameObject.Find("CraftedItemSprite").GetComponent<SpriteRenderer>();
        craftedItemSpriteRenderer.sprite = null;
    }
}
