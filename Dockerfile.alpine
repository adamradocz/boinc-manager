# Builder image
FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS build
WORKDIR /src

# Copy csprojs and restore as distinct layers
COPY ["BoincManagerWeb/BoincManagerWeb.csproj", "BoincManagerWeb/"]
COPY ["BoincRpc/BoincRpc.csproj", "BoincRpc/"]
COPY ["BoincManager/BoincManager.csproj", "BoincManager/"]
RUN dotnet restore "BoincManagerWeb/BoincManagerWeb.csproj"

# Copy everything else and build app
COPY . .
WORKDIR "/src/BoincManagerWeb"
RUN dotnet publish "BoincManagerWeb.csproj" -c Release --no-restore -o /app


# Runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine

# Set the ApplicationData folder path
ENV XDG_CONFIG_HOME="/app"

WORKDIR ${XDG_CONFIG_HOME}
EXPOSE 80 443

COPY --from=build /app .
ENTRYPOINT ["dotnet", "BoincManagerWeb.dll"]