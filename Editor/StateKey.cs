using System;

namespace CreatorKitTriggerEditor.Editor
{
    public readonly struct StateKey : IEquatable<StateKey>
    {
        public Target Target { get; }
        public string Key { get; }

        public StateKey(Target target, string key)
        {
            Target = target;
            Key = key;
        }

        public bool Equals(StateKey other)
        {
            return Target == other.Target && Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            return obj is StateKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Target * 397) ^ (Key != null ? Key.GetHashCode() : 0);
            }
        }
    }
}