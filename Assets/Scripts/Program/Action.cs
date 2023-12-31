using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace Program
{
    [Serializable]
    public struct ActionInfo
    {
        public string ActionName { get; set; }
        public string ActionDescription { get; set; }
        public ActionValueType ValueType { get; set; }
        [CanBeNull]
        public Type EnumType { get; set; }
        public int MaxFloatValue { get; set; }
        [CanBeNull]
        public string ParameterName { get; set; }
    }

    [Serializable]
    public enum ActionValueType
    {
        Unit,
        Enum,
        Float,
    }

    [Serializable]
    public struct ActionData: IEquatable<ActionData>
    {
        [field: SerializeField]
        public int ActionIndex { get; set; }
        [DoNotSerialize]
        [CanBeNull]
        public object StoredValue { get; set; }

        public bool Equals(ActionData other)
        {
            return ActionIndex == other.ActionIndex && Equals(StoredValue, other.StoredValue);
        }

        public override bool Equals(object obj)
        {
            return obj is ActionData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ActionIndex, StoredValue);
        }
    }
}