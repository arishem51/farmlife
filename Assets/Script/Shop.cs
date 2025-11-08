using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private void Start()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(2f, 2f);
            Debug.Log("Added BoxCollider2D to House");
        }
        else if (!collider.isTrigger)
        {
            collider.isTrigger = true;
            Debug.Log("Set isTrigger=true for House");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerToolHandler handler = other.GetComponent<PlayerToolHandler>();
            if (handler != null)
            {
                handler.SetNearbyShop(this);
                Debug.Log("Player entered shop area");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerToolHandler handler = other.GetComponent<PlayerToolHandler>();
            if (handler != null)
            {
                handler.SetNearbyShop(null);
                Debug.Log("Player left shop area");
            }
        }
    }
}
