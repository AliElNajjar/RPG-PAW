using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ActivePlayerButtons : MonoBehaviour
{
    public GameObject itemUI;
    public bool itemUsed;

    private HypeAction itemUsedHype = new HypeAction(new float[] { 0, 0, -5, -5, -5 });

    public void ItemAction()
    {
        itemUI.SetActive(true);
        Reading = false;
    }    

    public void WaitForItem()
    {
        StartCoroutine(WaitUntilItemIsUsed());
    }

    private IEnumerator WaitUntilItemIsUsed()
    {
        contextMenu.GetComponent<ToggleSpriteGroup>().Toggle(false);
        bigMenu.GetComponent<ToggleSpriteGroup>().Toggle(false);

        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        //intended way
        //while (!itemUsed)
        //{
        //    yield return null;
        //}

        yield return null;

        _battleManager.ExecutingAction = false;
        itemUsed = false;

        itemUsedHype.Execute();
    }
}
