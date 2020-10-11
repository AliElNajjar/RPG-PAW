using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTileGenerator : MonoBehaviour
{
    public GameObject startingTile, rightTile, bottomTile;
    public int columns, rows;
    public bool generate = false;

    private void OnValidate()
    {
        if (generate)
        {
            generate = false;
            GameObject temp = null;
            float width = rightTile.transform.position.x - startingTile.transform.position.x;
            float height = startingTile.transform.position.y - bottomTile.transform.position.y;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    temp = Instantiate(startingTile, new Vector3(transform.position.x + (width * j), transform.position.y - (height * i), 0), Quaternion.identity, transform);
                }
            }
        }
    }
}
