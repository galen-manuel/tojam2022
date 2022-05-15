using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public enum Side
    {
        Left,
        Right
    }

    [SerializeField] private Collider2D _collider;

    public Side WorldSide;
    public Action<Side, Thing> Scored;

    private void Awake()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        Messenger.AddListener(Constants.EVENT_GAME_OVER, OnGameOver);
    }

    private void Unsubscribe()
    {
        Messenger.RemoveListener(Constants.EVENT_GAME_OVER, OnGameOver);
    }

    #region Event Handlers

    private void OnTriggerExit2D(Collider2D collision)
    {
        var thing = collision.transform.root.GetComponent<Thing>();
        if (thing)
        {
            Messenger.Broadcast(Constants.EVENT_PORTAL_SCORED, WorldSide, thing, MessengerMode.REQUIRE_LISTENER);
            Messenger.Broadcast(Constants.EVENT_DESTROY_THING, thing, MessengerMode.REQUIRE_LISTENER);
        }
    }

    private void OnGameOver()
    {
        _collider.enabled = false;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    #endregion
}
