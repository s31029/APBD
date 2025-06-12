using System.Data.Common;
using Microsoft.Data.SqlClient;
using WebApplication2.Exceptions;
using WebApplication2.Models.DTOs;

namespace WebApplication2.Services
{
    public class DbService : IDbService
    {
        private readonly string _connectionString;
        
        public DbService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<ClientDto> GetClient(int clientId)
        {
            const string queryClient = @"
                SELECT ID, FirstName, LastName, Address
                  FROM clients
                 WHERE ID = @ClientId;";
            
            const string queryRental = @"
                SELECT c.VIN, col.Name AS Color, m.Name AS Model,
                       r.DateFrom, r.DateTo, r.TotalPrice
                  FROM car_rentals r
                  JOIN cars c ON r.CarID = c.ID
                  JOIN colors col ON c.ColorID = col.ID
                  JOIN models m ON c.ModelID = m.ID
                 WHERE r.ClientID = @ClientId;";

            ClientDto client;
            await using (var connection = new SqlConnection(_connectionString))
            await using (var command = new SqlCommand(queryClient, connection)) 
            {
                command.Parameters.AddWithValue("@ClientId", clientId);
                await connection.OpenAsync();
                await using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    throw new NotFoundException($"Client {clientId} not found.");

                client = new ClientDto
                {
                    Id = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Address = reader.GetString(3)
                };
            }

            await using (var connection = new SqlConnection(_connectionString))
            await using (var command = new SqlCommand(queryRental, connection))
            {
                command.Parameters.AddWithValue("@ClientId", clientId);
                await connection.OpenAsync();
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    client.Rentals.Add(new RentalDto
                    {
                        Vin = reader.GetString(0),
                        Color = reader.GetString(1),
                        Model = reader.GetString(2),
                        DateFrom = reader.GetDateTime(3),
                        DateTo = reader.GetDateTime(4),
                        TotalPrice = reader.GetInt32(5)
                    });
                }
            }

            return client;
        }

        public async Task<int> CreateClientWithRental(CreateClientRentalDto dto)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            DbTransaction transaction = await connection.BeginTransactionAsync();
            try
            {
                const string insertClient = @"
                    INSERT INTO clients(FirstName, LastName, Address)
                    VALUES(@FirstName, @LastName, @Address);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
                int newClientId;
                await using (var command = new SqlCommand(insertClient, connection, (SqlTransaction)transaction))
                {
                    command.Parameters.AddWithValue("@FirstName", dto.Client.FirstName);
                    command.Parameters.AddWithValue("@LastName", dto.Client.LastName);
                    command.Parameters.AddWithValue("@Address", dto.Client.Address);
                    newClientId = (int)await command.ExecuteScalarAsync();
                }

                const string queryCar = @"
                    SELECT PricePerDay
                    FROM cars
                    WHERE ID = @CarId;";
                int pricePerDay;
                await using (var command = new SqlCommand(queryCar, connection, (SqlTransaction)transaction))
                {
                    command.Parameters.AddWithValue("@CarId", dto.CarId);
                    var obj = await command.ExecuteScalarAsync();
                    if (obj is null)
                        throw new NotFoundException($"Car {dto.CarId} not found.");
                    pricePerDay = (int)obj;
                }

                var days = (int)(dto.DateTo.Date - dto.DateFrom.Date).TotalDays;
                var totalPrice = days * pricePerDay;

                const string insertRental = @"
                    INSERT INTO car_rentals(ClientID, CarID, DateFrom, DateTo, TotalPrice)
                    VALUES(@ClientId, @CarId, @DateFrom, @DateTo, @TotalPrice);";
                await using (var cmd = new SqlCommand(insertRental, connection, (SqlTransaction)transaction))
                {
                    cmd.Parameters.AddWithValue("@ClientId", newClientId);
                    cmd.Parameters.AddWithValue("@CarId", dto.CarId);
                    cmd.Parameters.AddWithValue("@DateFrom", dto.DateFrom);
                    cmd.Parameters.AddWithValue("@DateTo", dto.DateTo);
                    cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                    await cmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return newClientId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
