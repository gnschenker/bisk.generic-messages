FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch
WORKDIR /src
COPY ["common/common.csproj", "common/"]
COPY ["publisher/publisher.csproj", "publisher/"]
RUN dotnet restore "publisher/publisher.csproj"
COPY . .
RUN dotnet build "publisher/publisher.csproj"
CMD dotnet run -p publisher/publisher.csproj