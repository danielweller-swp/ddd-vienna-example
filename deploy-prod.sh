#!/bin/bash

if [ "$#" -ne 0 ]; then
  echo "USAGE: deploy-prod.sh"
  exit 1
fi

echo "This script will trigger a production deployment (by fast-forwarding
the release branch to the main branch).
"

read -p "Are you sure (y/N)?" -n 1 -r
echo

if [[ $REPLY =~ ^[Yy]$ ]]
then
  git fetch origin release
  git checkout release || exit 1
  git pull || exit 1
  git fetch origin main || exit 1
  git merge --ff-only origin/main || exit 1
  git push || exit 1
fi
