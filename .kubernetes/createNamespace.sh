#!/bin/bash

FeatureName="#{Octopus.Action[GetFN].Output.FeatureName}"

### Check for namespace cp-<featurename>, if it already exists delete it to make a clean restart

if [[ $(kubectl get namespaces | grep cp-$FeatureName) ]]
then
  echo "---> Namespace cp-$FeatureName already exists, deleting for a clean restart ..."
  kubectl delete namespace cp-$FeatureName
  sleep 30
fi

kubectl create namespace cp-$FeatureName