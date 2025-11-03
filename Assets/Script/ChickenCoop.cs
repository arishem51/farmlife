using UnityEngine;
using System.Linq;

public class ChickenCoop : MonoBehaviour
{
    [Header("Prefabs (Sẽ được gán từ AnimalSpawner)")]
    public GameObject[] chickenPrefabs = new GameObject[0];
    private GameObject[] spawnedChickens;
    private AnimalSpawner spawner;
    private bool prefabsAssigned = false;

    private void Start()
    {
        SetupCollider();
        if (prefabsAssigned)
            SpawnChickens();
    }

    public void SetSpawner(AnimalSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    public void SetChickenPrefabs(GameObject[] prefabs)
    {
        chickenPrefabs = prefabs;
        prefabsAssigned = true;
        if (Application.isPlaying && GameManager.Instance != null)
            SpawnChickens();
        Debug.Log($"✅ ChickenCoop nhận {chickenPrefabs.Length} chicken prefabs!");
    }

    private void SetupCollider()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(3f, 3f); // Kích thước mặc định nếu chưa có
        }
        else if (!collider.isTrigger)
        {
            collider.isTrigger = true;
        }
        // Giữ nguyên kích thước và offset bạn đã set trong Inspector
    }

    public void SpawnChickens()
    {
        // Xóa gà cũ
        if (spawnedChickens != null)
        {
            foreach (GameObject chicken in spawnedChickens)
            {
                if (chicken != null) Destroy(chicken);
            }
        }
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance is null! Đảm bảo GameManager tồn tại trong Scene.");
            return;
        }
        int chickenCount = GameManager.Instance.chickenCount;
        if (chickenCount == 0)
        {
            Debug.LogWarning("⚠️ Không có gà để spawn (chickenCount = 0)!");
            return;
        }
        if (chickenPrefabs == null || chickenPrefabs.Length == 0 || chickenPrefabs.Any(prefab => prefab == null))
        {
            Debug.LogError("❌ Chicken Prefabs chưa được gán hoặc chứa giá trị null! Kiểm tra AnimalSpawner.");
            return;
        }
        // Lấy thông tin BoxCollider2D
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector2 colliderSize = collider != null ? collider.size : new Vector2(3f, 3f);
        Vector2 colliderOffset = collider != null ? collider.offset : Vector2.zero;
        Vector3 parentScale = transform.localScale; // Lấy scale của ChickenCoop
        spawnedChickens = new GameObject[chickenCount];
        for (int i = 0; i < chickenCount; i++)
        {
            int randomIndex = Random.Range(0, chickenPrefabs.Length);
            GameObject chickenPrefab = chickenPrefabs[randomIndex];
            GameObject chickenObj = Instantiate(chickenPrefab, transform);
            chickenObj.transform.localScale = chickenPrefab.transform.localScale; // Giữ scale của prefab
            spawnedChickens[i] = chickenObj;
            // Tính phạm vi spawn dựa trên BoxCollider2D và scale
            float halfWidth = (colliderSize.x / 2f) / Mathf.Max(parentScale.x, 0.01f); // Tránh chia cho 0
            float halfHeight = (colliderSize.y / 2f) / Mathf.Max(parentScale.y, 0.01f);
            Vector2 randomPos = new Vector2(
                Random.Range(-halfWidth, halfWidth) + (colliderOffset.x / Mathf.Max(parentScale.x, 0.01f)),
                Random.Range(-halfHeight, halfHeight) + (colliderOffset.y / Mathf.Max(parentScale.y, 0.01f))
            );
            chickenObj.transform.localPosition = randomPos;
            Debug.Log($"🐔 Spawned Chicken #{i} (Prefab {randomIndex}, World Position: {chickenObj.transform.position}, Local Position: {randomPos}, Collider Size: {colliderSize}, Parent Scale: {parentScale})");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerToolHandler handler = other.GetComponent<PlayerToolHandler>();
            handler?.SetNearbyChickenCoop(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerToolHandler handler = other.GetComponent<PlayerToolHandler>();
            handler?.SetNearbyChickenCoop(null);
        }
    }

    public void CollectEggs()
    {
        if (GameManager.Instance == null) return;
        int eggsAvailable = GameManager.Instance.eggReadyToHarvest;
        if (eggsAvailable > 0)
        {
            GameManager.Instance.HarvestEggs(eggsAvailable); // Thu hoạch tất cả trứng sẵn sàng
            Debug.Log($"🥚 Thu hoạch {eggsAvailable} trứng từ {spawnedChickens?.Length ?? 0} con gà!");
        }
        else
        {
            Debug.Log("Chưa có trứng! Chờ 10 phút...");
        }
    }

    // Thêm hàm cho ăn gà
    public void FeedChickens()
    {
        GameManager.Instance.FeedChickens();
    }
}