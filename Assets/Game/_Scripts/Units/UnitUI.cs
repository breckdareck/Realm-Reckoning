using System.Globalization;
using Game._Scripts.Managers;
using Game._Scripts.UI;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Game._Scripts.Units
{
    public class UnitUI : MonoBehaviour
    {
        [Header("Unit Stats")]
        [SerializeField] private TMP_Text unitNameText;
        [SerializeField] private Image healthImage;
        [SerializeField] private Image barrierImage;
        
        [Header("Button/Anims")]
        [SerializeField] private Button unitButton;
        [SerializeField] private Transform activeUnitAnim;
        [SerializeField] private Transform enemyTargetAnim;

        [Header("Damage Text")]
        [SerializeField] private DamageText damageText;
        [SerializeField] private Transform playerDamageTextLocation;
        [SerializeField] private Transform enemyDamageTextLocation;

        [Header("Turn Progress")]
        [SerializeField] private Slider turnProgressSlider;
        [SerializeField] private TMP_Text debugTurnProgressText;

        private Unit _attachedUnit;

        public void InitializeUI(ref Unit unit)
        {
            _attachedUnit = unit;
            unitNameText.text = unit.UnitsData.unitName;
            UpdateHealthUI();
            UpdateBarrierUI();
            unitButton.onClick.AddListener(() => EventManager.Instance.InvokeOnUnitSelectedChanged(_attachedUnit));
        }

        public void UpdateHealthUI()
        {
            healthImage.fillAmount = (float)_attachedUnit.CurrentHealth /
                                     _attachedUnit.MaxHealth;
        }

        public void UpdateBarrierUI()
        {
            barrierImage.fillAmount = (float)_attachedUnit.CurrentBarrier / 
                                      _attachedUnit.MaxBarrier;
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
            enemyTargetAnim.gameObject.SetActive(!enemyTargetAnim.gameObject.activeInHierarchy);
        }

        public void CreateDamageText(string text)
        {
            if (_attachedUnit.IsAIUnit)
            {
                var tmpdmgTextGo =
                    Instantiate(damageText, enemyDamageTextLocation.position, quaternion.identity);
                tmpdmgTextGo.transform.SetParent(enemyDamageTextLocation);
                tmpdmgTextGo.SetDamageText(text, false, false);
            }
            else
            {
                var tmpdmgTextGo =
                    Instantiate(damageText, playerDamageTextLocation.position, quaternion.identity);
                tmpdmgTextGo.transform.SetParent(playerDamageTextLocation);
                tmpdmgTextGo.SetDamageText(text, false, false);
            }
        }
        
        public void CreateHealText(int amount)
        {
            // TODO : Fix isCrit when implemented
            if (_attachedUnit.IsAIUnit)
            {
                var tmpdmgTextGo =
                    Instantiate(damageText, enemyDamageTextLocation.position, quaternion.identity);
                tmpdmgTextGo.transform.SetParent(enemyDamageTextLocation);
                tmpdmgTextGo.SetDamageText(amount.ToString(), true, false);
            }
            else
            {
                var tmpdmgTextGo =
                    Instantiate(damageText, playerDamageTextLocation.position, quaternion.identity);
                tmpdmgTextGo.transform.SetParent(playerDamageTextLocation);
                tmpdmgTextGo.SetDamageText(amount.ToString(), true, false);
            }
        }
    }
}