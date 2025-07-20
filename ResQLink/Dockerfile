# -------- Stage 1: Build --------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the solution and restore dependencies
COPY ResQLink/*.csproj ./ResQLink/
RUN dotnet restore ResQLink/ResQLink.csproj

# Copy everything and publish the app
COPY . .
WORKDIR /app/ResQLink
RUN dotnet publish -c Release -o /out

# -------- Stage 2: Runtime --------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Expose port (optional, if running locally)
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "ResQLink.dll"]
