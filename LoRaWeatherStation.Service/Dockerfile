# Dockerfile to build container for API service. 
# Call with solution directory as build context or use ``docker-compose up service``
# See also: https://www.softwaredeveloper.blog/multi-project-dotnet-core-solution-in-docker-image

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /work

# Copy everything and build
COPY . ./
RUN dotnet publish LoRaWeatherStation.Service -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /work/out .
ENTRYPOINT ["dotnet", "LoRaWeatherStation.Service.dll"]