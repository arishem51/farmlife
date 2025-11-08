using UnityEngine;

public class House : MonoBehaviour
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
                handler.SetNearbyHouse(this);
                Debug.Log("Player entered house area");
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
                handler.SetNearbyHouse(null);
                Debug.Log("Player left house area");
            }
        }
    }

    public void Rest()
    {
        Debug.Log("Player is resting...");
        GameManager.Instance.EndDay();
    }
}