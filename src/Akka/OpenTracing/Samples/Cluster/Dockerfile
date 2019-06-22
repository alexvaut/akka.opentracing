FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/Akka/OpenTracing/Samples/Cluster/Akka.OpenTracing.Sample.Cluster.csproj", "src/Akka/OpenTracing/Samples/Cluster/"]
COPY ["src/Akka/OpenTracing/Lib/Akka.OpenTracing.csproj", "src/Akka/OpenTracing/Lib/"]
RUN dotnet restore "src/Akka/OpenTracing/Samples/Cluster/Akka.OpenTracing.Sample.Cluster.csproj"
COPY . .
WORKDIR "/src/src/Akka/OpenTracing/Samples/Cluster"
RUN dotnet build "Akka.OpenTracing.Sample.Cluster.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Akka.OpenTracing.Sample.Cluster.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Akka.OpenTracing.Sample.Cluster.dll"]