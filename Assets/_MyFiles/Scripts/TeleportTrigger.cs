using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] float xLoc;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 velocityBeforeTeleport = collision.GetComponent<Rigidbody2D>().velocity;

            collision.transform.position = new Vector3(xLoc, collision.transform.position.y, 0);
            float offset = collision.transform.position.x - Camera.main.transform.position.x;
            Camera.main.transform.position = new Vector3(xLoc + offset + velocityBeforeTeleport.x, Camera.main.transform.position.y, -10);

            collision.GetComponent<Rigidbody2D>().velocity = velocityBeforeTeleport;
        }
    }

}
