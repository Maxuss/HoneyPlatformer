using System;
using UnityEngine;

namespace DonDomain.BG
{
    public abstract class DonBackground: MonoBehaviour
    {
        public abstract void Change(BackgroundTweener bg);

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("DonBackground"))
                BackgroundChangeHandler.Instance.SignalEnter(this);
        }
    }
}