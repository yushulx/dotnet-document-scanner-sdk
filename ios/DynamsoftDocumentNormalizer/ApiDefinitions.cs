using System;
using Foundation;
using ObjCRuntime;
using UIKit;
using DynamsoftCore;

namespace Com.Dynamsoft.Ddn
{
	// @interface iDetectedQuadResult : NSObject
	[BaseType(typeof(NSObject))]
	interface iDetectedQuadResult
	{
		// @property (nonatomic) iQuadrilateral * _Nonnull location;
		[Export("location", ArgumentSemantic.Assign)]
		iQuadrilateral Location { get; set; }

		// @property (assign, nonatomic) NSInteger confidenceAsDocumentBoundary;
		[Export("confidenceAsDocumentBoundary")]
		nint ConfidenceAsDocumentBoundary { get; set; }
	}

	// @interface iNormalizedImageResult : NSObject
	[BaseType(typeof(NSObject))]
	interface iNormalizedImageResult
	{
		// @property (nonatomic) iImageData * _Nonnull image;
		[Export("image", ArgumentSemantic.Assign)]
		iImageData Image { get; set; }

		// -(BOOL)saveToFile:(NSString * _Nonnull)filePath error:(NSError * _Nullable * _Nullable)error __attribute__((swift_name("saveToFile(_:)"))) __attribute__((availability(ios, introduced=11.0)));
		[Export("saveToFile:error:")]
		bool SaveToFile(string filePath, [NullAllowed] out NSError error);
	}

	// @protocol DetectResultListener <NSObject>
	/*
	  Check whether adding [Model] to this declaration is appropriate.
	  [Model] is used to generate a C# class that implements this protocol,
	  and might be useful for protocols that consumers are supposed to implement,
	  since consumers can subclass the generated class instead of implementing
	  the generated interface. If consumers are not supposed to implement this
	  protocol, then [Model] is redundant and will generate code that will never
	  be used.
	*/
	[Protocol]
	[BaseType(typeof(NSObject))]
	interface DetectResultListener
	{
		// @required -(void)detectResultCallback:(NSInteger)frameId imageData:(iImageData * _Nonnull)imageData results:(NSArray<iDetectedQuadResult *> * _Nonnull)results;
		[Abstract]
		[Export("detectResultCallback:imageData:results:")]
		void ImageData(nint frameId, iImageData imageData, iDetectedQuadResult[] results);
	}

	// @interface DynamsoftDocumentNormalizer : NSObject
	[BaseType(typeof(NSObject))]
	interface DynamsoftDocumentNormalizer
	{
		// +(NSString * _Nonnull)getVersion;
		[Static]
		[Export("getVersion")]
		string Version { get; }

		// -(BOOL)initRuntimeSettingsFromFile:(NSString * _Nonnull)filePath error:(NSError * _Nullable * _Nullable)error __attribute__((swift_name("initRuntimeSettingsFromFile(_:)")));
		[Export("initRuntimeSettingsFromFile:error:")]
		bool InitRuntimeSettingsFromFile(string filePath, [NullAllowed] out NSError error);

		// -(BOOL)initRuntimeSettingsFromString:(NSString * _Nonnull)JSONString error:(NSError * _Nullable * _Nullable)error __attribute__((swift_name("initRuntimeSettingsFromString(_:)")));
		[Export("initRuntimeSettingsFromString:error:")]
		bool InitRuntimeSettingsFromString(string JSONString, [NullAllowed] out NSError error);

		// -(BOOL)outputRuntimeSettingsToFile:(NSString * _Nonnull)filePath templateName:(NSString * _Nonnull)templateName error:(NSError * _Nullable * _Nullable)error __attribute__((swift_name("outputRuntimeSettingsToFile(_:templateName:)")));
		[Export("outputRuntimeSettingsToFile:templateName:error:")]
		bool OutputRuntimeSettingsToFile(string filePath, string templateName, [NullAllowed] out NSError error);

		// -(NSString * _Nullable)outputRuntimeSettings:(NSString * _Nonnull)templateName error:(NSError * _Nullable * _Nullable)error __attribute__((swift_name("outputRuntimeSettings(_:)")));
		[Export("outputRuntimeSettings:error:")]
		[return: NullAllowed]
		string OutputRuntimeSettings(string templateName, [NullAllowed] out NSError error);

		// -(NSArray<iDetectedQuadResult *> * _Nullable)detectQuadFromFile:(NSString * _Nonnull)sourceFilePath error:(NSError * _Nullable * _Nullable)error __attribute__((swift_name("detectQuadFromFile(_:)")));
		[Export("detectQuadFromFile:error:")]
		[return: NullAllowed]
		iDetectedQuadResult[] DetectQuadFromFile(string sourceFilePath, [NullAllowed] out NSError error);

		// -(NSArray<iDetectedQuadResult *> * _Nullable)detectQuadFromBuffer:(iImageData * _Nonnull)data error:(NSError * _Nullable * _Nullable)error __attribute__((swift_name("detectQuadFromBuffer(_:)")));
		[Export("detectQuadFromBuffer:error:")]
		[return: NullAllowed]
		iDetectedQuadResult[] DetectQuadFromBuffer(iImageData data, [NullAllowed] out NSError error);

		// -(NSArray<iDetectedQuadResult *> * _Nullable)detectQuadFromImage:(UIImage * _Nonnull)image error:(NSError * _Nullable * _Nullable)error __attribute__((swift_name("detectQuadFromImage(_:)")));
		[Export("detectQuadFromImage:error:")]
		[return: NullAllowed]
		iDetectedQuadResult[] DetectQuadFromImage(UIImage image, [NullAllowed] out NSError error);

		// -(iNormalizedImageResult * _Nullable)normalizeFile:(NSString * _Nonnull)sourceFilePath quad:(iQuadrilateral * _Nullable)quad error:(NSError * _Nullable * _Nullable)error __attribute__((swift_name("normalizeFile(_:quad:)")));
		[Export("normalizeFile:quad:error:")]
		[return: NullAllowed]
		iNormalizedImageResult NormalizeFile(string sourceFilePath, [NullAllowed] iQuadrilateral quad, [NullAllowed] out NSError error);

		// -(iNormalizedImageResult * _Nullable)normalizeBuffer:(iImageData * _Nonnull)data quad:(iQuadrilateral * _Nullable)quad error:(NSError * _Nullable * _Nullable)error __attribute__((swift_name("normalizeBuffer(_:quad:)")));
		[Export("normalizeBuffer:quad:error:")]
		[return: NullAllowed]
		iNormalizedImageResult NormalizeBuffer(iImageData data, [NullAllowed] iQuadrilateral quad, [NullAllowed] out NSError error);

		// -(iNormalizedImageResult * _Nullable)normalizeImage:(UIImage * _Nonnull)image quad:(iQuadrilateral * _Nullable)quad error:(NSError * _Nullable * _Nullable)error __attribute__((swift_name("normalizeImage(_:quad:)")));
		[Export("normalizeImage:quad:error:")]
		[return: NullAllowed]
		iNormalizedImageResult NormalizeImage(UIImage image, [NullAllowed] iQuadrilateral quad, [NullAllowed] out NSError error);

		// -(void)setCameraEnhancer:(DynamsoftCameraEnhancer * _Nullable)cameraInstance __attribute__((deprecated("DEPRECATED.")));
		//[Export("setCameraEnhancer:")]
		//void SetCameraEnhancer([NullAllowed] DynamsoftCameraEnhancer cameraInstance);

		// -(void)setImageSource:(id<ImageSource> _Nullable)source;
		[Export("setImageSource:")]
		void SetImageSource([NullAllowed] ImageSource source);

		// -(void)startDetecting;
		[Export("startDetecting")]
		void StartDetecting();

		// -(void)stopDetecting;
		[Export("stopDetecting")]
		void StopDetecting();

		// -(void)setDetectResultListener:(id<DetectResultListener> _Nonnull)listener;
		[Export("setDetectResultListener:")]
		void SetDetectResultListener(DetectResultListener listener);
	}
}