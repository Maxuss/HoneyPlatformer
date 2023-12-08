using UnityEngine;

namespace Program
{
    [RequireComponent(typeof(IActionContainer))]
    public class BlacklistActions: MonoBehaviour
    {
        /// <summary>
        /// These actions will be blacklisted from selecting and configuring only
        /// for this object instance.
        /// </summary>
        public int[] BlacklistedActions;
    }
}