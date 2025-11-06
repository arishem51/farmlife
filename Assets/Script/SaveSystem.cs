using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private string saveFile => Path.Combine(Application.persistentDataPath, "farm_save.json");
    private string recordFile => Path.Combine(Application.persistentDataPath, "record_save.json"); // File riêng cho kỷ lục

    public void Save()
    {
        FarmSaveData farm = new FarmSaveData();
        SoilTile[] allTiles = FindObjectsOfType<SoilTile>();
        foreach (var tile in allTiles)
        {
            Vector2 gp = tile.GetGridPosition();
            SoilTileData d = new SoilTileData()
            {
                x = gp.x,
                y = gp.y,
                isTilled = tile.IsTilled,
                isWatered = tile.IsWatered,
                isPlanted = tile.IsPlanted,
                plantID = tile.PlantID,
                growthStage = tile.GrowthStage,
                timeRemaining = tile.TimeRemaining,
                hasBeenWateredOnce = tile.HasBeenWateredOnce
            };
            farm.soilTiles.Add(d);
        }
        farm.gold = GameManager.Instance.gold;
        farm.stamina = GameManager.Instance.stamina;
        farm.day = GameManager.Instance.day;
        farm.pumpkinCount = GameManager.Instance.pumpkinCount;
        farm.cornCount = GameManager.Instance.cornCount;
        farm.carrotCount = GameManager.Instance.carrotCount;
        farm.watermelonCount = GameManager.Instance.watermelonCount;
        farm.cowCount = GameManager.Instance.cowCount;
        farm.chickenCount = GameManager.Instance.chickenCount;
        farm.milkCount = GameManager.Instance.milkCount;
        farm.eggCount = GameManager.Instance.eggCount;
        farm.nextCowProduceTime = GameManager.Instance.nextCowProduceTime;
        farm.nextChickenProduceTime = GameManager.Instance.nextChickenProduceTime;
        farm.gameHour = TimeManager.Instance.gameHour;
        farm.gameMinute = TimeManager.Instance.gameMinute;

        // Lưu dữ liệu game
        string json = JsonUtility.ToJson(farm, true);
        File.WriteAllText(saveFile, json);

        // Cập nhật kỷ lục nếu cần
        UpdateRecordIfNeeded(farm.day, TimeManager.Instance.gameHour, TimeManager.Instance.gameMinute);

        Debug.Log("Game saved to: " + saveFile);
    }

    public void Load()
    {
        if (!File.Exists(saveFile))
        {
            Debug.LogWarning("Save file not found: " + saveFile);
            return;
        }
        string json = File.ReadAllText(saveFile);
        FarmSaveData farm = JsonUtility.FromJson<FarmSaveData>(json);
        SoilTile[] allTiles = FindObjectsOfType<SoilTile>();
        foreach (var data in farm.soilTiles)
        {
            foreach (var tile in allTiles)
            {
                Vector2 gp = tile.GetGridPosition();
                if (Mathf.Abs(gp.x - data.x) < 0.001f && Mathf.Abs(gp.y - data.y) < 0.001f)
                {
                    tile.ApplyLoadedData(data.isTilled, data.isWatered, data.isPlanted, data.plantID, data.growthStage, data.timeRemaining, data.hasBeenWateredOnce);
                    break;
                }
            }
        }
        GameManager.Instance.gold = farm.gold;
        GameManager.Instance.stamina = farm.stamina;
        GameManager.Instance.day = farm.day;
        GameManager.Instance.pumpkinCount = farm.pumpkinCount;
        GameManager.Instance.cornCount = farm.cornCount;
        GameManager.Instance.carrotCount = farm.carrotCount;
        GameManager.Instance.watermelonCount = farm.watermelonCount;
        GameManager.Instance.cowCount = farm.cowCount;
        GameManager.Instance.chickenCount = farm.chickenCount;
        GameManager.Instance.milkCount = farm.milkCount;
        GameManager.Instance.eggCount = farm.eggCount;
        GameManager.Instance.nextCowProduceTime = farm.nextCowProduceTime;
        GameManager.Instance.nextChickenProduceTime = farm.nextChickenProduceTime;
        TimeManager.Instance.gameHour = farm.gameHour;
        TimeManager.Instance.gameMinute = farm.gameMinute;
        UIManager.Instance.UpdateAllUI();

        // Respawn bò và gà
        AnimalSpawner[] spawners = FindObjectsOfType<AnimalSpawner>();
        foreach (var spawner in spawners)
        {
            spawner.RespawnAnimals();
        }

        // Load kỷ lục (không ảnh hưởng đến game hiện tại)
        LoadRecord();

        Debug.Log("Game loaded from: " + saveFile);
    }

    private void UpdateRecordIfNeeded(int currentDay, int currentHour, int currentMinute)
    {
        RecordData currentRecord = LoadRecordData();
        bool updateRecord = false;

        // So sánh ngày
        if (currentRecord.maxDay < currentDay)
        {
            updateRecord = true;
        }
        else if (currentRecord.maxDay == currentDay)
        {
            // Nếu ngày bằng, so sánh giờ
            if (currentRecord.maxHour < currentHour)
            {
                updateRecord = true;
            }
            else if (currentRecord.maxHour == currentHour)
            {
                // Nếu giờ bằng, so sánh phút
                if (currentRecord.maxMinute < currentMinute)
                {
                    updateRecord = true;
                }
            }
        }

        if (updateRecord)
        {
            RecordData newRecord = new RecordData
            {
                maxDay = currentDay,
                maxHour = currentHour,
                maxMinute = currentMinute
            };
            string json = JsonUtility.ToJson(newRecord, true);
            File.WriteAllText(recordFile, json);
            Debug.Log($"New record set: Day {currentDay}, {currentHour:00}:{currentMinute:00}");
        }
    }

    public RecordData LoadRecordData()
    {
        if (File.Exists(recordFile))
        {
            string json = File.ReadAllText(recordFile);
            return JsonUtility.FromJson<RecordData>(json);
        }
        else
        {
            // Nếu không có file kỷ lục, tạo kỷ lục mặc định (ngày 0, giờ 0, phút 0)
            return new RecordData { maxDay = 0, maxHour = 0, maxMinute = 0 };
        }
    }

    private void LoadRecord()
    {
        RecordData record = LoadRecordData();
        Debug.Log($"Current record: Day {record.maxDay}, {record.maxHour:00}:{record.maxMinute:00}");
        // Có thể hiển thị kỷ lục trên UI nếu muốn (cần thêm UIManager)
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) Save();
        if (Input.GetKeyDown(KeyCode.L)) Load();
    }
    public void PlayAgain()
    {
        // Không xóa file save, chỉ reset trạng thái trong game

        // Đặt lại các giá trị trong GameManager
        GameManager.Instance.gold = 200; // Giá trị khởi đầu mặc định
        GameManager.Instance.stamina = 100;
        GameManager.Instance.day = 0;
        GameManager.Instance.pumpkinCount = 0;
        GameManager.Instance.cornCount = 0;
        GameManager.Instance.carrotCount = 0;
        GameManager.Instance.watermelonCount = 0;
        GameManager.Instance.cowCount =0; // Số bò khởi đầu
        GameManager.Instance.chickenCount = 0; // Số gà khởi đầu
        GameManager.Instance.milkCount = 0;
        GameManager.Instance.eggCount = 0;
        GameManager.Instance.nextCowProduceTime = TimeManager.Instance.GetGameTimeInSeconds() + 86400f; // 24 giờ
        GameManager.Instance.nextChickenProduceTime = TimeManager.Instance.GetGameTimeInSeconds() + 86400f;
        GameManager.Instance.hasFedCowsToday = false;
        GameManager.Instance.hasFedChickensToday = false;

        // Đặt lại thời gian trong TimeManager
        TimeManager.Instance.gameHour = 6; // Bắt đầu từ 6:00 sáng
        TimeManager.Instance.gameMinute = 0;

        // Reset các SoilTile
        SoilTile[] allTiles = FindObjectsOfType<SoilTile>();
        foreach (var tile in allTiles)
        {
            tile.ResetTile(); // Giả định có phương thức ResetTile trong SoilTile
        }

        // Respawn bò và gà với số lượng khởi đầu
        AnimalSpawner[] spawners = FindObjectsOfType<AnimalSpawner>();
        foreach (var spawner in spawners)
        {
            spawner.RespawnAnimals(); // Respawn dựa trên cowCount và chickenCount
        }

        // Cập nhật UI
        UIManager.Instance.UpdateAllUI();

        Debug.Log("Game restarted with new playthrough! Save file preserved.");
    }
}