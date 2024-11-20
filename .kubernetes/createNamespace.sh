#!/bin/bash

FeatureName="#{Octopus.Action[GetFN].Output.FeatureName}"

### Check for namespace observability-demo-<featurename>, if it already exists delete it to make a clean restart

if [[ $(kubectl get namespaces | grep observability-demo-$FeatureName) ]]
then
  echo "---> Namespace observability-demo-$FeatureName already exists, deleting for a clean restart ..."
  kubectl delete namespace observability-demo-$FeatureName
  sleep 30
fi

kubectl create namespace observability-demo-$FeatureName