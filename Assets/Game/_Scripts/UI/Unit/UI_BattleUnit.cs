using System.Globalization;
using Game._Scripts.Managers;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Game._Scripts.UI.Unit
{
    public class UI_BattleUnit : MonoBehaviour
    {
        [Header("Unit Stats")] [SerializeField]
        private TMP_Text unitNameText;

        [SerializeField] private Image healthImage;
        [SerializeField] private Image barrierImage;

        [Header("Button/Anims")] [SerializeField]
        private Button unitButton;

        [SerializeField] private Transform activeUnitAnim;
        [SerializeField] private Transform enemyTargetAnim;

        [Header("Damage Text")] [SerializeField]
        private DamageText damageText;

        [SerializeField] private Transform playerDamageTextLocation;
        [SerializeField] private Transform enemyDamageTextLocation;

        [Header("Turn Progress")] [SerializeField]
        private Slider turnProgressSlider;

        [SerializeField] private TMP_Text debugTurnProgressText;

        private _Scripts.Battle.BattleUnit _attachedBattleUnit;

        public void InitializeUI(ref _Scripts.Battle.BattleUnit battleUnit)
        {
            _attachedBattleUnit = battleUnit;
            unitNameText.text = battleUnit.Unit.UnitData.unitName;
            UpdateHealthUI();
            UpdateBarrierUI();
            unitButton.onClick.AddListener(() => EventManager.Instance.InvokeOnUnitSelectedChanged(_attachedBattleUnit));
        }

        public void UpdateHealthUI()
        {
            healthImage.fillAmount = (float)_attachedBattleUnit.CurrentHealth /
                                     _attachedBattleUnit.MaxHealth;
        }

        public void UpdateBarrierUI()
        {
            barrierImage.fillAmount = (float)_attachedBattleUnit.CurrentBarrier /
                                      _attachedBattleUnit.MaxBarrier;
        }

        public void UpdateTurnSliderValue(float value)
        {
            turnProgressSlider.value = value;
            debugTurnProgressText.text = value.ToString(CultureInfo.InvariantCulture);
        }

        public void SetActiveUnitAnim()
        {
            activeUnitAnim.gameObject.SetActive(!activeUnitAnim.gameObject.activeInHierarchy);
        }

        public void SetEnemyTargetAnim()
        {
            if (enemyTargetAnim != null)
                enemyTargetAnim.gameObject.SetActive(!enemyTargetAnim.gameObject.activeInHierarchy);
        }

        public void CreateDamageText(string damageAmount)
        {
            CreateText(damageAmount, false);
        }

        public void CreateHealText(int healAmount)
        {
            CreateText(healAmount.ToString(), true);
        }
        
        private void CreateText(string text, bool isHeal)
        {
            var textLocation = playerDamageTextLocation;
            if (_attachedBattleUnit.IsControlledByAI)
            {
                textLocation = enemyDamageTextLocation;
            }

            var damageTextObj = Instantiate(damageText, textLocation.position, quaternion.identity);
            damageTextObj.transform.SetParent(textLocation);
            damageTextObj.SetDamageText(text, isHeal, false);
        }
    }
}