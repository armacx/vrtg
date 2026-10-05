using System;
using System.Collections.Generic;
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        public event Action<RewardType, int> OnRewardAdded;
        public event Action OnInventoryCleared;

        private readonly Dictionary<RewardType, int> _currentRunRewards = new Dictionary<RewardType, int>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void AddReward(RewardType type, int amount)
        {
            if (type == RewardType.Bomb || amount <= 0) return;

            if (_currentRunRewards.ContainsKey(type))
                _currentRunRewards[type] += amount;
            else
                _currentRunRewards[type] = amount;

            OnRewardAdded?.Invoke(type, _currentRunRewards[type]);
        }

        public int GetRewardAmount(RewardType type)
        {
            return _currentRunRewards.TryGetValue(type, out int amount) ? amount : 0;
        }

        public Dictionary<RewardType, int> GetAllRewards()
        {
            return new Dictionary<RewardType, int>(_currentRunRewards);
        }

        public void ClearCurrentRun()
        {
            _currentRunRewards.Clear();
            OnInventoryCleared?.Invoke();
        }
    }
}