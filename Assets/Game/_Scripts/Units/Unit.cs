using System;
using Game._Scripts.Battle;
using Game._Scripts.Managers;
using Game._Scripts.UI;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game._Scripts.Units
{
    [Serializable]
    public class Unit : MonoBehaviour
    {
        [SerializeField] private TMP_Text unitNameText;
        [SerializeField] private Button unitButton;
        [SerializeField] private Transform activeUnitAnim;
        [SerializeField] private Transform enemyTargetAnim;
        [SerializeField] private Image healthImage;

        [SerializeField] private DamageText damageText;
        [SerializeField] private Transform playerDamageTextLocation;
        [SerializeField] private Transform enemyDamageTextLocation;

        public bool IsAIUnit { get; private set; }
        [ShowInInspector] public UnitData UnitsData { get; private set; }

        public int CurrentHealth { get; private set; }

        public void Initialize(UnitData data, bool isAIUnit)
        {
            IsAIUnit = isAIUnit;
            UnitsData = data;
            SetupCurrentUnitDataStats();
            CurrentHealth = (int)data.currentStats.GetStatValue(Stat.Health);
            unitNameText.text = UnitsData.unitName;
            
            unitButton.onClick.AddListener(() => EventManager.Instance.InvokeOnUnitSelectedChanged(this));
        }

        private void SetupCurrentUnitDataStats()
        {
            UnitsData.currentStats = ScriptableObject.CreateInstance<Stats>();
            UnitsData.currentStats.stats.Add(Stat.Health, (int)(UnitsData.baseStats.GetStatValue(Stat.Health) + UnitsData.baseStats.GetStatValue(Stat.Level) * UnitsData.baseStats.GetStatValue(Stat.HealthPerLevel)));
            UnitsData.currentStats.stats.Add(Stat.Speed, (int)UnitsData.baseStats.GetStatValue(Stat.Speed));
        }

        public void SetActiveUnitAnim()
        {
            activeUnitAnim.gameObject.SetActive(!activeUnitAnim.gameObject.activeInHierarchy);
        }

        public void SetEnemyTargetAnim()
        {
            enemyTargetAnim.gameObject.SetActive(!enemyTargetAnim.gameObject.activeInHierarchy);
        }

        public void TakeDamage(int damageAmount)
        {
            CurrentHealth -= Mathf.Clamp(damageAmount, 0, (int)UnitsData.currentStats.GetStatValue(Stat.Health));
            UpdateHealthUI();
            if (IsAIUnit)
            {
                DamageText tmpdmgTextGo =
                    Instantiate(damageText, enemyDamageTextLocation.position, quaternion.identity);
                tmpdmgTextGo.transform.SetParent(enemyDamageTextLocation);
                tmpdmgTextGo.SetDamageText(damageAmount.ToString(), false);
            }
            else
            {
                DamageText tmpdmgTextGo =
                    Instantiate(damageText, playerDamageTextLocation.position, quaternion.identity);
                tmpdmgTextGo.transform.SetParent(playerDamageTextLocation);
                tmpdmgTextGo.SetDamageText(damageAmount.ToString(), false);
            }
            
            Debug.Log($"{this.name} - Damage Taken: {damageAmount}");
        }

        private void UpdateHealthUI()
        {
            healthImage.fillAmount = (CurrentHealth / UnitsData.currentStats.GetStatValue(Stat.Health));
        }

        public void ApplyBuff(Stat stat, int buffAmount)
        {
            Debug.Log($"{stat} was buffed by {buffAmount} points");
        }

        public void ApplyDebuff(Stat stat, int debuffAmount)
        {
            Debug.Log($"{stat} was debuffed by {debuffAmount} points");
        }
    }
}