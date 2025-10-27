using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemButton : MonoBehaviour
{
    private string foodType;
    private int count;
    public void Setup(string type, int count, Sprite icon)
    {
        this.foodType = type;
        this.count = count;
        transform.Find("Background").GetComponent<Image>().enabled = true;
        transform.Find("Icon").GetComponent<Image>().sprite = icon;
        transform.Find("Icon").GetComponent<Image>().enabled = true;
        transform.Find("CountText").GetComponent<TextMeshProUGUI>().text = "x" + count;
        transform.Find("CountText").GetComponent<TextMeshProUGUI>().enabled = true;
        // Gắn sự kiện click
        GetComponent<Button>().onClick.AddListener(OnItemClicked);
    }

    private void OnItemClicked()
    {
        UIManager.Instance.ShowDetailPanel(foodType, count);
        AudioManager.Instance.PlaySFX("ui_click");
    }
}