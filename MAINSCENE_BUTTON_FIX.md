# MainScene Button Fix Guide

## Problem: Level Button is not clickable in MainScene

## Quick Fix (5 minutes)

### Step 1: Add Debug Script
1. In MainScene, create an empty GameObject named **MainMenuDebugger**
2. Add the **MainMenuDebugger** script component
3. Press **Play** and check the **Console** window for error messages

### Step 2: Check Essential Components

#### 2.1 EventSystem (Most Common Issue)
1. In Hierarchy, look for **EventSystem** GameObject
2. If NOT found, create it:
   - Right-click in Hierarchy > **UI > Event System**
3. Make sure EventSystem is **enabled**

#### 2.2 Canvas Setup
1. Select your **Canvas** in Hierarchy
2. In Inspector, check these components:
   - ✅ **Canvas** component (enabled)
   - ✅ **Canvas Scaler** component
   - ✅ **Graphic Raycaster** component (enabled)
3. If any missing, add them

#### 2.3 Button Setup
1. Select your **LevelButton** in Hierarchy
2. In Inspector, check:
   - ✅ **Button** component (enabled, interactable = true)
   - ✅ **Image** component (enabled, raycast target = true)
   - ✅ **Rect Transform** (proper size and position)

### Step 3: Script Connections
1. Select **MainMenuController** GameObject
2. In Inspector, drag:
   - **LevelButton** to **Level Button** field
   - **LevelText** to **Level Button Text** field
3. Make sure both fields are NOT empty

### Step 4: Test
1. Press **Play**
2. Check Console for debug messages
3. Try clicking the button
4. If still not working, use **MainMenuDebugger** context menu options

## Detailed Troubleshooting

### Issue 1: "LevelManager.Instance is NULL"
**Solution:**
1. Create empty GameObject named **LevelManager**
2. Add **LevelManager** script component
3. Make sure it's **enabled**

### Issue 2: "No EventSystem found"
**Solution:**
1. Right-click in Hierarchy > **UI > Event System**
2. Make sure it's **enabled**

### Issue 3: "Button has no Image component"
**Solution:**
1. Select LevelButton
2. Add **Image** component if missing
3. Set **Raycast Target** to **true**
4. Assign a sprite (use button.png from Resources/UI/Menu/)

### Issue 4: "Canvas has no GraphicRaycaster"
**Solution:**
1. Select Canvas
2. Add **Graphic Raycaster** component
3. Make sure it's **enabled**

### Issue 5: "Button not interactable"
**Solution:**
1. Select LevelButton
2. In Button component, check:
   - **Interactable** = true
   - **Transition** = ColorTint (or any)
   - **Navigation** = Automatic

## Complete MainScene Setup Checklist

### Required GameObjects:
- [ ] **LevelManager** (with LevelManager script)
- [ ] **MainMenuController** (with MainMenuController script)
- [ ] **EventSystem** (UI > Event System)
- [ ] **Canvas** (with Canvas, CanvasScaler, GraphicRaycaster)
- [ ] **LevelButton** (with Button, Image components)
- [ ] **LevelText** (TextMeshPro component)

### Required Script Connections:
- [ ] MainMenuController.Level Button = LevelButton
- [ ] MainMenuController.Level Button Text = LevelText

### Required Component Settings:
- [ ] EventSystem enabled
- [ ] Canvas enabled
- [ ] CanvasScaler: Scale With Screen Size, 1080x1920, Match=0.5
- [ ] GraphicRaycaster enabled
- [ ] Button interactable = true
- [ ] Image raycast target = true

## Test Commands

### In Console (while playing):
```csharp
// Test button click
FindObjectOfType<MainMenuDebugger>().TestButtonClick();

// Force load level scene
FindObjectOfType<MainMenuDebugger>().ForceLoadLevelScene();

// Check build settings
FindObjectOfType<MainMenuDebugger>().CheckBuildSettings();
```

### Context Menu (right-click MainMenuDebugger):
- Test Button Click
- Force Load LevelScene
- Check Build Settings

## Common Mistakes to Avoid

1. **Missing EventSystem** - Most common cause
2. **Button not assigned in script** - Check Inspector connections
3. **Image raycast target disabled** - Button won't receive clicks
4. **Canvas not enabled** - UI won't work
5. **Wrong scene in build settings** - Scene won't load
6. **Button behind other UI elements** - Check hierarchy order

## Final Test
1. Press Play in MainScene
2. Console should show: "✅ LevelManager found!"
3. Console should show: "✅ MainMenuController found!"
4. Console should show: "✅ Level Button found!"
5. Console should show: "✅ EventSystem found!"
6. Click the button → Should load LevelScene

If any step shows ❌, fix that specific issue first. 