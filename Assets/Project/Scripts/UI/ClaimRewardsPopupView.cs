using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class ClaimRewardsPopupView : MonoBehaviour
    {
        [Header("Popup Root")]
        [SerializeField] private GameObject popupRoot;

        [Header("UI References")]
        [SerializeField] private Transform containerClaimItems;
        [SerializeField] private RewardItemUI rewardItemPrefab;
        [SerializeField] private Button buttonClaim;

        private readonly List<RewardItemUI> _spawnedItems = new List<RewardItemUI>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (popupRoot == null)
            {
                popupRoot = gameObject;
            }

            if (buttonClaim == null)
            {
                Button[] buttons = GetComponentsInChildren<Button>(true);
                foreach (Button btn in buttons)
                {
                    string lower = btn.name.ToLower();
                    if (lower.Contains("claim") || lower.Contains("collect") || lower.Contains("ok"))
                    {
                        buttonClaim = btn;
                        break;
                    }
                }
            }
        }
#endif

        private void Awake()
        {
            if (buttonClaim != null)
                buttonClaim.onClick.AddListener(OnClaimButtonClicked);

            Hide();
        }

        public void Show()
        {
            ClearItems();

            if (InventoryManager.Instance != null && rewardItemPrefab != null && containerClaimItems != null)
            {
                Dictionary<string, int> rewards = InventoryManager.Instance.GetAllRewards();
                Dictionary<string, Sprite> sprites = InventoryManager.Instance.GetAllSprites();

                foreach (KeyValuePair<string, int> kvp in rewards)
                {
                    sprites.TryGetValue(kvp.Key, out Sprite icon);
                    RewardItemUI item = Instantiate(rewardItemPrefab, containerClaimItems);
                    item.Setup(RewardType.Item, kvp.Value, icon);
                    _spawnedItems.Add(item);
                }
            }

            if (popupRoot != null)
                popupRoot.SetActive(true);
        }

        public void Hide()
        {
            ClearItems();
            if (popupRoot != null)
                popupRoot.SetActive(false);
        }

        private void ClearItems()
        {
            for (int i = 0; i < _spawnedItems.Count; i++)
            {
                if (_spawnedItems[i] != null)
                    Destroy(_spawnedItems[i].gameObject);
            }
            _spawnedItems.Clear();
        }

        private void OnClaimButtonClicked()
        {
            Hide();
            InventoryManager.Instance?.CollectAndClaimRewards();
            ZoneManager.Instance?.StartNewGame();
        }

        private void OnDestroy()
        {
            if (buttonClaim != null)
                buttonClaim.onClick.RemoveListener(OnClaimButtonClicked);
        }
    }
}