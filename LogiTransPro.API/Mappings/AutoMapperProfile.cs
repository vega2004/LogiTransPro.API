using AutoMapper;
using LogiTransPro.API.Models.DTOs.Auth;
using LogiTransPro.API.Models.DTOs.Cliente;
using LogiTransPro.API.Models.DTOs.Dashboard;
using LogiTransPro.API.Models.DTOs.Mantenimiento;
using LogiTransPro.API.Models.DTOs.OrdenCarga;
using LogiTransPro.API.Models.DTOs.Ruta;
using LogiTransPro.API.Models.DTOs.Vehiculo;
using LogiTransPro.API.Models.DTOs.Viaje;
using LogiTransPro.API.Models.Entities;

namespace LogiTransPro.API.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ======================================================
            // AUTH MAPPINGS
            // ======================================================
            CreateMap<Usuario, LoginResponseDTO>()
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol != null ? src.Rol.NombreRol : "Operador"))
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.TokenExpiracion, opt => opt.Ignore());

            // ======================================================
            // VEHICULO MAPPINGS
            // ======================================================
            CreateMap<Vehiculo, VehiculoDTO>();
            CreateMap<CrearVehiculoDTO, Vehiculo>();
            CreateMap<ActualizarVehiculoDTO, Vehiculo>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ======================================================
            // MANTENIMIENTO MAPPINGS
            // ======================================================
            // Entity → DTO
            CreateMap<Mantenimiento, MantenimientoDTO>()
                .ForMember(dest => dest.VehiculoPlaca, opt => opt.MapFrom(src => src.Vehiculo != null ? src.Vehiculo.Placa : string.Empty))
                .ForMember(dest => dest.Partes, opt => opt.MapFrom(src => src.Partes));

            // Entity → DTO (Partes)
            CreateMap<ParteMantenimiento, ParteMantenimientoDTO>();

            // DTO → Entity (Crear)
            CreateMap<CrearMantenimientoDTO, Mantenimiento>()
                .ForMember(dest => dest.Partes, opt => opt.MapFrom(src => src.Partes))
                .ForMember(dest => dest.Estatus, opt => opt.MapFrom(src => "P"))
                .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.MantenimientoId, opt => opt.Ignore())
                .ForMember(dest => dest.Vehiculo, opt => opt.Ignore());

            // DTO → Entity (Partes) - mismo DTO se usa para crear y mostrar
            CreateMap<ParteMantenimientoDTO, ParteMantenimiento>()
                .ForMember(dest => dest.ParteId, opt => opt.Ignore())
                .ForMember(dest => dest.MantenimientoId, opt => opt.Ignore())
                .ForMember(dest => dest.Mantenimiento, opt => opt.Ignore());
            // ======================================================
            // VIAJE MAPPINGS
            // ======================================================
            CreateMap<Viaje, ViajeDTO>()
                .ForMember(dest => dest.NumeroOrden, opt => opt.MapFrom(src => src.OrdenCarga != null ? src.OrdenCarga.NumeroOrden : string.Empty))
                .ForMember(dest => dest.VehiculoPlaca, opt => opt.MapFrom(src => src.Vehiculo != null ? src.Vehiculo.Placa : string.Empty))
                .ForMember(dest => dest.ChoferNombre, opt => opt.MapFrom(src => src.Chofer != null ? src.Chofer.NombreCompleto : string.Empty))
                .ForMember(dest => dest.RutaNombre, opt => opt.MapFrom(src => src.Ruta != null ? src.Ruta.NombreRuta : string.Empty))
                .ForMember(dest => dest.Origen, opt => opt.MapFrom(src => src.Ruta != null ? src.Ruta.Origen : string.Empty))
                .ForMember(dest => dest.Destino, opt => opt.MapFrom(src => src.Ruta != null ? src.Ruta.Destino : string.Empty))
                .ForMember(dest => dest.KilometrajeRecorrido, opt => opt.MapFrom(src =>
                    src.KilometrajeFinal.HasValue && src.KilometrajeInicial.HasValue
                        ? src.KilometrajeFinal - src.KilometrajeInicial
                        : (int?)null))
                .ForMember(dest => dest.TiempoTranscurrido, opt => opt.Ignore())
                .ForMember(dest => dest.EstaRetrasado, opt => opt.Ignore());

            CreateMap<CrearViajeDTO, Viaje>()
                .ForMember(dest => dest.NumeroViaje, opt => opt.Ignore())
                .ForMember(dest => dest.Estatus, opt => opt.MapFrom(src => "P"))
                .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)));

            // Mapeo para actualización (solo propiedades que existen en la entidad)
            CreateMap<ViajeDTO, Viaje>()
                .ForMember(dest => dest.ViajeId, opt => opt.Ignore())
                .ForMember(dest => dest.NumeroViaje, opt => opt.Ignore())
                .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore())
                .ForMember(dest => dest.OrdenCarga, opt => opt.Ignore())
                .ForMember(dest => dest.Vehiculo, opt => opt.Ignore())
                .ForMember(dest => dest.Chofer, opt => opt.Ignore())
                .ForMember(dest => dest.Ruta, opt => opt.Ignore())
                .ForMember(dest => dest.Incidentes, opt => opt.Ignore());

            // ======================================================
            // RUTA MAPPINGS
            // ======================================================
            CreateMap<Ruta, RutaDTO>();
            CreateMap<RutaDTO, Ruta>()
                .ForMember(dest => dest.RutaId, opt => opt.Ignore())  // Ignorar la llave primaria
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true));

            // ======================================================
            // CLIENTE MAPPINGS
            // ======================================================
            CreateMap<Cliente, ClienteDTO>();
            CreateMap<CrearClienteDTO, Cliente>()
                .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true));
            // Agregar este mapeo para actualizaciones
            CreateMap<ClienteDTO, Cliente>()
                .ForMember(dest => dest.ClienteId, opt => opt.Ignore())  // Ignorar la llave primaria
                .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore())  // No modificar fecha de registro
                .ForMember(dest => dest.OrdenesCarga, opt => opt.Ignore());  // Ignorar navegación
                                                                             // ======================================================
                                                                             // ORDEN CARGA MAPPINGS
                                                                             // ======================================================
            CreateMap<OrdenCarga, OrdenCargaDTO>()
                .ForMember(dest => dest.ClienteNombre, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.NombreRazonSocial : string.Empty));

            CreateMap<CrearOrdenCargaDTO, OrdenCarga>()
                .ForMember(dest => dest.NumeroOrden, opt => opt.Ignore())
                .ForMember(dest => dest.FechaSolicitud, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Estatus, opt => opt.MapFrom(src => "P"));

            // Mapeo para actualización
            CreateMap<OrdenCargaDTO, OrdenCarga>()
                .ForMember(dest => dest.OrdenCargaId, opt => opt.Ignore())
                .ForMember(dest => dest.NumeroOrden, opt => opt.Ignore())
                .ForMember(dest => dest.FechaSolicitud, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore())
                .ForMember(dest => dest.Viajes, opt => opt.Ignore());

            // ======================================================
            // DASHBOARD MAPPINGS
            // ======================================================
            CreateMap<ProximoMantenimientoDTO, ProximoMantenimientoDTO>();
        }
    }
}