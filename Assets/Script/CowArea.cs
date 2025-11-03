using UnityEngine;
using System.Linq;

public class CowArea : MonoBehaviour
{
    [Header("Prefabs (Sẽ được gán từ AnimalSpawner)")]
    public GameObject[] cowPrefabs = new GameObject[0];
    private GameObject[] spawnedCows;
    private AnimalSpawner spawner;
    private bool prefabsAssigned = false;

    private void Start()
    {
        SetupCollider();
        if (prefabsAssigned)
            SpawnCows();
    }

    public void SetSpawner(AnimalSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    public void SetCowPrefabs(GameObject[] prefabs)
    {
        cowPrefabs = prefabs;
        prefabsAssigned = true;
        if (Application.isPlaying && GameManager.Instance != null)
            SpawnCows();
        Debug.Log($"✅ CowArea nhận {cowPrefabs.Length} cow prefabs!");
    }

    private void SetupCollider()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(4f, 4f); // Kích thước mặc định nếu chưa có
        }
        else if (!collider.isTrigger)
        {
            collider.isTrigger = true;
        }
        // Giữ nguyên kích thước và offset bạn đã set trong Inspector
    }

    public void SpawnCows()
    {
        // Xóa bò cũ
        if (spawnedCows != null)
        {
            foreach (GameObject cow in spawnedCows)
            {
                if (cow != null) Destroy(cow);
            }
        }
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance is null! Đảm bảo GameManager tồn tại trong Scene.");
            return;
        }
        int cowCount = GameManager.Instance.cowCount;
        if (cowCount == 0)
        {
            Debug.LogWarning("⚠️ Không có bò để spawn (cowCount = 0)!");
            return;
        }
        if (cowPrefabs == null || cowPrefabs.Length == 0 || cowPrefabs.Any(prefab => prefab == null))
        {
            Debug.LogError("❌ Cow Prefabs chưa được gán hoặc chứa giá trị null! Kiểm tra AnimalSpawner.");
            return;
        }
        // Lấy thông tin BoxCollider2D
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector2 colliderSize = collider != null ? collider.size : new Vector2(4f, 4f);
        Vector2 colliderOffset = collider != null ? collider.offset : Vector2.zero;
        Vector3 parentScale = transform.localScale; // Lấy scale của CowArea
        spawnedCows = new GameObject[cowCount];
        for (int i = 0; i < cowCount; i++)
        {
            int randomIndex = Random.Range(0, cowPrefabs.Length);
            GameObject cowPrefab = cowPrefabs[randomIndex];
            GameObject cowObj = Instantiate(cowPrefab, transform);
            cowObj.transform.localScale = cowPrefab.transform.localScale; // Giữ scale của prefab
            spawnedCows[i] = cowObj;
            // Tính phạm vi spawn dựa trên BoxCollider2D và scale
            float halfWidth = (colliderSize.x / 2f) / Mathf.Max(parentScale.x, 0.01f); // Tránh chia cho 0
            float halfHeight = (colliderSize.y / 2f) / Mathf.Max(parentScale.y, 0.01f);
            Vector2 randomPos = new Vector2(
                Random.Range(-halfWidth, halfWidth) + (colliderOffset.x / Mathf.Max(parentScale.x, 0.01f)),
                Random.Range(-halfHeight, halfHeight) + (colliderOffset.y / Mathf.Max(parentScale.y, 0.01f))
            );
            cowObj.transform.localPosition = randomPos;
            Debug.Log($"🐄 Spawned Cow #{i} (Prefab {randomIndex}, World Position: {cowObj.transform.position}, Local Position: {randomPos}, Collider Size: {colliderSize}, Parent Scale: {parentScale})");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerToolHandler handler = other.GetComponent<PlayerToolHandler>();
            handler?.SetNearbyCowArea(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerToolHandler handler = other.GetComponent<PlayerToolHandler>();
            handler?.SetNearbyCowArea(null);
        }
    }

    public void CollectMilk()
    {
        if (GameManager.Instance == null) return;
        int milkAvailable = GameManager.Instance.milkReadyToHarvest;
        if (milkAvailable > 0)
        {
            GameManager.Instance.HarvestMilk(milkAvailable); // Thu hoạch tất cả sữa sẵn sàng
            Debug.Log($"🥛 Thu hoạch {milkAvailable} sữa từ {spawnedCows?.Length ?? 0} con bò!");
        }
        else
        {
            Debug.Log("Chưa có sữa! Chờ 10 phút...");
        }
    }

    // Thêm hàm cho ăn bò
    public void FeedCows()
    {
        GameManager.Instance.FeedCows();
    }
}