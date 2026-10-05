using System.Collections.Generic;
using UnityEngine;

namespace VertigoCase.Data
{
    [CreateAssetMenu(fileName = "Zone_1_Config", menuName = "VertigoCase/Zone Config")]
    public class ZoneConfig : ScriptableObject
    {
        [Header("Zone Settings")]
        public int zoneNumber;
        public WheelTier wheelTier;
        public bool isSafeZone;
        public bool isSuperZone;

        [Header("Visuals")]
        public Sprite wheelBaseSprite;
        public Sprite wheelIndicatorSprite;

        [Header("Wheel Slices (8 Slices Expected)")]
        public List<WheelSliceData> slices = new List<WheelSliceData>();
    }
}