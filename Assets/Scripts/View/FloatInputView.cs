using TMPro;

namespace View
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;

    public class FloatInputView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private float min = 0f;
        [SerializeField] private float max = 100f;
        [SerializeField] private int decimals = 2;

        [System.Serializable]
        public class FloatEvent : UnityEvent<float> { }

        public FloatEvent OnValueChanged = new FloatEvent();

        private float currentValue;

        public float Value
        {
            get => currentValue;
            set => SetValue(value, true);
        }

        private void Awake()
        {
            inputField.onEndEdit.AddListener(OnInputChanged);
            SetValue(currentValue, false);
        }

        private void OnInputChanged(string input)
        {
            if (float.TryParse(input.Replace(',', '.'), out float val))
            {
                val = Mathf.Clamp(val, min, max);
                SetValue(val);
            }
            else
            {
                inputField.text = currentValue.ToString($"F{decimals}");
            }
        }

        private void SetValue(float value, bool notify = true)
        {
            value = Mathf.Clamp(value, min, max);
            currentValue = value;
            inputField.text = value.ToString($"F{decimals}");

            if (notify)
                OnValueChanged.Invoke(value);
        }
    }

}