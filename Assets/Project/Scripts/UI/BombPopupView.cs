using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.Core;

namespace VertigoCase.UI
{
    public class BombPopupView : MonoBehaviour
    {
        [Header("Popup Root")]
        [SerializeField] private GameObject popupRoot;

        [Header("Buttons")]
        [SerializeField] private Button buttonGiveUp;
        [SerializeField] private Button buttonRevive;

        [Header("Cost Configuration")]
        [SerializeField] private int baseReviveGoldCost = 100;
        [SerializeField] private TextMeshProUGUI text_revive_cost_value;

        private int _reviveCount;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (popupRoot == null)
                popupRoot = gameObject;

            if (buttonGiveUp == null || buttonRevive == null)
            {
                var buttons = GetComponentsInChildren<Button>(true);
                foreach (Button btn in buttons)
                {
                    string lowerName = btn.name.ToLower();
                    if (lowerName.Contains("give_up") || lowerName.Contains("giveup"))
                        buttonGiveUp = btn;
                    else if (lowerName.Contains("revive"))
                        buttonRevive = btn;
                }
            }

            if (text_revive_cost_value == null && buttonRevive != null)
            {
                text_revive_cost_value = buttonRevive.GetComponentInChildren<TextMeshProUGUI>(true);
            }
        }
#endif

        private void Awake()
        {
            if (buttonGiveUp != null)
                buttonGiveUp.onClick.AddListener(OnGiveUpClicked);

            if (buttonRevive != null)
                buttonRevive.onClick.AddListener(OnReviveClicked);

            Hide();
        }

        public void Show()
        {
            UpdateReviveState();
            if (popupRoot != null)
                popupRoot.SetActive(true);
        }

        public void Hide()
        {
            if (popupRoot != null)
                popupRoot.SetActive(false);
        }

        private int GetCurrentCost()
        {
            return baseReviveGoldCost * (_reviveCount + 1);
        }

        private void UpdateReviveState()
        {
            int cost = GetCurrentCost();
            bool hasEnough = InventoryManager.Instance != null && InventoryManager.Instance.HasEnoughGold(cost);

            if (buttonRevive != null)
                buttonRevive.interactable = hasEnough;

            if (text_revive_cost_value != null)
            {
                text_revive_cost_value.text = $"{cost}\nREVIVE";
            }
        }

        private void OnGiveUpClicked()
        {
            Hide();
            _reviveCount = 0;
            InventoryManager.Instance?.ClearCurrentRun();
            ZoneManager.Instance?.StartNewGame();
        }

        private void OnReviveClicked()
        {
            if (InventoryManager.Instance == null) return;

            int cost = GetCurrentCost();
            if (!InventoryManager.Instance.SpendGold(cost))
                return;

            _reviveCount++;
            Hide();
            ZoneManager.Instance?.AdvanceNextZone();
        }

        private void OnDestroy()
        {
            if (buttonGiveUp != null)
                buttonGiveUp.onClick.RemoveListener(OnGiveUpClicked);

            if (buttonRevive != null)
                buttonRevive.onClick.RemoveListener(OnReviveClicked);
        }
    }
}