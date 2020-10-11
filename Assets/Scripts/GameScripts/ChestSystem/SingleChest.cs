using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleChest : MonoBehaviour
{
    [SerializeField]
    private Sprite openedChest, closedChest;
    [SerializeField]
    private List<Sprite> animationSprites = new List<Sprite>();
    [SerializeField]
    private GameObject openCollider, closeCollider, myVFX;
    [SerializeField]
    private List<EquipmentInfo> equipments = new List<EquipmentInfo>();
    [SerializeField]
    private List<ItemInfo> items = new List<ItemInfo>();
    [SerializeField]
    private int sceneChestIndex = 1;

    private SpriteRenderer mySR;
    private float animTimeInSec = 1.5f;
    private Coroutine getSubmitButtonCR;
    private string mySceneName
    {
        get
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }
    }
    private bool isChestOpened
    {
        get
        {
            return SaveSystem.GetBool("isChest" + sceneChestIndex + mySceneName + "opened", false);
        }
        set
        {
            SaveSystem.GetBool("isChest" + sceneChestIndex + mySceneName + "opened", value);
        }
    }
    //public int 
    //public List<string>

    private void Awake()
    {
        mySR = GetComponentInChildren<SpriteRenderer>();
        Initialize();
    }

    private void Initialize()
    {
        GetComponent<Collider2D>().enabled = !isChestOpened;
        openCollider.SetActive(isChestOpened);
        closeCollider.SetActive(!isChestOpened);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            getSubmitButtonCR = StartCoroutine(GetSubmitButtonRoutine());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(getSubmitButtonCR != null)
            {
                StopCoroutine(getSubmitButtonCR);
            }
        }
    }

    private IEnumerator GetSubmitButtonRoutine()
    {
        while (true)
        {
            if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
            {
                OpenTheChest();
            }
            yield return null;
        }
    }

    private void OpenTheChest()
    {
        if (getSubmitButtonCR != null)
        {
            StopCoroutine(getSubmitButtonCR);
        }
        isChestOpened = true;
        Initialize();
        StartCoroutine(OpenChestRoutine());
    }

    private IEnumerator OpenChestRoutine()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(animTimeInSec / animationSprites.Count);
        int index = 0;
        while(index < animationSprites.Count)
        {
            mySR.sprite = animationSprites[index];
            if(index > animationSprites.Count / 1.5f)
            {
                myVFX.SetActive(true);
            }
            index++;
            yield return waitForSeconds;
        }
        yield return waitForSeconds;
        AddContent();
        ShowContent();
    }

    private void ShowContent()
    {
        List<string> contentNames = new List<string>();

        for (int i = 0; i < equipments.Count; i++)
        {
            contentNames.Add(equipments[i].equipmentName);
        }

        for (int i = 0; i < items.Count; i++)
        {
            contentNames.Add(items[i].itemName);
        }

        if(contentNames.Count > 0)
        {
            MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0.5f, 0.5f));
            string str = "\t\t¡Botín! You found:\n\n";
            for (int i = 0; i < contentNames.Count; i++)
            {
                str += "\t\t";
                str += contentNames[i];
                str += "\n";
            }
            MessagesManager.Instance.BuildMessageBox(str, 16, 20);
        }
    }

    private void AddContent()
    {
        PlayerEquipmentInventory inventory = GameObject.Find("PlayerInventory").GetComponent<PlayerEquipmentInventory>();
        for (int i = 0; i < equipments.Count; i++)
        {
            if(inventory !=null)
                inventory.currentEquipment.Add(equipments[i].gameObject);
        }

        for (int i = 0; i < items.Count; i++)
        {
            if(inventory != null)
                inventory.currentEquipment.Add(items[i].gameObject);
        }
    }
}
