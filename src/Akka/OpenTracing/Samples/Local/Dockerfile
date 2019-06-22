FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/Akka/OpenTracing/Samples/Local/Akka.OpenTracing.Sample.Local.csproj", "src/Akka/OpenTracing/Samples/Local/"]
RUN dotnet restore "src/Akka/OpenTracing/Samples/Local/Akka.OpenTracing.Sample.Local.csproj"
COPY . .
WORKDIR "/src/src/Akka/OpenTracing/Samples/Local"
RUN dotnet build "Akka.OpenTracing.Sample.Local.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Akka.OpenTracing.Sample.Local.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Akka.OpenTracing.Sample.Local.dll"]