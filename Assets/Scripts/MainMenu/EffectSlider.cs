using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace MainMenu
{
    public class EffectSlider: MonoBehaviour
    {
        private Slider _slider;

        private void Start()
        {
            _slider = GetComponent<Slider>();
            _slider.value = SettingManager.Instance.SfxVolume;
            _slider.onValueChanged.AddListener(OnChanged);
        }

        private void OnChanged(float newVal)
        {
            SettingManager.Instance.SfxVolume = newVal;
        }
    }
}