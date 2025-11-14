using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using NewLook.Models;
using NewLook.Services.Interfaces;

namespace NewLook.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(IOptions<CloudinarySettings> config, ILogger<CloudinaryService> logger)
        {
            _logger = logger;

            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true; // Use HTTPS URLs
        }

        public async Task<string> UploadImageAsync(IBrowserFile file, string folder = "inventories")
        {
            try
            {
                // Validate file
                if (file == null || file.Size == 0)
                {
                    throw new ArgumentException("No file provided");
                }

                // Validate file type (images only)
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    throw new ArgumentException("Only image files (JPEG, PNG, GIF, WebP) are allowed");
                }

                // Validate file size (max 10MB)
                const long maxFileSize = 10 * 1024 * 1024; // 10MB
                if (file.Size > maxFileSize)
                {
                    throw new ArgumentException("File size must be less than 10MB");
                }

                // Read file stream
                using var stream = file.OpenReadStream(maxAllowedSize: maxFileSize);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Generate a unique file name
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileNameWithoutExtension(file.Name)}";

                // Upload to Cloudinary
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, memoryStream),
                    Folder = folder,
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false,
                    Transformation = new Transformation()
                        .Width(1200)
                        .Height(1200)
                        .Crop("limit")
                        .Quality("auto")
                        .FetchFormat("auto")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    _logger.LogError("Cloudinary upload error: {Error}", uploadResult.Error.Message);
                    throw new Exception($"Upload failed: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Cloudinary");
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(publicId))
                {
                    return false;
                }

                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                return result.Result == "ok";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image from Cloudinary: {PublicId}", publicId);
                return false;
            }
        }

        public string ExtractPublicIdFromUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    return string.Empty;
                }

                // Cloudinary URL format: https://res.cloudinary.com/{cloud_name}/image/upload/{version}/{public_id}.{format}
                // Example: https://res.cloudinary.com/demo/image/upload/v1234567890/inventories/abc123.jpg

                var uri = new Uri(url);
                var pathSegments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                // Find the "upload" segment
                var uploadIndex = Array.IndexOf(pathSegments, "upload");
                if (uploadIndex == -1 || uploadIndex >= pathSegments.Length - 1)
                {
                    return string.Empty;
                }

                // Public ID starts after version (v1234567890)
                var startIndex = uploadIndex + 1;
                if (pathSegments[startIndex].StartsWith("v") && long.TryParse(pathSegments[startIndex][1..], out _))
                {
                    startIndex++; // Skip version
                }

                // Combine remaining segments and remove file extension
                var publicIdWithExtension = string.Join("/", pathSegments[startIndex..]);
                var publicId = Path.GetFileNameWithoutExtension(publicIdWithExtension);

                // If there are folder segments, include them
                if (pathSegments.Length - startIndex > 1)
                {
                    var folderParts = pathSegments[startIndex..^1];
                    publicId = string.Join("/", folderParts) + "/" + publicId;
                }

                return publicId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting public ID from URL: {Url}", url);
                return string.Empty;
            }
        }
    }
}
