using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerSoilDetector : MonoBehaviour
{
    private PlayerToolHandler toolHandler;

    private void Start()
    {
        toolHandler = GetComponentInParent<PlayerToolHandler>();
        if (toolHandler == null)
            Debug.LogWarning("PlayerToolHandler not found on parent!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Soil"))
        {
            SoilTile soil = other.GetComponent<SoilTile>();
            if (soil != null) toolHandler.SetNearbySoil(soil);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Soil"))
        {
            SoilTile soil = other.GetComponent<SoilTile>();
            if (soil != null)
            {
                toolHandler.SetNearbySoil(null);
            }
        }
    }
}