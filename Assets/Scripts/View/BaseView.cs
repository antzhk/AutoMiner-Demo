using System;
using TMPro;
using UnityEngine;

namespace View
{
    public class BaseView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI baseName;
    
        [SerializeField] private SliderSelector minerCount;
        [SerializeField] private SliderSelector minerSpeed;
        [SerializeField] private FloatInputView generationDelay;
    
        private Base headQuarter;
        
        private static BaseView instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            this.Hide();
        }

        private void AddListeners()
        {
            minerCount.onValueChanged.AddListener(ChangeMiners);
            minerSpeed.onValueChanged.AddListener(ChangeSpeed);
            generationDelay.OnValueChanged.AddListener(ChangeDelay);
        }

        private void RemoveListeners()
        {
            minerCount.onValueChanged.RemoveListener(ChangeMiners);
            minerSpeed.onValueChanged.RemoveListener(ChangeSpeed);
            generationDelay.OnValueChanged.RemoveListener(ChangeDelay);
        }

        public void Show(string baseId)
        {
            this.headQuarter = Base.GetBaseById(baseId);

            this.baseName.text = this.headQuarter.GetId();
            this.AddListeners();

            this.minerCount.CurrentValue = Miner.GetAllBaseMiners(this.headQuarter.GetId()).Count;
            this.minerSpeed.CurrentValue = this.headQuarter.GetMinersSpeed();
        
            this.panel.SetActive(true);
        }

        public void Hide()
        {
            this.RemoveListeners();
            this.headQuarter = null;
            this.panel.SetActive(false);
        }

        public static BaseView Get()
        {
            return instance;
        }
    
        private void ChangeMiners(int count)
        {
            this.headQuarter.ChangeMinersCount(count);
        }

        private void ChangeSpeed(int speed)
        {
            this.headQuarter.ChangeMinerSpeed(speed);
        }

        private void ChangeDelay(float value)
        {
            ResourceSpawner.Get().ChangeDelay(value);
        }
    }
}