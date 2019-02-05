This is the repository for the main game.

# Git rules

Code your feature on a dedicated branch. When it's done, start a pull request to merge it into `develop`.
`master` is always up-to-date with the latest working version of the project.

# How to compile and run

Unity version should be higher or equal to 2018.3. To compile it, just add the scenes to the build in the Build Settings.

# Architecture

From the root (`Assets/`):
- **Packages**: Folder to store the packages from the Asset Store. Please, try to categorize the packages you download into sub-folders
  - **Networking**: Network-related packages. 
  - ...
- **Scenes**: Unity Scenes of the game.
- **Scripts**: Various scripts attached to game objects. Please, try to categorize them into sub-folders.
- **Prefabs**: Prefabs folder. Please try to categorize them into sub-folders.