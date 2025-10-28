using UnityEngine;

public enum ToolType
{
    None,
    Hoe,
    WateringCan,
    SeedBag
}

[CreateAssetMenu(menuName = "Farm/Tool")]
public class Tool : ScriptableObject
{
    public ToolType toolType;
    public string toolName;
    public Sprite icon;
    [Header("For SeedBag only")]
    public int seedPlantID = -1; // ID của cây khi trồng (liên kết với PlantData)
}