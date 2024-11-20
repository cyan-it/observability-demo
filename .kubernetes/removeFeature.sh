#!/bin/bash

### check if CI_COMMIT_TITLE includes the string "Merge branch 'feature/[[:alnum:]]\+' into" (after removing dashes), if so,
### -> get and clean-up feature name,
### -> delete old feature domain if it exists,
### -> delete complete namespace with that feature name
### -> delete feature db

cd #{PaketName}
isFeature=$(cat ./gitlab-commit.env | sed ' s/-//g ' | grep "Merge branch 'feature/[[:alnum:]]\+' into")

if [[ $isFeature ]]
then
   ### install doctl
   cd ~
   wget #{WGET_PARAM} https://github.com/digitalocean/doctl/releases/download/v1.71.0/doctl-1.71.0-linux-amd64.tar.gz
   tar xf ~/doctl-1.71.0-linux-amd64.tar.gz
   mv ~/doctl /usr/local/bin

   ### authenticate
   doctl auth init -t #{DOCTL_AUTH_KEY}

  FeatureNumber=$(echo $isFeature | grep -o -E feature/[[:alnum:]]\+ |  sed ' s/[a-zA-Z\/]//g ')
  FeatureName=$(echo "feature$FeatureNumber")
  FeatureNameDNS=$(echo "$FeatureName-portal")
  DNSRecordId=$(doctl compute domain records list #{BASE_DOMAIN} | grep $FeatureNameDNS | awk ' { print $1 } ')
  FeatureNamespace=$(kubectl get namespaces | grep "cp-$FeatureName")

  echo "---> Now deleting subdomain if necessary ..."

  if [[ $DNSRecordId ]]
  then
    echo "---> Deleting subdomain ID $DNSRecordId / $FeatureNameDNS from #{BASE_DOMAIN} ..."
    doctl compute domain records delete #{BASE_DOMAIN} $DNSRecordId -f
  else
    echo "---> Subdomain $FeatureNameDNS does not exist, nothing to delete ..."
  fi

  echo "---> Now deleting namespace if necessary ..."

  if [[ $FeatureNamespace ]]
  then
    echo "---> Deleting namespace cp-$FeatureName ..."
    kubectl delete namespace cp-$FeatureName
  else
    echo "---> Namespace cp-$FeatureName does not exist, nothing to delete ..."
  fi

  echo "---> Now deleting database if necessary ..."
  kubectl exec -n cp-feature-db -c cp-feature-db db-0 -- /opt/mssql/featuredb/DBdrop_feature.sh $FeatureName
else
  echo "---> Not a feature merge, doing nothing ..."
fi
