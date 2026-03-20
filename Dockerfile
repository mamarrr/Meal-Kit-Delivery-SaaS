# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY "webapp2025s.sln" ./
COPY "WebApp/WebApp.csproj" ./WebApp/
COPY "App.BLL/App.BLL.csproj" ./App.BLL/
COPY "App.Contracts.BLL/App.Contracts.BLL.csproj" ./App.Contracts.BLL/
COPY "App.Contracts.DAL/App.Contracts.DAL.csproj" ./App.Contracts.DAL/
COPY "App.DAL.EF/App.DAL.EF.csproj" ./App.DAL.EF/
COPY "App.DTO/App.DTO.csproj" ./App.DTO/
COPY "App.Resources/App.Resources.csproj" ./App.Resources/
COPY "App.Domain/App.Domain.csproj" ./App.Domain/
COPY "Base.Contracts.DAL/Base.Contracts.DAL.csproj" ./Base.Contracts.DAL/
COPY "Base.Contracts.Domain/Base.Contracts.Domain.csproj" ./Base.Contracts.Domain/
COPY "Base.DAL.EF/Base.DAL.EF.csproj" ./Base.DAL.EF/
COPY "Base.Resources/Base.Resources.csproj" ./Base.Resources/

RUN dotnet restore "WebApp/WebApp.csproj"

COPY . .
WORKDIR "/src/WebApp"
RUN dotnet build "WebApp.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "WebApp.csproj" -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends tzdata \
    && ln -sf /usr/share/zoneinfo/Europe/Tallinn /etc/localtime \
    && echo "Europe/Tallinn" > /etc/timezone \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "WebApp.dll"]
