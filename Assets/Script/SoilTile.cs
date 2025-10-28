using UnityEngine;
using System.Collections.Generic;

public class SoilTile : MonoBehaviour
{
    // State
    [SerializeField] private bool isTilled;
    [SerializeField] private bool isWatered;
    [SerializeField] private bool isPlanted;
    [SerializeField] private int plantID = -1; // -1 = none
    [SerializeField] private int growthStage = 0;
    [SerializeField] private float timeRemaining = 0f;
    [SerializeField] private bool hasBeenWateredOnce = false; // Đã tưới ít nhất 1 lần
    // Properties để Save/Load
    public bool IsTilled { get { return isTilled; } set { isTilled = value; } }
    public bool IsWatered { get { return isWatered; } set { isWatered = value; } }
    public bool IsPlanted { get { return isPlanted; } set { isPlanted = value; } }
    public int PlantID { get { return plantID; } set { plantID = value; } }
    public int GrowthStage { get { return growthStage; } set { growthStage = value; } }
    public float TimeRemaining { get { return timeRemaining; } set { timeRemaining = value; } }
    public bool HasBeenWateredOnce { get { return hasBeenWateredOnce; } set { hasBeenWateredOnce = value; } }
    // Visuals
    public SpriteRenderer soilRenderer;
    public SpriteRenderer plantRenderer;
    public Sprite normalSoil;
    public Sprite tilledSoil;
    public Sprite wateredSoil;
    // Plant system
    private PlantData currentPlantData;
    private Dictionary<int, PlantData> plantDataCache = new Dictionary<int, PlantData>();
    [SerializeField] private Vector2 gridPosition; // ✅ Sử dụng Vector2 thay vì Vector2Int
    private void Start()
    {
        UpdateVisuals();
        if (gridPosition == Vector2.zero)
        {
            gridPosition = transform.position; // ✅ Lấy vị trí thực tế (float)
        }
        CachePlantData();
    }
    private void Update()
    {
        if (isPlanted && timeRemaining > 0 && hasBeenWateredOnce)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                if (growthStage < 3)
                {
                    growthStage++;
                    if (currentPlantData != null && growthStage < currentPlantData.growthTimes.Length)
                    {
                        timeRemaining = currentPlantData.growthTimes[growthStage];
                    }
                    else
                    {
                        timeRemaining = 60f;
                    }
                    UpdateVisuals();
                    Debug.Log($"[{gridPosition}] {currentPlantData?.plantName} -> stage {growthStage + 1}/4");
                }
            }
        }
        // DEBUG: Hiển thị trạng thái (xóa sau khi test)
        if (isPlanted && Time.frameCount % 300 == 0)
        {
            string waterStatus = hasBeenWateredOnce ? "✅ WATERED" : "❌ NEED WATER";
            Debug.Log($"[{gridPosition}] {currentPlantData?.plantName} | Stage:{growthStage} | Time:{timeRemaining:F1}s | {waterStatus}");
        }
    }
    private void CachePlantData()
    {
        PlantData[] allPlants = Resources.LoadAll<PlantData>("Plants");
        foreach (var plant in allPlants)
        {
            plantDataCache[plant.plantID] = plant;
        }
    }
    public void Interact(ToolType tool, int seedPlantID = -1)
    {
        switch (tool)
        {
            case ToolType.Hoe:
                if (!isTilled)
                {
                    isTilled = true;
                    isWatered = false;
                    ResetPlant();
                    GameManager.Instance.ChangeStamina(-1);
                    AudioManager.Instance.PlaySFX("dig");

                    UpdateVisuals();                   
                    Debug.Log($"[{gridPosition}] Soil tilled");
                }
                else
                {
                    Debug.Log("Soil already tilled.");
                }
                break;
            case ToolType.WateringCan:
                if (!isTilled || isWatered==true)
                {
                    Debug.Log("Can't water: soil not tilled.");
                    return;
                }
                hasBeenWateredOnce = true;
                isWatered = true;
                AudioManager.Instance.PlaySFX("water");

                GameManager.Instance.ChangeStamina(-1);
                UpdateVisuals();
                Debug.Log($"[{gridPosition}] Soil watered! 🌊 (hasBeenWateredOnce = true)");
                break;
            case ToolType.SeedBag:
                if (!isTilled)
                {
                    Debug.Log("Can't plant: soil not tilled.");
                    return;
                }
                if (isPlanted)
                {
                    Debug.Log("Already planted.");
                    return;
                }
                if (seedPlantID == -1)
                {
                    Debug.Log("Invalid seed!");
                    return;
                }
                isPlanted = true;
                plantID = seedPlantID;
                currentPlantData = plantDataCache.ContainsKey(plantID) ? plantDataCache[plantID] : null;
                growthStage = 0;
                timeRemaining = currentPlantData != null && currentPlantData.growthTimes.Length > 0
                    ? currentPlantData.growthTimes[0] : 60f;
                GameManager.Instance.ChangeStamina(-1);
                GameManager.Instance.ChangeGold(-1);
                UpdateVisuals();
                Debug.Log($"[{gridPosition}] Planted {currentPlantData?.plantName}  (Need water to grow!)");
                break;
        }
    }
    public void TryHarvest()
    {
        if (!isPlanted)
        {
            Debug.Log("No plant to harvest.");
            return;
        }
        if (growthStage < 3)
        {
            Debug.Log($"[{gridPosition}] {currentPlantData?.plantName} not ready! Stage: {growthStage + 1}/4");
            return;
        }
        if (!hasBeenWateredOnce)
        {
            Debug.Log($"[{gridPosition}] Can't harvest! Plant never watered! ");
            return;
        }
        Debug.Log($" Harvested {currentPlantData?.plantName} (ID: {plantID}) at [{gridPosition}]!");
        GameManager.Instance.ChangeStamina(-1);
        if (plantID == 1) GameManager.Instance.carrotCount += 1;
        if (plantID == 2) GameManager.Instance.watermelonCount += 1;
        if (plantID == 3) GameManager.Instance.pumpkinCount += 1;
        if (plantID == 4) GameManager.Instance.cornCount += 1;
        ResetPlant();
        isWatered= false;
        isTilled= false;
        UpdateVisuals();
        // TODO: Add to inventory
    }
    private void ResetPlant()
    {
        isPlanted = false;
        plantID = -1;
        growthStage = 0;
        timeRemaining = 0f;
        currentPlantData = null;
        hasBeenWateredOnce = false;

        // hasBeenWateredOnce giữ nguyên
    }
    public void UpdateVisuals()
    {
        if (soilRenderer == null || plantRenderer == null) return;
        if (isWatered) soilRenderer.sprite = wateredSoil;
        else if (isTilled) soilRenderer.sprite = tilledSoil;
        else soilRenderer.sprite = normalSoil;
        if (isPlanted && plantID >= 0 && currentPlantData != null && growthStage < currentPlantData.growthSprites.Length)
        {
            plantRenderer.sprite = currentPlantData.growthSprites[growthStage];
            plantRenderer.enabled = true;
            Debug.Log($"[{gridPosition}] Updated plantRenderer with {currentPlantData.plantName} stage {growthStage}");
        }
        else
        {
            plantRenderer.enabled = false;
            Debug.Log($"[{gridPosition}] Disabled plantRenderer (no plant or invalid stage)");
        }
    }
    public Vector2 GetGridPosition() => gridPosition; // ✅ Trả về Vector2
    public void SetGridPosition(Vector2 pos) => gridPosition = pos; // ✅ Nhận Vector2
    // Phương thức để áp dụng dữ liệu đã load
    public void ApplyLoadedData(bool isTilled, bool isWatered, bool isPlanted, int plantID, int growthStage, float timeRemaining, bool hasBeenWateredOnce)
    {
        this.isTilled = isTilled;
        this.isWatered = isWatered;
        this.isPlanted = isPlanted;
        this.plantID = plantID;
        this.growthStage = growthStage;
        this.timeRemaining = timeRemaining;
        this.hasBeenWateredOnce = hasBeenWateredOnce;
        // Gán lại currentPlantData dựa trên plantID
        if (plantID >= 0)
        {
            currentPlantData = plantDataCache.ContainsKey(plantID) ? plantDataCache[plantID] : null;
        }
        else
        {
            currentPlantData = null;
        }
        UpdateVisuals();
        Debug.Log($"[{gridPosition}] Applied loaded data: Planted={isPlanted}, PlantID={plantID}, Stage={growthStage}, PlantRenderer={plantRenderer.enabled}");
    }
    public void ResetTile()
    {
        IsTilled = false;
        IsWatered = false;
        IsPlanted = false;
        PlantID = -1;
        GrowthStage = 0;
        TimeRemaining = 0f;
        HasBeenWateredOnce = false;
        // Cập nhật hiển thị nếu có (tùy thuộc vào cách bạn vẽ tile)
        UpdateVisuals();
    }
}