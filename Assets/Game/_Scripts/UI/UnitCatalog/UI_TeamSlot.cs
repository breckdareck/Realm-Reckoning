using System.Collections;
using System.Collections.Generic;
using Game._Scripts.Enums;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_TeamSlot : MonoBehaviour
{
    [FoldoutGroup("Assets")] [SerializeField]
    private Sprite starOffImage;

    [FoldoutGroup("Assets")] [SerializeField]
    private Sprite starOnImage;

    [SerializeField] private Button slotButton;
    [SerializeField] private Image unitImage;
    [SerializeField] private TMP_Text unitNameText;
    [SerializeField] private List<Image> starImages;

    [SerializeField] private Game._Scripts.Unit attachedUnitData = null;
    public Game._Scripts.Unit AttachedUnitData => attachedUnitData;
    
    public void InitializeUI()
    {
        EnableDisableUI(true);
        
        unitNameText.text = attachedUnitData.UnitData.unitName;
        for (var i = starImages.Count; i > 0; i--)
            // Count of 10, if starrating is 0 i > starrating i--
            starImages[i - 1].sprite =
                attachedUnitData.currentUnitStats[GeneralStat.StarRating] > starImages.Count - i
                    ? starOnImage
                    : starOffImage;
    }

    public void EnableDisableUI(bool value)
    {
        unitImage.enabled = value ;
        unitNameText.enabled  = value;
        foreach (var starImage in starImages)
        {
            starImage.enabled = value;
        }
    }

    public void SetAttachedUnitData(Game._Scripts.Unit newUnitData)
    {
        attachedUnitData = newUnitData;
    }
}
