namespace LogiTransPro.API.Constants
{
    public static class RolesConstants
    {
        // Nombres de roles
        public const string Admin = "Admin";
        public const string Supervisor = "Supervisor";
        public const string Operador = "Operador";
        public const string Chofer = "Chofer";

        // IDs de roles (si los necesitas)
        public static class Ids
        {
            public const int Admin = 1;
            public const int Supervisor = 2;
            public const int Operador = 3;
            public const int Chofer = 4;
        }

        // Descripciones de roles
        public static class Descripciones
        {
            public const string Admin = "Administrador del sistema con todos los permisos";
            public const string Supervisor = "Supervisor con permisos de monitoreo y reportes";
            public const string Operador = "Operador con permisos básicos de operación";
            public const string Chofer = "Chofer con acceso a viajes asignados";
        }

        // Lista de todos los roles
        public static readonly string[] AllRoles = new[]
        {
            Admin,
            Supervisor,
            Operador,
            Chofer
        };

        // Roles con permisos administrativos
        public static readonly string[] AdminRoles = new[]
        {
            Admin,
            Supervisor
        };

        // Método para verificar si un rol es válido
        public static bool IsValidRole(string role)
        {
            return AllRoles.Contains(role);
        }

        // Método para obtener descripción de un rol
        public static string GetDescripcion(string role)
        {
            return role switch
            {
                Admin => Descripciones.Admin,
                Supervisor => Descripciones.Supervisor,
                Operador => Descripciones.Operador,
                Chofer => Descripciones.Chofer,
                _ => "Rol desconocido"
            };
        }
    }
}