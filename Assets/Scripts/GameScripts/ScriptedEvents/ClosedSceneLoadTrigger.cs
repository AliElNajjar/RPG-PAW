using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ClosedSceneLoadTrigger : MonoBehaviour
{
    public string[] message;
    public Looking directionToBackAway = Looking.Down;

    private UnitOverworldMovement player;
    private SceneLoadTrigger loadTrigger;
    private BoxCollider2D _collider;

    private void Awake()
    {
        loadTrigger = GetComponentInParent<SceneLoadTrigger>();

        loadTrigger.active = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<UnitOverworldMovement>();
            MessagesManager.Instance.BuildMessageBox(message, 16, 4, -1, PlayerBackAway);
        }
    }

    private IEnumerator PlayerBackAway()
    {
        float counter = 0;

        if (directionToBackAway == Looking.Down)
        {
            yield return null;
            player.DisableMovement();
            yield return null;

            player.MovementValue = Vector3.down;
            player.LookingDir = Looking.Down;
            player.SetOrKeepState(State.WalkDown);
            player.GetComponent<SimpleAnimator>().Play("WalkDown");

            while (counter < 1f)
            {
                player.Translate(player.MovementValue);
                counter += Time.deltaTime;
                yield return null;
            }

            player.EnableMovement();

            player = null;
        }
    }
}
