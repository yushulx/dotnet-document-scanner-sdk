#import <Foundation/Foundation.h>

#import <DynamsoftCore/DynamsoftCore.h>

@class DynamsoftCameraEnhancer;

NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM(NSInteger, EnumDDNErrorCode) {

    DDN_CONTENT_NOT_FOUND                   = -50001,

    DM_QUADRILATERAL_INVALID                = -10057,

    DM_IMAGE_ORIENTATION_INVALID            = -10060
};
/** 
 * The detected result of the library.
 * An iDetectedQuadResult can be return by detect*** methods.
 */
@interface iDetectedQuadResult : NSObject

/** The location information of the detected quadrilateral.*/
@property (nonatomic, nonnull) iQuadrilateral *location;

/** The confidence as document boundary.*/
@property (nonatomic, assign) NSInteger confidenceAsDocumentBoundary;

@end // iDetectedQuadResult

/** 
 * The normalize result of the library.
 * An iNormalizedImageResult can be return by normalize*** methods.
 */
@interface iNormalizedImageResult : NSObject

/** The normalized image.*/
@property (nonatomic, nonnull) iImageData *image;

/**
 * Save the normalized image to the target file format.
 *
 * @param [in] filePath The file path where you want to out put the file. You can also specify the file format in the NSString.
 * @param [in,out] error Input a pointer to an error object. If an error occurs, this pointer is set to an actual error object containing the error information. You may specify nil for this parameter if you do not want the error information.
 */
- (BOOL)saveToFile:(NSString *)filePath error:(NSError *_Nullable *_Nullable)error NS_SWIFT_NAME(saveToFile(_:)) API_AVAILABLE(ios(11.0));

@end // iNormalizedImageResult

/** The protocol to handle callbacks when the detection results are returned.*/
@protocol DetectResultListener <NSObject>

@required
/**
 * The callback method to handle the detection results returned by the library.
 * It is triggered each time when iDetectedQuadResult is output.
 *
 * @param [in] frameId The ID of the frame.
 * @param [in] imageData The image data of frame.
 * @param [in] results Detected results of the frame.
 */
- (void)detectResultCallback:(NSInteger)frameId
                   imageData:(iImageData *)imageData
                     results:(NSArray<iDetectedQuadResult *> *)results;

@end // DetectResultListener

@interface DynamsoftDocumentNormalizer : NSObject

/** Initialize a instance of DynamsoftDocumentNormalizer.*/
- (instancetype)init;

/** Get version information of SDK.*/
+ (NSString*)getVersion;

/**
 * Initialize runtime settings from a given JSON file.
 *
 * @param [in] filePath The file path where you locate your JSON template file.
 * @param [in,out] error Input a pointer to an error object. If an error occurs, this pointer is set to an actual error object containing the error information. You may specify nil for this parameter if you do not want the error information.
 */
- (BOOL)initRuntimeSettingsFromFile:(nonnull NSString *)filePath
                         error:(NSError *_Nullable *_Nullable)error NS_SWIFT_NAME(initRuntimeSettingsFromFile(_:));

/**
 * Initialize runtime settings from a given JSON string.
 *
 * @param [in] JSONString A JSON string.
 * @param [in,out] error Input a pointer to an error object. If an error occurs, this pointer is set to an actual error object containing the error information. You may specify nil for this parameter if you do not want the error information.
 */
- (BOOL)initRuntimeSettingsFromString:(nonnull NSString *)JSONString
                           error:(NSError *_Nullable *_Nullable)error NS_SWIFT_NAME(initRuntimeSettingsFromString(_:));

/**
 * Output the settings to a JSON file.
 *
 * @param [in] filePath The file path where you want to output your JSON file.
 * @param [in] templateName A unique name for declaring runtime settings.
 * @param [in,out] error Input a pointer to an error object. If an error occurs, this pointer is set to an actual error object containing the error information. You may specify nil for this parameter if you do not want the error information.
 */
- (BOOL)outputRuntimeSettingsToFile:(nonnull NSString *)filePath
                templateName:(nonnull NSString*)templateName
                       error:(NSError *_Nullable *_Nullable)error NS_SWIFT_NAME(outputRuntimeSettingsToFile(_:templateName:));

/**
 * Output the settings to a JSON string.
 *
 * @param [in] templateName A unique name for declaring runtime settings.
 * @param [in,out] error Input a pointer to an error object. If an error occurs, this pointer is set to an actual error object containing the error information. You may specify nil for this parameter if you do not want the error information.
 */
- (NSString *_Nullable)outputRuntimeSettings:(nonnull NSString*)templateName
                       error:(NSError *_Nullable *_Nullable)error NS_SWIFT_NAME(outputRuntimeSettings(_:));

/**
 * Detect quad from an image file.
 *
 * @param [in] sourceFilePath A string defining the file path. It supports BMP, TIFF, JPG, PNG files.
 * @param [in,out] error Input a pointer to an error object. If an error occurs, this pointer is set to an actual error object containing the error information. You may specify nil for this parameter if you do not want the error information.
 */
- (NSArray<iDetectedQuadResult *> *_Nullable)detectQuadFromFile:(nonnull NSString *)sourceFilePath
                                           error:(NSError *_Nullable *_Nullable)error NS_SWIFT_NAME(detectQuadFromFile(_:));

/**
 * Detect quad from the memory buffer containing image pixels in defined format.
 *
 * @param [in] data The memory buffer containing image pixels in defined format.
 * @param [in,out] error Input a pointer to an error object. If an error occurs, this pointer is set to an actual error object containing the error information. You may specify nil for this parameter if you do not want the error information.
 */
- (NSArray<iDetectedQuadResult *> *_Nullable)detectQuadFromBuffer:(nonnull iImageData *)data
                                             error:(NSError *_Nullable *_Nullable)error NS_SWIFT_NAME(detectQuadFromBuffer(_:));

/**
 * Detect quad from a buffered image (UIImage).
 *
 * @param [in] image The UIImage to be detected.
 * @param [in,out] error Input a pointer to an error object. If an error occurs, this pointer is set to an actual error object containing the error information. You may specify nil for this parameter if you do not want the error information.
 */
- (NSArray<iDetectedQuadResult *> *_Nullable)detectQuadFromImage:(nonnull UIImage *)image
                                            error:(NSError *_Nullable *_Nullable)error NS_SWIFT_NAME(detectQuadFromImage(_:));

/**
 * Normalize an image file.
 *
 * @param [in] sourceFilePath A string defining the file path. It supports BMP, TIFF, JPG, PNG files.
 * @param [in] quad The detected quad for normalizing.
 * @param [in,out] error Input a pointer to an error object. If an error occurs, this pointer is set to an actual error object containing the error information. You may specify nil for this parameter if you do not want the error information.
 */
- (iNormalizedImageResult *_Nullable)normalizeFile:(nonnull NSString *)sourceFilePath
                        quad:(nullable iQuadrilateral *)quad
                       error:(NSError *_Nullable *_Nullable)error NS_SWIFT_NAME(normalizeFile(_:quad:));

/**
 * Normalize image from the memory buffer containing image pixels in defined format.
 *
 * @param [in] data The memory buffer containing image pixels in defined format.
 * @param [in] quad The detected quad for normalizing.
 * @param [in,out] error Input a pointer to an error object. If an error occurs, this pointer is set to an actual error object containing the error information. You may specify nil for this parameter if you do not want the error information.
 */
- (iNormalizedImageResult *_Nullable)normalizeBuffer:(nonnull iImageData *)data
                          quad:(nullable iQuadrilateral *)quad
                         error:(NSError *_Nullable *_Nullable)error NS_SWIFT_NAME(normalizeBuffer(_:quad:));

/**
 * Normalize a buffered image (UIImage).
 *
 * @param [in] image The UIImage to be normalized.
 * @param [in] quad The detected quad for normalizing.
 * @param [in,out] error Input a pointer to an error object. If an error occurs, this pointer is set to an actual error object containing the error information. You may specify nil for this parameter if you do not want the error information.
 */
- (iNormalizedImageResult *_Nullable)normalizeImage:(nonnull UIImage *)image
                         quad:(nullable iQuadrilateral *)quad
                        error:(NSError *_Nullable *_Nullable)error NS_SWIFT_NAME(normalizeImage(_:quad:));


- (void)setCameraEnhancer:(nullable DynamsoftCameraEnhancer *)cameraInstance DEPRECATED_MSG_ATTRIBUTE("DEPRECATED.");

/**
 * Sets an instance of ImageSource to get images.
 *
 * @param [in] source An instance of ImageSource. If you are using Dynamsoft Camera Enhancer(DCE) to capture camera frames, pass an instance of DynamsoftCameraEnhancer.
 */
- (void)setImageSource:(nullable id<ImageSource>)source;

/** Start the document quad detection thread in the video streaming scenario.*/
- (void)startDetecting;

/** Stop the document quad detection thread in the video streaming scenario.*/
- (void)stopDetecting;

/**
 * Set the callback interface to process detection results generated during frame detecting.
 *
 * @param [in] listener An instance of DetectResultListener.
 */
- (void)setDetectResultListener:(nonnull id<DetectResultListener>)listener;

@end // DynamsoftDocumentNormalizer

NS_ASSUME_NONNULL_END
