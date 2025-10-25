using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main UI")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI staminaText;

    [Header("Inventory UI")]
    public GameObject inventoryPanel;
    public GameObject darkPanel;
    public Button openInventoryButton;
    public Button closeInventoryButton;
    public Transform inventoryContent;
    public GameObject inventoryItemPrefab;

    [Header("Detail Panel")]
    public GameObject detailPanel;
    private string currentFoodType;
    private bool isUpdatingInput;

    public CowArea cowArea;
    public ChickenCoop chickenCoop;
    public int stateInventory = 1;// 1= inven khi ở nhà, 2 inven khi ở shop, 3 inven khi gặp thương gia siêu giàu(sk đb)
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateAllUI();
        SetupInventoryButtons();
        SetupDetailPanelButtons();
        stateInventory=1;
    }

    private void SetupInventoryButtons()
    {
        if (openInventoryButton != null)
        {
            openInventoryButton.onClick.AddListener(OpenInventory);

        }
        if (closeInventoryButton != null)
        {
            closeInventoryButton.onClick.AddListener(CloseInventory);
        }
    }

    private void SetupDetailPanelButtons()
    {
        if (detailPanel != null)
        {
            Button sellButton = detailPanel.transform.Find("SellButton").GetComponent<Button>();
            Button buyButton = detailPanel.transform.Find("BuyButton").GetComponent<Button>();
            Button eatButton = detailPanel.transform.Find("EatButton").GetComponent<Button>();
            TMP_InputField inputField = detailPanel.transform.Find("SellInputField").GetComponent<TMP_InputField>();
            
            if (sellButton != null && inputField != null)
            {
                sellButton.onClick.AddListener(() =>
                {
                    if (!string.IsNullOrEmpty(currentFoodType) && int.TryParse(inputField.text, out int amount))
                    {
                        amount = Mathf.Clamp(amount, 0, GetFoodCount(currentFoodType));
                        GameManager.Instance.ChangeFoodCount(currentFoodType, -amount);
                        if (stateInventory == 2)
                        {
                            switch (currentFoodType.ToLower())
                            {
                                case "pumpkin":
                                    GameManager.Instance.ChangeGold(9 * amount);
                                    break;
                                case "corn":
                                    GameManager.Instance.ChangeGold(7 * amount);
                                    break;
                                case "carrot":
                                    GameManager.Instance.ChangeGold(5 * amount);
                                    break;
                                case "watermelon":
                                    GameManager.Instance.ChangeGold(10 * amount);
                                    break;
                                case "milk":
                                    GameManager.Instance.ChangeGold(15 * amount); // Giá sữa
                                    break;
                                case "egg":
                                    GameManager.Instance.ChangeGold(3 * amount); // Giá trứng
                                    break;
                                case "cow":
                                    GameManager.Instance.ChangeGold(100 * amount);
                                    if (cowArea != null) // Gọi SpawnCows trên instance CowArea trong scene
                                    {
                                        cowArea.SpawnCows();
                                    }
                                    break;
                                case "chicken":
                                    GameManager.Instance.ChangeGold(20 * amount);
                                    if (chickenCoop != null)
                                    {
                                        chickenCoop.SpawnChickens();
                                    }
                                    break;
                            }
                        }else if (stateInventory == 3)
                        {
                            switch (currentFoodType.ToLower())
                            {
                                case "pumpkin":
                                    GameManager.Instance.ChangeGold(13 * amount);
                                    break;
                                case "corn":
                                    GameManager.Instance.ChangeGold(10 * amount);
                                    break;
                                case "carrot":
                                    GameManager.Instance.ChangeGold(7 * amount);
                                    break;
                                case "watermelon":
                                    GameManager.Instance.ChangeGold(14 * amount);
                                    break;
                                case "milk":
                                    GameManager.Instance.ChangeGold(22 * amount); // Giá sữa
                                    break;
                                case "egg":
                                    GameManager.Instance.ChangeGold(4 * amount); // Giá trứng
                                    break;
                                case "cow":
                                    GameManager.Instance.ChangeGold(150 * amount);
                                    if (cowArea != null) // Gọi SpawnCows trên instance CowArea trong scene
                                    {
                                        cowArea.SpawnCows();
                                    }
                                    break;
                                case "chicken":
                                    GameManager.Instance.ChangeGold(30 * amount);
                                    if (chickenCoop != null)
                                    {
                                        chickenCoop.SpawnChickens();
                                    }
                                    break;
                            }
                        }
                        inputField.SetTextWithoutNotify("");
                        RefreshInventoryUI();
                        CloseDetailPanel();
                    }
                });

                inputField.onValueChanged.AddListener((value) =>
                {
                    if (isUpdatingInput) return;
                    isUpdatingInput = true;
                    if (int.TryParse(value, out int amount))
                    {
                        if (amount <= 0)
                        {
                            inputField.SetTextWithoutNotify("1");
                        }
                        else if (amount > GetFoodCount(currentFoodType))
                        {
                            inputField.SetTextWithoutNotify(GetFoodCount(currentFoodType).ToString());
                        }
                    }
                    else
                    {
                        inputField.SetTextWithoutNotify("1");
                    }
                    isUpdatingInput = false;
                });
            }
            if (buyButton != null && inputField != null)
            {
                buyButton.onClick.AddListener(() =>
                {
                    if (!string.IsNullOrEmpty(currentFoodType) && int.TryParse(inputField.text, out int amount))
                    {
                        amount = Mathf.Clamp(amount, 0, 100);
                        GameManager.Instance.ChangeFoodCount(currentFoodType, amount);
                        switch (currentFoodType.ToLower())
                        {
                            case "pumpkin":
                                GameManager.Instance.ChangeGold(-11 * amount);
                                break;
                            case "corn":
                                GameManager.Instance.ChangeGold(-9 * amount);
                                break;
                            case "carrot":
                                GameManager.Instance.ChangeGold(-7 * amount);
                                break;
                            case "watermelon":
                                GameManager.Instance.ChangeGold(-12 * amount);
                                break;
                            case "milk":
                                GameManager.Instance.ChangeGold(-17 * amount); // Giá sữa
                                break;
                            case "egg":
                                GameManager.Instance.ChangeGold(-5 * amount); // Giá trứng
                                break;
                            case "cow":
                                GameManager.Instance.ChangeGold(-120 * amount);
                                if (cowArea != null) // Gọi SpawnCows trên instance CowArea trong scene
                                {
                                    cowArea.SpawnCows();
                                }
                                break;
                            case "chicken":
                                GameManager.Instance.ChangeGold(-30 * amount);
                                if (chickenCoop != null)
                                {
                                    chickenCoop.SpawnChickens();
                                }
                                break;
                        }
                        inputField.SetTextWithoutNotify("");
                        RefreshInventoryUI();
                        CloseDetailPanel();
                    }
                });

                inputField.onValueChanged.AddListener((value) =>
                {
                    if (isUpdatingInput) return;
                    isUpdatingInput = true;
                    if (int.TryParse(value, out int amount))
                    {
                        if (amount <= 0)
                        {
                            inputField.SetTextWithoutNotify("1");
                        }
                        
                    }
                    else
                    {
                        inputField.SetTextWithoutNotify("1");
                    }
                    isUpdatingInput = false;
                });
            }
            if (eatButton != null && inputField != null)
            {
                eatButton.onClick.AddListener(() =>
                {
                    if (!string.IsNullOrEmpty(currentFoodType) && int.TryParse(inputField.text, out int amount))
                    {
                        amount = Mathf.Clamp(amount, 0, GetFoodCount(currentFoodType));
                        GameManager.Instance.ChangeFoodCount(currentFoodType, -amount);
                        switch (currentFoodType.ToLower())
                        {
                            case "pumpkin":
                                GameManager.Instance.ChangeStamina(9 * amount);
                                break;
                            case "corn":
                                GameManager.Instance.ChangeStamina(7 * amount);
                                break;
                            case "carrot":
                                GameManager.Instance.ChangeStamina(5 * amount);
                                break;
                            case "watermelon":
                                GameManager.Instance.ChangeStamina(10 * amount);
                                break;
                            case "milk":
                                GameManager.Instance.ChangeStamina(12 * amount); // Thể lực từ sữa
                                break;
                            case "egg":
                                GameManager.Instance.ChangeStamina(4 * amount); // Thể lực từ trứng
                                break;
                        }
                        inputField.SetTextWithoutNotify("");
                        RefreshInventoryUI();
                        CloseDetailPanel();
                    }
                });

                inputField.onValueChanged.AddListener((value) =>
                {
                    if (isUpdatingInput) return;
                    isUpdatingInput = true;
                    if (int.TryParse(value, out int amount))
                    {
                        if (amount <= 0)
                        {
                            inputField.SetTextWithoutNotify("1");
                        }
                        else if (amount > GetFoodCount(currentFoodType))
                        {
                            inputField.SetTextWithoutNotify(GetFoodCount(currentFoodType).ToString());
                        }
                    }
                    else
                    {
                        inputField.SetTextWithoutNotify("1");
                    }
                    isUpdatingInput = false;
                });
            }

        }
    }

    public void OpenInventory()
    {
        if (inventoryPanel != null && darkPanel != null)
        {
            AudioManager.Instance.PlaySFX("ui_click");
            inventoryPanel.SetActive(true);
            darkPanel.SetActive(true);
            RefreshInventoryUI();
        }
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null && darkPanel != null)
        {
            AudioManager.Instance.PlaySFX("ui_click");
            inventoryPanel.SetActive(false);
            darkPanel.SetActive(false);
            CloseDetailPanel();
        }
    }

    public void RefreshInventoryUI()
    {
        if (inventoryContent == null) return;
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }
        //if (GameManager.Instance.pumpkinCount < 0)
            CreateInventoryButton("Pumpkin", GameManager.Instance.pumpkinCount, 0);
        //if (GameManager.Instance.cornCount > 0)
            CreateInventoryButton("Corn", GameManager.Instance.cornCount, 1);
        //if (GameManager.Instance.carrotCount > 0)
            CreateInventoryButton("Carrot", GameManager.Instance.carrotCount, 2);
        //if (GameManager.Instance.watermelonCount > 0)
            CreateInventoryButton("Watermelon", GameManager.Instance.watermelonCount, 3);
        //if (GameManager.Instance.milkCount > 0)
            CreateInventoryButton("Milk", GameManager.Instance.milkCount, 4);
        //if (GameManager.Instance.eggCount > 0)
            CreateInventoryButton("Egg", GameManager.Instance.eggCount, 5);
        //if (GameManager.Instance.cowCount > 0)
            CreateInventoryButton("Cow", GameManager.Instance.cowCount, 6);
        //if (GameManager.Instance.chickenCount > 0)
            CreateInventoryButton("Chicken", GameManager.Instance.chickenCount, 7);
    }

    private void CreateInventoryButton(string name, int count, int iconID)
    {
        if (inventoryItemPrefab == null) return;
        GameObject item = Instantiate(inventoryItemPrefab, inventoryContent);
        InventoryItemButton button = item.GetComponent<InventoryItemButton>();
        if (button != null)
            button.Setup(name, count, GetFoodIcon(iconID));
    }

    private Sprite GetFoodIcon(int iconID)
    {
        switch (iconID)
        {
            case 0: return Resources.Load<Sprite>("Icons/Pumpkin");
            case 1: return Resources.Load<Sprite>("Icons/Corn");
            case 2: return Resources.Load<Sprite>("Icons/Carrot");
            case 3: return Resources.Load<Sprite>("Icons/Watermelon");
            case 4: return Resources.Load<Sprite>("Icons/Milk");
            case 5: return Resources.Load<Sprite>("Icons/Egg");
            case 6: return Resources.Load<Sprite>("Icons/Cow");
            case 7: return Resources.Load<Sprite>("Icons/Chicken");

            default: return null;
        }
    }

    public void ShowDetailPanel(string foodType, int count)
    {
        if (detailPanel == null) return;
        Button eatButton = detailPanel.transform.Find("EatButton").GetComponent<Button>();
        Button buyButton = detailPanel.transform.Find("BuyButton").GetComponent<Button>();
        Button sellButton = detailPanel.transform.Find("SellButton").GetComponent<Button>();
        currentFoodType = foodType;
        detailPanel.SetActive(true);
        detailPanel.transform.Find("FoodImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + foodType);
        if (stateInventory == 1 || stateInventory == 2)
        {
            switch (foodType.ToLower())
            {

                case "pumpkin":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Bí ngô";
                    detailPanel.transform.Find("CostBuy").GetComponent<TextMeshProUGUI>().text = "Giá mua: 11 vàng";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 9 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "Thể lực phục hồi: 9";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(true);
                    else if (stateInventory == 2)
                    {
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "corn":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Ngô";
                    detailPanel.transform.Find("CostBuy").GetComponent<TextMeshProUGUI>().text = "Giá mua: 9 vàng";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 7 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "Thể lực phục hồi: 7";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(true);
                    else if (stateInventory == 2)
                    {
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "carrot":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Cà rốt";
                    detailPanel.transform.Find("CostBuy").GetComponent<TextMeshProUGUI>().text = "Giá mua: 7 vàng";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 5 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "Thể lực phục hồi: 5";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(true);
                    else if (stateInventory == 2)
                    {
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "watermelon":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Dưa hấu";
                    detailPanel.transform.Find("CostBuy").GetComponent<TextMeshProUGUI>().text = "Giá mua: 12 vàng";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 10 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "Thể lực phục hồi: 10";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(true);
                    else if (stateInventory == 2)
                    {
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "milk":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Sữa";
                    detailPanel.transform.Find("CostBuy").GetComponent<TextMeshProUGUI>().text = "Giá mua: 17 vàng";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 15 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "Thể lực phục hồi: 12";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(true);
                    else if (stateInventory == 2)
                    {
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "egg":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Trứng";
                    detailPanel.transform.Find("CostBuy").GetComponent<TextMeshProUGUI>().text = "Giá mua: 5 vàng";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 3 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "Thể lực phục hồi: 4";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(true);
                    else if (stateInventory == 2)
                    {
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "cow":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Bò";
                    detailPanel.transform.Find("CostBuy").GetComponent<TextMeshProUGUI>().text = "Giá mua: 120 vàng";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 100 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(false);
                    else if (stateInventory == 2)
                    {
                        eatButton.gameObject.SetActive(false);
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {eatButton.gameObject.SetActive(false);
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "chicken":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Gà";
                    detailPanel.transform.Find("CostBuy").GetComponent<TextMeshProUGUI>().text = "Giá mua: 30 vàng";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 20 vàng";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(false);
                    else if (stateInventory == 2)
                    {eatButton.gameObject.SetActive(false);
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {eatButton.gameObject.SetActive(false);
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
            }
        }
        else
        {
            switch (foodType.ToLower())
            {

                case "pumpkin":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Bí ngô";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 13 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "Thể lực phục hồi: 9";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(true);
                    else if (stateInventory == 2)
                    {
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "corn":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Ngô";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 10 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "Thể lực phục hồi: 7";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(true);
                    else if (stateInventory == 2)
                    {
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "carrot":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Cà rốt";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 7 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "Thể lực phục hồi: 5";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(true);
                    else if (stateInventory == 2)
                    {
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "watermelon":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Dưa hấu";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 14 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "Thể lực phục hồi: 10";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(true);
                    else if (stateInventory == 2)
                    {
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "milk":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Sữa";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 22 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "Thể lực phục hồi: 12";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(true);
                    else if (stateInventory == 2)
                    {
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "egg":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Trứng";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 4 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "Thể lực phục hồi: 4";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(true);
                    else if (stateInventory == 2)
                    {
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "cow":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Bò";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 150 vàng";
                    detailPanel.transform.Find("Stamina").GetComponent<TextMeshProUGUI>().text = "";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(false);
                    else if (stateInventory == 2)
                    {
                        eatButton.gameObject.SetActive(false);
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        eatButton.gameObject.SetActive(false);
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
                case "chicken":
                    detailPanel.transform.Find("FoodNameText").GetComponent<TextMeshProUGUI>().text = "Gà";
                    detailPanel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Giá bán: 30 vàng";
                    if (stateInventory == 1)
                        eatButton.gameObject.SetActive(false);
                    else if (stateInventory == 2)
                    {
                        eatButton.gameObject.SetActive(false);
                        buyButton.gameObject.SetActive(true);
                        sellButton.gameObject.SetActive(true);
                    }
                    else if (stateInventory == 3)
                    {
                        eatButton.gameObject.SetActive(false);
                        buyButton.gameObject.SetActive(false);
                        sellButton.gameObject.SetActive(true);
                    }
                    break;
            }
        }
        detailPanel.transform.Find("FoodCountText").GetComponent<TextMeshProUGUI>().text = $"SL: {count}";
        detailPanel.transform.Find("SellInputField").GetComponent<TMP_InputField>().SetTextWithoutNotify("1");
    }

    public void CloseDetailPanel()
    {
        if (detailPanel != null)
        {
            detailPanel.SetActive(false);
            currentFoodType = null;
        }
    }

    private int GetFoodCount(string foodType)
    {
        switch (foodType.ToLower())
        {
            case "pumpkin": return GameManager.Instance.pumpkinCount;
            case "corn": return GameManager.Instance.cornCount;
            case "carrot": return GameManager.Instance.carrotCount;
            case "watermelon": return GameManager.Instance.watermelonCount;
            case "milk": return GameManager.Instance.milkCount;
            case "egg": return GameManager.Instance.eggCount;
            case "cow": return GameManager.Instance.cowCount;
            case "chicken": return GameManager.Instance.chickenCount;

            default: return 0;
        }
    }

    public void UpdateAllUI()
    {
        UpdateDayUI();
        UpdateTimeUI();
        UpdateGoldUI();
        UpdateStaminaUI();
        UpdatePumpkinUI();
        UpdateCornUI();
        UpdateCarrotUI();
        UpdateWatermelonUI();
        UpdateMilkUI();
        UpdateEggUI();
    }

    public void UpdateDayUI()
    {
        if (dayText != null)
            dayText.text = $"{GameManager.Instance.day}";
    }

    public void UpdateTimeUI()
    {
        if (timeText != null)
            timeText.text = $"{TimeManager.Instance.gameHour:00}:{TimeManager.Instance.gameMinute:00}";
    }

    public void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = $" {GameManager.Instance.gold}";
    }

    public void UpdateStaminaUI()
    {
        if (staminaText != null)
            staminaText.text = $" {GameManager.Instance.stamina}";
    }

    public void UpdatePumpkinUI()
    {
        if (inventoryPanel != null && inventoryPanel.activeInHierarchy)
            RefreshInventoryUI();
    }

    public void UpdateCornUI()
    {
        if (inventoryPanel != null && inventoryPanel.activeInHierarchy)
            RefreshInventoryUI();
    }

    public void UpdateCarrotUI()
    {
        if (inventoryPanel != null && inventoryPanel.activeInHierarchy)
            RefreshInventoryUI();
    }

    public void UpdateWatermelonUI()
    {
        if (inventoryPanel != null && inventoryPanel.activeInHierarchy)
            RefreshInventoryUI();
    }

    public void UpdateMilkUI()
    {
        if (inventoryPanel != null && inventoryPanel.activeInHierarchy)
            RefreshInventoryUI();
    }

    public void UpdateEggUI()
    {
        if (inventoryPanel != null && inventoryPanel.activeInHierarchy)
            RefreshInventoryUI();
    }
    public void UpdateCowUI()
    {
        if (inventoryPanel != null && inventoryPanel.activeInHierarchy)
            RefreshInventoryUI();
    }
    public void UpdateChickenUI()
    {
        if (inventoryPanel != null && inventoryPanel.activeInHierarchy)
            RefreshInventoryUI();
    }
}