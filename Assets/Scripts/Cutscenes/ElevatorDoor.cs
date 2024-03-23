using UnityEngine;

namespace Cutscenes
{
    public class ElevatorDoor: MonoBehaviour
    {
        public void Close()
        {
            GetComponent<Animator>().Play("Close");
        }

        public void Open()
        {
            GetComponent<Animator>().Play("Open");
        }
    }
}