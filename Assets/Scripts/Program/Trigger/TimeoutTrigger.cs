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

        public string Name => $"Wait for {_timeoutSeconds} seconds";

        public bool ShouldTrigger(Programmable programmable)
        {
            return !_activated && Time.time - _timeInit > _timeoutSeconds;
        }

        public void Start()
        {
            _timeInit = Time.time;
            _activated = false;
        }

        public ITrigger Copy()
        {
            return new TimeoutTrigger(_timeoutSeconds);
        }

        public void Activated()
        {
            _activated = true;
        }
    }
}