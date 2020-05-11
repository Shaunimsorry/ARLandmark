#if  UNITY_ANDROID || UNITY_EDITOR 


namespace SenseARInternal
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
    Justification = "Internal")]
    public enum ApiVertexFormat
    {
        Position = 0,
        Position_Normal = 1,
        Position_Normal_Color = 2,
        Not_Valid = 3
    }
}

#endif
