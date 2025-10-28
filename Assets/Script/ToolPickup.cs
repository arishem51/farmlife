using UnityEngine;

public class ToolPickup : MonoBehaviour
{
    public Tool toolData;
    private Vector2 initialPosition;
    public bool isPickedUp = false; // Trạng thái công cụ

    private void Start()
    {
        initialPosition = transform.position;
        Debug.Log($"Initialized {toolData?.toolName} at {initialPosition}, isPickedUp: {isPickedUp}");

        // Đảm bảo BoxCollider2D
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.5f, 0.5f);
            Debug.Log($"Added BoxCollider2D to {toolData?.toolName}");
        }
        else if (!collider.isTrigger)
        {
            collider.isTrigger = true;
            Debug.Log($"Set isTrigger=true for {toolData?.toolName}");
        }
    }

    public Vector2 GetInitialPosition()
    {
        return initialPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger Enter: {other.gameObject.name} with tag {other.tag}, isPickedUp: {isPickedUp}");
        if (other.CompareTag("Player") && !isPickedUp)
        {
            PlayerToolHandler handler = other.GetComponent<PlayerToolHandler>();
            if (handler != null)
            {
                handler.SetNearbyTool(this);
                Debug.Log($"Player near {toolData?.toolName} at {transform.position}, isPickedUp: {isPickedUp}");
            }
            else
            {
                Debug.LogWarning("Player missing PlayerToolHandler!");
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
                handler.SetNearbyTool(null);
                Debug.Log($"Player left {toolData?.toolName}, isPickedUp: {isPickedUp}");
            }
        }
    }
}