using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Program.UI
{
    public class ParameterSlider: MonoBehaviour
    {
        [FormerlySerializedAs("minText")] [SerializeField]
        private TMP_Text currentValueText;

        public void HandleUpdate(float newValue)
        {
            currentValueText.text = Mathf.RoundToInt(newValue).ToString();
        }
    }
}