using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace View
{
    using UnityEngine;
    using UnityEngine.UI;

    public class SliderSelector : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI valueLabel;

        [SerializeField] private int min = 1;
        [SerializeField] private int max = 100;

        public UnityEvent<int> onValueChanged;

        public int CurrentValue
        {
            get => Mathf.RoundToInt(slider.value);
            set
            {
                int clamped = Mathf.Clamp(value, min, max);
                slider.value = clamped;
                valueLabel.text = $"{value}";
            }
        }

        private void Awake()
        {
            slider.minValue = min;
            slider.maxValue = max;
            slider.wholeNumbers = true;
            slider.onValueChanged.AddListener(OnSliderChanged);

            UpdateLabelAndInvoke(slider.value);
        }

        private void OnSliderChanged(float value)
        {
            int intVal = Mathf.RoundToInt(value);
            UpdateLabelAndInvoke(intVal);
        }

        private void UpdateLabelAndInvoke(float value)
        {
            int intVal = Mathf.RoundToInt(value);
            valueLabel.text = $"{intVal}";
            onValueChanged?.Invoke(intVal);
        }
    }

}