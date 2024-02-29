using ObjCRuntime;

namespace Com.Dynamsoft.DocumentNormalizer
{
	[Native]
	public enum EnumDDNErrorCode : long
	{
		DnContentNotFound = -50001,
		MQuadrilateralInvalid = -10057,
		MImageOrientationInvalid = -10060
	}
}
