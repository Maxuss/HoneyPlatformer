using I18N;
using TMPro;
using UnityEngine;
using Utils;

namespace MainMenu
{
    public class FilterButton : MonoBehaviour
    {
        [SerializeField]
        private NewMenuManager menu;

        private void Start()
        {
            var dd = GetComponent<TMP_Dropdown>();
            dd.value = SettingManager.Instance.EnablePixels ? 1 : 0;
            dd.onValueChanged.AddListener(lang =>
            {
                SettingManager.Instance.EnablePixels = lang == 1;
                menu.ReloadSaves();
            });
        }
    }
}