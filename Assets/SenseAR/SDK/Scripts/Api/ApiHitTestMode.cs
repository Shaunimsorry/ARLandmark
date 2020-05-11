#if  UNITY_ANDROID || UNITY_EDITOR 


namespace SenseARInternal
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
    Justification = "Internal")]
    public enum ApiHitTestMode
    {
        /// frame hit test with polygon plane only
        PolygonOnly = 0,

        /// frame hit test with polygon plane and zero horizontal plane
        PolygonAndHorizonPlane = 1,

        /// frame hit test with polygon infinite plane and persist until
        /// intersects with new polygon.
        PolygonPersistence = 2,
    }
}

 #endif