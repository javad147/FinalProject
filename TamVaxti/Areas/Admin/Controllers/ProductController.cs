﻿using TamVaxti.Data;
using TamVaxti.Helpers;
using TamVaxti.Helpers.Extensions;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.AttributeOptionSKU;
using TamVaxti.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using TamVaxti.Helpers.Enums;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISubcategoryService _subCategoryService;
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;

        public ProductController(IProductService productService,
                                 ICategoryService categoryService,
                                 IWebHostEnvironment env,
                                 AppDbContext context,
                                 ISubcategoryService subCategoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _env = env;
            _context = context;
            _subCategoryService = subCategoryService;
        }



        [HttpPost]
        public async Task<IActionResult> UpdateReviewStatus(int ReviewId, string PublishStatus)
        {
            var review = await _context.Product_Reviews
                .SingleOrDefaultAsync(r => r.ReviewId == ReviewId);

            if (review != null)
            {
                review.Status = PublishStatus == "Published";
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }


        [HttpGet]
        public async Task<IActionResult> ProductReview()
        {
            var reviews = await _context.Product_Reviews
                                         .Include(r => r.Product)
                                         .Include(r => r.User)
                                         .Select(r => new ProductReviewVM
                                         {
                                             ReviewId = r.ReviewId,
                                             CustomerName = r.User.UserName,
                                             ProductName = r.Product.Name,
                                             Rating = r.Rating,
                                             ReviewDescription = r.ReviewDescription,
                                             PublishStatus = r.Status ? "Published" : "Unpublished"
                                         })
                                         .ToListAsync();

            return View(reviews);
        }


        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var paginatedDatas = await _productService.GetAllPaginatedAsync(page, 100);

            List<ProductVM> mappedDatas = _productService.GetMappedDatas(paginatedDatas);


            int pageCount = await GetPageCountAsync(1);
            
            Paginate<ProductVM> model = new(mappedDatas, pageCount, page);

            return View(model);
        }

        private async Task<int> GetPageCountAsync(int take)
        {
            int count = await _productService.GetCountAsync();

            return (int)Math.Ceiling((decimal)count / take);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();

            Product product = await _productService.GetByIdAsync((int)id);

            if (product is null) return NotFound();


            List<ProductImageVM> productImages = new();

            /*foreach (var item in product.ProductImages)
            {
                productImages.Add(new ProductImageVM
                {
                    Image = item.ImageUrl,
                    IsMain = item.IsMain,
                });
            }*/

            ProductDetailVM model = new()
            {
                Name = product.Name,
                Description = product.Description,
                Category = product.Category.Name,
                Price = product.Price,
                Images = productImages
            };


            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ProductCreateVM();

            var Attributes = await _productService.GetAttributes();
            var AttributeOptions = await _productService.GetAttributeOptions();

            List<ProductAttributeVM> AttributeList = new List<ProductAttributeVM>();

            foreach (var option in Attributes)
            {
                var res = new ProductAttributeVM
                {
                    Id = option.Id,
                    Name = option.Name,
                    AttributeOptions = AttributeOptions.Where(ao => ao.AttributeId == option.Id)
                    .Select(ao => new SelectListItem { Value = ao.Id.ToString(), Text = ao.Value }).ToList()
                };

                AttributeList.Add(res);
                model.SKUs[0].SKUAttributes.Add(new AttributeOptionSKUVM());
            }

            ViewBag.AttributeList = AttributeList;
            ViewBag.AttributeListJson = JsonSerializer.Serialize(AttributeList);

            ViewBag.categories = await _subCategoryService.GetAllBySelectedAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM request)
        {

            if (ModelState.IsValid)
            {
                Product product = new()
                {
                    Name = request.Name,
                    Description = request.Description,
                    CategoryId = request.CategoryId
                };

                if (request.MainImage != null)
                {
                    if (!request.MainImage.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("MainImage", "File type must be image");
                        return View(request);
                    }

                    if (!request.MainImage.CheckFileSize(500))
                    {
                        ModelState.AddModelError("MainImage", "Image size must be less than 500KB");
                        return View(request);
                    }
                    product.MainImage = await ImgFileActionAsync(request.MainImage, request);
                }
                else
                {
                    ModelState.AddModelError("MainImage", "Main image is required.");
                    return View(request);
                }

                _context.Add(product); //await _productService.CreateAsync(product);
                await _context.SaveChangesAsync();

                for (int i = 0; i < request.SKUs.Count; i++)
                {
                    var sku = request.SKUs[i];
                    if (sku.SkuCode == null || sku.SkuCode == "")
                    {
                        ModelState.AddModelError($"SKUs[{i}].SkuCode", "Code is required.");
                        return View(request);
                    }

                    if (sku.Price <= 0)
                    {
                        ModelState.AddModelError($"SKUs[{i}].Price", "Price is required and must be greater than 0.");
                        return View(request);
                    }

                    SKU skus = new SKU
                    {
                        ProductId = product.Id,
                        SkuCode = sku.SkuCode,
                        Price = sku.Price,
                        SkuStock = new List<SkuStock> { new SkuStock { Quantity = sku.Quantity } },
                        ImageUrl1 = (sku.ImageUrl1 != null) ? await ImgFileActionAsync(sku.ImageUrl1, request) : "",
                        ImageUrl2 = (sku.ImageUrl2 != null) ? await ImgFileActionAsync(sku.ImageUrl2, request) : "",
                        ImageUrl3 = (sku.ImageUrl2 != null) ? await ImgFileActionAsync(sku.ImageUrl3, request) : "",
                        ImageUrl4 = (sku.ImageUrl3 != null) ? await ImgFileActionAsync(sku.ImageUrl4, request) : "",
                        CreatedOn = DateTime.Now,
                        /*AttributeOptionSKUs = sku.SKUAttributes.Select(id => new AttributeOptionSKU { AttributeOptionId = id.AttributeOptionId })
                            .ToList()*/
                    };

                    _context.SKUs.Add(skus);
                    await _context.SaveChangesAsync();

                    for (int j = 0; j < sku.SKUAttributes.Count; j++)
                    {
                        if (sku.SKUAttributes[j].AttributeOptionId <= 0)
                        {
                            ModelState.AddModelError($"SKUs[{i}].SKUAttributes[{j}].AttributeOptionId", "Option is required.");
                            return View(request);
                        }

                        AttributeOptionSKU options = new AttributeOptionSKU
                        {
                            SkuId = skus.Id,
                            AttributeOptionId = sku.SKUAttributes[j].AttributeOptionId
                        };

                        _context.AttributeOptionSKUs.Add(options);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        public async Task<dynamic> ImgFileActionAsync(IFormFile img, dynamic model)
        {
            /*if (!img.CheckFileType("image/"))
            {
                ModelState.AddModelError("MainImage", "File type must be image");
                return View(model);
            }

            if (!img.CheckFileSize(500))
            {
                ModelState.AddModelError("MainImage", "Image size must be less than 500KB");
                return View(model);
            }*/

            string fileName = Guid.NewGuid().ToString() + "-" + img.FileName;

            string path = Path.Combine(_env.WebRootPath, "img", "product", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await img.CopyToAsync(fileStream);
            }
            return fileName;
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();

            Product product = await _productService.GetProductByIdAsync((int)id);

            //return Ok(product);

            if (product is null) return NotFound();

            List<AttributeOptionSKU> SkuAttributeList = new List<AttributeOptionSKU>();

            var ProductSkus = product.SKUs;

            for (int j = 0; j < ProductSkus.Count; j++)
            {
                product.SKUs[j].Quantity = product.SKUs[j].SkuStock.Sum(stock => stock.Quantity);
                for (int k = 0; k < ProductSkus[j].AttributeOptionSKUs.Count; k++)
                {
                    var curr = await _productService.GetAttributeOptionsById(ProductSkus[j].AttributeOptionSKUs[k].AttributeOptionId);
                    product.SKUs[j].AttributeOptionSKUs[k].AttributeId = curr != null ? curr.AttributeId : 0;
                }
            }

            var Attributes = await _productService.GetAttributes();
            var AttributeOptions = await _productService.GetAttributeOptions();

            List<ProductAttributeVM> AttributeList = new List<ProductAttributeVM>();

            var AddAttributeOptions = false;
            if (product.SKUs.Count <= 0)
            {
                AddAttributeOptions = true;
                product.SKUs = new List<SKU> { new SKU() };
                product.SKUs[0].AttributeOptionSKUs = new List<AttributeOptionSKU>();
            }

            for (int i = 0; i < Attributes.Count; i++)
            {
                var option = Attributes[i];
                    var res = new ProductAttributeVM
                    {
                        Id = option.Id,
                        Name = option.Name,
                        AttributeOptions = AttributeOptions.Where(ao => ao.AttributeId == option.Id)
                        .Select(ao => new SelectListItem { Value = ao.Id.ToString(), Text = ao.Value }).ToList()
                    };

                AttributeList.Add(res);

                if (AddAttributeOptions)
                {
                        product.SKUs[0].AttributeOptionSKUs.Add(new AttributeOptionSKU());
                }
            }


            ViewBag.AttributeList = AttributeList;
            ViewBag.AttributeListJson = JsonSerializer.Serialize(AttributeList);

            ViewBag.categories = await _subCategoryService.GetAllBySelectedAsync();

            //return Ok(product);

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product request)
        {
            //return Ok(request);

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            bool MainFileChk = false;

            if (request.MainImageFile != null)
            {
                if (!request.MainImageFile.CheckFileType("image/"))
                {
                    ModelState.AddModelError("MainImage", "File type must be image");
                    return View(request);
                }

                if (!request.MainImageFile.CheckFileSize(500))
                {
                    ModelState.AddModelError("MainImage", "Image size must be less than 500KB");
                    return View(request);
                }
                //this.DeleteImg(request.MainImage);
                request.MainImage = await ImgFileActionAsync(request.MainImageFile, request);
                MainFileChk = true;
            }

            for (int i = 0; i < request.SKUs.Count; i++)
            {
                var sku = request.SKUs[i];
                if (sku.SkuCode == null || sku.SkuCode == "")
                {
                    ModelState.AddModelError($"SKUs[{i}].SkuCode", "Code is required.");
                    return View(request);
                }

                if (sku.Price <= 0)
                {
                    ModelState.AddModelError($"SKUs[{i}].Price", "Price is required and must be greater than 0.");
                    return View(request);
                }

                var totalQuantity = _productService.GetSkuStockSum(request.SKUs[i].Id);
                int finalQuantity = sku.Quantity - totalQuantity;
                if(finalQuantity != 0)
                {
                    request.SKUs[i].SkuStock = new List<SkuStock> { new SkuStock { Quantity = finalQuantity } };
                }

                if (request.SKUs[i].ImageUrl1File != null) {
                    //this.DeleteImg(request.SKUs[i].ImageUrl1);
                    request.SKUs[i].ImageUrl1 = await ImgFileActionAsync(request.SKUs[i].ImageUrl1File, request);
                }

                if (request.SKUs[i].ImageUrl2File != null) {
                    //this.DeleteImg(request.SKUs[i].ImageUrl1);
                    request.SKUs[i].ImageUrl2 = await ImgFileActionAsync(request.SKUs[i].ImageUrl2File, request);
                }

                if (request.SKUs[i].ImageUrl3File != null) {
                    //this.DeleteImg(request.SKUs[i].ImageUrl1);
                    request.SKUs[i].ImageUrl3 = await ImgFileActionAsync(request.SKUs[i].ImageUrl3File, request);
                }

                if (request.SKUs[i].ImageUrl4File != null) {
                    //this.DeleteImg(request.SKUs[i].ImageUrl1);
                    request.SKUs[i].ImageUrl4 = await ImgFileActionAsync(request.SKUs[i].ImageUrl4File, request);
                }

                for (int j = 0; j < sku.AttributeOptionSKUs.Count; j++)
                {
                    if (sku.AttributeOptionSKUs[j].AttributeOptionId <= 0)
                    {
                        ModelState.AddModelError($"SKUs[{i}].AttributeOptionSKUs[{j}].AttributeOptionId", "Option is required.");
                        return View(request);
                    }
                }
            }

            _context.Update(request);
            _context.Entry(request).Property(x => x.MainImage).IsModified = MainFileChk;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            Product product = await _productService.GetByIdAsync((int)id);

            if (product is null) return NotFound();

            foreach (var item in product.ProductImages)
            {
                string path = Path.Combine(_env.WebRootPath, "img", item.ImageUrl);
                path.DeleteFileFromLocal();
            }

            await _productService.DeleteAsync(product);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (id is null) BadRequest();

            ProductImage productImage = await _productService.ProductImageByIdAsync((int)id);

            if (productImage is null) NotFound();


            if (productImage.IsMain == true)
            {
                return Problem();
            }


            string path = Path.Combine(_env.WebRootPath, "img", productImage.ImageUrl);

            path.DeleteFileFromLocal();

            await _productService.DeleteProductImageAsync(productImage);

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> ChangeMainImage(int? id, int? productId)
        {
            if (id is null || productId is null) BadRequest();

            Product product = await _productService.GetByIdAsync((int)productId);

            if (product is null) NotFound();

            await _productService.ChangeMainImage(product, (int)id);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSku(int? id)
        {
            if (id is null) return BadRequest();

            SKU sku = await _productService.GetSkuByIdAsync((int)id);

            if (sku is null) return NotFound();

            this.DeleteImg(sku.ImageUrl1);
            this.DeleteImg(sku.ImageUrl2);
            this.DeleteImg(sku.ImageUrl3);
            this.DeleteImg(sku.ImageUrl4);

            await _productService.DeleteSkuAsync(sku);

            return RedirectToAction(nameof(Index));
        }

        public void DeleteImg(string img)
        {
            if (!string.IsNullOrEmpty(img))
            {
                string path = Path.Combine(_env.WebRootPath, "img", "product", img);
                path.DeleteFileFromLocal();
            }
        }

    }
}