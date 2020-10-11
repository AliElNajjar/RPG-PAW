using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustSortOrderDependents : MonoBehaviour
{
    [SerializeField]
    private OrderableInLayer[] _dependenants;

    public void UpdateDependentsOrder(int order)
    {
        for (int i = 0; i < _dependenants.Length; i++)
        {
            _dependenants[i].UpdateOrder(order);
        }
    }

    [System.Serializable]
    public struct OrderableInLayer
    {
        [SerializeField]
        SpriteRenderer _spriteRenderer;
        [SerializeField]
        int _relativeOrder;
        public void UpdateOrder(int baseOrder)
        {
            _spriteRenderer.sortingOrder = baseOrder + _relativeOrder;
        }
        public OrderableInLayer(SpriteRenderer spriteRenderer, int relativeOrder)
        {
            this._spriteRenderer = spriteRenderer;
            this._relativeOrder = relativeOrder;
        }
    }
}
