using System;
using System.Collections.Generic;
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    public enum WheelTier
    {
        Bronze,
        Silver,
        Gold
    }

    public class ZoneManager : MonoBehaviour
    {
        public static ZoneManager Instance { get; private set; }

        [Header("Zone Configurations")]
        [SerializeField] private ZoneConfig bronzeZoneTemplate;
        [SerializeField] private ZoneConfig silverSafeZoneTemplate;
        [SerializeField] private ZoneConfig goldSuperZoneTemplate;

        public int CurrentZone { get; private set; } = 1;

        public event Action<int, WheelTier, ZoneConfig> OnZoneChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void StartNewGame()
        {
            CurrentZone = 1;
            BroadcastCurrentZone();
        }

        public void AdvanceNextZone()
        {
            CurrentZone++;
            BroadcastCurrentZone();
        }

        public bool IsSafeZone(int zone) => zone % 5 == 0 && zone % 30 != 0;
        public bool IsSuperZone(int zone) => zone % 30 == 0;

        public WheelTier GetCurrentTier()
        {
            if (IsSuperZone(CurrentZone)) return WheelTier.Gold;
            if (IsSafeZone(CurrentZone)) return WheelTier.Silver;
            return WheelTier.Bronze;
        }

        public ZoneConfig GetCurrentZoneConfig()
        {
            if (IsSuperZone(CurrentZone)) return goldSuperZoneTemplate;
            if (IsSafeZone(CurrentZone)) return silverSafeZoneTemplate;
            return bronzeZoneTemplate;
        }

        private void BroadcastCurrentZone()
        {
            WheelTier tier = GetCurrentTier();
            ZoneConfig config = GetCurrentZoneConfig();
            OnZoneChanged?.Invoke(CurrentZone, tier, config);
        }
    }
}