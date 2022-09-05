find . -name bin -type d -exec rm -rf {} \;
find . -name obj -type d -exec rm -rf {} \;
for file in $(find . -type f -name "*.csproj"); do dotnet clean $file; done
for file in $(find . -type f -name "*.csproj"); do dotnet restore $file; done
for file in $(find . -type f -name "*.csproj"); do dotnet build $file; done

for file in $(find . -type f -name "*.csproj"); do dotnet list $file package --outdated; done

for file in $(find . -type f -name "*.csproj"); do dotnet sln ./sln/dhc.sln add $file; done
