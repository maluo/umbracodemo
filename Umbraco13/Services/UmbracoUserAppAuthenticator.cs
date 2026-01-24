
using System.Runtime.Serialization;
using Google.Authenticator;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace Umbraco13.Services
{
    /// <summary>
    /// Create a model with the information required to set up the 2FA provider
    /// </summary>
    [DataContract]
    public class TwoFactorAuthInfo
    {
        [DataMember(Name = "qrCodeSetupImageUrl")]
        public string? QrCodeSetupImageUrl { get; set; }

        [DataMember(Name = "secret")]
        public string? Secret { get; set; }
    }

/// <summary>
/// Implement the `ITwoFactorProvider` with the use of the `TwoFactorAuthenticator` from the GoogleAuthenticator NuGet package
/// </summary>
public class UmbracoUserAppAuthenticator : ITwoFactorProvider
{
    private readonly IUserService _userService;

    /// <summary>
    /// The unique name of the ITwoFactorProvider. This is saved in a constant for reusability.
    /// </summary>
    public const string Name = "UmbracoUserAppAuthenticator";

    /// <summary>
    /// Initializes a new instance of the <see cref="UmbracoUserAppAuthenticator"/> class.
    /// </summary>
    public UmbracoUserAppAuthenticator(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// The unique provider name of ITwoFactorProvider implementation.
    /// </summary>
    /// <remarks>
    /// This value will be saved in the database to connect the member with this  ITwoFactorProvider.
    /// </remarks>
    public string ProviderName => Name;

    /// <summary>
    /// Returns the required data to setup this specific ITwoFactorProvider implementation. In this case it will contain the url to the QR-Code and the secret.
    /// </summary>
    /// <param name="userOrMemberKey">The key of the user or member</param>
    /// <param name="secret">The secret that ensures only this user can connect to the authenticator app</param>
    /// <returns>The required data to setup the authenticator app</returns>
    public Task<object> GetSetupDataAsync(Guid userOrMemberKey, string secret)
    {
        IUser? user = _userService.GetByKey(userOrMemberKey);

        ArgumentNullException.ThrowIfNull(user);

        var applicationName = "Umbraco Demo";
        var twoFactorAuthenticator = new TwoFactorAuthenticator();
        SetupCode setupInfo = twoFactorAuthenticator.GenerateSetupCode(applicationName, user.Username, secret, false);
        return Task.FromResult<object>(new TwoFactorAuthInfo()
        {
            QrCodeSetupImageUrl = setupInfo.QrCodeSetupImageUrl,
            Secret = secret
        });
    }

    /// <summary>
    /// Validated the code and the secret of the user.
    /// </summary>
    public bool ValidateTwoFactorPIN(string secret, string code)
    {
        var twoFactorAuthenticator = new TwoFactorAuthenticator();
        return twoFactorAuthenticator.ValidateTwoFactorPIN(secret, code);
    }

    /// <summary>
    /// Validated the two factor setup
    /// </summary>
    /// <remarks>Called to confirm the setup of two factor on the user. In this case we confirm in the same way as we login by validating the PIN.</remarks>
    public bool ValidateTwoFactorSetup(string secret, string token) => ValidateTwoFactorPIN(secret, token);
}
}
