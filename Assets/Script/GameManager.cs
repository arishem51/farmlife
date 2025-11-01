using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int gold = 200;
    public int stamina = 100;
    public int day = 1;

    public bool hasGoneToTownToday = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UIManager.Instance.UpdateAllUI();
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
        UIManager.Instance.UpdateStaminaUI();

        if (stamina == 0)
        {
            if (gold >= 100)
            {
                gold -= 100;
                stamina = 30;
                Debug.Log("Bạn kiệt sức, mất 100 vàng để chữa trị");
            }
            else
            {
                GameOver("Kiệt sức mà không đủ 100 vàng để chữa bệnh!");
            }
        }
    }

    public void EndDay()
    {
        gold -= 20; // phí bảo vệ
        if (gold <= 0)
        {
            GameOver("Bạn phá sản!");
            return;
        }

        day++;
        hasGoneToTownToday = false;
        stamina = Mathf.Min(stamina + 30, 100);

        UIManager.Instance.UpdateAllUI();
        Debug.Log("Ngày mới bắt đầu!");
    }

    public void GameOver(string reason)
    {
        Debug.Log("GAME OVER: " + reason);
        Time.timeScale = 0;
    }
}
