FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY bin/Release/net5.0/publish App/
WORKDIR /App
ENV DOTNET_ENVIRONMENT="Production"
ENV TZ="Europe/Warsaw"
ENTRYPOINT ["dotnet", "NETDiscordBot.dll"]