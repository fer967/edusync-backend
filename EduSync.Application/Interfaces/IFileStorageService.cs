namespace EduSync.Application.Interfaces;

public interface IFileStorageService
{
    Task<(string Url, string PublicId)> UploadFileAsync(Stream fileStream, string fileName);
    Task DeleteFileAsync(string publicId);
}