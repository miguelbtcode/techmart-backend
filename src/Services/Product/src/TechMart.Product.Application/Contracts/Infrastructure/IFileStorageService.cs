namespace TechMart.Product.Application.Contracts.Infrastructure;

/// <summary>
/// Service for file storage operations.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file to storage.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="content">The file content.</param>
    /// <param name="contentType">The content type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URL of the uploaded file.</returns>
    Task<string> UploadFileAsync(string fileName, Stream content, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from storage.
    /// </summary>
    /// <param name="fileUrl">The file URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the download URL for a file.
    /// </summary>
    /// <param name="fileUrl">The file URL.</param>
    /// <param name="expiresIn">The expiration time for the URL.</param>
    /// <returns>The download URL.</returns>
    Task<string> GetDownloadUrlAsync(string fileUrl, TimeSpan? expiresIn = null);
}