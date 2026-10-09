using TMPro;
using UnityEngine;
using VertigoCase.Core;

namespace VertigoCase.UI
{
    public class CurrencyBarView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text_gold_value;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (text_gold_value == null)
            {
                text_gold_value = GetComponentInChildren<TextMeshProUGUI>(true);
            }
        }
#endif

        private void Start()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnTotalGoldChanged += UpdateDisplay;
                UpdateDisplay(InventoryManager.Instance.TotalGold);
            }
        }

        private void UpdateDisplay(int currentGold)
        {
            if (text_gold_value != null)
            {
                text_gold_value.text = currentGold.ToString("N0");
            }
        }

        private void OnDestroy()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnTotalGoldChanged -= UpdateDisplay;
            }
        }
    }
}