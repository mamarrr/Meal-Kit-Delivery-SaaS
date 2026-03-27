FROM mcr.microsoft.com/dotnet/sdk:latest AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
# copy ALL the projects
COPY App.BLL/*.csproj ./App.BLL/.
COPY App.Contracts.BLL/*.csproj ./App.Contracts.BLL/.
COPY App.Contracts.DAL/*.csproj ./App.Contracts.DAL/.
COPY App.DAL.EF/*.csproj ./App.DAL.EF/.
COPY App.Domain/*.csproj ./App.Domain/.
COPY App.DTO/*.csproj ./App.DTO/.
COPY App.Helpers/*.csproj ./App.Helpers/.
COPY App.Resources/*.csproj ./App.Resources/.
COPY Base.Contracts.DAL/*.csproj ./Base.Contracts.DAL/.
COPY Base.Contracts.Domain/*.csproj ./Base.Contracts.Domain/.
COPY Base.DAL.EF/*.csproj ./Base.DAL.EF/.
COPY Base.Resources/*.csproj ./Base.Resources/.
COPY WebApp/*.csproj ./WebApp/.
RUN dotnet restore

# copy everything else and build app
# copy all the projects
COPY App.BLL/. ./App.BLL/
COPY App.Contracts.BLL/. ./App.Contracts.BLL/
COPY App.Contracts.DAL/. ./App.Contracts.DAL/
COPY App.DAL.EF/. ./App.DAL.EF/
COPY App.Domain/. ./App.Domain/
COPY App.DTO/. ./App.DTO/
COPY App.Helpers/. ./App.Helpers/
COPY App.Resources/. ./App.Resources/
COPY Base.Contracts.DAL/. ./Base.Contracts.DAL/
COPY Base.Contracts.Domain/. ./Base.Contracts.Domain/
COPY Base.DAL.EF/. ./Base.DAL.EF/
COPY Base.Resources/. ./Base.Resources/
COPY WebApp/. ./WebApp/
WORKDIR /app/WebApp
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime
WORKDIR /app
EXPOSE 8081
COPY --from=build /app/out ./
ENV ConnectionStrings:DefaultConnection="Host=postgres;Port=5432;Database=mealdeliverysaas;Username=postgres;Password=postgres"
ENTRYPOINT ["dotnet", "WebApp.dll"]