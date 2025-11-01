using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI staminaText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateAllUI()
    {
        UpdateDayUI();
        UpdateTimeUI();
        UpdateGoldUI();
        UpdateStaminaUI();
    }

    public void UpdateDayUI()
    {
        dayText.text = $"Ngày: {GameManager.Instance.day}";
    }

    public void UpdateTimeUI()
    {
        timeText.text = $"{TimeManager.Instance.gameHour:00}:{TimeManager.Instance.gameMinute:00}";
    }

    public void UpdateGoldUI()
    {
        goldText.text = $"Vàng: {GameManager.Instance.gold}";
    }

    public void UpdateStaminaUI()
    {
        staminaText.text = $"Thể lực: {GameManager.Instance.stamina}";
    }
}