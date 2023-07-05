using System;
using System.Linq;
using System.Collections.Generic;
using Litium.FieldFramework;
using Litium.Products;
using Litium.Accelerator.Constants;

namespace Litium.Accelerator.Definitions.Products
{
    internal class ProductsFieldTemplateSetup : FieldTemplateSetup
    {
        private readonly DisplayTemplateService _displayTemplateService;

        public ProductsFieldTemplateSetup(DisplayTemplateService displayTemplateService)
        {
            _displayTemplateService = displayTemplateService;
        }
        public override IEnumerable<FieldTemplate> GetTemplates()
        {
            var categoryDisplayTemplateId = _displayTemplateService.Get<CategoryDisplayTemplate>("Category")?.SystemId ?? Guid.Empty;
            var productDisplayTemplateId = _displayTemplateService.Get<ProductDisplayTemplate>("Product")?.SystemId ?? Guid.Empty;
            var productWithVariantListDisplayTemplateId = _displayTemplateService.Get<ProductDisplayTemplate>("ProductWithVariantList")?.SystemId ?? Guid.Empty;

            if (categoryDisplayTemplateId == Guid.Empty || productDisplayTemplateId == Guid.Empty || productWithVariantListDisplayTemplateId == Guid.Empty)
            {
                return Enumerable.Empty<FieldTemplate>();
            }

            var fieldTemplates = new FieldTemplate[]
            {
                new CategoryFieldTemplate("Category", categoryDisplayTemplateId)
                {
                    CategoryFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                SystemFieldDefinitionConstants.Name,
                                SystemFieldDefinitionConstants.Description,
                                SystemFieldDefinitionConstants.Url,
                                SystemFieldDefinitionConstants.SeoTitle,
                                SystemFieldDefinitionConstants.SeoDescription,
                                ProductFieldNameConstants.AcceleratorFilterFields,
                                ProductFieldNameConstants.OrganizationsPointer
                            }
                        }
                    }
                },
                new ProductFieldTemplate("ProductWithOneVariant", productDisplayTemplateId)
                {
                    ProductFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                SystemFieldDefinitionConstants.Name,
                                ProductFieldNameConstants.OrganizationsPointer,
                            },
                        }
                    },
                    VariantFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                SystemFieldDefinitionConstants.Name,
                                SystemFieldDefinitionConstants.Description,
                                SystemFieldDefinitionConstants.Url,
                                SystemFieldDefinitionConstants.SeoTitle,
                                SystemFieldDefinitionConstants.SeoDescription,
                            },
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "Product information",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Produktinformation" },
                                ["en-US"] = { Name = "Product information" }
                            },
                            Fields =
                            {
                                ProductFieldNameConstants.News,
                                ProductFieldNameConstants.Brand,
                                ProductFieldNameConstants.Color,
                                ProductFieldNameConstants.Size,
                                ProductFieldNameConstants.ProductSheet,
                            },
                            UseInStorefront = true,
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "Product specification",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Produktspecifikation" },
                                ["en-US"] = { Name = "Product specification" }
                            },
                            Fields =
                            {
                                ProductFieldNameConstants.Specification,
                                ProductFieldNameConstants.Weight,
                            },
                            UseInStorefront = true,
                        }
                    }
                },
                new ProductFieldTemplate("ProductWithVariants", productDisplayTemplateId)
                {
                    ProductFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                SystemFieldDefinitionConstants.Name,
                                SystemFieldDefinitionConstants.Description,
                                ProductFieldNameConstants.Brand,
                                ProductFieldNameConstants.Type,
                                ProductFieldNameConstants.OrganizationsPointer,
                            },
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "Product information",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Produktinformation" },
                                ["en-US"] = { Name = "Product information" }
                            },
                            Fields =
                            {
                                ProductFieldNameConstants.News,
                                ProductFieldNameConstants.ProductSheet,
                            },
                            UseInStorefront = true,
                        }
                    },
                    VariantFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                SystemFieldDefinitionConstants.Name,
                                SystemFieldDefinitionConstants.Description,
                                SystemFieldDefinitionConstants.Url,
                                SystemFieldDefinitionConstants.SeoTitle,
                                SystemFieldDefinitionConstants.SeoDescription,
                            },
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "Product information",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Produktinformation" },
                                ["en-US"] = { Name = "Product information" }
                            },
                            Fields =
                            {
                                ProductFieldNameConstants.Color,
                                ProductFieldNameConstants.Size,
                            },
                            UseInStorefront = true,
                        }
                    }
                },
                new ProductFieldTemplate("ProductWithVariantsList", productWithVariantListDisplayTemplateId)
                {
                    ProductFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                SystemFieldDefinitionConstants.Name,
                                SystemFieldDefinitionConstants.Description,
                                ProductFieldNameConstants.Brand,
                                ProductFieldNameConstants.Type,
                                SystemFieldDefinitionConstants.Url,
                                SystemFieldDefinitionConstants.SeoTitle,
                                SystemFieldDefinitionConstants.SeoDescription,
                                ProductFieldNameConstants.OrganizationsPointer,
                            },
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "Product specification",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Produktspecifikation" },
                                ["en-US"] = { Name = "Product specification" }
                            },
                            Fields =
                            {
                                ProductFieldNameConstants.Specification,
                                ProductFieldNameConstants.ProductSheet,
                            },
                            UseInStorefront = true,
                        }
                    },
                    VariantFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                SystemFieldDefinitionConstants.Name,
                                ProductFieldNameConstants.Color,
                                ProductFieldNameConstants.Size,
                            },
                        }
                    }
                }
            };
            return fieldTemplates;
        }
    }
}
