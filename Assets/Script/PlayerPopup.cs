using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerPopup : MonoBehaviour
{
    public GameObject popupObject; // GameObject chứa SpriteRenderer (nền)
    public TextMeshPro popupText; // TextMeshPro cho nội dung
    public GameObject popupNoti;
    public TextMeshProUGUI popupNotiText;
    public GameObject darkPanel;
    public GameObject menuPopup;
    public TextMeshProUGUI recordText;
    public GameObject canvasHome;
    public GameObject canvasLanding;
    public GameObject landing;
    private SaveSystem saveSystem;
    private void Start()
    {
        // Ẩn popup khi khởi tạo
        HidePopup();
        HidePopupNoti();
        ShowLanding();
        saveSystem = GetComponent<SaveSystem>();
        if (saveSystem == null)
        {
            Debug.Log("Can not found save system");
        }
    }

    public void ShowPopup(string message)
    {
        if (popupObject != null && popupText != null)
        {
            popupText.text = message;
            popupObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PopupObject hoặc PopupText chưa được gán!");
        }
    }

    public void HidePopup()
    {
        if (popupObject != null)
        {
            popupObject.SetActive(false);
            if (popupText != null)
                popupText.text = "";
        }
    }
    public void ShowPopupNoti(string message)
    {
        if (popupNoti != null && popupNotiText != null)
        {
            darkPanel.SetActive(true);
            popupNotiText.text = message;
            popupNoti.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PopupObject hoặc PopupText chưa được gán!");
        }
    }

    public void HidePopupNoti()
    {
        if (popupNoti != null)
        {
            darkPanel.SetActive(false);
            popupNoti.SetActive(false);
            if (popupNotiText != null)
                popupText.text = "";
        }
    }
    public void ShowMenupopup()
    {
        if (menuPopup != null )
        {
            AudioManager.Instance.PlaySFX("ui_click");
            darkPanel.SetActive(true);
            menuPopup.SetActive(true);
            Time.timeScale = 0f;
            if(GameManager.Instance.gold<=0)
            {
                menuPopup.transform.Find("HeaderText").GetComponent<TextMeshProUGUI>().text = "Game Over";
            }
            else
            {
                menuPopup.transform.Find("HeaderText").GetComponent<TextMeshProUGUI>().text = "Menu";
            }
            if (recordText != null)
            {
                RecordData record = new SaveSystem().LoadRecordData();
                recordText.text = $"Ngày {record.maxDay}, {record.maxHour:00}:{record.maxMinute:00}";
            }
        }
        else
        {
            Debug.LogWarning("PopupObject hoặc PopupText chưa được gán!");
        }
    }
    public void ShowLanding()
    {
        if (menuPopup != null)
        {
            canvasLanding.SetActive(true);
            canvasHome.SetActive(false);
            landing.SetActive(true);
            RecordData record = new SaveSystem().LoadRecordData();
            TextMeshProUGUI RCText = canvasLanding.transform.Find("RCText").GetComponent<TextMeshProUGUI>();
            RCText.text = $"Ngày {record.maxDay}, {record.maxHour:00}:{record.maxMinute:00}";

        }
        else
        {
            Debug.LogWarning("PopupObject hoặc PopupText chưa được gán!");
        }
    }
    public void Continue()
    {
        if (menuPopup != null)
        {
            canvasHome.SetActive(true);
            canvasLanding.SetActive(false);
            landing.SetActive(false);
            darkPanel.SetActive(false);
            menuPopup.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            Debug.LogWarning("PopupObject hoặc PopupText chưa được gán!");
        }
    }
    public void ContinueGameSave()
    {
        if (menuPopup != null)
        {
            canvasHome.SetActive(true);
            canvasLanding.SetActive(false);
            landing.SetActive(false);
            darkPanel.SetActive(false);
            menuPopup.SetActive(false);
            Time.timeScale = 1f;
            saveSystem.Load();
        }
        else
        {
            Debug.LogWarning("PopupObject hoặc PopupText chưa được gán!");
        }
    }
    public void playAgain()
    {
        canvasHome.SetActive(true);
        canvasLanding.SetActive(false);
        landing.SetActive(false);
        darkPanel.SetActive(false);
        menuPopup.SetActive(false);
        Time.timeScale = 1f;
        saveSystem.PlayAgain();
    }
}