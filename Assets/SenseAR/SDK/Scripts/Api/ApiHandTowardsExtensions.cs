#if  UNITY_ANDROID || UNITY_EDITOR 


namespace SenseARInternal
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using SenseAR;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
    Justification = "Internal")]
    public static class ApiHandTowardsExtensions
    {
        public static HandTowards ToHandTowards(this ApiHandTowards ApiHandTowards)
        {
            switch (ApiHandTowards)
            {
                case ApiHandTowards.ARHAND_TOWARDS_UNKNOWN:
                    return HandTowards.ARHAND_TOWARDS_UNKNOWN;
                case ApiHandTowards.ARHAND_TOWARDS_PALM:
                    return HandTowards.ARHAND_TOWARDS_PALM;
                case ApiHandTowards.ARHAND_TOWARDS_THE_BACK_OF_HAND:
                    return HandTowards.ARHAND_TOWARDS_THE_BACK_OF_HAND;
                case ApiHandTowards.ARHAND_TOWARDS_SIDE_HAND:
                    return HandTowards.ARHAND_TOWARDS_SIDE_HAND;
                default:
                    return HandTowards.ARHAND_TOWARDS_UNKNOWN;
            }
        }
    }
}

 #endif