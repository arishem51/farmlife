using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    // Tiền tệ và stamina
    public int gold = 200;
    public int stamina = 100;
    public int day = 1;
    public bool hasGoneToTownToday = false;
    // Thực phẩm
    public int pumpkinCount = 0;
    public int cornCount = 0;
    public int carrotCount = 0;
    public int watermelonCount = 0;
    // Quản lý bò và gà
    public int cowCount = 0; // Số lượng bò
    public int chickenCount = 0; // Số lượng gà
    public int milkCount = 0; // Số lượng sữa trong túi đồ
    public int eggCount = 0; // Số lượng trứng trong túi đồ
    public float nextCowProduceTime = 0f; // Thời gian bò sản xuất sữa tiếp theo
    public float nextChickenProduceTime = 0f; // Thời gian gà đẻ trứng tiếp theo
    // Số lượng sẵn sàng thu hoạch (không trong túi đồ)
    public int milkReadyToHarvest = 0; // Sữa sẵn sàng thu hoạch
    public int eggReadyToHarvest = 0; // Trứng sẵn sàng thu hoạch
    private const float produceInterval = 86400f; // 24 giờ trong game (tính bằng giây)

    // Thêm trạng thái cho ăn hôm nay
    public bool hasFedCowsToday = false;
    public bool hasFedChickensToday = false;
    public PlayerPopup playerPopup;
    public SaveSystem saveSystem;
    public bool isHome = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playerPopup = GetComponent<PlayerPopup>();
        if (playerPopup == null)
        {
            Debug.Log("PlayerPopup component not found on Player!");
        }
        saveSystem = GetComponent<SaveSystem>();
        if (saveSystem == null)
        {
            Debug.Log("PlayerPopup component not found on Player!");
        }
        UIManager.Instance.UpdateAllUI();
        // Khởi tạo thời gian sản xuất
        nextCowProduceTime = TimeManager.Instance.GetGameTimeInSeconds() + produceInterval;
        nextChickenProduceTime = TimeManager.Instance.GetGameTimeInSeconds() + produceInterval;
    }

    private void Update()
    {
        // Kiểm tra sản xuất sữa và trứng
        float currentTime = TimeManager.Instance.GetGameTimeInSeconds();
        if (currentTime >= nextCowProduceTime)
        {
            ProduceMilk();
            nextCowProduceTime += produceInterval;
        }
        if (currentTime >= nextChickenProduceTime)
        {
            ProduceEggs();
            nextChickenProduceTime += produceInterval;
        }
    }

    public void ChangeFoodCount(string foodType, int amount)
    {
        switch (foodType.ToLower())
        {
            case "pumpkin":
                pumpkinCount += amount;
                if (pumpkinCount < 0) pumpkinCount = 0;
                UIManager.Instance.UpdatePumpkinUI();
                break;
            case "corn":
                cornCount += amount;
                if (cornCount < 0) cornCount = 0;
                UIManager.Instance.UpdateCornUI();
                break;
            case "carrot":
                carrotCount += amount;
                if (carrotCount < 0) carrotCount = 0;
                UIManager.Instance.UpdateCarrotUI();
                break;
            case "watermelon":
                watermelonCount += amount;
                if (watermelonCount < 0) watermelonCount = 0;
                UIManager.Instance.UpdateWatermelonUI();
                break;
            case "milk":
                milkCount += amount;
                if (milkCount < 0) milkCount = 0;
                UIManager.Instance.UpdateMilkUI();
                break;
            case "egg":
                eggCount += amount;
                if (eggCount < 0) eggCount = 0;
                UIManager.Instance.UpdateEggUI();
                break;
            case "cow":
                cowCount += amount;
                if (cowCount < 0) cowCount = 0;
                UIManager.Instance.UpdateCowUI();
                break;
            case "chicken":
                chickenCount += amount;
                if (chickenCount < 0) chickenCount = 0;
                UIManager.Instance.UpdateChickenUI();
                break;
        }
    }

    public int GetTotalFoodCount()
    {
        return pumpkinCount + cornCount + carrotCount + watermelonCount + milkCount + eggCount;
    }

    private void ProduceMilk()
    {
        if (cowCount > 0)
        {
            milkReadyToHarvest += cowCount; // Tăng số sữa sẵn sàng thu hoạch
            UIManager.Instance.UpdateMilkUI(); // Cập nhật UI (nếu hiển thị cả sẵn sàng)
            Debug.Log($"🥛 Sản xuất sẵn {cowCount} sữa từ {cowCount} con bò!");
        }
    }

    private void ProduceEggs()
    {
        if (chickenCount > 0)
        {
            int eggs = 0;
            for (int i = 0; i < chickenCount; i++)
            {
                eggs += Random.Range(1, 3); // Mỗi con gà đẻ 1-2 trứng
            }
            eggReadyToHarvest += eggs; // Tăng số trứng sẵn sàng thu hoạch
            UIManager.Instance.UpdateEggUI(); // Cập nhật UI (nếu hiển thị cả sẵn sàng)
            Debug.Log($"🥚 Sản xuất sẵn {eggs} trứng từ {chickenCount} con gà!");
        }
    }

    // Cập nhật hàm thu hoạch để kiểm tra trạng thái cho ăn
    public void HarvestMilk(int amount)
    {
        if (!hasFedCowsToday)
        {
            Debug.Log("Phải cho bò ăn hôm nay trước khi thu hoạch sữa!");
            return;
        }
        if (milkReadyToHarvest >= amount)
        {
            milkReadyToHarvest -= amount;
            ChangeFoodCount("milk", amount); // Cộng vào inventory
            Debug.Log($"🥛 Thu hoạch {amount} sữa vào túi đồ! Tổng: {milkCount}");
        }
    }

    public void HarvestEggs(int amount)
    {
        if (!hasFedChickensToday)
        {
            Debug.Log("Phải cho gà ăn hôm nay trước khi thu hoạch trứng!");
            return;
        }
        if (eggReadyToHarvest >= amount)
        {
            eggReadyToHarvest -= amount;
            ChangeFoodCount("egg", amount); // Cộng vào inventory
            Debug.Log($"🥚 Thu hoạch {amount} trứng vào túi đồ! Tổng: {eggCount}");
        }
    }

    public void ChangeGold(int amount)
    {
        gold += amount;
        UIManager.Instance.UpdateGoldUI();
    }

    public void ChangeStamina(int amount)
    {
        stamina += amount;
        if (stamina < 0) stamina = 0;
        if (stamina >100) stamina = 100;
        UIManager.Instance.UpdateStaminaUI();
        if (stamina == 0)
        {
            if (gold >= 100)
            {
                gold -= 100;
                stamina = 30;
                playerPopup.ShowPopupNoti("Bạn đã kiệt sức, mất 100 vàng để chữa trị");
            }
            else
            {
                saveSystem.Save();
                playerPopup.ShowMenupopup();
                playerPopup.menuPopup.transform.Find("ConButton").GetComponent<Button>().gameObject.SetActive(false);
                playerPopup.menuPopup.transform.Find("SaveButton").GetComponent<Button>().gameObject.SetActive(false);
                AudioManager.Instance.PlaySFX("gameover");

            }
        }
    }

    public void EndDay()
    {
        // Kiểm tra thời gian trong game có nằm trong khoảng 20:00 đến 00:00 không
        if (TimeManager.Instance.gameHour < 20 || TimeManager.Instance.gameHour >= 24)
        {
            Debug.Log("Chỉ có thể kết thúc ngày trong khoảng từ 20:00 đến 00:00!");
            return;
        }

        gold -= 20; // Phí bảo vệ
        if (gold <= 0)
        {
            saveSystem.Save();
            playerPopup.ShowMenupopup();

            playerPopup.menuPopup.transform.Find("ConButton").GetComponent<Button>().gameObject.SetActive(false);
            playerPopup.menuPopup.transform.Find("SaveButton").GetComponent<Button>().gameObject.SetActive(false);

            AudioManager.Instance.PlaySFX("gameover");

            return;
        }
        GameManager.Instance.ChangeStamina(30);
        day++;
        hasGoneToTownToday = false;
        // Reset trạng thái cho ăn khi ngày mới bắt đầu
        hasFedCowsToday = false;
        hasFedChickensToday = false;

        // Cộng thêm 7 tiếng vào thời gian trong game, giữ nguyên phút
        TimeManager.Instance.gameHour += 7;
        if (TimeManager.Instance.gameHour >= 24)
        {
            TimeManager.Instance.gameHour -= 24; // Chuyển sang ngày mới
        }

        // Cập nhật thời gian sản xuất sữa và trứng
        float currentTime = TimeManager.Instance.GetGameTimeInSeconds();
        if (nextCowProduceTime <= currentTime)
        {
            nextCowProduceTime = currentTime + produceInterval;
        }
        if (nextChickenProduceTime <= currentTime)
        {
            nextChickenProduceTime = currentTime + produceInterval;
        }

        UIManager.Instance.UpdateAllUI();
        playerPopup.ShowPopupNoti("Một này mới đã bắt đầu. Hãy thức dậy và tiếp tục phát triển nông trại của bạn ^^");
    }

    public void GameOver(string reason)
    {
        Debug.Log("GAME OVER: " + reason);
        Time.timeScale = 0;
    }

    // Hàm cho ăn bò
    public bool FeedCows()
    {
        if (hasFedCowsToday)
        {
            Debug.Log("Bò đã được cho ăn hôm nay!");
            return false;
        }
        int staminaCost = cowCount; // 1 stamina/con
        if (stamina >= staminaCost)
        {
            ChangeStamina(-staminaCost);
            hasFedCowsToday = true;
            Debug.Log($"Cho ăn {cowCount} con bò, mất {staminaCost} stamina!");
            return true;
        }
        else
        {
            Debug.Log("Không đủ stamina để cho bò ăn!");
            return false;
        }
    }

    // Hàm cho ăn gà
    public bool FeedChickens()
    {
        if (hasFedChickensToday)
        {
            Debug.Log("Gà đã được cho ăn hôm nay!");
            return false;
        }
        int staminaCost = chickenCount; // 1 stamina/con
        if (stamina >= staminaCost)
        {
            ChangeStamina(-staminaCost);
            hasFedChickensToday = true;
            Debug.Log($"Cho ăn {chickenCount} con gà, mất {staminaCost} stamina!");
            return true;
        }
        else
        {
            Debug.Log("Không đủ stamina để cho gà ăn!");
            return false;
        }
    }
}