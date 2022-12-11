FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# copy all the layers' csproj files into respective folders
COPY ["./POSsystem.Contracts/POSsystem.Contracts.csproj", "src/POSsystem.Contracts/"]
COPY ["./POSsystem.Migrations/POSsystem.Migrations.csproj", "src/POSsystem.Migrations/"]
COPY ["./POSsystem.Infrastructure/POSsystem.Infrastructure.csproj", "src/POSsystem.Infrastructure/"]
COPY ["./POSsystem.Core/POSsystem.Core.csproj", "src/POSsystem.Core/"]
COPY ["./POSsystem.API/POSsystem.API.csproj", "src/POSsystem.API/"]

# run restore over API project - this pulls restore over the dependent projects as well
RUN dotnet restore "src/POSsystem.API/POSsystem.API.csproj"

COPY . .

# run build over the API project
WORKDIR "/src/POSsystem.API/"
RUN dotnet build -c Release -o /app/build

# run publish over the API project
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS runtime
WORKDIR /app

COPY --from=publish /app/publish .
RUN ls -l
ENTRYPOINT [ "dotnet", "POSsystem.API.dll" ]