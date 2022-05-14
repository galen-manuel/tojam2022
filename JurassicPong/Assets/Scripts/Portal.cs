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
    public Action<Side, string> Scored;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Constants.TAG_GOOD_THING ||
            collision.gameObject.tag == Constants.TAG_BAD_THING)
        {
            Scored?.Invoke(WorldSide, collision.gameObject.tag);
            Destroy(collision.gameObject);
        }
    }
}
