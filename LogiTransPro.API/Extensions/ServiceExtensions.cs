using LogiTransPro.API.Services.Auth;
using LogiTransPro.API.Services.Vehiculo;
using LogiTransPro.API.Services.Viaje;
using LogiTransPro.API.Services.Ruta;
using LogiTransPro.API.Services.Cliente;
using LogiTransPro.API.Services.OrdenCarga;
using LogiTransPro.API.Services.Mantenimiento;
using LogiTransPro.API.Services.Dashboard;

namespace LogiTransPro.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            // Auth services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<JWTService>();

            // Vehiculo services
            services.AddScoped<IVehiculoService, VehiculoService>();

            // Viaje services
            services.AddScoped<IViajeService, ViajeService>();

            // Ruta services
            services.AddScoped<IRutaService, RutaService>();

            // Cliente services
            services.AddScoped<IClienteService, ClienteService>();

            // OrdenCarga services
            services.AddScoped<IOrdenCargaService, OrdenCargaService>();

            // Mantenimiento services
            services.AddScoped<IMantenimientoService, MantenimientoService>();

            // Dashboard services
            services.AddScoped<IDashboardService, DashboardService>();

            return services;
        }
    }
}