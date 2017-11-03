@echo off

dotnet clean
dotnet restore --packages "./packages"
dotnet build --configuration=Release --no-restore
dotnet test "./test/SocialMediaServices.Tests.Unit/SocialMediaServices.Tests.Unit.csproj" --configuration=Release --no-restore --no-build
dotnet pack "./src/SocialMediaServices/SocialMediaServices.csproj" --configuration=Release --no-restore --no-build --output "../../artifacts"