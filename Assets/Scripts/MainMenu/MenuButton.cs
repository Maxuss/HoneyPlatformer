using UnityEngine;
using UnityEngine.EventSystems;

namespace MainMenu
{
    public class MenuButton: MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField]
        private AudioClip hover;

        [SerializeField]
        private AudioClip click;

        [SerializeField]
        private NewMenuManager menu;

        public void OnPointerEnter(PointerEventData eventData)
        {
            menu.PlaySound(hover);
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            menu.PlaySound(click);
        }
    }
}