using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoCase.UI
{
    public class ZoneSlotUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text_zone_number;
        [SerializeField] private Image image_indicator_frame;
        [SerializeField] private Image image_indicator_arrow;

        public void Setup(int zoneNumber, bool isCurrent, Color textColor)
        {
            if (text_zone_number != null)
            {
                text_zone_number.text = zoneNumber.ToString();
                text_zone_number.color = textColor;
            }

            if (image_indicator_frame != null)
                image_indicator_frame.gameObject.SetActive(isCurrent);

            if (image_indicator_arrow != null)
                image_indicator_arrow.gameObject.SetActive(isCurrent);
        }
    }
}