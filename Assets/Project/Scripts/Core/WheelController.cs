using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.Data;
using VertigoCase.UI;

namespace VertigoCase.Core
{
    public class WheelController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private WheelView wheelView;
        [SerializeField] private Button spinButton;
        [SerializeField] private BombPopupView bombPopupView;

        [Header("Zone & Tier Text Displays")]
        [SerializeField] private TextMeshProUGUI text_zone_value;
        [SerializeField] private TextMeshProUGUI text_tier_value;

        private ZoneConfig _currentZoneConfig;
        private bool _isSpinning;

        public bool IsSpinning => _isSpinning;
        public event Action<bool> OnSpinStateChanged;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (spinButton == null)
            {
                var buttons = GetComponentsInChildren<Button>(true);
                foreach (Button btn in buttons)
                {
                    if (btn.name.ToLower().Contains("spin"))
                    {
                        spinButton = btn;
                        break;
                    }
                }
            }

            if (wheelView == null)
            {
                wheelView = GetComponentInChildren<WheelView>(true);
            }

            if (bombPopupView == null)
            {
                bombPopupView = GetComponentInChildren<BombPopupView>(true);
            }

            if (text_zone_value == null || text_tier_value == null)
            {
                var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (TextMeshProUGUI txt in texts)
                {
                    string lowerName = txt.name.ToLower();
                    if (lowerName.Contains("zone")) text_zone_value = txt;
                    else if (lowerName.Contains("tier")) text_tier_value = txt;
                }
            }
        }
#endif

        private void Start()
        {
            if (spinButton != null)
                spinButton.onClick.AddListener(OnSpinClicked);

            if (ZoneManager.Instance != null)
            {
                ZoneManager.Instance.OnZoneChanged += HandleZoneChanged;
                ZoneManager.Instance.StartNewGame();
            }
        }

        private void HandleZoneChanged(int zoneNumber, WheelTier tier, ZoneConfig config)
        {
            _currentZoneConfig = config;
            if (wheelView != null && _currentZoneConfig != null)
            {
                wheelView.InitializeWheel(_currentZoneConfig);
            }

            UpdateZoneUI(zoneNumber, tier);
            SetSpinningState(false);
        }

        private void UpdateZoneUI(int zoneNumber, WheelTier tier)
        {
            if (text_zone_value != null)
            {
                text_zone_value.text = $"ZONE {zoneNumber}";
            }

            if (text_tier_value != null)
            {
                Color goldColor = new Color(1.0f, 0.84f, 0.0f);
                Color silverColor = new Color(0.75f, 0.75f, 0.75f);
                Color bronzeColor = new Color(0.8f, 0.5f, 0.2f);

                switch (tier)
                {
                    case WheelTier.Gold:
                        text_tier_value.text = "GOLDEN SPIN";
                        text_tier_value.color = goldColor;
                        break;
                    case WheelTier.Silver:
                        text_tier_value.text = "SILVER SPIN";
                        text_tier_value.color = silverColor;
                        break;
                    default:
                        text_tier_value.text = "BRONZE SPIN";
                        text_tier_value.color = bronzeColor;
                        break;
                }
            }
        }

        private void OnSpinClicked()
        {
            if (_isSpinning || _currentZoneConfig == null || _currentZoneConfig.slices == null || _currentZoneConfig.slices.Count == 0)
                return;

            SetSpinningState(true);

            int targetIndex = GetWeightedRandomSliceIndex(_currentZoneConfig.slices);

            wheelView.SpinToSlice(targetIndex, _currentZoneConfig.slices.Count, () =>
            {
                OnSpinFinished(targetIndex);
            });
        }

        private int GetWeightedRandomSliceIndex(List<WheelSliceData> slices)
        {
            float totalWeight = 0f;
            for (int i = 0; i < slices.Count; i++)
            {
                totalWeight += Mathf.Max(0f, slices[i].dropWeight);
            }

            if (totalWeight <= 0f)
            {
                return UnityEngine.Random.Range(0, slices.Count);
            }

            float randomRoll = UnityEngine.Random.Range(0f, totalWeight);
            float currentSum = 0f;

            for (int i = 0; i < slices.Count; i++)
            {
                currentSum += Mathf.Max(0f, slices[i].dropWeight);
                if (randomRoll <= currentSum)
                {
                    return i;
                }
            }

            return slices.Count - 1;
        }

        private void SetSpinningState(bool spinning)
        {
            _isSpinning = spinning;
            if (spinButton != null) 
                spinButton.interactable = !spinning;

            OnSpinStateChanged?.Invoke(_isSpinning);
        }

        private void OnSpinFinished(int landedIndex)
        {
            SetSpinningState(false);

            WheelSliceData landedSlice = _currentZoneConfig.slices[landedIndex];

            if (landedSlice.rewardType == RewardType.Bomb)
            {
                if (bombPopupView != null)
                {
                    bombPopupView.Show();
                }
                else
                {
                    InventoryManager.Instance?.ClearCurrentRun();
                    ZoneManager.Instance?.StartNewGame();
                }
            }
            else
            {
                InventoryManager.Instance?.AddReward(landedSlice.rewardType, landedSlice.amount, landedSlice.icon);
                ZoneManager.Instance?.AdvanceNextZone();
            }
        }

        private void OnDestroy()
        {
            if (spinButton != null)
                spinButton.onClick.RemoveListener(OnSpinClicked);

            if (ZoneManager.Instance != null)
                ZoneManager.Instance.OnZoneChanged -= HandleZoneChanged;
        }
    }
}