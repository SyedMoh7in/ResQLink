# Step 1: Build stage using .NET 9 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /app

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the source
COPY . . 
RUN dotnet publish -c Release -o /out

# Step 2: Runtime stage using .NET 9 ASP.NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS runtime
WORKDIR /app

# Copy the published output
COPY --from=build /out .

# Expose default port
EXPOSE 80

# Entry point
ENTRYPOINT ["dotnet", "ResQLink.dll"]
