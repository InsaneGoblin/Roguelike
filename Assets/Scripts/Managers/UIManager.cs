using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

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

    public bool IsMessageHistoryOpen { get { return isMessageHistoryOpen; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        AddMessage("Hello and welcome, adventurer, to yet another dungeon!", "#0da2ff"); // Light blue
    }

    public void SetHealthMax (int maxHP)
    {
        hpSlider.value = maxHP; 
    }

    public void SetHealth (int hp, int maxHP)
    {
        hpSlider.value = hp;
        hpSliderText.text = $"HP: {hp}/{maxHP}";
    }

    public void ToggleMessageHistory()
    {
        messageHistory.SetActive(!messageHistory.activeSelf);
        isMessageHistoryOpen = messageHistory.activeSelf;
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

    private Color GetColorFromHex (string v)
    {
        Color color;

        if (ColorUtility.TryParseHtmlString(v, out color))
            return color;
        else
        {
            Debug.Log($"{v} is invalid.");
            return Color.white;
        }
    }
}
