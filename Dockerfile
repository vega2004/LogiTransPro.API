FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar el archivo .sln y los proyectos
COPY LogiTransPro.API.sln .
COPY LogiTransPro.API/*.csproj ./LogiTransPro.API/

# Restaurar dependencias
RUN dotnet restore

# Copiar todo el código
COPY LogiTransPro.API/. ./LogiTransPro.API/

# Publicar la aplicación
WORKDIR /app/LogiTransPro.API
RUN dotnet publish -c Release -o out

# Imagen de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copiar los archivos publicados
COPY --from=build /app/LogiTransPro.API/out ./

# Configurar el punto de entrada
ENTRYPOINT ["dotnet", "LogiTransPro.API.dll"]