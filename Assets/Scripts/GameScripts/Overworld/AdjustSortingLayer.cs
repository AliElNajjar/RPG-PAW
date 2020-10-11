using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustSortingLayer : MonoBehaviour
{
    [SerializeField, Tooltip("Attach box collider (Trigger) that will be checking how sorting layer changes")]
    BoxCollider2D _boxCollider;
    [ReadOnly] public int initialSortingOrder;

    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        initialSortingOrder = _spriteRenderer.sortingOrder;
    }

    void SetSortingOrder(SpriteRenderer other)
    {
        if (_spriteRenderer == null)
            return;
        if ( _spriteRenderer.bounds.min.y > other.bounds.min.y) 
            _spriteRenderer.sortingOrder = other.sortingOrder - 1;
    }

    void SwapSortingOrder(SpriteRenderer current, SpriteRenderer other)
    {
        int aux = current.sortingOrder;
        current.sortingOrder = other.sortingOrder;
        other.sortingOrder = aux;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name != this.name && other.GetComponent<SpriteRenderer>())
        {
            Debug.Log(other.name + "entered in " + gameObject.name + " region.");
            SetSortingOrder(other.GetComponent<SpriteRenderer>());
        }
    }

    void OnTriggerStay2D(Collider2D other) 
    {
        if (other.name != this.name && other.GetComponent<SpriteRenderer>())
        {
            //SetSortingOrder(other.GetComponent<SpriteRenderer>());

            if ( _spriteRenderer.bounds.min.y > other.bounds.min.y)
            {
                if (_spriteRenderer.sortingOrder > other.GetComponent<SpriteRenderer>().sortingOrder)
                {
                    SwapSortingOrder(_spriteRenderer, other.GetComponent<SpriteRenderer>());
                }
                else
                {
                    SetSortingOrder(other.GetComponent<SpriteRenderer>());
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.GetComponent<SpriteRenderer>())
            return;
        
        if (_spriteRenderer.sortingOrder != 0 && other.GetComponent<SpriteRenderer>().sortingOrder != 0)
            return;

        _spriteRenderer.sortingOrder = 0;
    }
}
