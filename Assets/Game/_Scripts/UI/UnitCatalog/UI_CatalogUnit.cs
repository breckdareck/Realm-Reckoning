using System.Collections.Generic;
using Game._Scripts.Enums;
using Game._Scripts.Scriptables;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game._Scripts.UI.UnitCatalog
{
    public class UI_CatalogUnit : MonoBehaviour
    {
        [FoldoutGroup("Assets")] [SerializeField]
        private Sprite starOffImage;

        [FoldoutGroup("Assets")] [SerializeField]
        private Sprite starOnImage;

        [SerializeField] private Button unitButton;
        [SerializeField] private Image unitImage;
        [SerializeField] private TMP_Text unitNameText;
        [SerializeField] private List<Image> starImages;

        [SerializeField] private Game._Scripts.Unit attachedUnitData;
        public Game._Scripts.Unit AttachedUnitData => attachedUnitData;

        private void OnEnable()
        {
            unitButton.onClick.AddListener(
                delegate { UI_UnitCollection.Instance.UpdateSelectedUnit(attachedUnitData); });
        }

        private void OnDisable()
        {
            unitButton.onClick.RemoveAllListeners();
        }

        public void InitializeUI()
        {
            unitNameText.text = attachedUnitData.UnitData.unitName;
            for (var i = starImages.Count; i > 0; i--)
                // Count of 10, if starrating is 0 i > starrating i--
                starImages[i - 1].sprite =
                    attachedUnitData.currentUnitStats[GeneralStat.StarRating] > starImages.Count - i
                        ? starOnImage
                        : starOffImage;
        }

        public void SetAttachedUnitData(Game._Scripts.Unit newUnitData)
        {
            attachedUnitData = newUnitData;
        }
    }
}