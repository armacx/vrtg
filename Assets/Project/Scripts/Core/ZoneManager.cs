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

        [Header("Progression Multiplier")]
        [SerializeField] private float rewardMultiplierPerZone = 0.15f;

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
            ZoneConfig baseTemplate;
            if (IsSuperZone(CurrentZone)) baseTemplate = goldSuperZoneTemplate;
            else if (IsSafeZone(CurrentZone)) baseTemplate = silverSafeZoneTemplate;
            else baseTemplate = bronzeZoneTemplate;

            if (baseTemplate == null) return null;

            ZoneConfig runtimeConfig = ScriptableObject.Instantiate(baseTemplate);

            runtimeConfig.zoneNumber = CurrentZone;
            runtimeConfig.isSafeZone = IsSafeZone(CurrentZone);
            runtimeConfig.isSuperZone = IsSuperZone(CurrentZone);

            float multiplier = 1f + ((CurrentZone - 1) * rewardMultiplierPerZone);

            if (runtimeConfig.slices != null)
            {
                List<WheelSliceData> scaledSlices = new List<WheelSliceData>();
                for (int i = 0; i < runtimeConfig.slices.Count; i++)
                {
                    WheelSliceData slice = runtimeConfig.slices[i];
                    WheelSliceData clonedSlice = new WheelSliceData
                    {
                        rewardType = slice.rewardType,
                        icon = slice.icon,
                        dropWeight = slice.dropWeight,
                        amount = slice.rewardType == RewardType.Bomb 
                            ? 0 
                            : Mathf.Max(1, Mathf.RoundToInt(slice.amount * multiplier))
                    };
                    scaledSlices.Add(clonedSlice);
                }
                runtimeConfig.slices = scaledSlices;
            }

            return runtimeConfig;
        }

        private void BroadcastCurrentZone()
        {
            WheelTier tier = GetCurrentTier();
            ZoneConfig config = GetCurrentZoneConfig();
            OnZoneChanged?.Invoke(CurrentZone, tier, config);
        }
    }
}