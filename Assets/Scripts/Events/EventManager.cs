/**
 * File: EventManager.cs
 * Author: Kryštof Glos
 * Last Modified: 2.5.2024
 * Description: Singleton class responsible for managing game events, including event selection, initialization, and handling player input.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class EventManager : Singleton<EventManager>
{
    private List<IGameEvent> events = new List<IGameEvent>();
    private IGameEvent activeEvent;

    public GameObject popupMenu;
    public List<GameObject> enemyUnitPrefabs = new List<GameObject>();
    public List<Tilemap> bannedTilemaps = new List<Tilemap>();
    public float timeBetweenEvents = 60f;
    private float timer = 0f;
    private bool isRunning = false;
    public TextMeshProUGUI textBox;
    public TextMeshProUGUI acceptText;
    public TextMeshProUGUI denieText;
    public GameObject popupAccept, popupDenie;
    private bool playerInputReceived;
    public int playerUnitCount;
    public GameObject blackoutPanel;
    private bool ended = false;
    private int mapHeight, mapWidth;
    private float squareSize = 5f;
    [SerializeField] List<GameObject> playerUnitsToSpawn = new List<GameObject>();

    void Start()
    {
        mapHeight = MapGenerator.GetInstance().mapHeight;
        mapWidth = MapGenerator.GetInstance().mapWidth;
        if (SelectedValues.isSet)
        {
            mapHeight = SelectedValues.mapSize;
            mapWidth = SelectedValues.mapSize;
            playerUnitsToSpawn = SelectedValues.playerPrefabList;
        }

        SpawnPlayerUnits();
        // Inicializace
        LoadEvents();
        InitializeEvents();
        playerUnitCount = GameObject.FindGameObjectsWithTag("selectable").Length;
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
        if (IsGameOver())
        {
            ShowGameOverPopup();
        }
    }

    private void SpawnPlayerUnits()
    {
        Vector3 centerPosition = CalculateSpawnPosition();

        foreach (var playerUnitPrefab in playerUnitsToSpawn)
        {
            Vector3 spawnPosition = GetRandomPositionInsideSquare(centerPosition, squareSize);
            Instantiate(playerUnitPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 CalculateSpawnPosition()
    {
        Vector3 centerOfMap = new Vector3(mapWidth / 2, mapHeight / 2, 0);
        centerOfMap.z = 0;

        return centerOfMap;
    }

    private Vector3 GetRandomPositionInsideSquare(Vector3 centerPosition, float size)
    {
        // Vypoèítání náhodné pozice uvnitø ètverce
        float halfSize = size / 2f;
        float randomX = UnityEngine.Random.Range(-halfSize, halfSize);
        float randomZ = UnityEngine.Random.Range(-halfSize, halfSize);

        return centerPosition + new Vector3(randomX, 0, randomZ);
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

    private bool IsGameOver()
    {
        if (playerUnitCount == 0 && !ended)
        {
            ended = true;
            return true;
        }
        return false;
    }

    private void ShowGameOverPopup()
    {
        Time.timeScale = 0;

        popupMenu.SetActive(true);
        textBox.text = "Game Over";
        textBox.fontSize = 20;
        textBox.alignment = TextAlignmentOptions.Center;

        popupDenie.SetActive(false);

        popupAccept.GetComponentInChildren<TextMeshProUGUI>().text = "Continue to main menu";
        popupAccept.GetComponent<Button>().onClick.RemoveAllListeners();
        popupAccept.GetComponent<Button>().onClick.AddListener(ReturnToMainMenu);

        blackoutPanel.SetActive(true);
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        popupMenu.SetActive(false);
        blackoutPanel.SetActive(false);

        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

}
