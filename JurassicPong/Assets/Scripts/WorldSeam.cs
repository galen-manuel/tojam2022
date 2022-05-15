using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSeam : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player)
        {
            player.Respawn();
        }
    }
}
