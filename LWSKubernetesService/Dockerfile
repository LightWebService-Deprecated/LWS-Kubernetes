FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LWSKubernetesService/LWSKubernetesService.csproj", "LWSKubernetesService/"]
RUN dotnet restore "LWSKubernetesService/LWSKubernetesService.csproj"
COPY . .
WORKDIR "/src/LWSKubernetesService"
RUN dotnet build "LWSKubernetesService.csproj" -c Debug -o /app/build
RUN dotnet publish "LWSKubernetesService.csproj" -c Debug -o /app/publish
WORKDIR /app/publish
ENTRYPOINT ["dotnet", "LWSKubernetesService.dll"]
