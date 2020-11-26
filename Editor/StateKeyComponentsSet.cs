using System;
using UnityEngine;

namespace CreatorKitTriggerEditor.Editor
{
    public readonly struct StateKeyComponentsSet : IEquatable<StateKeyComponentsSet>
    {
        public StateKey StateKey { get; }
        public Component[] Components { get; }

        public StateKeyComponentsSet(StateKey stateKey, Component[] components)
        {
            StateKey = stateKey;
            Components = components;
        }

        public bool Equals(StateKeyComponentsSet other)
        {
            return StateKey.Equals(other.StateKey) && Equals(Components, other.Components);
        }

        public override bool Equals(object obj)
        {
            return obj is StateKeyComponentsSet other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StateKey.GetHashCode() * 397) ^ (Components != null ? Components.GetHashCode() : 0);
            }
        }
    }
}