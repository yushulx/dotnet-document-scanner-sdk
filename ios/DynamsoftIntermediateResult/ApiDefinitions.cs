using Foundation;

namespace Com.Dynamsoft.IntermediateResult
{
    // @interface DynamsoftIntermediateResult : NSObject
    [BaseType(typeof(NSObject))]
    interface DynamsoftIntermediateResult
    {
        // +(NSString * _Nullable)getVersion;
        [Static]
        [NullAllowed, Export("getVersion")]
        string Version { get; }
    }

}
