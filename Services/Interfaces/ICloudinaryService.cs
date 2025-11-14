using Microsoft.AspNetCore.Components.Forms;

namespace NewLook.Services.Interfaces
{
    public interface ICloudinaryService
    {
        /// <summary>
        /// Uploads an image file to Cloudinary
        /// </summary>
        /// <param name="file">The image file to upload</param>
        /// <param name="folder">Optional folder name in Cloudinary (e.g., "inventories")</param>
        /// <returns>The URL of the uploaded image</returns>
        Task<string> UploadImageAsync(IBrowserFile file, string folder = "inventories");

        /// <summary>
        /// Deletes an image from Cloudinary by its public ID
        /// </summary>
        /// <param name="publicId">The public ID of the image to delete</param>
        /// <returns>True if deletion was successful</returns>
        Task<bool> DeleteImageAsync(string publicId);

        /// <summary>
        /// Extracts the public ID from a Cloudinary URL
        /// </summary>
        /// <param name="url">The Cloudinary image URL</param>
        /// <returns>The public ID</returns>
        string ExtractPublicIdFromUrl(string url);
    }
}
