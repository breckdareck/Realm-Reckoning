using System;
using Game._Scripts.Abilities;
using Game._Scripts.Battle;
using Game._Scripts.Scriptables;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Game._Scripts.Managers
{
    public sealed class EventManager : MonoBehaviour
    {
        public static EventManager Instance;

        // Battle Event
        public event Action OnStateChangedEvent;
        public event Action<string> OnStepChangedEvent;
        public event Action<AbilitySO> OnAbilitySelectionChangedEvent;
        public event Action<Unit> OnUnitSelectedChangedEvent;


        // Energy Events
        public event Action OnEnergyChangedEvent;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }


        #region Battle Events

        public void InvokeOnStateChanged()
        {
            //Debug.Log("Invoking OnStateChanged");
            OnStateChangedEvent?.Invoke();
        }

        public void InvokeOnStepChanged(string text)
        {
            //Debug.Log("Invoking OnStepChanged");
            OnStepChangedEvent?.Invoke(text);
        }

        public void InvokeOnAbilitySelectionChanged(AbilitySO abilitySo)
        {
            //Debug.Log("Invoking OnAbilitySelectionChanged");
            OnAbilitySelectionChangedEvent?.Invoke(abilitySo);
        }

        public void InvokeOnUnitSelectedChanged(Unit unit)
        {
            //Debug.Log("Invoking OnUnitSelectedChanged()");
            OnUnitSelectedChangedEvent?.Invoke(unit);
        }

        #endregion


        #region Energy Events

        public void InvokeOnEnergyChanged()
        {
            //Debug.Log("Invoking OnEnergyChanged");
            OnEnergyChangedEvent?.Invoke();
        }

        #endregion
    }
}