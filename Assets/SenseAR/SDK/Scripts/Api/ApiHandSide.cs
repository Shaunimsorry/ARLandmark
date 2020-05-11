#if  UNITY_ANDROID || UNITY_EDITOR 


namespace SenseARInternal
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
    Justification = "Internal")]
    public enum ApiHandSide
    {
        ARHAND_SIDE_HAND_UNKNOWN = -1,
        ARHAND_SIDE_RIGHT_HAND = 0 ,
        ARHAND_SIDE_LEFT_HAND  = 1,

    }
}

 #endif