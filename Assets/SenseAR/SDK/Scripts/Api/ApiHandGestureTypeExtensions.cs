#if  UNITY_ANDROID || UNITY_EDITOR 


namespace SenseARInternal
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using SenseAR;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
    Justification = "Internal")]
    public static class ApiHandGestureTypeExtensions
    {
        public static HandGestureType ToHandGestureType(this ApiHandGestureType apiHandGestureType)
        {
            switch (apiHandGestureType)
            {
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_UNKNOWN:
                    return HandGestureType.ARHAND_GESTURE_TYPE_UNKNOWN;
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_OK:
                    return HandGestureType.ARHAND_GESTURE_TYPE_OK;
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_SCISSORS:
                    return HandGestureType.ARHAND_GESTURE_TYPE_SCISSORS;
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_THUMBS_UP:
                    return HandGestureType.ARHAND_GESTURE_TYPE_THUMBS_UP;
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_PAPER:
                    return HandGestureType.ARHAND_GESTURE_TYPE_PAPER; 
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_GUN:
                    return HandGestureType.ARHAND_GESTURE_TYPE_GUN;
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_ROCK:
                    return HandGestureType.ARHAND_GESTURE_TYPE_ROCK;
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_FINGER_HEART:
                    return HandGestureType.ARHAND_GESTURE_TYPE_FINGER_HEART;   
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_FINGERTIP:
                    return HandGestureType.ARHAND_GESTURE_TYPE_FINGERTIP;
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_WELL_PLAYED:
                    return HandGestureType.ARHAND_GESTURE_TYPE_WELL_PLAYED;
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_THREE_FINGERS:
                    return HandGestureType.ARHAND_GESTURE_TYPE_THREE_FINGERS;
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_FOUR_FINGERS:
                    return HandGestureType.ARHAND_GESTURE_TYPE_FOUR_FINGERS;
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_I_LOVE_YOU:
                    return HandGestureType.ARHAND_GESTURE_TYPE_I_LOVE_YOU;
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_INDEX_FINGER_AND_LITTLE_FINGER:
                    return HandGestureType.ARHAND_GESTURE_TYPE_INDEX_FINGER_AND_LITTLE_FINGER;
                case ApiHandGestureType.ARHAND_GESTURE_TYPE_LITTLE_FINGER:
                    return HandGestureType.ARHAND_GESTURE_TYPE_LITTLE_FINGER;
                default:
                    return HandGestureType.ARHAND_GESTURE_TYPE_UNKNOWN;
            }
        }
    }
}

 #endif