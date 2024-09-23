using Agones;

namespace AgonesAspNetCore;

// multi instance in single server may required to acces IAgonesSDK to operate SDK status directly.
/// <summary>
/// Offer IAgonesSDK access.
/// </summary>
public static class AgonesSDKAccessor
{
    /// <summary>
    /// Offer IAgonesSDK access. null represent IAgonesSDK is not yet available.
    /// </summary>
    public static IAgonesSDK? AgonesSdk { get; private set; }

    /// <summary>
    /// Set IAgonesSDK to be accessible.
    /// </summary>
    /// <param name="agonesSDK"></param>
    public static void SetAgonesSDK(IAgonesSDK agonesSDK) => AgonesSdk = agonesSDK;
}
