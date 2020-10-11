using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftingGrid
{
    public GameObject[] gridObjects;
    public Transform[,] gridPositions;
    public GameObject[,] gridObjs;
    public int gridWidth = 3;
    public int gridHeigth = 3;
    [ReadOnly] public Vector2 currentIndex = Vector2.zero;
    
    public CraftingGrid()
    {
        gridPositions = new Transform[gridWidth, gridHeigth];
    }

    public CraftingGrid(int width, int heigth)
    {
        gridPositions = new Transform[width, heigth];
        gridObjs = new GameObject[width, heigth];
    }

    public void Navigate(Vector2 dir)
    {
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.craftingSelect);

        dir = new Vector2(
            Mathf.Clamp(dir.x, -1, 1),
            Mathf.Clamp(dir.y, -1, 1)
            );

        currentIndex += dir;

        currentIndex = new Vector2(
            Mathf.Clamp(currentIndex.x, 0, gridPositions.GetLength(0) - 1),
            Mathf.Clamp(currentIndex.y, 0, gridPositions.GetLength(1) - 1)
            );
    }

    public void GoToTarget(Vector2 targetPos)
    {
        targetPos = new Vector2(
            Mathf.Clamp(targetPos.x, 0, gridPositions.GetLength(0)),
            Mathf.Clamp(targetPos.y, 0, gridPositions.GetLength(1))
            );

        currentIndex = targetPos;
    }
}

