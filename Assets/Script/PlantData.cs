using UnityEngine;

[CreateAssetMenu(menuName = "Farm/PlantData")]
public class PlantData : ScriptableObject
{
    public string plantName;
    public int plantID; // ID duy nhất (1, 2, 3...)
    [Header("Growth Times (seconds per stage)")]
    public float[] growthTimes = new float[4] { 60f, 90f, 120f, 150f }; // Thời gian mỗi giai đoạn
    [Header("Sprites (4 stages)")]
    public Sprite[] growthSprites = new Sprite[4]; // 4 sprite cho 4 giai đoạn
}