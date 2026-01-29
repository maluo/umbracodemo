namespace Umbraco13.Services;

public interface IDownloadTokenService
{
    string CurrentVersion { get; }
    string GenerateDownloadToken(string downloadType);
    bool ValidateDownloadToken(string token, string downloadType);
}
