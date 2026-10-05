using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class WheelSliceView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI amountText;

        public void Setup(WheelSliceData data)
        {
            if (data == null) return;

            if (iconImage != null)
            {
                iconImage.sprite = data.icon;
                iconImage.enabled = data.icon != null;
            }

            if (amountText != null)
            {
                if (data.rewardType == RewardType.Bomb || data.amount <= 1)
                {
                    amountText.text = string.Empty;
                }
                else
                {
                    amountText.text = $"x{data.amount}";
                }
            }
        }
    }
}