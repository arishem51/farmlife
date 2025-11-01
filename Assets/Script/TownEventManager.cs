using UnityEngine;

public class TownEventManager : MonoBehaviour
{
    public static TownEventManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerTownEvent()
    {
        if (GameManager.Instance.hasGoneToTownToday)
        {
            Debug.Log("Hôm nay bạn đã đi thị trấn rồi!");
            return;
        }

        GameManager.Instance.hasGoneToTownToday = true;

        int r = Random.Range(0, 100);

        if (r < 26)
        {
            GameManager.Instance.ChangeGold(Random.Range(10, 50));
            Debug.Log("Sự kiện tốt: bạn được thương gia tặng vàng!");
        }
        else if (r < 52)
        {
            Debug.Log("Bạn bị quái vật tấn công, mất một số hàng hóa!");
        }
        else if (r < 78)
        {
            GameManager.Instance.ChangeGold(-Random.Range(10, 50));
            Debug.Log("Sự kiện xấu: bạn bị lừa mất vàng!");
        }
        else if (r < 90)
        {
            Debug.Log("Không có gì xảy ra trên đường đi.");
        }
        else
        {
            Debug.Log("Sự kiện hiếm: thương gia siêu giàu trả x1.5 giá!");
            // TODO: mở UI giao dịch thương nhân giàu
        }
    }
}
