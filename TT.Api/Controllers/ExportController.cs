using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using TT.Lib;
using TT.Lib.Entities;
using Newtonsoft.Json.Linq;

namespace TT.Api.Controllers
{
    /// <summary>
    /// ExportController - Task 3 Implementation
    /// Purpose: Export products with dynamic property tree using Entity Framework
    /// Requirements: Static functions (ID, Name) + Dynamic functions (Properties hierarchy)
    /// Features: Real-time database changes reflection, recursive property building
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly TTDbContext _context;

        /// <summary>
        /// Constructor - Uses Entity Framework (contrast with DataController)
        /// </summary>
        public ExportController(TTDbContext context)
        {
            _context = context;
        }

        #region TASK 3 

        /// <summary>
        /// TASK 3 - Main export functionality
        /// Combines static data (product info) with dynamic properties from database
        /// Features: Real-time reflection, recursive hierarchy, nested JSON structure
        /// Example: /export/brand/SM2L returns product with ui.samo:"Levski" if added to DB - as Stefan sent via email
        /// </summary>
        /// <param name="key">Product key (e.g., SM2L, PK_INTP_DSL50_1016)</param>
        /// <returns>JSON with static product info + dynamic property tree</returns>
        [HttpGet]
        [Route("brand/{key}")]
        public async Task<IActionResult> ExportProductBrand(string key)
        {
            try
            {
                // STEP 1: Get static product data using Entity Framework
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Key == key);

                if (product == null)
                {
                    return NotFound(new { error = $"Product with key '{key}' not found" });
                }

                // STEP 2: Get dynamic properties using EF with Include (joins ProductProperties + Properties)
                var productProperties = await _context.ProductProperties
                    .Include(pp => pp.Property) // EF automatically joins the tables
                    .Where(pp => pp.ProductId == product.Id)
                    .ToListAsync();

                // STEP 3: Get all properties to build the hierarchy tree
                var allProperties = await _context.Properties.ToListAsync();
                
                // STEP 4: Build the nested property tree starting from root (recursive)
                var propertyTree = BuildPropertyTree(productProperties, allProperties);

                // STEP 5: Construct final JSON response
                var result = new JObject
                {
                    ["id"] = product.Key,    // Static data
                    ["name"] = product.Name  // Static data
                };

                // STEP 6: Add the dynamic property tree to response
                if (propertyTree != null)
                {
                    foreach (var prop in propertyTree.Properties())
                    {
                        result[prop.Name] = prop.Value;
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        #endregion

        #region HELPER METHODS (Useful for demonstrations and testing)

        /// <summary>
        /// Helper: Shows available products for testing export functionality
        /// Useful for demonstrating what product keys can be used
        /// </summary>
        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> GetAvailableProducts()
        {
            try
            {
                var products = await _context.Products
                    .Select(p => new { p.Id, p.Key, p.Name })
                    .Take(10)
                    .ToListAsync();

                return Ok(new { products = products, message = "Available products for testing /export/brand/{key}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Helper: Find property IDs for setting up real-time demo
        /// Shows UI, root, and product properties for demo preparation
        /// </summary>
        [HttpGet]
        [Route("find-properties")]
        public async Task<IActionResult> FindProperties()
        {
            try
            {
                var properties = await _context.Properties
                    .Where(p => p.Name.ToLower().Contains("ui") || p.Name.ToLower().Contains("root") || p.Name.ToLower().Contains("product"))
                    .Select(p => new { p.Id, p.Name, p.ParentId })
                    .ToListAsync();

                return Ok(new { properties = properties, message = "Properties containing UI, root, or product" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        #endregion

        #region PRIVATE HELPER METHODS (Internal logic for building property tree)

        /// <summary>
        /// Builds the dynamic property tree from database properties
        /// This is where the magic happens - converts flat property list into nested JSON
        /// </summary>
        private JObject BuildPropertyTree(List<ProductProperty> productProperties, List<Property> allProperties)
        {
            // Create lookup dictionaries
            var propertyLookup = allProperties.ToDictionary(p => p.Id, p => p);
            
            var result = new JObject();
            
            // Build tree for each product property (handle duplicates by processing all values)
            foreach (var productProperty in productProperties)
            {
                if (propertyLookup.ContainsKey(productProperty.PropertyId))
                {
                    var propertyInfo = propertyLookup[productProperty.PropertyId];
                    var path = GetPropertyPath(propertyInfo.Id, propertyLookup);
                    
                    // For duplicate properties, we'll use the last value or combine them
                    SetNestedProperty(result, path, productProperty.Value, productProperty.Type);
                }
            }

            return result;
        }

        private List<string> GetPropertyPath(int propertyId, Dictionary<int, Property> propertyLookup)
        {
            var path = new List<string>();
            var currentId = propertyId;
            var visited = new HashSet<int>(); // Prevent infinite loops
            
            while (propertyLookup.ContainsKey(currentId) && currentId != 0 && !visited.Contains(currentId))
            {
                visited.Add(currentId);
                var prop = propertyLookup[currentId];
                
                // Skip "Root" from path as it's not needed in final output
                if (prop.Name.ToLower() != "root")
                {
                    path.Insert(0, prop.Name.ToLower()); // Insert at beginning to reverse the path
                }
                
                currentId = prop.ParentId;
                
                // Safety check
                if (path.Count > 10) break;
            }

            return path;
        }

        private void SetNestedProperty(JObject obj, List<string> path, string value, string type)
        {
            if (path.Count == 0) return;

            var current = obj;
            
            // Navigate to the correct nested level
            for (int i = 0; i < path.Count - 1; i++)
            {
                var key = path[i];
                if (current[key] == null)
                {
                    current[key] = new JObject();
                }
                current = (JObject)current[key];
            }

            // Set the final value
            var finalKey = path[path.Count - 1];
            if (!string.IsNullOrEmpty(value))
            {
                current[finalKey] = value;
            }
            else
            {
                // Create empty object for properties without values
                current[finalKey] = new JObject();
            }
        }

        #endregion
    }
}