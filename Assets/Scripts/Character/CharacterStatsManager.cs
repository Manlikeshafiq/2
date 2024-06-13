using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;


namespace MSA
{
    public class CharacterStatsManager : MonoBehaviour
    {
        CharacterManager character;


        [Header("Stamina Regen")]
        private float staminaRegenTimer = 0;
        private float staminaTickTimer = 0;
        [SerializeField] float staminaRegenDelay = 1;
        [SerializeField] int staminaRegenAmount = 8;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {

        }
        public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
        {
            float stamina = 0;

            //  CREATE AN EQUATION FOR HOW YOU WANT YOUR STAMINA TO BE CALCULATED

            stamina = endurance * 10;

            return Mathf.RoundToInt(stamina);
        }

        public int CalculateHealthBasedOnVitalityLevel(int vitality)
        {
            float health = 0;

            //  CREATE AN EQUATION FOR HOW YOU WANT YOUR STAMINA TO BE CALCULATED

            health = vitality * 15;

            return Mathf.RoundToInt(health);
        }


        public virtual void RegenerateStamina()
        {
            if (!character.IsOwner)
                return;

            staminaRegenTimer += Time.deltaTime;

            if (staminaRegenTimer >= staminaRegenDelay)
            {
                if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    staminaTickTimer += Time.deltaTime;
                    if (staminaTickTimer >= 0.1)
                    {
                        staminaTickTimer = 0;
                        character.characterNetworkManager.currentStamina.Value += staminaRegenAmount;
                    }
                }
            }
        }

        public virtual void ResetStaminaRegenTimer(float oldvalue, float newValue)
        {
            if (newValue > oldvalue)
            {
                staminaRegenTimer = 0;
            }
        }
    }

}
