using Examensarbete.Data;
using Examensarbete.Models;
using Examensarbete.Services.Interfaces;
using Examensarbete.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Examensarbete.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly ApplicationDbContext _context;

        public MaterialService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Relaterat till CRUD
        public async Task<List<Material>> GetAllMaterialsAsync()
        {
            return await _context.Materials.ToListAsync();
        }
        public async Task<Material> GetMaterialByIdAsync(int id)
        {
            return await _context.Materials.FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<Material> CreateMaterialAsync(Material material)
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }

            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            return material;
        }
        public async Task<IEnumerable<Material>> CreateMaterialsAsync(IEnumerable<Material> materials)
        {
            if (materials == null)
            {
                throw new ArgumentNullException(nameof(materials));
            }

            _context.Materials.AddRange(materials);
            await _context.SaveChangesAsync();

            return materials;
        }
        public async Task DeleteMaterialAsync(int materialId)
        {
            var material = await _context.Materials.FindAsync(materialId);
            if (material != null)
            {
                _context.Materials.Remove(material);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateMaterialAsync(Material material)
        {
            _context.Materials.Update(material);
            await _context.SaveChangesAsync();
        }


        // Relaterat till Produkter
        public async Task<List<SelectListItem>> GetMaterialOptionsAsync()
        {
            var materials = await _context.Materials.ToListAsync();
            return materials.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Type
            }).ToList();
        }
        public async Task<IList<MaterialViewModel>> GetProductMaterialsAsync(int productId)
        {
            return await _context.ProductMaterials
                .Where(pm => pm.ProductId == productId)
                .Select(pm => new MaterialViewModel
                {
                    MaterialId = pm.MaterialId,
                    MaterialName = pm.Material.Type,
                    Percentage = pm.Percentage
                }).ToListAsync();
        }
        public async Task UpdateMaterialsAsync(Product product, List<ProductMaterial> updatedMaterials, List<ProductMaterial> newMaterials, List<int> removedMaterialIds)
        {
            var productToUpdate = await _context.Products
                .Include(p => p.ProductMaterials)
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            if (productToUpdate == null)
            {
                throw new Exception("Produkten hittades inte.");
            }

            // Lägg till nya material
            if (newMaterials != null)
            {
                foreach (var newMaterial in newMaterials)
                {
                    productToUpdate.ProductMaterials.Add(new ProductMaterial
                    {
                        ProductId = productToUpdate.Id,
                        MaterialId = newMaterial.MaterialId,
                        Percentage = newMaterial.Percentage
                    });
                }
            }

            // Uppdatera befintliga material
            if (updatedMaterials != null)
            {
                foreach (var updatedMaterial in updatedMaterials)
                {
                    var existingMaterial = productToUpdate.ProductMaterials.FirstOrDefault(pm => pm.MaterialId == updatedMaterial.MaterialId);
                    if (existingMaterial != null)
                    {
                        existingMaterial.Percentage = updatedMaterial.Percentage;
                        existingMaterial.MaterialId = updatedMaterial.MaterialId;
                    }
                }
            }

            // Ta bort material
            if (removedMaterialIds != null)
            {
                foreach (var materialId in removedMaterialIds)
                {
                    var materialToRemove = productToUpdate.ProductMaterials.FirstOrDefault(pm => pm.MaterialId == materialId);
                    if (materialToRemove != null)
                    {
                        _context.ProductMaterials.Remove(materialToRemove);
                    }
                }
            }

            // Uppdatera förpackningsmaterial
            productToUpdate.PackagingMaterialId = product.PackagingMaterialId;

            await _context.SaveChangesAsync();
        }
    }
}
