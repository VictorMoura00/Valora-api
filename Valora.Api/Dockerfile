# Estágio Base (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Estágio de Build (SDK)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. COPIAR TODOS OS PROJETOS (.csproj)
# O Docker precisa copiar os arquivos de projeto de TODAS as camadas
# para conseguir resolver as dependências durante o 'restore'.
COPY ["Valora.Api/Valora.Api.csproj", "Valora.Api/"]
COPY ["Valora.Application/Valora.Application.csproj", "Valora.Application/"]
COPY ["Valora.Domain/Valora.Domain.csproj", "Valora.Domain/"]
COPY ["Valora.Infra/Valora.Infra.csproj", "Valora.Infra/"]

# 2. RESTAURAR DEPENDÊNCIAS
# Ao restaurar a API, ele puxa automaticamente as dependências dos outros projetos copiados acima
RUN dotnet restore "Valora.Api/Valora.Api.csproj"

# 3. COPIAR O RESTANTE DO CÓDIGO
COPY . .

# 4. BUILD
WORKDIR "/src/Valora.Api"
RUN dotnet build "Valora.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Estágio de Publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Valora.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Estágio Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Valora.Api.dll"]