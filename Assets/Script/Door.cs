using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject townTile;
    public GameObject townLand;
    public GameObject character;
    public GameObject homeLand;

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
                handler.SetNearbyDoor(this);
                Debug.Log("Player entered door area");
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
                handler.SetNearbyDoor(null);
                Debug.Log("Player left door area");
            }
        }
    }
    public void upOrderInLayerChar()
    {
        SpriteRenderer spriteRenderer = character.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 11; // set order in layer 
        }
    }
    public void downOrderInLayerChar()
    {
        SpriteRenderer spriteRenderer = character.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 4; 
        }
    }
    public void TriggerTownEvent()
    {
        if (GameManager.Instance.hasGoneToTownToday)
        {
            GameManager.Instance.playerPopup.ShowPopupNoti($"Hôm nay bạn đã đi thị trấn rồi!");
            Debug.Log("Hôm nay bạn đã đi thị trấn rồi!");
            return;
            GameManager.Instance.isHome = true;
        }

        if (TimeManager.Instance.gameHour < 24 && TimeManager.Instance.gameHour >= 18)
        {
            GameManager.Instance.playerPopup.ShowPopupNoti($"Không thể lên thị trấn!");
            Debug.Log("Hôm nay bạn đã đi thị trấn rồi!");
            return;
            GameManager.Instance.isHome = true;
        }
        GameManager.Instance.hasGoneToTownToday = true;
        int r = Random.Range(0, 100);

        if (r < 26)
        {
            // Sự kiện tốt: bạn được thương gia tặng vàng
            int goldGained = Random.Range(10, 50);
            GameManager.Instance.ChangeGold(goldGained);
            upOrderInLayerChar();
            UIManager.Instance.stateInventory = 2;
            GameManager.Instance.isHome = false;
            homeLand.SetActive(false);
            townLand.SetActive(true);
            townTile.SetActive(true);
            GameManager.Instance.playerPopup.ShowPopupNoti($"Bạn được thương gia tặng {goldGained} vàng!");
            Debug.Log($"Sự kiện tốt: bạn được thương gia tặng {goldGained} vàng!");
        }
        else if (r < 52)
        {
            // Sự kiện xấu: bạn bị quái vật tấn công, mất một số hàng hóa
            int totalFood = GameManager.Instance.GetTotalFoodCount();
            if (totalFood > 0)
            {
                int itemsToLose = Random.Range(1, Mathf.Min(5, totalFood + 1)); // Mất 1-5 món, tùy theo số lượng thực phẩm
                string[] foodTypes = { "pumpkin", "corn", "carrot", "watermelon", "milk", "egg" };
                string lostItems = "";
                for (int i = 0; i < itemsToLose; i++)
                {
                    string foodType = foodTypes[Random.Range(0, foodTypes.Length)];
                    if (GameManager.Instance.GetTotalFoodCount() > 0)
                    {
                        // Kiểm tra xem loại thực phẩm đó có tồn tại không
                        int foodCount = 0;
                        switch (foodType)
                        {
                            case "pumpkin": foodCount = GameManager.Instance.pumpkinCount; break;
                            case "corn": foodCount = GameManager.Instance.cornCount; break;
                            case "carrot": foodCount = GameManager.Instance.carrotCount; break;
                            case "watermelon": foodCount = GameManager.Instance.watermelonCount; break;
                            case "milk": foodCount = GameManager.Instance.milkCount; break;
                            case "egg": foodCount = GameManager.Instance.eggCount; break;
                        }
                        if (foodCount > 0)
                        {
                            GameManager.Instance.ChangeFoodCount(foodType, -1);
                            lostItems += $"{foodType}, ";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(lostItems))
                {
                    lostItems = lostItems.TrimEnd(',', ' ');
                    upOrderInLayerChar();
                    UIManager.Instance.stateInventory = 2;
                    GameManager.Instance.isHome = false;
                    homeLand.SetActive(false);
                    townLand.SetActive(true);
                    townTile.SetActive(true);
                    GameManager.Instance.playerPopup.ShowPopupNoti($"Bạn bị quái vật tấn công, mất {lostItems}!");
                    Debug.Log($"Bạn bị quái vật tấn công, mất {lostItems}!");
                }
                else
                {
                    upOrderInLayerChar();
                    UIManager.Instance.stateInventory = 2;
                    GameManager.Instance.isHome = false;
                    homeLand.SetActive(false);
                    townLand.SetActive(true);
                    townTile.SetActive(true);
                    GameManager.Instance.playerPopup.ShowPopupNoti("Bạn bị quái vật tấn công, nhưng may mắn không mất gì!");
                    Debug.Log("Bạn bị quái vật tấn công, nhưng không mất hàng hóa vì không có gì trong túi!");
                }
            }
            else
            {
                upOrderInLayerChar();
                UIManager.Instance.stateInventory = 2;
                GameManager.Instance.isHome = false;
                homeLand.SetActive(false);
                townLand.SetActive(true);
                townTile.SetActive(true);
                GameManager.Instance.playerPopup.ShowPopupNoti("Bạn bị quái vật tấn công, nhưng may mắn không mất gì!");
                Debug.Log("Bạn bị quái vật tấn công, nhưng không mất hàng hóa vì không có gì trong túi!");
            }
        }
        else if (r < 78)
        {
            // Sự kiện xấu: bạn bị lừa mất vàng
            int goldLost = Random.Range(10, 50);
            GameManager.Instance.ChangeGold(-goldLost);
            upOrderInLayerChar();
            UIManager.Instance.stateInventory = 2;
            GameManager.Instance.isHome = false;
            homeLand.SetActive(false);
            townLand.SetActive(true);
            townTile.SetActive(true);
            GameManager.Instance.playerPopup.ShowPopupNoti($"Bạn bị lừa mất {goldLost} vàng!");
            Debug.Log($"Sự kiện xấu: bạn bị lừa mất {goldLost} vàng!");
        }
        else if (r < 90)
        {
            // Không có gì xảy ra
            upOrderInLayerChar();
            UIManager.Instance.stateInventory = 2;
            GameManager.Instance.isHome = false;
            homeLand.SetActive(false);
            townLand.SetActive(true);
            townTile.SetActive(true);
            GameManager.Instance.playerPopup.ShowPopupNoti("Không có gì xảy ra trên đường đi.");
            Debug.Log("Không có gì xảy ra trên đường đi.");
        }
        else
        {
            upOrderInLayerChar();
            UIManager.Instance.stateInventory = 3;
            GameManager.Instance.isHome = false;
            homeLand.SetActive(false);
            townLand.SetActive(true);
            townTile.SetActive(true);
            // Sự kiện hiếm: thương gia siêu giàu trả x1.5 giá
            GameManager.Instance.playerPopup.ShowPopupNoti("Sự kiện hiếm: gặp thương gia siêu giàu trả x1.5 giá!");
            Debug.Log("Sự kiện hiếm: thương gia siêu giàu trả x1.5 giá!");
        }
    }
    public void GoToHome()
    {
        UIManager.Instance.stateInventory = 1;
        homeLand.SetActive(true);
        townLand.SetActive(false);
        townTile.SetActive(false);
    }
}
