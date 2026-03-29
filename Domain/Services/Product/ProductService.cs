using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;
using Domain.SearchParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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

            var productSaved = await _Repository.InsertAsync(product.WithoutRelations(product), actorId);

            var productFromDb = await _Repository.GetAsync(productSaved.Id);

            return productFromDb;
        } catch (Exception e)
        {
            throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
        }
    }

    public override async Task<Product> UpdateWithImageAsync(Product product, IFormFile? image, List<IFormFile>? additionalImages, string? actorId = null)
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

            await _Repository.UpdateAsync(product.WithoutRelations(product), actorId);

            return await _Repository.GetAsync(product.Id);
        }
        catch (Exception)
        {
            throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
        }
    }

    public override async Task<List<Product>> GetProductsByCategoryAsync(ProductSearchParameter searchParameter, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _Repository.GetByExpression(p => true);

            if (searchParameter.Category.HasValue)
            {
                query = query.Where(p => p.Category == searchParameter.Category.Value);
            }

            if (searchParameter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= searchParameter.MinPrice.Value);
            }

            if (searchParameter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= searchParameter.MaxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchParameter.SearchTerm))
            {
                var searchTerm = searchParameter.SearchTerm.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchTerm)
                                      || p.Description.ToLower().Contains(searchTerm));
            }

            if (searchParameter.InStock.HasValue && searchParameter.InStock.Value)
            {
                query = query.Where(p => p.StockAmount > 0);
            }

            return await query.ToListAsync(cancellationToken);
        }
        catch (Exception)
        {
            throw;
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
