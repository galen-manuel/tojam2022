using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSeam : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        BadThing bt = collision.GetComponent<BadThing>();
        if (bt != null)
        {
            bt.Swap();
        }
    }
}
