# Use the official .NET 8 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy the .csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy the remaining source code and build the application
COPY . ./
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/out .

# Expose the application port
EXPOSE 8080

# Define the entry point for your application
ENTRYPOINT ["dotnet", "Goarif-Main.dll"]
