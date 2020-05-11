#if  UNITY_ANDROID || UNITY_EDITOR 


namespace SenseARInternal
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
    Justification = "Internal")]
    public enum ApiHandTowards
    {
        ARHAND_TOWARDS_UNKNOWN = -1,
        ARHAND_TOWARDS_PALM = 0,
        ARHAND_TOWARDS_THE_BACK_OF_HAND = 1,
        ARHAND_TOWARDS_SIDE_HAND = 2,

    }
}

 #endif