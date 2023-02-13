FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NetNuxt/NetNuxt.csproj", "NetNuxt/"]
RUN dotnet restore "NetNuxt/NetNuxt.csproj"
COPY . .
WORKDIR "/src/NetNuxt"
RUN dotnet build "NetNuxt.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NetNuxt.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetNuxt.dll"]
