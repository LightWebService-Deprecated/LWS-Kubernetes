FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LWSKubernetesService/LWSKubernetesService.csproj", "LWSKubernetesService/"]
RUN dotnet restore "LWSKubernetesService/LWSKubernetesService.csproj"
COPY . .
WORKDIR "/src/LWSKubernetesService"
RUN dotnet build "LWSKubernetesService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LWSKubernetesService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LWSKubernetesService.dll"]
