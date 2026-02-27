using MySql.Data.MySqlClient;
using NexumApp.Models;
using System;

namespace NexumApp.Services
{
    public class AuthService
    {
        private string connectionString = "server=localhost;port=3306;database=nexumdb;uid=root;pwd=;";

        public Usuario Login(string email, string password)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT Id, Nombre, Apellidos, Email, PasswordHash, EsAdmin FROM Usuarios WHERE Email = @email AND Activo = 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string passwordHash = reader.GetString("PasswordHash");

                                // Verificar contraseña con BCrypt
                                if (BCrypt.Net.BCrypt.Verify(password, passwordHash))
                                {
                                    return new Usuario
                                    {
                                        Id = reader.GetInt32("Id"),
                                        Nombre = reader.GetString("Nombre"),
                                        Apellidos = reader.GetString("Apellidos"),
                                        Email = reader.GetString("Email"),
                                        PasswordHash = passwordHash,
                                        EsAdmin = reader.GetBoolean("EsAdmin")
                                    };
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Aquí podrías registrar el error en un archivo de log si quisieras
                // Pero no mostramos nada al usuario
            }

            return null;
        }

        public void RegenerarTodosLosHashes()
        {
            string passwordPorDefecto = "123456";
            string nuevoHash = BCrypt.Net.BCrypt.HashPassword(passwordPorDefecto);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Actualizar TODOS los usuarios con el mismo hash válido
                string query = "UPDATE Usuarios SET PasswordHash = @hash";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@hash", nuevoHash);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}