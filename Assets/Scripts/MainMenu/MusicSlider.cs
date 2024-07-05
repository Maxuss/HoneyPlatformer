using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace MainMenu
{
    public class MusicSlider: MonoBehaviour
    {
        private Slider _slider;
        [SerializeField]
        private NewMenuManager menu;

        private void Start()
        {
            _slider = GetComponent<Slider>();
            _slider.value = SettingManager.Instance.MusicVolume;
            _slider.onValueChanged.AddListener(OnChanged);
        }

        private void OnChanged(float newVal)
        {
            SettingManager.Instance.MusicVolume = newVal;
            menu.AudioSource.volume = newVal;
        }
    }
}