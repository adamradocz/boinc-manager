FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-stretch AS build
WORKDIR /src
COPY ["BoincManagerWeb/BoincManagerWeb.csproj", "BoincManagerWeb/"]
COPY ["BoincRpc/BoincRpc.csproj", "BoincRpc/"]
COPY ["BoincManager/BoincManager.csproj", "BoincManager/"]
RUN dotnet restore "BoincManagerWeb/BoincManagerWeb.csproj"
COPY . .
WORKDIR "/src/BoincManagerWeb"
RUN dotnet build "BoincManagerWeb.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BoincManagerWeb.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BoincManagerWeb.dll"]