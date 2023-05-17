#!/usr/bin/env bash
set -euo pipefail

# install apt-add-repository
apt update 
apt install software-properties-common -y
apt update


dotnet tool install dotnet-ef -g

cat << \EOF >> ~/.bash_profile
# Add .NET Core SDK tools
export PATH="$PATH:/root/.dotnet/tools"
EOF