#if  UNITY_ANDROID || UNITY_EDITOR 


namespace SenseARInternal
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
    Justification = "Internal")]
    public enum ARPixelFormat
    {
        UNKNOW = 0,

        LUMINANCE8 = 1,
        RG16 = 2,
        RGB24 = 3,
        RGBA32 = 4,
        DEPTH16 = 5,

        YUV_NV21 = 100,
    }
}

#endif