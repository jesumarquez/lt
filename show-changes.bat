@echo off
echo MERGES:
git rev-list --tags --merges --pretty=oneline --abbrev-commit master --not %1 
echo.
echo NO MERGES - BRANCH:
git rev-list --tags --no-merges --pretty=oneline --abbrev-commit master --not %1 --simplify-by-decoration
echo.
echo NO MERGES - MASTER:
git rev-list --tags --no-merges --pretty=oneline --abbrev-commit master --not %1

 