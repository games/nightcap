#!/bin/bash
set -e 

publish () {
  project_name=$1
  
  project_file=./src/"$project_name"/"$project_name".fsproj
  nupkgDir="./src/$project_name/nupkg"
  
#  find "$nupkgDir" -type f -name "*.nupkg" -delete
  if [ -d "$nupkgDir" ]; then
    rm -r "$nupkgDir"
  fi
  
  dotnet pack "$project_file" --configuration Release
  
  files=("$nupkgDir/*.nupkg")
  nupkgFile="${files[0]}"
  dotnet nuget push "$nupkgFile" --source https://api.nuget.org/v3/index.json --api-key "$API_KEY" --skip-duplicate
  
}

if [[ -f ".env" ]]; then

  export "$(xargs < .env)"
  publish "Nightcap"
  publish "Nightcap.Codecs"
  publish "Nightcap.Crypto"
  publish "Nightcap.Web"

else
  # .env file template
  # API_KEY=your_api_key_for_nuget
  echo ".env file is not found"
fi