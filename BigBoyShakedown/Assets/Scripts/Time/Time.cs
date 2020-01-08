﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Time : MonoBehaviour
{  
    private static bool isRunning;
    public static bool IsRunning { get => isRunning; private set => isRunning = value; }

    private static float timeEnlapsed;
    public static float TimeEnlapsed { get => timeEnlapsed; private set => timeEnlapsed = value; }

    public event Action GamePausedEvent;
    public event Action GameContinuedEvent;

    static Dictionary<VariableReference<bool>, float> startedTimers;

    private void Awake()
    {
        startedTimers = new Dictionary<VariableReference<bool>, float>();
    }
    void Update()
    {
        if (IsRunning)
        {
            TimeEnlapsed += UnityEngine.Time.deltaTime;

            CheckAllTimers();
        }
    }

    private void CheckAllTimers()
    {
        var keysToRemove = new List<VariableReference<bool>>();
        foreach(var keyValuePair in startedTimers)
        {
            if (keyValuePair.Value <= timeEnlapsed)
            {
                keysToRemove.Add(keyValuePair.Key);
                break;
            }
        }
        foreach (var key in keysToRemove)
        {
            key.Set(key.valueToSetToo);
            keysToRemove.Remove(key);
        }
    }

    //changes boolean to true after time has run out
    public static void StartTimer(VariableReference<bool> variableToChange, float timeToChange)
    {
        if (startedTimers.ContainsKey(variableToChange)) startedTimers.Remove(variableToChange);
        startedTimers.Add(variableToChange, timeEnlapsed + timeToChange);
    }


    private void OnEnable()
    {
        IsRunning = true;
        GamePausedEvent += GamePausedEventHandler;
        GameContinuedEvent += GameContinuedEventHandler;
    }
    private void OnDisable()
    {
        IsRunning = false;
        GamePausedEvent -= GamePausedEventHandler;
        GameContinuedEvent -= GameContinuedEventHandler;
    }

    private void GamePausedEventHandler()
    {
        IsRunning = false;
    }
    private void GameContinuedEventHandler()
    {
        IsRunning = true;
    }

    public static void PauseGame()
    {
        FindObjectOfType<Time>().GamePausedEvent();
    }
    public static void ContinueGame()
    {
        FindObjectOfType<Time>().GameContinuedEvent();
    }
}

public class VariableReference<T>
{
    public Func<T> Get { get; private set; }
    public Action<T> Set { get; private set; }

    public T valueToSetToo;

    public VariableReference(Func<T> getter, Action<T> setter)
    {
        Get = getter;
        Set = setter;
    }
    public VariableReference<T> SetEndValue(T valueToSetToo)
    {
        this.valueToSetToo = valueToSetToo;
        return this;
    }
}
