using Foundation;

namespace Com.Dynamsoft.ImageProcessing
{
	// @interface DynamsoftImageProcessing : NSObject
	[BaseType(typeof(NSObject))]
	interface DynamsoftImageProcessing
	{
		// +(NSString * _Nullable)getVersion;
		[Static]
		[NullAllowed, Export("getVersion")]
		string Version { get; }
	}
}
