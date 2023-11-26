using System;
using Game._Scripts.Abilities;
using Game._Scripts.Battle;
using Game._Scripts.Managers;
using Game._Scripts.Units;
using Sirenix.Utilities;
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

        [SerializeField] private TMP_Text debugUnitStatsText;

        public static UI_Battle Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            endBattleButton.onClick.AddListener(
                () => BattleSystem.Instance.BattleStateMachine.SetState(BattleState.End));
        }

        private void OnEnable()
        {
            EventManager.Instance.OnStateChangedEvent += UpdateStateText;
            EventManager.Instance.OnStateChangedEvent += UpdateDebugUnitStatsText;

            EventManager.Instance.OnStepChangedEvent += UpdateStepText;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnStateChangedEvent -= UpdateStateText;
            EventManager.Instance.OnStateChangedEvent -= UpdateDebugUnitStatsText;

            EventManager.Instance.OnStepChangedEvent -= UpdateStepText;
        }

        private void UpdateStateText()
        {
            currentStateText.text =
                $"Current State: {BattleSystem.Instance.BattleStateMachine.CurrentState.ToString()}";
        }

        private void UpdateStepText(string text)
        {
            nextStepText.text = $"Next Step: {text}";
        }

        private void UpdateDebugUnitStatsText()
        {
            debugUnitStatsText.text = $"{BattleSystem.Instance.BattleStateMachine.GetActiveUnit().name} Stats \n";
            BattleSystem.Instance.BattleStateMachine.GetActiveUnit().UnitsData.currentStats.generalStats
                .ForEach(x => debugUnitStatsText.text += ($"{x.Key} : {x.Value} \n"));
        }

        public void SetupAbilityButton(Ability ability, int buttonIndex)
        {
            abilityButtons[buttonIndex].gameObject.SetActive(ability != null);

            abilityButtons[buttonIndex].onClick.RemoveAllListeners();
            abilityButtons[buttonIndex].onClick.AddListener(() => OnAbilityButtonClick(ability));
            abilityButtons[buttonIndex].GetComponentInChildren<TMP_Text>().text = ability != null ? ability.abilityName : "";
        }

        private void OnAbilityButtonClick(Ability ability)
        {
            EventManager.Instance.InvokeOnAbilitySelectionChanged(ability);
        }
    }
}