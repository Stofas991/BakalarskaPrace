using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EventManager : Singleton<EventManager>
{
    private List<IGameEvent> events = new List<IGameEvent>();
    private IGameEvent activeEvent;

    public List<GameObject> enemyUnitPrefabs = new List<GameObject>();
    public List<Tilemap> bannedTilemaps = new List<Tilemap>();
    public float timeBetweenEvents = 30f;
    private float timer = 0f;
    private bool isRunning = false;

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
            isRunning = activeEvent.UpdateEvent();
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
}
