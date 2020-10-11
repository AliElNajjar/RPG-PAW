using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonCarFireEffect : MonoBehaviour
{
    public List<Collider2D> collidersInRange;
    public float damagePeriod;
    public float lastDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lastDamage + damagePeriod < Time.time)
        {
            lastDamage = Time.time;
            DamageDamagablesInRange();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collidersInRange.Add(collision);
        //collision.GetComponent<Damagable>()?.TakeDamage(15);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        collidersInRange.Remove(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    private void DamageDamagablesInRange()
    {
        foreach (Collider2D collider in collidersInRange)
        {
            if (collider.tag=="Player")
            {
                collider.GetComponent<Damagable>()?.TakeDamage(15);

                Vector2 knockDirection = ((Vector2)collider.transform.position - (Vector2)transform.position).normalized*.5f;
                RaycastHit2D raycastHit2D;
                raycastHit2D = Physics2D.Raycast(transform.position, knockDirection.normalized, knockDirection.magnitude);
                if (raycastHit2D)
                    knockDirection = knockDirection.normalized * raycastHit2D.distance;
                collider.GetComponent<Damagable>()?.GetComponent<Rigidbody2D>().MovePosition((Vector2)collider.transform.position + knockDirection);

                print("Player on fire");
                //foreach (PlayerBattleUnitHolder member in PlayerParty.Instance.playerParty.activePartyMembers)
                //{
                //    member.TakeDamage(15, DamageType.Fire);
                //    //PlayerParty.Instance.playerParty.activePartyMembers[0].TakeDamage(15);
                //}
            }
        }
    }

    private void OnDisable()
    {
        collidersInRange.Clear();
    }
}
