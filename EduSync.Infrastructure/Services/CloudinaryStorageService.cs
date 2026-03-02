using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EduSync.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EduSync.Infrastructure.Services;

public class CloudinaryStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryStorageService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"];
        var apiKey = configuration["Cloudinary:ApiKey"];
        var apiSecret = configuration["Cloudinary:ApiSecret"];

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<(string Url, string PublicId)> UploadFileAsync(Stream fileStream, string fileName)
    {
        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(fileName, fileStream)
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        return (uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
    }

    public async Task DeleteFileAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Raw
        };

        await _cloudinary.DestroyAsync(deleteParams);
    }
}