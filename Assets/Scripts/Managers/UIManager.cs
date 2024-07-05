using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Drawing;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private bool isMenuOpen = false;

    [Header("Health UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpSliderText;

    [Header("Message UI")]
    [SerializeField] private int sameMessageCount = 0;
    [SerializeField] private string lastMessage;
    [SerializeField] private bool isMessageHistoryOpen = false;
    [SerializeField] private GameObject messageHistory;
    [SerializeField] private GameObject messageHistoryContent;
    [SerializeField] private GameObject lastFiveMessagesContent;

    [Header("Inventory UI")]
    [SerializeField] private bool isInventoryOpen = false;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject inventoryContent;

    [Header("Drop Menu UI")]
    [SerializeField] private bool isDropMenuOpen = false;
    [SerializeField] private GameObject dropMenu;
    [SerializeField] private GameObject dropMenuContent;

    public bool IsMessageHistoryOpen { get { return isMessageHistoryOpen; } }
    public bool IsMenuOpen { get { return isMenuOpen; } }
    public bool IsInventoryOpen { get { return isInventoryOpen; } }
    public bool IsDropMenuOpen { get { return isDropMenuOpen; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        AddMessage("Hello and welcome, adventurer, to yet another dungeon!", "light blue");
    }

    public void SetHealthMax (int maxHP)
    {
        hpSlider.maxValue = maxHP; 
    }

    public void SetHealth (int hp, int maxHP)
    {
        hpSlider.value = hp;
        hpSliderText.text = $"HP: {hp}/{maxHP}";
        UnityEngine.Color color;

        if (hp <= maxHP / 4)
            ColorUtility.TryParseHtmlString(ConvertColorNameToHex("red"), out color);
        else
            ColorUtility.TryParseHtmlString(ConvertColorNameToHex("white"), out color);

        hpSliderText.color = color;
    }

    public void ToggleMenu()
    {
        if (isMenuOpen)
        {
            isMenuOpen = !isMenuOpen;

            if (isMessageHistoryOpen)
                ToggleMessageHistory();

            if (isInventoryOpen)
                ToggleInventory();

            if (isDropMenuOpen)
                ToggleDropMenu();

            return;
        }
    }

    public void ToggleMessageHistory()
    {
        messageHistory.SetActive(!messageHistory.activeSelf);
        isMessageHistoryOpen = messageHistory.activeSelf;
    }

    public void ToggleInventory(Actor actor = null)
    {
        inventory.SetActive(!inventory.activeSelf);
        isMenuOpen = inventory.activeSelf;
        isInventoryOpen = inventory.activeSelf;

        if (isMenuOpen)
            UpdateMenu(actor, inventoryContent);
    }
    public void ToggleDropMenu(Actor actor = null)
    {
        dropMenu.SetActive(!dropMenu.activeSelf);
        isMenuOpen = dropMenu.activeSelf;
        isDropMenuOpen = dropMenu.activeSelf;

        if (isMenuOpen)
            UpdateMenu(actor, dropMenuContent);
    }

    public void AddMessage (string newMessage, string colorHex)
    {
        if (lastMessage == newMessage)
        {
            TextMeshProUGUI messageHistoryLastChild = messageHistoryContent.transform.GetChild(messageHistoryContent.transform.childCount - 1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lastFiveHistoryLastChild = lastFiveMessagesContent.transform.GetChild(lastFiveMessagesContent.transform.childCount - 1).GetComponent<TextMeshProUGUI>();
            ++sameMessageCount;
            messageHistoryLastChild.text = $"{newMessage} (x{sameMessageCount+1})";
            lastFiveHistoryLastChild.text = $"{newMessage} (x{sameMessageCount+1})";
            return;
        }
        else if (sameMessageCount > 0)
        {
            sameMessageCount = 0;
        }



        lastMessage = newMessage;

        TextMeshProUGUI messagePrefab = Instantiate(Resources.Load<TextMeshProUGUI>("Prefabs/Message")) as TextMeshProUGUI;
        messagePrefab.text = newMessage;
        messagePrefab.color = GetColorFromHex(colorHex);
        messagePrefab.transform.SetParent(messageHistoryContent.transform, false);

        for (int i = 0; i < lastFiveMessagesContent.transform.childCount; i++)
        {
            if (messageHistoryContent.transform.childCount - 1 < i)
                return;

            TextMeshProUGUI lastFiveHistoryChild = lastFiveMessagesContent.transform.GetChild(lastFiveMessagesContent.transform.childCount - 1 - i).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI messageHistoryChild = messageHistoryContent.transform.GetChild(messageHistoryContent.transform.childCount - 1 - i).GetComponent<TextMeshProUGUI>();
            lastFiveHistoryChild.text = messageHistoryChild.text;
            lastFiveHistoryChild.color = messageHistoryChild.color;
        }
    }

    private UnityEngine.Color GetColorFromHex (string v)
    {
        UnityEngine.Color color;

        if (ColorUtility.TryParseHtmlString(ConvertColorNameToHex(v), out color))
            return color;
        else
        {
            Debug.Log($"{v} is invalid.");
            return UnityEngine.Color.white;
        }
    }
    private string ConvertColorNameToHex(string wantedColor)
    {
        wantedColor = wantedColor.ToLowerInvariant();

        switch (wantedColor)
        {
            case "black":
                return "#000000";
            case "gray":
            case "grey":
                return "#808080";
            case "white":
                return "#FFFFFF";
            case "red":
                return "#FF0000";
            case "light red":
                return "#d1a3a4";
            case "dark red":
            case "crimson":
                return "#DC143C";
            case "green":
                return "#00FF00";
            case "dark green":
                return "#013220";
            case "lime":
                return "#00FFAE";
            case "light green":
                return "#00FF00";
            case "blue":
                return "#0000FF";
            case "yellow":
                return "#FFFF00";
            case "orange":
                return "#FFA500";
            case "purple":
                return "#800080";
            case "pink":
                return "#FFC0CB";
            case "brown":
                return "#A52A2A";
            case "aqua":
                return "#00FFFF";
            case "teal":
            case "light blue":
                return "#008080";
            case "dark blue":
            case "navy":
                return "#000080";
            case "maroon":
                return "#800000";
            case "olive":
                return "#808000";
            case "fuchsia":
                return "#FF00FF";
            case "gold":
                return "#FFD700";
            case "silver":
                return "#C0C0C0";
            case "indigo":
                return "#4B0082";
            case "violet":
                return "#EE82EE";
            case "turquoise":
                return "#40E0D0";
            default:
                {
                    Debug.Log($"{wantedColor} isn't a registered color.");
                    return "#FFFFFF";
                }
        }
    }

    private void UpdateMenu(Actor actor, GameObject menuContent)
    {
        for (int resetNum = 0; resetNum < menuContent.transform.childCount; resetNum++)
        {
            GameObject menuContentChild = menuContent.transform.GetChild(resetNum).gameObject;
            menuContentChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            menuContentChild.GetComponent<Button>().onClick.RemoveAllListeners();
            menuContentChild.SetActive(false);
        }

        char c = 'a';

        for (int itemNum = 0; itemNum < actor.Inventory.Items.Count; itemNum++)
        {
            GameObject menuContentChild = menuContent.transform.GetChild(itemNum).gameObject;
            Item item = actor.Inventory.Items[itemNum];
            menuContentChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"({c++}) {item.name}";
            menuContentChild.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (menuContent == inventoryContent)
                    Action.UseAction(actor, item);
                else if (menuContent == dropMenuContent)
                    Action.DropAction(actor, item);

                UpdateMenu(actor, menuContent);
            });
            menuContentChild.SetActive(true);
        }

        eventSystem.SetSelectedGameObject(menuContent.transform.GetChild(0).gameObject);
    }
}
