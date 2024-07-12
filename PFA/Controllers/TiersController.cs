using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PFA.Models;

namespace PFA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TiersController : ControllerBase
    {
        private readonly string _connectionString = "Data Source=DALI_\\DALI;Initial Catalog=EDIDynamiqueWebuy;Integrated Security=true";

        [HttpGet("Tiers")]
        public IActionResult GetTiers()
        {
            try
            {
                List<Tiers> tiersList = new List<Tiers>();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT * FROM tiers";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Tiers tiers = new Tiers
                            {
                                Cref = reader["Cref"] == DBNull.Value ? null : reader["Cref"].ToString(),
                                Cf = reader["Cf"] == DBNull.Value ? null : reader["Cf"].ToString(),
                                Typec = reader["Typec"] == DBNull.Value ? null : reader["Typec"].ToString(),
                                Cnom = reader["Cnom"] == DBNull.Value ? null : reader["Cnom"].ToString(),
                                Cprenom = reader["Cprenom"] == DBNull.Value ? null : reader["Cprenom"].ToString(),
                                Craisonsocial = reader["Craisonsocial"] == DBNull.Value ? null : reader["Craisonsocial"].ToString()
                            };

                            tiersList.Add(tiers);
                        }
                    }
                }

                return Ok(tiersList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Add other CRUD operations (POST, PUT, DELETE) as needed
    }
}
