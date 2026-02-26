using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.AspNetCore.Http;

namespace Domain.Services;

public class ProductService(
    IProductRepository repository,
    IProductRepository productRepository,
    IFileStorageService fileStorageService) : IProductService(repository)
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IFileStorageService _fileStorageService = fileStorageService;

    public override async Task<Product> CreateAsync(Product product, string? actorId = null)
    {
        var productSaved = await _Repository.InsertAsync(product.WithoutRelations(product), actorId);

        return productSaved;
    }

    public override async Task<Product> CreateWithImageAsync(Product product, IFormFile image, List<IFormFile>? additionalImages, string? actorId = null)
    {
        try
        {
            if (image != null && image.Length > 0)
            {
                ValidateImage(image);

                using var stream = image.OpenReadStream();
                var imagePath = await _fileStorageService.UploadFileAsync(stream, image.FileName, "products");

                product.ImagePath = imagePath;
                product.ImageUrl = _fileStorageService.GetFileUrl(imagePath);
                product.ImageFileName = image.FileName;
            }

            if (additionalImages != null && additionalImages.Count > 0)
            {
                var additionalUrls = new List<string>();

                foreach (var additionalImage in additionalImages)
                {
                    if (additionalImage != null && additionalImage.Length > 0)
                    {
                        ValidateImage(additionalImage);

                        using var stream = additionalImage.OpenReadStream();
                        var imagePath = await _fileStorageService.UploadFileAsync(stream, additionalImage.FileName, "products");
                        var imageUrl = _fileStorageService.GetFileUrl(imagePath);

                        additionalUrls.Add(imageUrl);
                    }
                }

                product.AdditionalImagesUrls = additionalUrls;
            }

            var productSaved = await _Repository.InsertAsync(product.WithoutRelations(product));

            return productSaved;
        } catch (Exception e)
        {
            throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
        }
    }



    #region .: PRIVATE METHODS :.

    private void ValidateImage(IFormFile image)
    {
        const long maxFileSize = 5 * 1024 * 1024; // 5MB

        if (image.Length > maxFileSize)
            throw new Exception("Image size exceeds the maximum allowed limit of 5MB.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new Exception("Image format/extension is not allowed.");
    }

    #endregion .: PRIVATE METHODS :.
}
