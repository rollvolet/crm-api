FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app

COPY . ./
RUN cd src/Rollvolet.CRM.API \
    && dotnet restore \
    && dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/src/Rollvolet.CRM.API/out .
ENTRYPOINT ["dotnet", "Rollvolet.CRM.API.dll"]
