FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch
WORKDIR /src
COPY ["common/common.csproj", "common/"]
COPY ["consumer/consumer.csproj", "consumer/"]
RUN dotnet restore "consumer/consumer.csproj"
COPY . .
RUN dotnet build "consumer/consumer.csproj"
CMD dotnet run -p consumer/consumer.csproj