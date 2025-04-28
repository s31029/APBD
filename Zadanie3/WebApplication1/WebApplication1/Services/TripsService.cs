using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services
{
    public class TripsService : ITripsService
    {
        private readonly string _connectionString =
            "Data Source=DESKTOP-NCPTLIL\\SQLEXPRESS;Initial Catalog=APBD;Integrated Security=True;TrustServerCertificate=True;";

        public async Task<List<TripDTO>> GetTripsAsync()
        {
            var trips = new Dictionary<int, TripDTO>();

            var sql = @"
                SELECT 
                  t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,
                  c.IdCountry, c.Name AS CountryName
                FROM Trip t
                LEFT JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
                LEFT JOIN Country c ON ct.IdCountry = c.IdCountry;";

            using var conn = new SqlConnection(_connectionString);
            using var cmd  = new SqlCommand(sql, conn);

            await conn.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();

            int ordId        = rdr.GetOrdinal("IdTrip");
            int ordName      = rdr.GetOrdinal("Name");
            int ordDesc      = rdr.GetOrdinal("Description");
            int ordFrom      = rdr.GetOrdinal("DateFrom");
            int ordTo        = rdr.GetOrdinal("DateTo");
            int ordMax       = rdr.GetOrdinal("MaxPeople");
            int ordCountryId = rdr.GetOrdinal("IdCountry");
            int ordCountryNm = rdr.GetOrdinal("CountryName");

            while (await rdr.ReadAsync())
            {
                var id = rdr.GetInt32(ordId);
                if (!trips.ContainsKey(id))
                {
                    trips[id] = new TripDTO
                    {
                        IdTrip      = id,
                        Name        = rdr.GetString(ordName),
                        Description = rdr.GetString(ordDesc),
                        DateFrom    = rdr.GetDateTime(ordFrom),
                        DateTo      = rdr.GetDateTime(ordTo),
                        MaxPeople   = rdr.GetInt32(ordMax)
                    };
                }

                if (!rdr.IsDBNull(ordCountryId))
                {
                    trips[id].Countries.Add(new CountryDTO
                    {
                        IdCountry = rdr.GetInt32(ordCountryId),
                        Name      = rdr.GetString(ordCountryNm)
                    });
                }
            }

            return trips.Values.ToList();
        }
    }
}
