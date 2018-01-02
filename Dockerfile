FROM microsoft/aspnetcore-build:2.0.3 AS build-env
WORKDIR /app

COPY . ./
RUN cd src/Rollvolet.CRM.API \
    && dotnet restore \
    && dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0.3
WORKDIR /app
COPY --from=build-env /app/src/Rollvolet.CRM.API/out .
ENTRYPOINT ["dotnet", "Rollvolet.CRM.API.dll"]
