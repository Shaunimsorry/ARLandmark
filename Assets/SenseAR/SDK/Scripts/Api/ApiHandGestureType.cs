#if  UNITY_ANDROID || UNITY_EDITOR 


namespace SenseARInternal
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
    Justification = "Internal")]
    public enum ApiHandGestureType
    {
        ARHAND_GESTURE_TYPE_UNKNOWN = -1,
        ARHAND_GESTURE_TYPE_OK = 0,
        ARHAND_GESTURE_TYPE_SCISSORS = 1,
        ARHAND_GESTURE_TYPE_THUMBS_UP = 2,
        ARHAND_GESTURE_TYPE_PAPER = 3,
        ARHAND_GESTURE_TYPE_GUN = 4,
        ARHAND_GESTURE_TYPE_ROCK = 5,
        ARHAND_GESTURE_TYPE_FINGER_HEART = 6,
        ARHAND_GESTURE_TYPE_FINGERTIP = 7,
        ARHAND_GESTURE_TYPE_WELL_PLAYED = 8,
        ARHAND_GESTURE_TYPE_THREE_FINGERS = 9,
        ARHAND_GESTURE_TYPE_FOUR_FINGERS = 10,
        ARHAND_GESTURE_TYPE_I_LOVE_YOU = 11,
        ARHAND_GESTURE_TYPE_INDEX_FINGER_AND_LITTLE_FINGER = 12,
        ARHAND_GESTURE_TYPE_LITTLE_FINGER = 13,
    }
}

 #endif