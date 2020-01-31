using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Game.PowerUp
{
    public class Attackable : MonoBehaviour
    {
        [SerializeField] float health;
        bool destroyed = false;

        PowerUpCallback callback;

        //on death animation TODO

        private void Awake()
        {
            if (!TryGetComponent<PowerUpCallback>(out callback))
            {
                throw new Exception("no PowerUpCallback component found");
            }
        }

        private void Update()
        {
            if (Time.IsRunning)
            {
                if (destroyed)
                {
                    DestroyAttackable();
                }
            }
        }

        /// <summary>
        /// destroy this, possibly with effect/animation
        /// </summary>
        private void DestroyAttackable()
        {
            //play on death animation
            callback.RelayPowerUpEnded();
            Destroy(this.gameObject);
        }

        /// <summary>
        /// function called when anything damages this attackable
        /// </summary>
        /// <param name="from">the player damaging this</param>
        /// <param name="damageIntended">the damage they intend to deal</param>
        public void DamageAttackable(Player.Controller.PlayerController from, float damageIntended)
        {
            if (from) from.HitCallback(this, (damageIntended>health)?health:damageIntended);

            health -= damageIntended;
            if (health <= 0)
            {
                destroyed = true;
            }
        }
    }
}
