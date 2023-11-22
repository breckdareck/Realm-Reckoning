using System;
using Game._Scripts.Abilities;
using Game._Scripts.Battle;
using Game._Scripts.Managers;
using Game._Scripts.Units;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game._Scripts.UI
{
    public class UI_Battle : MonoBehaviour
    {
        [SerializeField] private TMP_Text currentStateText;
        [SerializeField] private Button endBattleButton;
        [SerializeField] private TMP_Text nextStepText;

        [SerializeField] private Button[] abilityButtons;
        
        public static UI_Battle Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            endBattleButton.onClick.AddListener(() => BattleSystem.Instance.BattleStateMachine.SetState(BattleState.End));
            
        }

        private void OnEnable()
        {
            EventManager.Instance.OnStateChangedEvent += UpdateStateText;
            EventManager.Instance.OnStepChangedEvent += UpdateStepText;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnStateChangedEvent -= UpdateStateText;
            EventManager.Instance.OnStepChangedEvent -= UpdateStepText;
        }

        private void UpdateStateText()
        {
            currentStateText.text = $"Current State: {BattleSystem.Instance.BattleStateMachine.CurrentState.ToString()}";
        }
        
        private void UpdateStepText(string text)
        {
            nextStepText.text = $"Next Step: {text}";
        }

        public void SetupAbilityButton(Ability ability, int buttonIndex)
        {
            abilityButtons[buttonIndex].onClick.RemoveAllListeners();
            abilityButtons[buttonIndex].onClick.AddListener(() => OnAbilityButtonClick(ability));
            abilityButtons[buttonIndex].GetComponentInChildren<TMP_Text>().text = ability.abilityName;
        }

        private void OnAbilityButtonClick(Ability ability)
        {
            EventManager.Instance.InvokeOnAbilitySelectionChanged(ability);
        }
    }
}
