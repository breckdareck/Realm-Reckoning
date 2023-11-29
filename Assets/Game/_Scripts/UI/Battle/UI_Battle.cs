using Game._Scripts.Abilities;
using Game._Scripts.Battle;
using Game._Scripts.Enums;
using Game._Scripts.Managers;
using Game._Scripts.Scriptables;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game._Scripts.UI.Battle
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
            var unit = BattleSystem.Instance.BattleStateMachine.GetActiveUnit();
            debugUnitStatsText.text =
                $"{BattleSystem.Instance.BattleStateMachine.GetActiveUnit().name} Base Stats + Bonus Battle Stats \n";
            unit.UnitsDataSo.persistentDataSo.stats
                .ForEach(x =>
                    debugUnitStatsText.text +=
                        $"{x.Key} : {x.Value} + {(unit.BattleBonusStats.TryGetValue(x.Key, out var stat) ? stat : 0)} \n");
        }

        public void SetupAbilityButton(AbilitySO abilitySo, int buttonIndex)
        {
            abilityButtons[buttonIndex].gameObject.SetActive(abilitySo != null);

            abilityButtons[buttonIndex].onClick.RemoveAllListeners();
            abilityButtons[buttonIndex].onClick.AddListener(() => OnAbilityButtonClick(abilitySo));
            abilityButtons[buttonIndex].GetComponentInChildren<TMP_Text>().text =
                abilitySo != null ? abilitySo.abilityName : "";
        }

        private void OnAbilityButtonClick(AbilitySO abilitySo)
        {
            EventManager.Instance.InvokeOnAbilitySelectionChanged(abilitySo);
        }
    }
}