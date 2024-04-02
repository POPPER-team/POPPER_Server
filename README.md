# POPPER_Server

Backend aplication for project POPPER
- [POPPER_Server](#popper_server)
  - [GIT cheat sheet](#git-cheat-sheet)
    - [Adding code](#adding-code)
    - [resolving conflicts](#resolving-conflicts)
!!! do not push to dev or main branch directly!!!
create a new branch and create a pull request to dev branch!!!
---
### Requirements

install docker / [docker desktop](https://www.docker.com/products/docker-desktop/) and [docker-compose](https://docs.docker.com/compose/install/)

recommend [rider](https://www.jetbrains.com/rider/download/#section=linux) for development

find appsettings.json and .env files in shared [onedrive](https://algebrapou-my.sharepoint.com/:f:/r/personal/fcvok_algebra_hr/Documents/POPPER?csf=1&web=1&e=kZLTLJ) folder named POPPER



---
https://github.com/killi1812/POPPER_IphoneApp
---
## Setup the environment

```
docker-compose up
```

for removing the environment

```
docker-compose down
```
---
## GIT cheat sheet


---
basic command for checking information about the branch you are on
```
git status
```

---
### Adding code

create a new branch where branch_name is the name of the branch

```
git checkout -b branch_name
```

make changes to the code and stage those changes

```
git add .
```

commit the changes where commit message is the message for the commit

```
git commit -m "commit message"
```

use for staging and commiting in one command

```
git commit -am "commit message"
```

setup the remote repository where branch_name is the name of the branch

```
git push --set-upstream POPPER_Server branch_name
```

push the changes to the remote repository

```
git push
```

crate a pull request on the github page and add the reviewers (For now only fran)

---
### resolving conflicts

fetch the changes from the remote repository

```
git fetch
```

merge the changes from the remote repository

```
git merge origin/branch_name
```

resolve the conflicts in the code then commit and push
