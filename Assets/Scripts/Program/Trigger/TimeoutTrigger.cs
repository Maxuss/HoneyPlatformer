using UnityEngine;

namespace Program.Trigger
{
    public class TimeoutTrigger: ITrigger
    {

        private readonly float _timeoutSeconds;
        private float _timeInit;
        private bool _activated;

        public TimeoutTrigger(float timeoutSeconds)
        {
            _timeoutSeconds = timeoutSeconds;
        }

        public bool ShouldTrigger(Programmable programmable)
        {
            return !_activated && Time.time - _timeInit > _timeoutSeconds;
        }

        public void Start()
        {
            _timeInit = Time.time;
            _activated = false;
        }

        public void Activated()
        {
            _activated = true;
        }
    }
}