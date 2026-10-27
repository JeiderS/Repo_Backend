using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Inventory.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Inventory.Infrastructure.Media;

/// <summary>
/// Signed server-side upload to Cloudinary. The API secret never leaves the
/// backend — only the computed signature does, same idea as JwtTokenGenerator
/// never exposing the signing key.
/// </summary>
public class CloudinaryImageUploader(HttpClient httpClient, IOptions<CloudinaryOptions> options) : ICloudinaryImageUploader
{
    private readonly CloudinaryOptions _options = options.Value;

    public long MaxSizeBytes => _options.MaxImageSizeMb * 1024L * 1024L;

    public async Task<string> UploadAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var paramsToSign = new SortedDictionary<string, string> { ["timestamp"] = timestamp };
        if (!string.IsNullOrWhiteSpace(_options.UploadPreset))
            paramsToSign["upload_preset"] = _options.UploadPreset;

        var signatureBase = string.Join("&", paramsToSign.Select(kv => $"{kv.Key}={kv.Value}"));
        var signature = ComputeSignature(signatureBase + _options.ApiSecret);

        using var form = new MultipartFormDataContent();
        using var streamContent = new StreamContent(content);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        form.Add(streamContent, "file", fileName);
        form.Add(new StringContent(_options.ApiKey), "api_key");
        form.Add(new StringContent(timestamp), "timestamp");
        form.Add(new StringContent(signature), "signature");
        if (!string.IsNullOrWhiteSpace(_options.UploadPreset))
            form.Add(new StringContent(_options.UploadPreset), "upload_preset");

        using var response = await httpClient.PostAsync(
            $"https://api.cloudinary.com/v1_1/{_options.CloudName}/image/upload",
            form,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<CloudinaryUploadResponse>(cancellationToken: cancellationToken);

        return payload?.secure_url ?? throw new InvalidOperationException("Respuesta vacia de Cloudinary.");
    }

    private static string ComputeSignature(string value) =>
        Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private sealed record CloudinaryUploadResponse(string secure_url);
}
