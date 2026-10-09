using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class RewardPanelView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform containerRewards;
        [SerializeField] private RewardItemUI rewardItemPrefab;
        [SerializeField] private Button buttonExit;

        [Header("Wheel Reference")]
        [SerializeField] private WheelController wheelController;

        [Header("Claim Popup Reference")]
        [SerializeField] private ClaimRewardsPopupView claimRewardsPopupView;

        private readonly Dictionary<string, RewardItemUI> _activeItems = new Dictionary<string, RewardItemUI>();
        private bool _isSafeOrSuperZone;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (buttonExit == null)
            {
                var buttons = GetComponentsInChildren<Button>(true);
                foreach (Button btn in buttons)
                {
                    string lower = btn.name.ToLower();
                    if (lower.Contains("exit") || lower.Contains("cikis") || lower.Contains("leave"))
                    {
                        buttonExit = btn;
                        break;
                    }
                }
            }

            if (wheelController == null)
            {
                wheelController = FindFirstObjectByType<WheelController>();
            }

            if (claimRewardsPopupView == null)
            {
                claimRewardsPopupView = FindFirstObjectByType<ClaimRewardsPopupView>();
            }
        }
#endif

        private void Start()
        {
            if (buttonExit != null)
                buttonExit.onClick.AddListener(OnExitClicked);

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnRewardAdded += HandleRewardAdded;
                InventoryManager.Instance.OnInventoryCleared += ClearAllItems;
            }

            if (ZoneManager.Instance != null)
            {
                ZoneManager.Instance.OnZoneChanged += HandleZoneChanged;
            }

            if (wheelController != null)
            {
                wheelController.OnSpinStateChanged += HandleSpinStateChanged;
            }

            UpdateExitButtonState(false);
        }

        private void HandleZoneChanged(int zoneNumber, VertigoCase.Core.WheelTier tier, ZoneConfig config)
        {
            _isSafeOrSuperZone = ZoneManager.Instance.IsSafeZone(zoneNumber) || ZoneManager.Instance.IsSuperZone(zoneNumber);
            bool isSpinning = wheelController != null && wheelController.IsSpinning;
            UpdateExitButtonState(!isSpinning && _isSafeOrSuperZone);
        }

        private void HandleSpinStateChanged(bool isSpinning)
        {
            UpdateExitButtonState(!isSpinning && _isSafeOrSuperZone);
        }

        private void UpdateExitButtonState(bool interactable)
        {
            if (buttonExit != null)
                buttonExit.interactable = interactable;
        }

        private void HandleRewardAdded(string rewardKey, int totalAmount, Sprite icon)
        {
            if (_activeItems.TryGetValue(rewardKey, out RewardItemUI existingItem))
            {
                existingItem.UpdateAmount(totalAmount);
            }
            else
            {
                if (rewardItemPrefab == null || containerRewards == null) return;

                RewardItemUI newItem = Instantiate(rewardItemPrefab, containerRewards);
                newItem.Setup(RewardType.Item, totalAmount, icon);
                _activeItems.Add(rewardKey, newItem);
            }
        }

        private void ClearAllItems()
        {
            foreach (var kvp in _activeItems)
            {
                if (kvp.Value != null)
                    Destroy(kvp.Value.gameObject);
            }
            _activeItems.Clear();
        }

        private void OnExitClicked()
        {
            if (claimRewardsPopupView != null)
            {
                claimRewardsPopupView.Show();
                ClearAllItems();
            }
            else
            {
                ClearAllItems();
                InventoryManager.Instance?.CollectAndClaimRewards();
                ZoneManager.Instance?.StartNewGame();
            }
        }

        private void OnDestroy()
        {
            if (buttonExit != null)
                buttonExit.onClick.RemoveListener(OnExitClicked);

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnRewardAdded -= HandleRewardAdded;
                InventoryManager.Instance.OnInventoryCleared -= ClearAllItems;
            }

            if (ZoneManager.Instance != null)
                ZoneManager.Instance.OnZoneChanged -= HandleZoneChanged;

            if (wheelController != null)
                wheelController.OnSpinStateChanged -= HandleSpinStateChanged;
        }
    }
}