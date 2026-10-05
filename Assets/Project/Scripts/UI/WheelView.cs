using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class WheelView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image wheelBaseImage;
        [SerializeField] private Image wheelIndicatorImage;
        [SerializeField] private Transform spinnerTransform;
        [SerializeField] private WheelSliceView slicePrefab;
        [SerializeField] private Transform sliceContainer;

        [Header("Settings")]
        [SerializeField] private float sliceRadius = 220f;
        [SerializeField] private float spinDuration = 4f;
        [SerializeField] private int minFullTurns = 5;

        private readonly List<WheelSliceView> _spawnedSlices = new List<WheelSliceView>();
        private Tween _spinTween;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (wheelBaseImage == null || spinnerTransform == null || sliceContainer == null)
            {
                Transform spinner = transform.Find("ui_wheel_spinner");
                if (spinner != null)
                {
                    if (wheelBaseImage == null)
                        wheelBaseImage = spinner.GetComponent<Image>();

                    if (spinnerTransform == null)
                        spinnerTransform = spinner;

                    if (sliceContainer == null)
                        sliceContainer = spinner;
                }
            }

            if (wheelIndicatorImage == null)
            {
                Transform indicator = transform.Find("ui_wheel_indicator");
                if (indicator != null)
                {
                    wheelIndicatorImage = indicator.GetComponent<Image>();
                }
            }
        }
#endif

        public void InitializeWheel(ZoneConfig config)
        {
            if (config == null) return;

            if (wheelBaseImage != null && config.wheelBaseSprite != null)
                wheelBaseImage.sprite = config.wheelBaseSprite;

            if (wheelIndicatorImage != null && config.wheelIndicatorSprite != null)
                wheelIndicatorImage.sprite = config.wheelIndicatorSprite;

            ClearSlices();

            int sliceCount = config.slices.Count;
            if (sliceCount == 0) return;

            float angleStep = 360f / sliceCount;

            for (int i = 0; i < sliceCount; i++)
            {
                WheelSliceView slice = Instantiate(slicePrefab, sliceContainer);
                _spawnedSlices.Add(slice);

                float angle = i * angleStep;
                slice.transform.localRotation = Quaternion.Euler(0f, 0f, -angle);

                Vector3 position = Quaternion.Euler(0f, 0f, -angle) * Vector3.up * sliceRadius;
                slice.transform.localPosition = position;

                slice.Setup(config.slices[i]);
            }
        }

        public void SpinToSlice(int targetSliceIndex, int totalSlices, Action onComplete)
        {
            _spinTween?.Kill();

            float anglePerSlice = 360f / totalSlices;
            float targetNormalizedAngle = targetSliceIndex * anglePerSlice;
            float currentNormalizedAngle = Mathf.Repeat(spinnerTransform.localEulerAngles.z, 360f);

            float angleDifference = targetNormalizedAngle - currentNormalizedAngle;
            if (angleDifference <= 0f)
            {
                angleDifference += 360f;
            }

            float targetAngle = spinnerTransform.localEulerAngles.z + (minFullTurns * 360f) + angleDifference;

            _spinTween = spinnerTransform.DOLocalRotate(new Vector3(0f, 0f, targetAngle), spinDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                });
        }

        private void ClearSlices()
        {
            foreach (var slice in _spawnedSlices)
            {
                if (slice != null)
                    Destroy(slice.gameObject);
            }
            _spawnedSlices.Clear();
        }

        private void OnDestroy()
        {
            _spinTween?.Kill();
        }
    }
}