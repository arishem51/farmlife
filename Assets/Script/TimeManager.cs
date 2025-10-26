using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    public float realSecondsPerGameMinute = 0.4167f;
    private float timer;
    public int gameHour = 6;
    public int gameMinute = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= realSecondsPerGameMinute)
        {
            timer = 0;
            gameMinute++;
            if (gameMinute >= 60)
            {
                gameMinute = 0;
                gameHour++;
                GameManager.Instance.ChangeStamina(-1);
            }
            if (gameHour >= 24)
            {
                gameHour = 0;
            }
            UIManager.Instance.UpdateTimeUI();
        }
    }

    public float GetGameTimeInSeconds()
    {
        return (gameHour * 3600) + (gameMinute * 60);
    }
}