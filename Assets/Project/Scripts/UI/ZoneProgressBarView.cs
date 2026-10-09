using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class ZoneProgressBarView : MonoBehaviour
    {
        [Header("Tape Configuration")]
        [SerializeField] private Transform containerSlots;
        [SerializeField] private ZoneSlotUI slotPrefab;
        [SerializeField] private int visibleSlotCount = 11;

        [Header("Target Badges (Right Top)")]
        [SerializeField] private TextMeshProUGUI text_next_safe_zone_value;
        [SerializeField] private TextMeshProUGUI text_next_super_zone_value;

        [Header("Colors")]
        [SerializeField] private Color colorNormal = Color.white;
        [SerializeField] private Color colorSafe = new Color(0.2f, 0.85f, 0.2f, 1f);
        [SerializeField] private Color colorSuper = new Color(1f, 0.8f, 0.1f, 1f);

        private readonly List<ZoneSlotUI> _spawnedSlots = new List<ZoneSlotUI>();

        private void Start()
        {
            InitializeSlots();

            if (ZoneManager.Instance != null)
            {
                ZoneManager.Instance.OnZoneChanged += UpdateProgressBar;
                UpdateProgressBar(ZoneManager.Instance.CurrentZone, VertigoCase.Core.WheelTier.Bronze, null);
            }
        }

        private void InitializeSlots()
        {
            if (slotPrefab == null || containerSlots == null) return;

            for (int i = 0; i < visibleSlotCount; i++)
            {
                ZoneSlotUI slot = Instantiate(slotPrefab, containerSlots);
                _spawnedSlots.Add(slot);
            }
        }

        private void UpdateProgressBar(int currentZone, VertigoCase.Core.WheelTier tier, ZoneConfig config)
        {
            int half = visibleSlotCount / 2;
            int startZone = Mathf.Max(1, currentZone - half);

            for (int i = 0; i < _spawnedSlots.Count; i++)
            {
                int zoneNum = startZone + i;
                bool isCurrent = (zoneNum == currentZone);

                _spawnedSlots[i].gameObject.SetActive(true);

                bool isSafe = (zoneNum % 5 == 0 && zoneNum % 30 != 0);
                bool isSuper = (zoneNum % 30 == 0);

                Color textColor = colorNormal;
                if (isSuper) textColor = colorSuper;
                else if (isSafe) textColor = colorSafe;

                _spawnedSlots[i].Setup(zoneNum, isCurrent, textColor);
            }

            UpdateTargetBadges(currentZone);
        }

        private void UpdateTargetBadges(int currentZone)
        {
            int nextSafe = ((currentZone / 5) + 1) * 5;
            if (currentZone % 5 == 0) nextSafe = currentZone;

            int nextSuper = ((currentZone / 30) + 1) * 30;
            if (currentZone % 30 == 0) nextSuper = currentZone;

            if (text_next_safe_zone_value != null)
                text_next_safe_zone_value.text = nextSafe.ToString();

            if (text_next_super_zone_value != null)
                text_next_super_zone_value.text = nextSuper.ToString();
        }

        private void OnDestroy()
        {
            if (ZoneManager.Instance != null)
            {
                ZoneManager.Instance.OnZoneChanged -= UpdateProgressBar;
            }
        }
    }
}