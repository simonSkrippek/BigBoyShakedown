using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BigBoyShakedown.Game.Event
{
    public class EventSpawner : MonoBehaviour
    {
        bool active;

        [SerializeField] GameObject[] eventPrefabs;
        [Header("Timings")]
        [SerializeField] float initialEventDelay = 0f;
        [SerializeField] float timeBetweenEvents = 2f;
        [SerializeField] float timeBetweenEventChances = 1f;
        [Header("Chance"), SerializeField, Range(0f, 1f)] float eventSpawnChance = .5f;
       

        private void Start()
        {
            Time.StartTimer(new VariableReference<bool>(() => (false), (val) => { RollEventChance(); }), initialEventDelay);
        }

        public void RollEventChance()
        {
            if (active)
            {
                float roll = Random.Range(0f, 1f);
                if (roll <= eventSpawnChance) SpawnEvent();
                else Time.StartTimer(new VariableReference<bool>(() => (false), (val) => { RollEventChance(); }), timeBetweenEventChances);
            }
        }

        private void SpawnEvent()
        {
            if (eventPrefabs.Length <= 0) throw new Exception("no event prefabs assigned");
            int eventIndex = Random.Range(0, eventPrefabs.Length);
            var eventObject = Instantiate(eventPrefabs[eventIndex], new Vector3(0,0,0), Quaternion.identity);
            EventSpawnPositionData data;
            if (eventObject.TryGetComponent<EventSpawnPositionData>(out data))
            {
                int spawnPositionIndex = Random.Range(0, data.eventSpawnLocations.Length);
                eventObject.transform.position = data.eventSpawnLocations[spawnPositionIndex].position;
            }
            else
            {
                throw new Exception("problem with event prefab; no eventSpawnPositionData Component found");
            }
            EventCallback callback;
            if (eventObject.TryGetComponent<EventCallback>(out callback))
            {
                callback.OnEventEnded += OnEventEndedHandler;
            }
            else
            {
                throw new Exception("problem with event prefab; no eventCallback Component found");
            }
        }

        private void OnEventEndedHandler()
        {
            Time.StartTimer(new VariableReference<bool>(() => (false), (val) => { RollEventChance(); }), timeBetweenEvents);
        }
        
        private void OnEnable()
        {
            active = true;
        }
        private void OnDisable()
        {
            active = false;
        }
    }
}