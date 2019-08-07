FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

COPY . ./
RUN cd src/Rollvolet.CRM.API \
    && dotnet restore \
    && dotnet publish -c Release -o out

# Build runtime image
FROM  mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/src/Rollvolet.CRM.API/out .
ENTRYPOINT ["dotnet", "Rollvolet.CRM.API.dll"]
