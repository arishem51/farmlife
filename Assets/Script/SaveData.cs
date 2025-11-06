using System;
using System.Collections.Generic;

[Serializable]
public class SoilTileData
{
    public float x; 
    public float y; 
    public bool isTilled;
    public bool isWatered;
    public bool isPlanted;
    public int plantID;
    public int growthStage;
    public float timeRemaining;
    public bool hasBeenWateredOnce;
}
[Serializable]
public class RecordData
{
    public int maxDay;
    public int maxHour;
    public int maxMinute;
}
[Serializable]
public class FarmSaveData
{
    public List<SoilTileData> soilTiles = new List<SoilTileData>();
    public int gold;
    public int stamina;
    public int day;
    public int gameHour;
    public int gameMinute;

    public int pumpkinCount;
    public int cornCount;
    public int carrotCount;
    public int watermelonCount;
    public int cowCount;
    public int chickenCount;
    public int milkCount;
    public int eggCount;
    public float nextCowProduceTime;
    public float nextChickenProduceTime;
}
