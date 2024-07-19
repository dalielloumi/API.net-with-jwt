using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public IActionResult GetTiers(string cf)
        {
            try
            {
                List<Tiers> tiersList = new List<Tiers>();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = $"SELECT * FROM Tiers WHERE CF = '{cf}'";

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
                                Craisonsocial = reader["Craisonsocial"] == DBNull.Value ? null : reader["Craisonsocial"].ToString(),
                                Cadresse = reader["Cadresse"] == DBNull.Value ? null : reader["Cadresse"].ToString(),
                                Ccodepostal = reader["Ccodepostal"] == DBNull.Value ? null : reader["Ccodepostal"].ToString(),
                                Cville = reader["Cville"] == DBNull.Value ? null : reader["Cville"].ToString(),
                                Crib = reader["Crib"] == DBNull.Value ? null : reader["Crib"].ToString(),
                                Cbanque = reader["Cbanque"] == DBNull.Value ? null : reader["Cbanque"].ToString(),
                                Ccin = reader["Ccin"] == DBNull.Value ? null : reader["Ccin"].ToString(),
                                Cterme = reader["Cterme"] == DBNull.Value ? false : Convert.ToBoolean(reader["Cterme"]),
                                Cbloq = reader["Cbloq"] == DBNull.Value ? false : Convert.ToBoolean(reader["Cbloq"]),
                                Ctva = reader["Ctva"] == DBNull.Value ? null : reader["Ctva"].ToString(),
                                Cpays = reader["Cpays"] == DBNull.Value ? null : reader["Cpays"].ToString(),
                                Csolde = reader["Csolde"] == DBNull.Value ? null : reader["Csolde"].ToString(),
                                Koniya = reader["Koniya"] == DBNull.Value ? null : reader["Koniya"].ToString(),
                                Codefidelite = reader["Codefidelite"] == DBNull.Value ? null : reader["Codefidelite"].ToString(),
                                Numdossier = reader["Numdossier"] == DBNull.Value ? null : reader["Numdossier"].ToString(),
                                Tel = reader["Tel"] == DBNull.Value ? null : reader["Tel"].ToString(),
                                Fax = reader["Fax"] == DBNull.Value ? null : reader["Fax"].ToString()
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


        [HttpPost("Tiers")]
        public IActionResult CreateTiers([FromBody] Tiers tiers)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Get the highest existing CRef value for the given CF type
                    string maxCRefQuery = @"
                    SELECT MAX(CAST(SUBSTRING(CRef, 3, LEN(CRef) - 2) AS INT)) 
                    FROM [EDIDynamiqueWebuy].[dbo].[Tiers]
                    WHERE CF = @CF";

                    using (SqlCommand maxCRefCommand = new SqlCommand(maxCRefQuery, connection))
                    {
                        maxCRefCommand.Parameters.AddWithValue("@CF", tiers.Cf ?? (object)DBNull.Value);
                        object result = maxCRefCommand.ExecuteScalar();
                        int maxCRefNumber = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                        string newCRef = $"{tiers.Cf}{(maxCRefNumber + 1).ToString("D4")}";
                        tiers.Cref = newCRef;
                    }

                    // Insert the new Tiers record
                    string insertQuery = @"
                    INSERT INTO [EDIDynamiqueWebuy].[dbo].[Tiers] ([CRef], [CF], [TypeC], [CNom], [CPrenom], [CRaisonSocial], 
                           [CAdresse], [CCodePostal], [CVille], [CRIB], [CBanque], [CCIN], [CTerme], [CBloq], [CTVA], [CPays], 
                           [CSolde], [Koniya], [CodeFidelite], [NumDossier], [Tel], [Fax])
                    VALUES (@CRef, @CF, @TypeC, @CNom, @CPrenom, @CRaisonSocial, @CAdresse, @CCodePostal, @CVille, 
                            @CRIB, @CBanque, @CCIN, @CTerme, @CBloq, @CTVA, @CPays, @CSolde, @Koniya, @CodeFidelite, 
                            @NumDossier, @Tel, @Fax)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CRef", tiers.Cref ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CF", tiers.Cf ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TypeC", tiers.Typec ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CNom", tiers.Cnom ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CPrenom", tiers.Cprenom ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CRaisonSocial", tiers.Craisonsocial ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CAdresse", tiers.Cadresse ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CCodePostal", tiers.Ccodepostal ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CVille", tiers.Cville ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CRIB", tiers.Crib ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CBanque", tiers.Cbanque ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CCIN", tiers.Ccin ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CTerme", tiers.Cterme);
                        command.Parameters.AddWithValue("@CBloq", tiers.Cbloq);
                        command.Parameters.AddWithValue("@CTVA", tiers.Ctva ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CPays", tiers.Cpays ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CSolde", tiers.Csolde ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Koniya", tiers.Koniya ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CodeFidelite", tiers.Codefidelite ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@NumDossier", tiers.Numdossier ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Tel", tiers.Tel ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Fax", tiers.Fax ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Tiers created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("Tiers/{cf}")]
        public IActionResult DeleteTiers(string cf)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "DELETE FROM [EDIDynamiqueWebuy].[dbo].[Tiers] WHERE CF = @CF";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CF", cf);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok("Tiers deleted successfully.");
                        }
                        else
                        {
                            return NotFound("Tiers not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
