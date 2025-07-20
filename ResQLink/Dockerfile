# -------- Stage 1: Build --------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy project file and restore
COPY ResQLink.csproj ./
RUN dotnet restore

# Copy everything else and publish
COPY . . 
RUN dotnet publish -c Release -o /out

# -------- Stage 2: Runtime --------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "ResQLink.dll"]
