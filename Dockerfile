FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
# EXPOSE 5001
WORKDIR /app
EXPOSE 5000
EXPOSE 5001
ENV ASPNETCORE_URLS=http://*:5000
ENV ASPNETCORE_HTTPS_PORT=5001
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_EnableDiagnostics=0
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "webapi.dll"]
