#!/bin/bash

### install doctl
cd ~
wget #{WGET_PARAM} https://github.com/digitalocean/doctl/releases/download/v1.71.0/doctl-1.71.0-linux-amd64.tar.gz
tar xf ~/doctl-1.71.0-linux-amd64.tar.gz
mv ~/doctl /usr/local/bin

### authenticate
doctl auth init -t #{DOCTL_AUTH_KEY}

### make sure domain does not already exist, then create it
if [[ ! $(doctl compute domain records list #{BASE_DOMAIN} | grep -i #{Octopus.Action[GetFN].Output.FeatureName}-portal) ]]
then

    echo "---> Now creating subdomain $FeatureName for #{BASE_DOMAIN} ..."

    doctl compute domain records create #{BASE_DOMAIN} \
        --record-type A \
        --record-name #{Octopus.Action[GetFN].Output.FeatureName}-portal \
        --record-data #{IP_INGRESS_CONTROLER}

else
    echo "---> DNS entry already exists, doing nothing ..."
fi
