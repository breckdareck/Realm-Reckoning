using System;
using System.Collections;
using System.Collections.Generic;
using Game._Scripts.Enums;
using Game._Scripts.Scriptables;
using Game._Scripts.UI.UnitCatalog;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CatalogUnit : MonoBehaviour
{
    [FoldoutGroup("Assets")] [SerializeField] private Sprite starOffImage;
    [FoldoutGroup("Assets")] [SerializeField] private Sprite starOnImage;

    [SerializeField] private Button unitButton;
    [SerializeField] private Image unitImage;
    [SerializeField] private TMP_Text unitNameText;
    [SerializeField] private List<Image> starImages;

    [SerializeField] private UnitDataSO attachedUnitData;
    public UnitDataSO AttachedUnitData => attachedUnitData;

    private void OnEnable()
    {
        unitButton.onClick.AddListener(delegate { UI_UnitCollection.Instance.UpdateSelectedUnit(attachedUnitData); });
    }

    private void OnDisable()
    {
        unitButton.onClick.RemoveAllListeners();
    }

    public void InitializeUI()
    {
        unitNameText.text = attachedUnitData.unitName;
        for (int i = starImages.Count; i > 0 ; i--)
        {
            // Count of 10, if starrating is 0 i > starrating i--
            starImages[i-1].sprite = attachedUnitData.currentUnitStats[GeneralStat.StarRating] > starImages.Count-i ? starOnImage : starOffImage;
        }
    }

    public void SetAttachedUnitData(UnitDataSO newUnitData)
    {
        attachedUnitData = newUnitData;
    }
}
