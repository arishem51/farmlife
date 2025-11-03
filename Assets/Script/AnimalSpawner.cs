using UnityEngine;
using System.Linq;

public class AnimalSpawner : MonoBehaviour
{
    [Header("Khu vực Bò (Kéo GameObject CowArea vào đây)")]
    public GameObject cowAreaObject;

    [Header("Khu vực Gà (Kéo GameObject ChickenCoop vào đây)")]
    public GameObject chickenCoopObject;

    [Header("Prefabs")]
    [Space(10)]
    public GameObject[] cowPrefabs = new GameObject[3];
    public GameObject[] chickenPrefabs = new GameObject[4];

    private CowArea cowArea;
    private ChickenCoop chickenCoop;
    private bool isInitialized = false;

    private void Start()
    {
        if (GameManager.Instance != null)
            Initialize();
        else
            Debug.LogError("❌ GameManager.Instance is null! Đảm bảo GameManager tồn tại trong Scene.");
    }

    [ContextMenu("Khởi tạo Animals (Chạy từ Inspector)")]
    public void Initialize()
    {
        if (isInitialized) return;

        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance is null! Không thể khởi tạo AnimalSpawner.");
            return;
        }

        if (cowAreaObject == null)
        {
            Debug.LogError("❌ CowAreaObject chưa được gán trong AnimalSpawner!");
            return;
        }
        cowArea = cowAreaObject.GetComponent<CowArea>();
        if (cowArea == null)
        {
            Debug.LogError("❌ CowAreaObject không có component CowArea!");
            return;
        }
        cowArea.SetSpawner(this);
        Debug.Log("✅ Kết nối CowArea thành công!");

        if (chickenCoopObject == null)
        {
            Debug.LogError("❌ ChickenCoopObject chưa được gán trong AnimalSpawner!");
            return;
        }
        chickenCoop = chickenCoopObject.GetComponent<ChickenCoop>();
        if (chickenCoop == null)
        {
            Debug.LogError("❌ ChickenCoopObject không có component ChickenCoop!");
            return;
        }
        chickenCoop.SetSpawner(this);
        Debug.Log("✅ Kết nối ChickenCoop thành công!");

        if (cowPrefabs.Any(prefab => prefab == null))
        {
            Debug.LogError("❌ Một hoặc nhiều Cow Prefabs chưa được gán!");
            return;
        }
        if (chickenPrefabs.Any(prefab => prefab == null))
        {
            Debug.LogError("❌ Một hoặc nhiều Chicken Prefabs chưa được gán!");
            return;
        }

        if (cowPrefabs.Length > 0) cowArea.SetCowPrefabs(cowPrefabs);
        if (chickenPrefabs.Length > 0) chickenCoop.SetChickenPrefabs(chickenPrefabs);

        isInitialized = true;
        Debug.Log("🎉 AnimalSpawner đã khởi tạo hoàn tất!");
    }

    public void RespawnAnimals()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance is null! Không thể respawn animals.");
            return;
        }
        cowArea?.SpawnCows();
        chickenCoop?.SpawnChickens();
    }

    private void OnValidate()
    {
        if (Application.isPlaying && !isInitialized && GameManager.Instance != null)
            Initialize();
    }
}