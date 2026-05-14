#!/bin/bash
# 1. Create namespace
kubectl create namespace argocd

# 2. Install Argo CD
kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml

# 3. Wait for Argo CD components to be ready
kubectl wait --for=condition=available --timeout=600s deployment/argocd-server -n argocd