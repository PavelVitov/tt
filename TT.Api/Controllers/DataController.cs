using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;

namespace TT.Api.Controllers
{
    /// DataController - Task 1 Implementation
    /// Purpose: Load data WITHOUT EF using raw SQL connections
    /// Requirements: 1.1 - Stored Procedures, 1.2 - Views
    [Route("[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly string _connectionString;

        /// Constructor - Sets up raw SQL connection        
        public DataController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TTDbContext");
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Connection string 'TTDbContext' not found in configuration.");
            }
        }

        #region TASK 1 

        /// TASK 1.1 - Load data with stored procedure
        /// Uses raw SQL stored procedure without EF
        /// returns: List of products from stored procedure or table fallback
        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = new List<object>();

            try
            {
                // STEP 1: Open raw SQL connection
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // STEP 2: Check if stored procedure exists in database
                bool spExists;
                using (var checkCommand = new SqlCommand("SELECT COUNT(*) FROM sys.objects WHERE type = 'P' AND name = 'GetProducts'", connection))
                {
                    spExists = (int)await checkCommand.ExecuteScalarAsync() > 0;
                }

                // STEP 3: Execute appropriate command with proper resource management
                using var command = spExists 
                    ? new SqlCommand("GetProducts", connection) { CommandType = CommandType.StoredProcedure }
                    : new SqlCommand("SELECT Id, Name, [Key], BrandId FROM Products", connection) { CommandType = CommandType.Text };

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    products.Add(new
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.IsDBNull("Name") ? null : reader.GetString("Name"),
                        Key = reader.IsDBNull("Key") ? null : reader.GetString("Key"),
                        BrandId = reader.GetInt32("BrandId")
                    });
                }

                return Ok(new { 
                    spExists,
                    count = products.Count,
                    products 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// TASK 1.2 - Work with a view
        /// Uses database view without EF
        /// returns: List of properties from database view
        [HttpGet]
        [Route("properties")]
        public async Task<IActionResult> GetProperties()
        {
            var properties = new List<object>();

            try
            {
                // STEP 1: Open raw SQL connection (NO Entity Framework)
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // STEP 2: Query the database view vw_Properties 
                using var command = new SqlCommand("SELECT Id, Name, ParentId FROM vw_Properties", connection)
                {
                    CommandType = CommandType.Text
                };

                // STEP 3: Read data from view and build result
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    properties.Add(new
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.IsDBNull("Name") ? null : reader.GetString("Name"),
                        ParentId = reader.IsDBNull("ParentId") ? (int?)null : reader.GetInt32("ParentId")
                    });
                }

                return Ok(new
                {
                    source = "vw_Properties",
                    count = properties.Count,
                    properties
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// Proves raw SQL works without EF
        /// returns:Connection status and database information
        [HttpGet]
        [Route("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                // Test raw SQL connection
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                var serverVersion = connection.ServerVersion;
                var database = connection.Database;
                
                return Ok(new
                {
                    success = true,
                    serverVersion,
                    database,
                    message = "Connection successful"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        #endregion

        #region HELPER METHODS (Useful for demonstrations and testing)

        /// Helper: Shows all foreign key relationships in the database
        [HttpGet]
        [Route("foreign-keys")]
        public async Task<IActionResult> GetForeignKeys()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(@"
                    SELECT 
                        fk.name AS ForeignKeyName,
                        tp.name AS ParentTable,
                        cp.name AS ParentColumn,
                        tr.name AS ReferencedTable,
                        cr.name AS ReferencedColumn
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
                    INNER JOIN sys.tables tp ON fkc.parent_object_id = tp.object_id
                    INNER JOIN sys.columns cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
                    INNER JOIN sys.tables tr ON fkc.referenced_object_id = tr.object_id
                    INNER JOIN sys.columns cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
                    ORDER BY tp.name, fk.name", connection);

                var foreignKeys = new List<object>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    foreignKeys.Add(new
                    {
                        foreignKeyName = reader.GetString("ForeignKeyName"),
                        parentTable = reader.GetString("ParentTable"),
                        parentColumn = reader.GetString("ParentColumn"),
                        referencedTable = reader.GetString("ReferencedTable"),
                        referencedColumn = reader.GetString("ReferencedColumn")
                    });
                }

                return Ok(new { database = "tt", foreignKeys = foreignKeys });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// Helper: Lists all tables in the database
        [HttpGet]
        [Route("all-tables")]
        public async Task<IActionResult> GetAllTables()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Get all user tables in the database
                using var command = new SqlCommand(@"
                    SELECT TABLE_NAME, TABLE_TYPE
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_TYPE = 'BASE TABLE'
                    ORDER BY TABLE_NAME", connection);

                var tables = new List<object>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    tables.Add(new
                    {
                        name = reader.GetString("TABLE_NAME"),
                        type = reader.GetString("TABLE_TYPE")
                    });
                }

                return Ok(new { database = "tt", actualTables = tables });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// Helper: Checks database schema and verifies Task 2 completion
        /// Shows all table structures and confirms Type field exists
        /// IMPORTANT: This proves Task 2 migration was successful!
        [HttpGet]
        [Route("schema-check")]
        public async Task<IActionResult> CheckSchema()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var tables = new Dictionary<string, object>();

                // Get all user tables with correct names
                var tableNames = new[] { "Products", "Properties", "ProductProperties", "Brand", "Hardware" };
                
                foreach (var tableName in tableNames)
                {
                    try
                    {
                        using var command = new SqlCommand(@"
                            SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
                            FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = @tableName 
                            ORDER BY ORDINAL_POSITION", connection);
                        command.Parameters.AddWithValue("@tableName", tableName);

                        var columns = new List<object>();
                        using var reader = await command.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            columns.Add(new
                            {
                                name = reader.GetString("COLUMN_NAME"),
                                dataType = reader.GetString("DATA_TYPE"),
                                nullable = reader.GetString("IS_NULLABLE"),
                                defaultValue = reader.IsDBNull("COLUMN_DEFAULT") ? null : reader.GetString("COLUMN_DEFAULT")
                            });
                        }
                        
                        tables[tableName] = new
                        {
                            exists = columns.Count > 0,
                            columnCount = columns.Count,
                            columns = columns
                        };
                    }
                    catch (Exception ex)
                    {
                        tables[tableName] = new { exists = false, error = ex.Message };
                    }
                }

                // Check if ProductProperties table has Type column (safe approach)
                bool hasTypeColumn = false;
                if (tables.TryGetValue("ProductProperties", out var productPropertiesTable))
                {
                    var tableInfo = productPropertiesTable as dynamic;
                    if (tableInfo?.columns is List<object> columns)
                    {
                        hasTypeColumn = columns.Any(c => 
                        {
                            var columnInfo = c as dynamic;
                            return columnInfo?.name?.ToString() == "Type";
                        });
                    }
                }

                return Ok(new
                {
                    database = "tt",
                    tables = tables,
                    productPropertiesHasType = hasTypeColumn
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        #endregion
    }
}