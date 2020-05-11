#if  UNITY_ANDROID || UNITY_EDITOR 


namespace SenseARInternal
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using SenseAR;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
    Justification = "Internal")]
    public static class ApiHandSideExtensions
    {
        public static HandSide ToHandSide(this ApiHandSide apiHandSide)
        {
            switch (apiHandSide)
            {
                case ApiHandSide.ARHAND_SIDE_HAND_UNKNOWN:
                    return HandSide.ARHAND_SIDE_HAND_UNKNOWN;
                case ApiHandSide.ARHAND_SIDE_LEFT_HAND:
                    return HandSide.ARHAND_SIDE_LEFT_HAND;
                case ApiHandSide.ARHAND_SIDE_RIGHT_HAND:
                    return HandSide.ARHAND_SIDE_RIGHT_HAND;
                default:
                    return HandSide.ARHAND_SIDE_HAND_UNKNOWN;
            }
        }
    }
}

 #endif