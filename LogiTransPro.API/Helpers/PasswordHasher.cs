using BCrypt.Net;

namespace LogiTransPro.API.Helpers
{
    public static class PasswordHasher
    {
        // Factor de trabajo: 10-12 es un buen balance entre seguridad y performance
        private const int WorkFactor = 12;

        /// <summary>
        /// Genera un hash seguro de la contraseña usando BCrypt
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash de la contraseña con salt incluido</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña no puede estar vacía");

            // BCrypt genera automáticamente un salt y lo incluye en el hash
            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        /// <summary>
        /// Verifica si una contraseña coincide con su hash
        /// </summary>
        /// <param name="password">Contraseña en texto plano a verificar</param>
        /// <param name="hashedPassword">Hash almacenado</param>
        /// <returns>True si la contraseña es correcta</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si la contraseña necesita ser re-hasheada (por cambio de WorkFactor)
        /// </summary>
        /// <param name="hashedPassword">Hash almacenado</param>
        /// <returns>True si necesita re-hash</returns>
        public static bool NeedsRehash(string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                return true;

            try
            {
                return BCrypt.Net.BCrypt.PasswordNeedsRehash(hashedPassword, WorkFactor);
            }
            catch
            {
                return true;
            }
        }
    }
}