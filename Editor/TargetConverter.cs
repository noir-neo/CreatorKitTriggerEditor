using System;
using System.Collections.Generic;
using ClusterVR.CreatorKit.Gimmick;
using ClusterVR.CreatorKit.Operation;

namespace CreatorKitTriggerEditor.Editor
{
    public static class TargetConverter
    {

        public static IEnumerable<Target> Convert(ClusterVR.CreatorKit.Trigger.TriggerTarget target)
        {
            switch (target)
            {
                case ClusterVR.CreatorKit.Trigger.TriggerTarget.Item:
                case ClusterVR.CreatorKit.Trigger.TriggerTarget.SpecifiedItem:
                    return new []{ Target.Item };
                case ClusterVR.CreatorKit.Trigger.TriggerTarget.Player:
                    return new []{ Target.Player };
                case ClusterVR.CreatorKit.Trigger.TriggerTarget.CollidedItemOrPlayer:
                    return new []{ Target.Item, Target.Player };
                case ClusterVR.CreatorKit.Trigger.TriggerTarget.Global:
                    return new []{ Target.Global };
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }

        public static Target Convert(TargetStateTarget target)
        {
            switch (target)
            {
                case TargetStateTarget.Item:
                    return Target.Item;
                case TargetStateTarget.Player:
                    return Target.Player;
                case TargetStateTarget.Global:
                    return Target.Global;
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }

        public static Target Convert(GimmickTarget target)
        {
            switch (target)
            {
                case GimmickTarget.Item:
                    return Target.Item;
                case GimmickTarget.Player:
                    return Target.Player;
                case GimmickTarget.Global:
                    return Target.Global;
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }
    }
}