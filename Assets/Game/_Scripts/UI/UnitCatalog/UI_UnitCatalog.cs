using UnityEngine;
using UnityEngine.UI;

namespace Game._Scripts.UI.UnitCatalog
{
    public class UI_UnitCatalog : MonoBehaviour
    {
        public Button backButton;

        public void CloseUnitCollectionMenu()
        {
            gameObject.SetActive(false);
        }
    }
}