using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class EventManager : Singleton<EventManager>
{
    private List<IGameEvent> events = new List<IGameEvent>();
    private IGameEvent activeEvent;

    public GameObject popupMenu;
    public List<GameObject> enemyUnitPrefabs = new List<GameObject>();
    public List<Tilemap> bannedTilemaps = new List<Tilemap>();
    public float timeBetweenEvents = 30f;
    private float timer = 0f;
    private bool isRunning = false;
    public TextMeshProUGUI textBox;
    public TextMeshProUGUI acceptText;
    public TextMeshProUGUI denieText;
    public GameObject popupAccept, popupDenie;
    private bool playerInputReceived;

    void Start()
    {
        // Inicializace
        LoadEvents();
        InitializeEvents();
    }

    void Update()
    {
        if (!isRunning)
        {
            timer += Time.deltaTime;
            if (timer >= timeBetweenEvents)
            {
                SelectRandomEvent();
                timer = 0f;
            }
        }
        else
        {
            isRunning = activeEvent.UpdateEvent();
            if (!isRunning)
            {
                var windowInfo = activeEvent.EndEvent();
                SetWindowProperties(windowInfo);
                ActivateWindow();
            }
        }
    }

    //Loading all events using reflection by type
    void LoadEvents()
    {
        var eventTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => type.GetCustomAttributes(typeof(GameEventAttribute), true).Length > 0);

        foreach (var eventType in eventTypes)
        {
            var eventInstance = Activator.CreateInstance(eventType) as IGameEvent;
            if (eventInstance != null)
            {
                events.Add(eventInstance);
            }
        }
    }

    void SelectRandomEvent()
    {
        // Vybrání náhodného eventu ze seznamu
        if (events.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, events.Count);
            activeEvent = events[randomIndex];
            isRunning = true;

            var windowInfo = activeEvent.GetPopupWindowInfo();
            SetWindowProperties(windowInfo);
            ActivateWindow();

            activeEvent.StartEvent();
        }
    }

    void InitializeEvents()
    {
        foreach (var value in events)
        {
            value.InitEvent();
        }
    }

    public List<GameObject> GetEnemyPrefabList()
    {
        return enemyUnitPrefabs;
    }

    public List<Tilemap> GetBannedTilemaps()
    {
        return bannedTilemaps;
    }

    private void ActivateWindow()
    {
        Time.timeScale = 0;

        popupMenu.SetActive(true);

        StartCoroutine(WaitForPlayerInput());
    }

    IEnumerator WaitForPlayerInput()
    {
        while (!playerInputReceived)
        {
            // Poèkej na další snímek
            yield return null;
        }

        // Obnova hry po hráèovì vstupu
        Time.timeScale = 1;
    }

    public void OnAcceptClick()
    {
        playerInputReceived = true;
        popupMenu.SetActive(false);
    }

    private void SetWindowProperties(PopupWindowInfo windowInfo)
    {
        acceptText.text = windowInfo.acceptContent;
        denieText.text = windowInfo.deniedContent;
        textBox.text = windowInfo.textBoxContent;

        if (!windowInfo.isDenieable)
            popupDenie.SetActive(false);
        else
            popupDenie.SetActive(true);
    }

}
