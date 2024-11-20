FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

RUN addgroup --system appgroup && \
    adduser --system --ingroup appgroup appuser
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["DemoApi/DemoApi.csproj", "DemoApi/"]

RUN dotnet restore "DemoApi/DemoApi.csproj"

COPY . .

WORKDIR "/src/DemoApi"
RUN dotnet build "DemoApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DemoApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER appuser

ENTRYPOINT ["dotnet", "DemoApi.dll"]
