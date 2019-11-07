FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["publisher/publisher.csproj", "publisher/"]
RUN dotnet restore "publisher/publisher.csproj"
COPY . .
WORKDIR "/src/publisher"
RUN dotnet build "publisher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "publisher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "publisher.dll"]