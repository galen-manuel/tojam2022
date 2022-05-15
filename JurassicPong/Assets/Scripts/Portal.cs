using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public enum Side
    {
        Left,
        Right
    }

    public Side WorldSide;
    public Action<Side, Thing> Scored;

    private void OnTriggerExit2D(Collider2D collision)
    {
        var thing = collision.transform.root.GetComponent<Thing>();
        if (thing)
        {
            Scored?.Invoke(WorldSide, thing);
            Destroy(collision.gameObject);
        }
    }
}
