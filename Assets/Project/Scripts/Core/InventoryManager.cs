using System;
using System.Collections.Generic;
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [SerializeField] private int startingGold = 100;

        public event Action<string, int, Sprite> OnRewardAdded;
        public event Action OnInventoryCleared;
        public event Action<int> OnTotalGoldChanged;

        private readonly Dictionary<string, int> _rewardAmounts = new Dictionary<string, int>();
        private readonly Dictionary<string, Sprite> _rewardSprites = new Dictionary<string, Sprite>();
        private int _totalGold;

        public int TotalGold => _totalGold;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                _totalGold = startingGold;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            OnTotalGoldChanged?.Invoke(_totalGold);
        }

        public void AddReward(RewardType type, int amount, Sprite icon)
        {
            if (type == RewardType.Bomb || amount <= 0) return;

            string key = icon != null ? icon.name : type.ToString();

            if (_rewardAmounts.ContainsKey(key))
                _rewardAmounts[key] += amount;
            else
            {
                _rewardAmounts[key] = amount;
                _rewardSprites[key] = icon;
            }

            OnRewardAdded?.Invoke(key, _rewardAmounts[key], icon);
        }

        public bool HasEnoughGold(int amount)
        {
            return _totalGold >= amount;
        }

        public bool SpendGold(int amount)
        {
            if (!HasEnoughGold(amount)) return false;

            _totalGold -= amount;
            OnTotalGoldChanged?.Invoke(_totalGold);
            return true;
        }

        public void CollectAndClaimRewards()
        {
            foreach (var kvp in _rewardAmounts)
            {
                if (kvp.Key.ToLower().Contains("gold"))
                {
                    _totalGold += kvp.Value;
                }
            }

            OnTotalGoldChanged?.Invoke(_totalGold);
            ClearCurrentRun();
        }

        public Dictionary<string, int> GetAllRewards()
        {
            return new Dictionary<string, int>(_rewardAmounts);
        }

        public Dictionary<string, Sprite> GetAllSprites()
        {
            return new Dictionary<string, Sprite>(_rewardSprites);
        }

        public void ClearCurrentRun()
        {
            _rewardAmounts.Clear();
            _rewardSprites.Clear();
            OnInventoryCleared?.Invoke();
        }
    }
}