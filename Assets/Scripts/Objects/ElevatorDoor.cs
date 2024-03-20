using UnityEngine;

namespace Objects
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