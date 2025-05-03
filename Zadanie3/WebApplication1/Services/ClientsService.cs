using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using WebApplication1.Models.DTOs;
using WebApplication1.Exceptions;

namespace WebApplication1.Services
{
    public class ClientsService : IClientsService
    {
        private readonly string _connectionString =
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";

        public async Task<List<ClientTripDTO>> GetClientTripsAsync(int clientId)
        {
            const string checkClientSql = @"
                SELECT COUNT(1)
                  FROM Client
                 WHERE IdClient = @IdClient;";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd  = new SqlCommand(checkClientSql, conn))
            {
                cmd.Parameters.AddWithValue("@IdClient", clientId);
                await conn.OpenAsync();
                if ((int)await cmd.ExecuteScalarAsync() == 0)
                    throw new NotFoundException($"Client with ID {clientId} not found.");
            }

            var trips = new List<ClientTripDTO>();
            const string sql = @"
                SELECT
                  t.IdTrip,
                  t.Name,
                  t.DateFrom,
                  t.DateTo,
                  t.MaxPeople,
                  ct.RegisteredAt,
                  ct.PaymentDate
                FROM Client_Trip ct
                JOIN Trip t ON ct.IdTrip = t.IdTrip
               WHERE ct.IdClient = @IdClient;";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd  = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@IdClient", clientId);
                await conn.OpenAsync();

                using var rdr = await cmd.ExecuteReaderAsync();
                int ordId          = rdr.GetOrdinal("IdTrip");
                int ordName        = rdr.GetOrdinal("Name");
                int ordFrom        = rdr.GetOrdinal("DateFrom");
                int ordTo          = rdr.GetOrdinal("DateTo");
                int ordMax         = rdr.GetOrdinal("MaxPeople");
                int ordRegAt       = rdr.GetOrdinal("RegisteredAt");
                int ordPaymentDate = rdr.GetOrdinal("PaymentDate");

                while (await rdr.ReadAsync())
                {
                    trips.Add(new ClientTripDTO
                    {
                        IdTrip       = rdr.GetInt32(ordId),
                        Name         = rdr.GetString(ordName),
                        DateFrom     = rdr.GetDateTime(ordFrom),
                        DateTo       = rdr.GetDateTime(ordTo),
                        MaxPeople    = rdr.GetInt32(ordMax),
                        RegisteredAt = rdr.GetInt32(ordRegAt),
                        PaymentDate  = rdr.IsDBNull(ordPaymentDate)
                                         ? (int?)null
                                         : rdr.GetInt32(ordPaymentDate)
                    });
                }
            }

            return trips;
        }

        public async Task<int> CreateClientAsync(CreateClientDTO client)
        {
            const string sql = @"
                INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
                VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var conn = new SqlConnection(_connectionString);
            using var cmd  = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
            cmd.Parameters.AddWithValue("@LastName",  client.LastName);
            cmd.Parameters.AddWithValue("@Email",     client.Email);
            cmd.Parameters.AddWithValue("@Telephone", client.Telephone);
            cmd.Parameters.AddWithValue("@Pesel",     client.Pesel);

            await conn.OpenAsync();
            return (int)await cmd.ExecuteScalarAsync();
        }

        public async Task RegisterClientToTripAsync(int clientId, int tripId)
        {
            const string chkClient = @"
                SELECT COUNT(1) FROM Client WHERE IdClient = @ClientId;";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd  = new SqlCommand(chkClient, conn))
            {
                cmd.Parameters.AddWithValue("@ClientId", clientId);
                await conn.OpenAsync();
                if ((int)await cmd.ExecuteScalarAsync() == 0)
                    throw new NotFoundException($"Client {clientId} not found.");
            }

            int maxPeople, currentCount;
            const string tripSql = "SELECT MaxPeople FROM Trip WHERE IdTrip = @TripId;";
            const string cntSql  = "SELECT COUNT(1) FROM Client_Trip WHERE IdTrip = @TripId;";

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand(tripSql, conn))
                {
                    cmd.Parameters.AddWithValue("@TripId", tripId);
                    var obj = await cmd.ExecuteScalarAsync();
                    if (obj == null) throw new NotFoundException($"Trip {tripId} not found.");
                    maxPeople = (int)obj;
                }

                using (var cmd = new SqlCommand(cntSql, conn))
                {
                    cmd.Parameters.AddWithValue("@TripId", tripId);
                    currentCount = (int)await cmd.ExecuteScalarAsync();
                }

                if (currentCount >= maxPeople)
                    throw new ConflictException($"Trip {tripId} is full.");
            }

            const string insSql = @"
                INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
                VALUES (@ClientId, @TripId, @RegisteredAt, NULL);";

            using var conn2 = new SqlConnection(_connectionString);
            using var cmd2  = new SqlCommand(insSql, conn2);

            cmd2.Parameters.AddWithValue("@ClientId", clientId);
            cmd2.Parameters.AddWithValue("@TripId",   tripId);

            int nowUnix = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            cmd2.Parameters.AddWithValue("@RegisteredAt", nowUnix);

            await conn2.OpenAsync();
            await cmd2.ExecuteNonQueryAsync();
        }

        public async Task UnregisterClientFromTripAsync(int clientId, int tripId)
        {
            const string chkSql = @"
                SELECT COUNT(1)
                  FROM Client_Trip
                 WHERE IdClient = @ClientId
                   AND IdTrip   = @TripId;";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd  = new SqlCommand(chkSql, conn))
            {
                cmd.Parameters.AddWithValue("@ClientId", clientId);
                cmd.Parameters.AddWithValue("@TripId",   tripId);
                await conn.OpenAsync();
                if ((int)await cmd.ExecuteScalarAsync() == 0)
                    throw new NotFoundException($"No registration for client {clientId} on trip {tripId}.");
            }

            const string delSql = @"
                DELETE
                  FROM Client_Trip
                 WHERE IdClient = @ClientId
                   AND IdTrip   = @TripId;";

            using var conn2 = new SqlConnection(_connectionString);
            using var cmd2  = new SqlCommand(delSql, conn2);

            cmd2.Parameters.AddWithValue("@ClientId", clientId);
            cmd2.Parameters.AddWithValue("@TripId",   tripId);

            await conn2.OpenAsync();
            await cmd2.ExecuteNonQueryAsync();
        }
    }
}
