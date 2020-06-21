#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Pi.Dns.Web.Statistics/Pi.Dns.Web.Statistics.csproj", "Pi.Dns.Web.Statistics/"]
RUN dotnet restore "Pi.Dns.Web.Statistics/Pi.Dns.Web.Statistics.csproj"
COPY . .
WORKDIR "/src/Pi.Dns.Web.Statistics"
RUN dotnet build "Pi.Dns.Web.Statistics.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pi.Dns.Web.Statistics.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pi.Dns.Web.Statistics.dll"]