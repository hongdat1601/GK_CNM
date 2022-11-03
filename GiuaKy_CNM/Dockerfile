#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GiuaKy_CNM/GiuaKy_CNM.csproj", "GiuaKy_CNM/"]
RUN dotnet restore "GiuaKy_CNM/GiuaKy_CNM.csproj"
COPY . .
WORKDIR "/src/GiuaKy_CNM"
RUN dotnet build "GiuaKy_CNM.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GiuaKy_CNM.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GiuaKy_CNM.dll"]