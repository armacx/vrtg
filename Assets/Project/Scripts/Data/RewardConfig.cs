using System;
using UnityEngine;

namespace VertigoCase.Data
{
    public enum RewardType
    {
        Cash,
        Gold,
        Chest,
        Item,
        Bomb
    }

    public enum WheelTier
    {
        Bronze,
        Silver,
        Gold
    }

    [Serializable]
    public class WheelSliceData
    {
        public RewardType rewardType;
        public Sprite icon;
        public int amount;
        
        [Range(0f, 100f)]
        public float dropWeight = 10f;
    }
}