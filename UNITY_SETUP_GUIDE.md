# Unity Editor Setup Guide for Rock The Obstacles Game

## Overview
This guide will help you set up the Unity Editor for the cube blasting game with proper scene management, UI setup, and game flow.

## Prerequisites
- Unity 6000.0.32f1
- All scripts are already in the Assets/Scripts folder
- All resources (images, prefabs) are in the Assets/Resources folder
- All level data is in Assets/Resources/Levels folder

## Step 1: Project Settings

### 1.1 Player Settings
1. Go to **Edit > Project Settings > Player**
2. Set **Default Orientation** to **Portrait**
3. Under **Allowed Orientations**, check only **Portrait**
4. Set **Target Device** to **Mobile**

### 1.2 Build Settings
1. Go to **File > Build Settings**
2. Add scenes in this order:
   - **MainScene** (index 0)
   - **LevelScene** (index 1)
3. Set **MainScene** as the default scene

## Step 2: MainScene Setup

### 2.1 Camera Setup
1. Select the **Main Camera** in the scene
2. In Inspector, set:
   - **Projection**: Orthographic
   - **Size**: 5
   - **Position**: (0, 0, -10)
   - **Clear Flags**: Solid Color
   - **Background Color**: White or light blue

### 2.2 GameObjects Setup
1. Create an empty GameObject named **LevelManager**
   - Add the **LevelManager** script component
2. Create an empty GameObject named **MainMenuController**
   - Add the **MainMenuController** script component

### 2.3 UI Setup
1. Create a **UI Canvas**:
   - Right-click in Hierarchy > UI > Canvas
   - Name it **MainMenuCanvas**
   - Add **Canvas Scaler** component if not present
   - Set **UI Scale Mode** to **Scale With Screen Size**
   - Set **Reference Resolution** to **1080x1920**
   - Set **Match** to **0.5**

2. Create Background:
   - Right-click on Canvas > UI > Image
   - Name it **Background**
   - Set **Color** to black or gradient
   - Set **Anchor** to stretch to fill entire screen

3. Create Level Button:
   - Right-click on Canvas > UI > Button
   - Name it **LevelButton**
   - Set **Image** component to use **button.png** from Resources/UI/Menu/
   - Position it in the center of the screen
   - Set **Anchor** to center

4. Create Level Text:
   - Right-click on LevelButton > UI > Text - TextMeshPro
   - Name it **LevelText**
   - Set text to **"Level 1"**
   - Set **Font Size** to 24
   - Set **Alignment** to center
   - Set **Color** to black

### 2.4 Script Connections
1. Select **MainMenuController** GameObject
2. In Inspector, drag:
   - **LevelText** to **Level Button Text** field
   - **LevelButton** to **Level Button** field

## Step 3: LevelScene Setup

### 3.1 Camera Setup
1. Select the **Main Camera** in the scene
2. In Inspector, set:
   - **Projection**: Orthographic
   - **Size**: 5 (will be adjusted by GridManager)
   - **Position**: (0, 0, -10)
   - **Clear Flags**: Solid Color
   - **Background Color**: White or light blue

### 3.2 GameObjects Setup
1. Create an empty GameObject named **GridManager**
   - Add the **GridManager** script component
2. Create an empty GameObject named **LevelLoader**
   - Add the **LevelLoader** script component
3. Create an empty GameObject named **GoalPanelManager**
   - Add the **GoalPanelManager** script component
4. Create an empty GameObject named **GameResultManager**
   - Add the **GameResultManager** script component

### 3.3 Game UI Canvas
1. Create a **UI Canvas** for game UI:
   - Right-click in Hierarchy > UI > Canvas
   - Name it **GameCanvas**
   - Set **Sorting Order** to 0
   - Add **Canvas Scaler** component
   - Set **UI Scale Mode** to **Scale With Screen Size**
   - Set **Reference Resolution** to **1080x1920**
   - Set **Match** to **0.5**

2. Create Top Panel:
   - Right-click on GameCanvas > UI > Panel
   - Name it **TopPanel**
   - Set **Anchor** to top stretch
   - Set **Height** to 150
   - Set **Color** to semi-transparent

3. Create Moves Text:
   - Right-click on TopPanel > UI > Text - TextMeshPro
   - Name it **MovesText**
   - Set text to **"Moves: 20"**
   - Position it on the left side
   - Set **Font Size** to 18

4. Create Goal Items:
   - Create 3 Image objects for Box, Stone, and Vase goals
   - Each should have:
     - Icon (Image component)
     - Count Text (TextMeshPro)
     - Check Mark (Image component, initially disabled)

### 3.4 Result UI Canvas
1. Create a **UI Canvas** for result screens:
   - Right-click in Hierarchy > UI > Canvas
   - Name it **ResultCanvas**
   - Set **Sorting Order** to 100
   - Add **Canvas Scaler** component
   - Set **UI Scale Mode** to **Scale With Screen Size**
   - Set **Reference Resolution** to **1080x1920**
   - Set **Match** to **0.5**

2. Create Win Screen:
   - Right-click on ResultCanvas > Create Empty
   - Name it **WinScreen**
   - Set **Active** to false initially
   
   - Add Background:
     - Right-click on WinScreen > UI > Image
     - Name it **Background**
     - Set **Color** to black with alpha 0.8
     - Set **Anchor** to stretch to fill
   
   - Add Popup Panel:
     - Right-click on WinScreen > UI > Image
     - Name it **PopupPanel**
     - Set **Image** to **popup_base.png** from Resources/UI/Gameplay/Popup/
     - Set **Anchor** to center
     - Set **Size** to 400x500
   
   - Add Level Complete Text:
     - Right-click on PopupPanel > UI > Text - TextMeshPro
     - Name it **LevelCompleteText**
     - Set text to **"Level 1 Complete!"**
     - Position at top of popup
     - Set **Font Size** to 24
   
   - Add Continue Button:
     - Right-click on PopupPanel > UI > Button
     - Name it **ContinueButton**
     - Set **Image** to **button.png** from Resources/UI/Menu/
     - Position in middle of popup
     - Add Text child: **"Continue"**
   
   - Add Return Button:
     - Right-click on PopupPanel > UI > Button
     - Name it **ReturnButton**
     - Set **Image** to **button.png** from Resources/UI/Menu/
     - Position at bottom of popup
     - Add Text child: **"Return"**

3. Create Lose Screen:
   - Right-click on ResultCanvas > Create Empty
   - Name it **LoseScreen**
   - Set **Active** to false initially
   
   - Add Background (same as WinScreen)
   - Add Popup Panel (same as WinScreen)
   
   - Add Game Over Text:
     - Right-click on PopupPanel > UI > Text - TextMeshPro
     - Name it **GameOverText**
     - Set text to **"Game Over!"**
     - Position at top of popup
     - Set **Font Size** to 24
   
   - Add Try Again Button:
     - Right-click on PopupPanel > UI > Button
     - Name it **TryAgainButton**
     - Set **Image** to **button.png** from Resources/UI/Menu/
     - Position in middle of popup
     - Add Text child: **"Try Again"**
   
   - Add Return Button (same as WinScreen)

### 3.5 Script Connections
1. Select **GridManager** GameObject:
   - Drag **Cube** prefab to **Cube Prefab** field
   - Drag **ObsacleBlock** prefab to **Obstacle Prefab** field
   - Drag **RocketBlock** prefab to **Rocket Prefab** field
   - Drag **LevelLoader** GameObject to **Level Loader** field

2. Select **GoalPanelManager** GameObject:
   - Drag **MovesText** to **Moves Text** field
   - Set up **Goal Items** array with Box, Stone, and Vase items

3. Select **GameResultManager** GameObject:
   - Drag **ResultCanvas** to **Result Canvas** field
   - Drag **WinScreen** to **Win Screen** field
   - Drag **LoseScreen** to **Lose Screen** field
   - Drag **ContinueButton** to **Continue Button** field
   - Drag **ReturnButton** to **Return Button** field
   - Drag **TryAgainButton** to **Try Again Button** field
   - Drag **LoseReturnButton** to **Lose Return Button** field
   - Drag **LevelCompleteText** to **Win Level Text** field
   - Drag **GameOverText** to **Game Over Text** field

## Step 4: Testing

### 4.1 Test MainScene
1. Open **MainScene**
2. Press **Play**
3. Verify:
   - Level button shows current level
   - Clicking level button loads LevelScene

### 4.2 Test LevelScene
1. Open **LevelScene**
2. Press **Play**
3. Verify:
   - Grid loads with cubes and obstacles
   - Top panel shows moves and goals
   - Clicking cubes blasts them
   - Win/lose screens appear correctly

### 4.3 Test Game Flow
1. Start from MainScene
2. Click Level button
3. Play the level
4. Test both win and lose scenarios
5. Verify buttons work correctly

## Step 5: Final Adjustments

### 5.1 Camera Positioning
The GridManager will automatically adjust the camera to fit the grid. You may need to fine-tune:
- **Camera Size** in GridManager script
- **Block Size** in GridManager script
- **Background Padding** in GridManager script

### 5.2 UI Scaling
If UI elements appear too small or large:
- Adjust **Reference Resolution** in Canvas Scaler
- Adjust **Match** value (0 = width, 1 = height, 0.5 = both)
- Adjust individual UI element sizes

### 5.3 Performance
For mobile optimization:
- Set **Quality Settings** to appropriate mobile level
- Enable **Static Batching** for UI elements
- Use **Sprite Atlases** for UI sprites

## Troubleshooting

### Common Issues:
1. **Scenes not loading**: Check Build Settings
2. **UI not scaling**: Check Canvas Scaler settings
3. **Camera not showing grid**: Check GridManager camera adjustment
4. **Buttons not working**: Check script connections in Inspector
5. **Prefabs missing**: Check Resources folder structure

### Debug Tips:
- Use **Console** window to check for errors
- Use **Scene** view to verify object positions
- Use **Game** view to test different screen sizes
- Use **Hierarchy** to verify object structure

## Notes
- The game supports portrait orientation (9:16)
- All 10 levels are included in Resources/Levels/
- The game uses Unity's built-in renderer (not URP)
- Follow OOP principles in code structure
- Use third-party tween libraries for animations if needed
- Don't use dependency injection libraries like Zenject 