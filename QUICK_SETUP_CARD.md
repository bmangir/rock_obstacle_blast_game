# Quick Setup Card - Rock The Obstacles Game

## Essential Unity Editor Steps (5 minutes)

### 1. Project Settings (1 min)
- **Edit > Project Settings > Player**
  - Default Orientation: Portrait
  - Allowed Orientations: Portrait only
  - Target Device: Mobile

- **File > Build Settings**
  - Add MainScene (index 0)
  - Add LevelScene (index 1)

### 2. MainScene Setup (2 min)
1. **Camera**: Orthographic, Size=5, Position=(0,0,-10)
2. **Create GameObjects**:
   - LevelManager (add LevelManager script)
   - MainMenuController (add MainMenuController script)
3. **Create UI Canvas**:
   - MainMenuCanvas (Scale With Screen Size, 1080x1920)
   - Background (black image, stretch to fill)
   - LevelButton (center, use button.png)
   - LevelText (child of button, "Level 1")
4. **Connect Scripts**: Drag LevelText and LevelButton to MainMenuController

### 3. LevelScene Setup (2 min)
1. **Camera**: Orthographic, Size=5, Position=(0,0,-10)
2. **Create GameObjects**:
   - GridManager (add GridManager script)
   - LevelLoader (add LevelLoader script)
   - GoalPanelManager (add GoalPanelManager script)
   - GameResultManager (add GameResultManager script)
3. **Create GameCanvas** (Sorting Order=0):
   - TopPanel (top stretch, height=150)
   - MovesText (left side, "Moves: 20")
   - Goal items (Box, Stone, Vase with icons and counts)
4. **Create ResultCanvas** (Sorting Order=100):
   - WinScreen (initially disabled)
   - LoseScreen (initially disabled)
   - Both with Background, PopupPanel, and buttons
5. **Connect Scripts**: Drag all prefabs and UI elements to respective managers

### 4. Test (1 min)
1. **MainScene**: Press Play, click Level button
2. **LevelScene**: Press Play, test cube clicking
3. **Add GameTestHelper** to LevelScene for testing win/lose screens

## Key Files to Check
- ✅ Assets/Scripts/Managers/GameResultManager.cs (NEW)
- ✅ Assets/Scripts/Managers/SceneSetupHelper.cs (NEW)
- ✅ Assets/Scripts/Managers/GameTestHelper.cs (NEW)
- ✅ Assets/Scripts/Managers/GoalPanelManager.cs (UPDATED)
- ✅ Assets/Resources/Levels/level_01.json to level_10.json
- ✅ Assets/Resources/UI/Menu/button.png
- ✅ Assets/Resources/UI/Gameplay/Popup/popup_base.png

## Common Issues & Solutions
- **Scenes not loading**: Check Build Settings
- **UI not scaling**: Check Canvas Scaler (1080x1920, Match=0.5)
- **Buttons not working**: Check script connections in Inspector
- **Camera issues**: GridManager auto-adjusts camera

## Test Commands (in Game view)
- Press **W**: Test win screen
- Press **L**: Test lose screen
- Press **N**: Next level
- Press **R**: Reset level

## Final Checklist
- [ ] MainScene loads and shows level button
- [ ] Level button loads LevelScene
- [ ] LevelScene shows grid with cubes
- [ ] Clicking cubes blasts them
- [ ] Win screen appears when all obstacles cleared
- [ ] Lose screen appears when moves run out
- [ ] Continue button goes to next level
- [ ] Return button goes to MainScene
- [ ] Try Again button restarts current level 