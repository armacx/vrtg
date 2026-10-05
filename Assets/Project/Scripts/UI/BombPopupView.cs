using UnityEngine;
using UnityEngine.UI;
using VertigoCase.Core;

namespace VertigoCase.UI
{
    public class BombPopupView : MonoBehaviour
    {
        [Header("Popup Root")]
        [SerializeField] private GameObject popupRoot;

        [Header("Buttons")]
        [SerializeField] private Button buttonGiveUp;
        [SerializeField] private Button buttonRevive;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (popupRoot == null)
            {
                popupRoot = gameObject;
            }

            if (buttonGiveUp == null || buttonRevive == null)
            {
                var buttons = GetComponentsInChildren(typeof(Button), true);
                foreach (Button btn in buttons)
                {
                    string lowerName = btn.name.ToLower();
                    if (lowerName.Contains("give_up") || lowerName.Contains("giveup"))
                    {
                        buttonGiveUp = btn;
                    }
                    else if (lowerName.Contains("revive"))
                    {
                        buttonRevive = btn;
                    }
                }
            }
        }
#endif

        private void Awake()
        {
            if (buttonGiveUp != null)
                buttonGiveUp.onClick.AddListener(OnGiveUpClicked);

            if (buttonRevive != null)
                buttonRevive.onClick.AddListener(OnReviveClicked);

            Hide();
        }

        public void Show()
        {
            if (popupRoot != null)
                popupRoot.SetActive(true);
        }

        public void Hide()
        {
            if (popupRoot != null)
                popupRoot.SetActive(false);
        }

        private void OnGiveUpClicked()
        {
            Hide();
            InventoryManager.Instance?.ClearCurrentRun();
            ZoneManager.Instance?.StartNewGame();
        }

        private void OnReviveClicked()
        {
            Hide();
            ZoneManager.Instance?.AdvanceNextZone();
        }

        private void OnDestroy()
        {
            if (buttonGiveUp != null)
                buttonGiveUp.onClick.RemoveListener(OnGiveUpClicked);

            if (buttonRevive != null)
                buttonRevive.onClick.RemoveListener(OnReviveClicked);
        }
    }
}