# Dockerfile to build container for API service. 
# Call with solution directory as build context or use ``docker-compose up service``
# See also: https://www.softwaredeveloper.blog/multi-project-dotnet-core-solution-in-docker-image

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /LoRaWeatherStation

# Copy everything and build
COPY . ./
RUN dotnet publish LoRaWeatherStation.Service -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /LoRaWeatherStation/out .
ENTRYPOINT ["dotnet", "LoRaWeatherStation.Service.dll"]