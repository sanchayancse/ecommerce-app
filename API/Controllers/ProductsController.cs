using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductRepository repo): ControllerBase
    {
      
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
        {
            return Ok(await repo.GetProductsAsync(brand, type, sort));
        }   

        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await repo.GetProductByIdAsync(id);

            if(product == null) return NotFound();
            return product;
        }

         [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            Console.WriteLine(product);
           repo.AddProduct(product);    
            if(await repo.SaveChangesAsync()){
                return CreatedAtAction("GetProduct", new {id = product.Id}, product);
            }

            return BadRequest("Problem saving changes");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
        {
            if(id != product.Id || !ProductExists(id)) return BadRequest("Cannot update this product");
           repo.UpdateProduct(product);
            if(await repo.SaveChangesAsync()) return Ok(product);

            return BadRequest("Problem update product");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await repo.GetProductByIdAsync(id);
            if(product == null) return NotFound();

            repo.DeleteProduct(product);
            if(await repo.SaveChangesAsync()) return Ok();
            
            return BadRequest("Problem deleting product");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetProductBrands()
        {
            return Ok(await repo.GetBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetProductTypes()
        {
            return Ok(await repo.GetTypesAsync());
        }

        private bool ProductExists(int id)
        {
            return repo.ProductExists(id);
        }
    }
}