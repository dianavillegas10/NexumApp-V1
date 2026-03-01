using MySql.Data.MySqlClient;
using NexumApp.Models;
using System;

namespace NexumApp.Services
{
    /// <summary>
    /// Enumeración que representa los posibles resultados de un intento de login.
    /// Permite al formulario mostrar mensajes de error específicos según cada caso.
    /// </summary>
    public enum ResultadoLogin
    {
        /// <summary>Login exitoso, el usuario ha sido autenticado correctamente.</summary>
        Exitoso,
        /// <summary>El email introducido no existe en la base de datos.</summary>
        UsuarioNoExiste,
        /// <summary>El email existe pero la contraseña no coincide.</summary>
        ContrasenaIncorrecta,
        /// <summary>El usuario existe pero su cuenta está desactivada.</summary>
        UsuarioInactivo,
        /// <summary>No se pudo conectar con la base de datos MySQL.</summary>
        ErrorConexion
    }

    /// <summary>
    /// Servicio de autenticación que gestiona el login, registro y validación de usuarios.
    /// Utiliza conexión directa a MySQL mediante MySql.Data y BCrypt para el hash de contraseñas.
    /// </summary>
    /// <remarks>
    /// Este servicio es el núcleo de la seguridad de la aplicación:
    /// - Las contraseñas nunca se almacenan en texto plano
    /// - Se utiliza BCrypt con salt automático para el hash
    /// - Se devuelven resultados específicos para cada tipo de error
    /// </remarks>
    public class AuthService
    {
        /// <summary>
        /// Cadena de conexión a la base de datos MySQL.
        /// Configurada para XAMPP local sin contraseña.
        /// </summary>
        private string connectionString = "server=localhost;port=3306;database=nexumdb;uid=root;pwd=;";

        /// <summary>
        /// Intenta autenticar un usuario contra la base de datos MySQL.
        /// </summary>
        /// <param name="email">Email del usuario (usado como identificador único).</param>
        /// <param name="password">Contraseña en texto plano introducida por el usuario.</param>
        /// <returns>
        /// Tupla con dos valores:
        /// - Usuario: Objeto Usuario si el login fue exitoso, null en caso contrario.
        /// - Resultado: Enum indicando el resultado específico del intento.
        /// </returns>
        /// <remarks>
        /// El proceso de autenticación sigue estos pasos:
        /// 1. Busca el usuario por email en la base de datos
        /// 2. Si no existe, devuelve UsuarioNoExiste
        /// 3. Si existe pero está inactivo, devuelve UsuarioInactivo
        /// 4. Verifica la contraseña con BCrypt.Verify
        /// 5. Si coincide, devuelve el usuario; si no, ContrasenaIncorrecta
        /// </remarks>
        public (Usuario Usuario, ResultadoLogin Resultado) Login(string email, string password)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Consulta parametrizada para prevenir SQL Injection
                    string query = "SELECT Id, Nombre, Apellidos, Email, PasswordHash, EsAdmin, Activo FROM Usuarios WHERE Email = @email";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Verificar primero si el usuario está activo
                                bool activo = reader.GetBoolean("Activo");
                                if (!activo)
                                {
                                    return (null, ResultadoLogin.UsuarioInactivo);
                                }

                                // Obtener el hash almacenado y verificar con BCrypt
                                string passwordHash = reader.GetString("PasswordHash");

                                // BCrypt.Verify compara la contraseña plana con el hash almacenado
                                if (BCrypt.Net.BCrypt.Verify(password, passwordHash))
                                {
                                    // Contraseña correcta: crear objeto Usuario con los datos
                                    var usuario = new Usuario
                                    {
                                        Id = reader.GetInt32("Id"),
                                        Nombre = reader.GetString("Nombre"),
                                        Apellidos = reader.GetString("Apellidos"),
                                        Email = reader.GetString("Email"),
                                        PasswordHash = passwordHash,
                                        EsAdmin = reader.GetBoolean("EsAdmin"),
                                        Activo = activo
                                    };
                                    return (usuario, ResultadoLogin.Exitoso);
                                }
                                else
                                {
                                    // Email correcto pero contraseña incorrecta
                                    return (null, ResultadoLogin.ContrasenaIncorrecta);
                                }
                            }
                            else
                            {
                                // No se encontró ningún usuario con ese email
                                return (null, ResultadoLogin.UsuarioNoExiste);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Error de conexión o consulta: registrar en debug y devolver error
                System.Diagnostics.Debug.WriteLine($"Error en login: {ex.Message}");
                return (null, ResultadoLogin.ErrorConexion);
            }
        }

        /// <summary>
        /// Registra un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="nombre">Nombre del usuario.</param>
        /// <param name="apellidos">Apellidos del usuario.</param>
        /// <param name="email">Email único del usuario.</param>
        /// <param name="password">Contraseña en texto plano (será hasheada).</param>
        /// <returns>True si el registro fue exitoso, False en caso de error.</returns>
        /// <remarks>
        /// El proceso de registro:
        /// 1. Hashea la contraseña con BCrypt (incluye salt automático)
        /// 2. Inserta el nuevo usuario con EsAdmin=0, Activo=1
        /// 3. Registra la fecha de registro actual
        /// 
        /// IMPORTANTE: Antes de llamar a este método, verificar que el email
        /// no existe usando EmailExiste() para evitar duplicados.
        /// </remarks>
        public bool RegistrarUsuario(string nombre, string apellidos, string email, string password)
        {
            try
            {
                // BCrypt.HashPassword genera automáticamente un salt único
                // El hash resultante incluye: versión + cost + salt + hash
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Los nuevos usuarios se crean como usuarios normales (EsAdmin=0) y activos (Activo=1)
                    string query = @"INSERT INTO Usuarios (Nombre, Apellidos, Email, PasswordHash, EsAdmin, Activo, FechaRegistro) 
                                     VALUES (@nombre, @apellidos, @email, @passwordHash, 0, 1, NOW())";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@apellidos", apellidos);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@passwordHash", passwordHash);

                        // ExecuteNonQuery devuelve el número de filas afectadas
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en registro: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si un email ya está registrado en la base de datos.
        /// </summary>
        /// <param name="email">Email a verificar.</param>
        /// <returns>True si el email ya existe, False si está disponible.</returns>
        /// <remarks>
        /// Debe llamarse antes de RegistrarUsuario para evitar
        /// errores de duplicado en la base de datos.
        /// </remarks>
        public bool EmailExiste(string email)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // COUNT(*) devuelve el número de coincidencias
                    string query = "SELECT COUNT(*) FROM Usuarios WHERE Email = @email";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        // Si COUNT > 0, el email ya existe
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch
            {
                // En caso de error, asumimos que no existe para no bloquear el registro
                return false;
            }
        }

        /// <summary>
        /// Prueba la conexión con la base de datos MySQL.
        /// </summary>
        /// <returns>True si la conexión es exitosa, False si hay error.</returns>
        /// <remarks>
        /// Útil para verificar que XAMPP está activo antes de intentar
        /// operaciones de login o registro.
        /// </remarks>
        public bool TestConexion()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
