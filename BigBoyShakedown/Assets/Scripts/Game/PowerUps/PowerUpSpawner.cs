using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BigBoyShakedown.Game.PowerUp
{
    public class PowerUpSpawner : MonoBehaviour
    {
        [SerializeField] PowerUpSpawnData[] powerUpSpawnLocations;
        [SerializeField] GameObject[] powerUps;
        [Header("Timings")]
        [SerializeField] float initialPowerUpDelay = 0f;
        [SerializeField] float timeBetweenPowerUps = 2f;
        [SerializeField] float timeBetweenPowerUpChances = 1f;
        [SerializeField] float powerUpLifeTime = 20f;
        [Header("Chance"), SerializeField, Range(0f, 1f)] float powerUpSpawnChance = .5f;
        [SerializeField, Range(0f, 1f)] float powerUpDoubleSpawnChance = .3f;
        

        private void Start()
        {
            Time.StartTimer(new VariableReference<bool>(() => (false), (val) => { RollPowerUpChance(); }), initialPowerUpDelay);
        }

        public void RollPowerUpChance()
        {
            float roll = Random.Range(0f, 1f);
            if (roll <= powerUpSpawnChance) SpawnPowerUp();
            else Time.StartTimer(new VariableReference<bool>(() => (false), (val) => { RollPowerUpChance(); }), timeBetweenPowerUpChances);
        }

        private void SpawnPowerUp()
        {
            if (powerUpSpawnLocations.Length <= 0) throw new Exception("no power up locations assigned");
            if (powerUps.Length <= 0) throw new Exception("no power up prefabs assigned");

            int roll;
            bool freePositionFound = false;
            PowerUpSpawnData freePosition = null;
            while (!freePositionFound)
            {
                roll = Random.Range(0, powerUpSpawnLocations.Length);
                if (!powerUpSpawnLocations[roll].occupied)
                {
                    freePositionFound = true;
                    freePosition = powerUpSpawnLocations[roll];
                }
            }
            roll = Random.Range(0, powerUps.Length);

            freePosition.Occupy(Instantiate(powerUps[roll]), powerUpLifeTime);
            Time.StartTimer(new VariableReference<bool>(() => (false), (val) => { RollPowerUpChance(); }), timeBetweenPowerUps);
        }
    }

    [Serializable]
    public class PowerUpSpawnData
    {
        public Transform spawnPosition;
        public GameObject powerUp;
        public float powerUpLifeTime;
        public bool occupied { get; private set; }

        public void Occupy(GameObject powerUp_, float powerUpLifeTime_)
        {
            if (occupied || !powerUp_ || powerUpLifeTime_ <= 0) return;

            occupied = true;
            powerUp = powerUp_;
            powerUpLifeTime = powerUpLifeTime_;
            powerUp.transform.position = spawnPosition.position;

            //subscribe powerUpCallback when picked up / removed
            PowerUpCallback callback;
            if (!powerUp.TryGetComponent<PowerUpCallback>(out callback))
            {
                throw new Exception("problem with prefab; no PowerUpCallback component found");
            }

            Time.StartTimer(new VariableReference<bool>(() => false, (val) => { DestroyPowerUpIfStillThere(); }), powerUpLifeTime);
        }

        public void OnPowerUpRemovedHandler()
        {
            occupied = false;
            powerUp = null;
            powerUpLifeTime = 0;
        }

        public void DestroyPowerUpIfStillThere()
        {
            if (occupied) GameObject.Destroy(powerUp);
        }
    }
}
