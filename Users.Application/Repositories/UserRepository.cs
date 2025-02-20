using Microsoft.Data.SqlClient;
using System.Data;
using Users.Application.Models;

namespace Users.Application.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString = "Server=DESKTOP-N9AC5OV;Database=UsersDB;Trusted_Connection=True; Encrypt=False";

    public async Task<bool> CreateAsync(User user, CancellationToken token = default)
    {
        string insertQuery = @"INSERT INTO Users (FirstName,
                                                  LastName,
                                                  Email,
                                                  DateOfBirth,
                                                  PhoneNumber)
                                          VALUES (@FirstName, 
                                                  @LastName, 
                                                  @Email, 
                                                  @DateOfBirth, 
                                                  @PhoneNumber)";

        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@FirstName", user.PersonalData.FirstName);
            command.Parameters.AddWithValue("@LastName", user.PersonalData.LastName);
            command.Parameters.AddWithValue("@Email", user.EmailAddress.Email);
            command.Parameters.AddWithValue("@DateOfBirth", user.PersonalData.DateOfBirth);
            command.Parameters.AddWithValue("@PhoneNumber", user.PersonalData.PhoneNumber);

            var rows = await command.ExecuteNonQueryAsync(token);
            await connection.CloseAsync();
            return rows > 0;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken token = default)
    {
        string getByIdQuery = @"SELECT Id,
                                       FirstName,
                                       LastName,
                                       Email,
                                       DateOfBirth,
                                       PhoneNumber 
                                  FROM Users 
                                 WHERE Id = @Id";
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(getByIdQuery, connection);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync(token);


            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    PersonalData = new PersonalData
                    {
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                        DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"))
                    },

                    EmailAddress = new EmailAddress
                    {
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                    }
                };
            }

            await connection.CloseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return null;
    }

    public async Task<IReadOnlyCollection<User>> GetAllAsync(GetAllUsersOptions options, CancellationToken token = default)
    {
        string getByIdQuery = @"SELECT Id,
                                       FirstName, 
                                       LastName,
                                       Email,
                                       DateOfBirth,
                                       PhoneNumber 
                                  FROM Users
                                 WHERE (@DateOfBirth IS NULL 
                                    OR DateOfBirth >= @DateOfBirth)
                              ORDER BY Id
                                OFFSET (@PageNumber - 1) * @PageSize ROWS
                            FETCH NEXT @PageSize ROWS ONLY";

        var users = new List<User>();

        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(getByIdQuery, connection);
            command.Parameters.Add("@DateOfBirth", SqlDbType.Date)
                .Value = options.Date.HasValue ? options.Date.Value : DBNull.Value;
            command.Parameters.AddWithValue("@PageSize", options.PageSize);
            command.Parameters.AddWithValue("@PageNumber", options.Page);


            await using var reader = await command.ExecuteReaderAsync(token);

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    PersonalData = new PersonalData
                    {
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                        DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"))
                    },

                    EmailAddress = new EmailAddress
                    {
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                    }
                });
            }

            await connection.CloseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return users;
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken token = default)
    {
        string query = @"UPDATE Users 
                            SET FirstName = @FirstName,
                                LastName = @LastName,
                                Email = @Email,
                                DateOfBirth = @DateOfBirth,
                                PhoneNumber = @PhoneNumber 
                          WHERE Id = @Id";

        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@FirstName", user.PersonalData.FirstName);
            command.Parameters.AddWithValue("@LastName", user.PersonalData.LastName);
            command.Parameters.AddWithValue("@Email", user.EmailAddress.Email);
            command.Parameters.AddWithValue("@DateOfBirth", user.PersonalData.DateOfBirth);
            command.Parameters.AddWithValue("@PhoneNumber", user.PersonalData.PhoneNumber);

            int rowsAffected = await command.ExecuteNonQueryAsync(token);
            await connection.CloseAsync();

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    public async Task<bool> DeleteByIdAsync(int id, CancellationToken token = default)
    {
        string query = @"DELETE FROM Users
                               WHERE Id = @Id";

        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = await command.ExecuteNonQueryAsync(token);
            await connection.CloseAsync();

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken token = default)
    {
        string getByIdQuery = @"SELECT Id,
                                       FirstName,
                                       LastName, 
                                       Email, 
                                       DateOfBirth, 
                                       PhoneNumber 
                                  FROM Users 
                                 WHERE Email = @Email";
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(getByIdQuery, connection);
            command.Parameters.AddWithValue("@Email", email);

            await using var reader = await command.ExecuteReaderAsync(token);

            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    PersonalData = new PersonalData
                    {
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                        DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"))
                    },

                    EmailAddress = new EmailAddress
                    {
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                    }
                };
            }

            await connection.CloseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return null;
    }

    public async Task<int> GetCountAsync(DateTime? date, CancellationToken token = default)
    {
        string getByIdQuery = @"SELECT COUNT(Id)
                                  FROM Users
                                 WHERE (@DateOfBirth IS NULL 
                                    OR DateOfBirth >= @DateOfBirth)";


        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(getByIdQuery, connection);
        command.Parameters.Add("@DateOfBirth", SqlDbType.Date)
            .Value = date.HasValue ? date.Value : DBNull.Value;

        int count = (int)await command.ExecuteScalarAsync(token);
        return count;

    }
}
