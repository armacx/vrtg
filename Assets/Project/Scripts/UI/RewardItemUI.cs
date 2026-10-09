using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class RewardItemUI : MonoBehaviour
    {
        [SerializeField] private Image icon_reward;
        [SerializeField] private TextMeshProUGUI text_amount_value;

        public RewardType Type { get; private set; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (icon_reward == null)
            {
                var imgs = GetComponentsInChildren<Image>(true);
                foreach (var img in imgs)
                {
                    if (img.name.ToLower().Contains("icon"))
                    {
                        icon_reward = img;
                        break;
                    }
                }
            }

            if (text_amount_value == null)
            {
                text_amount_value = GetComponentInChildren<TextMeshProUGUI>(true);
            }
        }
#endif

        public void Setup(RewardType type, int amount, Sprite icon)
        {
            Type = type;
            if (icon_reward != null && icon != null)
                icon_reward.sprite = icon;

            UpdateAmount(amount);
        }

        public void UpdateAmount(int amount)
        {
            if (text_amount_value != null)
                text_amount_value.text = amount >= 1000 ? $"{amount:N0}" : amount.ToString();
        }
    }
}