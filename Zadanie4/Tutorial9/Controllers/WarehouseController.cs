using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Tutorial9.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly string _connectionString;

        public WarehouseController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")!;
        }

        public class WarehouseDto
        {
            [Required] public int IdProduct { get; set; }
            [Required] public int IdWarehouse { get; set; }
            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
            public int Amount { get; set; }
            [Required] public DateTime CreatedAt { get; set; }
        }

        // Zadanie 1
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] WarehouseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                // 1. Sprawdzamy czy produkt o podanym id istnieje
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM Product WHERE IdProduct = @IdProduct",
                    connection, transaction))
                {
                    cmd.Parameters.Add("@IdProduct", SqlDbType.Int).Value = dto.IdProduct;
                    if ((int)await cmd.ExecuteScalarAsync() == 0)
                    {
                        await transaction.RollbackAsync();
                        return NotFound($"Product with Id {dto.IdProduct} not found.");
                    }
                }

                // 1. Sprawdzamy czy magazyn o podanym id istnieje
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM Warehouse WHERE IdWarehouse = @IdWarehouse",
                    connection, transaction))
                {
                    cmd.Parameters.Add("@IdWarehouse", SqlDbType.Int).Value = dto.IdWarehouse;
                    if ((int)await cmd.ExecuteScalarAsync() == 0)
                    {
                        await transaction.RollbackAsync();
                        return NotFound($"Warehouse with Id {dto.IdWarehouse} not found.");
                    }
                }

                // 2. Sprawdzamy czy w tabeli order istnieje rekord który odpowiada naszemu żądaniu
                int orderId;
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1 IdOrder FROM [Order]
                     WHERE IdProduct=@IdProduct
                       AND Amount=@Amount
                       AND CreatedAt<@CreatedAt
                     ORDER BY CreatedAt",
                    connection, transaction))
                {
                    cmd.Parameters.Add("@IdProduct", SqlDbType.Int).Value   = dto.IdProduct;
                    cmd.Parameters.Add("@Amount", SqlDbType.Int).Value      = dto.Amount;
                    cmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = dto.CreatedAt;

                    var obj = await cmd.ExecuteScalarAsync();
                    if (obj is not int oid)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest("No matching purchase order found to fulfill.");
                    }
                    orderId = oid;
                }

                // 3. Sprawdzamy czy zamówienie zostało zrealizowane
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM Product_Warehouse WHERE IdOrder = @OrderId",
                    connection, transaction))
                {
                    cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                    if ((int)await cmd.ExecuteScalarAsync() > 0)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"Order {orderId} has already been fulfilled.");
                    }
                }

                // 4. Aktualizacja kolumny FullfilledAt na aktualną datę
                var now = DateTime.UtcNow;
                using (var cmd = new SqlCommand(
                    "UPDATE [Order] SET FulfilledAt = @Now WHERE IdOrder = @OrderId",
                    connection, transaction))
                {
                    cmd.Parameters.Add("@Now", SqlDbType.DateTime).Value  = now;
                    cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value   = orderId;
                    await cmd.ExecuteNonQueryAsync();
                }

                // 5. Odczyt ceny
                decimal unitPrice;
                using (var cmd = new SqlCommand(
                    "SELECT Price FROM Product WHERE IdProduct = @IdProduct",
                    connection, transaction))
                {
                    cmd.Parameters.Add("@IdProduct", SqlDbType.Int).Value = dto.IdProduct;
                    unitPrice = (decimal)await cmd.ExecuteScalarAsync();
                }

                // 5. Wstawienie do tabeli Product_Warehouse
                int newId;
                using (var cmd = new SqlCommand(@"
                    INSERT INTO Product_Warehouse
                        (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
                    OUTPUT INSERTED.IdProductWarehouse
                    VALUES
                        (@IdWarehouse,@IdProduct,@OrderId,@Amount,@Price,@CreatedAtNew)",
                    connection, transaction))
                {
                    cmd.Parameters.Add("@IdWarehouse", SqlDbType.Int).Value     = dto.IdWarehouse;
                    cmd.Parameters.Add("@IdProduct", SqlDbType.Int).Value        = dto.IdProduct;
                    cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value          = orderId;
                    cmd.Parameters.Add("@Amount", SqlDbType.Int).Value           = dto.Amount;
                    cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value        = unitPrice * dto.Amount;
                    cmd.Parameters.Add("@CreatedAtNew", SqlDbType.DateTime).Value = now;

                    newId = (int)await cmd.ExecuteScalarAsync();
                }
                
                // 6. Zwracamy wartość klucza głównego
                await transaction.CommitAsync();
                return Ok(new { IdProductWarehouse = newId });
            }
            catch (SqlException ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }

        // Zadanie 2
        [HttpPost("with-proc")]
        public async Task<IActionResult> AddProductWithProc([FromBody] WarehouseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                await using var cmd = new SqlCommand("AddProductToWarehouse", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@IdProduct", SqlDbType.Int).Value    = dto.IdProduct;
                cmd.Parameters.Add("@IdWarehouse", SqlDbType.Int).Value  = dto.IdWarehouse;
                cmd.Parameters.Add("@Amount", SqlDbType.Int).Value       = dto.Amount;
                cmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = dto.CreatedAt;

                var result = await cmd.ExecuteScalarAsync();
                if (result == null)
                    return StatusCode(500, "Stored procedure did not return a new Id.");

                return Ok(new { IdProductWarehouse = Convert.ToInt32(result) });
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
