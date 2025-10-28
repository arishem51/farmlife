using UnityEngine;

public class PlayerToolHandler : MonoBehaviour
{
    public Tool currentTool;
    public SpriteRenderer toolRenderer;
    private ToolPickup nearbyTool;
    private SoilTile nearbySoil;
    private House nearbyHouse;
    private Door nearbyDoor;
    private Shop nearbyShop;
    private CowArea nearbyCowArea; // Khu vực bò
    private ChickenCoop nearbyChickenCoop; // Chuồng gà
    private PlayerPopup playerPopup;
    private float lastPopupTime = 0f;
    private float popupCooldown = 0.5f;
    private void Start()
    {
        playerPopup = GetComponent<PlayerPopup>();
        if (playerPopup == null)
        {
            Debug.LogError("PlayerPopup component not found on Player!");
        }
        if (toolRenderer != null) toolRenderer.enabled = false;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.6f, LayerMask.GetMask("Default"));
        ToolPickup closestTool = null;
        float closestDistance = float.MaxValue;
        Vector2 playerPos = transform.position;
        foreach (Collider2D hit in hits)
        {
            ToolPickup tool = hit.GetComponent<ToolPickup>();
            if (tool != null && !tool.isPickedUp)
            {
                float distance = Vector2.Distance(playerPos, tool.transform.position);
                Debug.Log($"Detected {tool.toolData?.toolName} at {tool.transform.position}, distance: {distance}");
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTool = tool;
                }
            }
        }
        if (closestTool != null)
        {
            SetNearbyTool(closestTool);
            Debug.Log($"Initial collision detected with closest tool: {closestTool.toolData?.toolName} at {closestTool.transform.position}, distance: {closestDistance}");
        }
    }

    private void Update()
    {
        if (nearbyTool != null && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log($"Attempting to pick up {nearbyTool.toolData?.toolName}, isPickedUp: {nearbyTool.isPickedUp}");
            if (nearbyTool != null && !nearbyTool.isPickedUp)
                PickUpTool();
            else
                Debug.LogWarning("Cannot pick up: Tool is already picked up or nearbyTool is null!");
        }
        if (nearbySoil != null && Input.GetKeyDown(KeyCode.J))
        {
            if (currentTool == null)
            {
                playerPopup.ShowPopup("Không có công cụ!");
                Debug.Log("No tool equipped.");
            }
            else
            {
                int seedID = (currentTool.toolType == ToolType.SeedBag) ? Random.Range(1, 5) : -1;
                nearbySoil.Interact(currentTool.toolType, seedID);
                Debug.Log("Seed ID: " + seedID);
            }
        }
        if (nearbySoil != null && Input.GetKeyDown(KeyCode.G))
        {
            CheckHarvestStatus();
        }
        if (nearbyHouse != null && Input.GetKeyDown(KeyCode.F))
        {
            nearbyHouse.Rest();
        }
        if (nearbyDoor != null && Input.GetKeyDown(KeyCode.F))
        {
            if (GameManager.Instance.isHome)
            {
                nearbyDoor.TriggerTownEvent();
                
            }
            else
            {
                nearbyDoor.GoToHome();
                GameManager.Instance.isHome = true;

            }
        }
        if (nearbyShop != null && Input.GetKeyDown(KeyCode.F))
        {
            UIManager.Instance.OpenInventory();
        }
        if (nearbyCowArea != null && Input.GetKeyDown(KeyCode.F))
        {
            nearbyCowArea.CollectMilk(); // Gọi hàm thu hoạch sữa
        }
        if (nearbyChickenCoop != null && Input.GetKeyDown(KeyCode.F))
        {
            nearbyChickenCoop.CollectEggs(); // Gọi hàm thu hoạch trứng
        }

        // Thêm logic cho ăn (J)
        if (nearbyCowArea != null && Input.GetKeyDown(KeyCode.J))
        {
            nearbyCowArea.FeedCows();
        }
        if (nearbyChickenCoop != null && Input.GetKeyDown(KeyCode.J))
        {
            nearbyChickenCoop.FeedChickens();
        }

        UpdateSoilPopup();
    }

    private void CheckHarvestStatus()
    {
        if (nearbySoil == null) return;
        if (!nearbySoil.IsPlanted)
        {
            playerPopup.ShowPopup("Không có cây để thu hoạch!");
            return;
        }
        if (!nearbySoil.HasBeenWateredOnce)
        {
            playerPopup.ShowPopup("Cây chưa được tưới nước!");
            return;
        }
        if (nearbySoil.GrowthStage < 3)
        {
            playerPopup.ShowPopup("Chưa thể thu hoạch!");
            return;
        }
        nearbySoil.TryHarvest();
        playerPopup.HidePopup();
    }

    private void UpdateSoilPopup()
    {
        if (Time.time - lastPopupTime < popupCooldown) return;
        lastPopupTime = Time.time;

        if (nearbyCowArea != null)
        {
            if (!GameManager.Instance.hasFedCowsToday)
            {
                playerPopup.ShowPopup($"J | Cho bò ăn (-{GameManager.Instance.cowCount} thể lực)");
            }
            else
            {
                int milkReady = GameManager.Instance?.milkReadyToHarvest ?? 0;
                playerPopup.ShowPopup($"F | Lấy sữa (x{milkReady})");
            }
            return;
        }
        if (nearbyChickenCoop != null)
        {
            if (!GameManager.Instance.hasFedChickensToday)
            {
                playerPopup.ShowPopup($"J | Cho gà ăn (-{GameManager.Instance.chickenCount} thể lực)");
            }
            else
            {
                int eggReady = GameManager.Instance?.eggReadyToHarvest ?? 0;
                playerPopup.ShowPopup($"F | Lấy trứng (x{eggReady})");
            }
            return;
        }
        if (nearbyTool != null)
        {
            if (nearbyTool.toolData?.toolName == "Hoe")
                playerPopup.ShowPopup($"F | Dùng xẻng");
            else if (nearbyTool.toolData?.toolName == "WateringCan")
                playerPopup.ShowPopup($"F | Dùng bình tưới");
            else if (nearbyTool.toolData?.toolName == "SeedBag")
                playerPopup.ShowPopup($"F | Dùng mầm cây");
            return;
        }
        if (nearbyHouse != null)
        {
            if (TimeManager.Instance.gameHour >= 20 && TimeManager.Instance.gameHour < 24)
            {
                playerPopup.ShowPopup("F | Nghỉ ngơi (+30 thể lực)");
            }
            else
            {
                playerPopup.ShowPopup("Bạn chỉ có thể bắt đầu nghỉ ngơi trong khoảng từ 20h -> 24h");
            }
            return;
        }
        if (nearbyDoor != null)
        {
            if (GameManager.Instance.isHome)
            {
                if (TimeManager.Instance.gameHour >= 0 && TimeManager.Instance.gameHour < 18)
                {
                    playerPopup.ShowPopup("F | Lên thị trấn");
                }
                else
                {
                    playerPopup.ShowPopup("Bạn chỉ có thể lên thị trấn trong khoảng từ 00h -> 18h");
                }
            }
            else
            {
                playerPopup.ShowPopup("F | Về nhà");
            }
            return;
        }
        if (nearbyShop != null)
        {
            playerPopup.ShowPopup("F | Mua bán vật phẩm");
            return;
        }
        if (nearbySoil != null)
        {
            if (currentTool != null)
            {
                if (currentTool.toolType == ToolType.Hoe && !nearbySoil.IsTilled)
                {
                    playerPopup.ShowPopup("J | Đào (-1 thể lực)");
                    return;
                }
                else if (currentTool.toolType == ToolType.SeedBag && nearbySoil.IsTilled && !nearbySoil.IsPlanted)
                {
                    playerPopup.ShowPopup("J | Trồng (-1 thể lực & -1 vàng)");
                    return;
                }
                else if (currentTool.toolType == ToolType.WateringCan && nearbySoil.IsTilled && !nearbySoil.IsWatered)
                {
                    playerPopup.ShowPopup("J | Tưới nước (-1 thể lực)");
                    return;
                }
            }
            if (!nearbySoil.IsPlanted)
            {
                playerPopup.HidePopup();
                return;
            }
            if (!nearbySoil.HasBeenWateredOnce)
            {
                playerPopup.ShowPopup("Cây chưa được tưới nước!");
                return;
            }
            if (nearbySoil.GrowthStage < 3)
            {
                playerPopup.ShowPopup("Chưa thể thu hoạch!");
                return;
            }
            playerPopup.ShowPopup("G | Thu hoạch");
        }
        else
        {
            playerPopup.HidePopup();
        }
    }

    public void SetNearbyTool(ToolPickup tool)
    {
        nearbyTool = tool;
        UpdateSoilPopup();
    }

    public void SetNearbySoil(SoilTile soil)
    {
        nearbySoil = soil;
        UpdateSoilPopup();
    }

    public void SetNearbyHouse(House house)
    {
        nearbyHouse = house;
        UpdateSoilPopup();
    }
    public void SetNearbyDoor(Door door)
    {
        nearbyDoor = door;
        UpdateSoilPopup();
    }
    public void SetNearbyShop(Shop shop)
    {
        nearbyShop = shop;
        UpdateSoilPopup();
    }
    public void SetNearbyCowArea(CowArea cowArea)
    {
        nearbyCowArea = cowArea;
        UpdateSoilPopup();
    }

    public void SetNearbyChickenCoop(ChickenCoop chickenCoop)
    {
        nearbyChickenCoop = chickenCoop;
        UpdateSoilPopup();
    }

    private void PickUpTool()
    {
        if (nearbyTool == null || nearbyTool.toolData == null)
        {
            Debug.LogWarning("No valid tool to pick up!");
            return;
        }
        if (currentTool != null)
        {
            DropTool();
        }
        currentTool = nearbyTool.toolData;
        nearbyTool.isPickedUp = true;
        Debug.Log($"Set isPickedUp to true for {nearbyTool.toolData?.toolName}");
        if (toolRenderer != null)
        {
            toolRenderer.sprite = currentTool.icon;
            toolRenderer.enabled = true;
        }
        Debug.Log($"Picked up: {currentTool.toolName}");
        playerPopup.HidePopup();
    }

    private void DropTool()
    {
        if (currentTool == null) return;
        ToolPickup[] allTools = FindObjectsOfType<ToolPickup>();
        foreach (ToolPickup tool in allTools)
        {
            if (tool.toolData == currentTool && tool.isPickedUp)
            {
                tool.isPickedUp = false;
                Debug.Log($"Set isPickedUp to false for {tool.toolData?.toolName}");
                tool.transform.position = tool.GetInitialPosition();
                Debug.Log($"Dropped {currentTool.toolName} at {tool.GetInitialPosition()}");
                break;
            }
        }
        currentTool = null;
        if (toolRenderer != null) toolRenderer.enabled = false;
    }
}