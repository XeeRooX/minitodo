FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["minitodo/minitodo.csproj", "minitodo/"]
RUN dotnet restore "minitodo/minitodo.csproj"
COPY . .
WORKDIR "/src/minitodo"

RUN dotnet build "minitodo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "minitodo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN mkdir -p /app/Data

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "minitodo.dll"]