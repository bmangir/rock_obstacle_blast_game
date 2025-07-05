# 🎮 Rock The Obstacles

**Dream Games Case Study** - Unity 2D Puzzle Game

<div align="center">

![Unity Version](https://img.shields.io/badge/Unity-6000.0.32f1-black.svg?style=flat&logo=unity)
![Platform](https://img.shields.io/badge/Platform-macOS%20%7C%20Windows%20%7C%20Mobile-blue.svg)
![Language](https://img.shields.io/badge/Language-C%23-purple.svg)
![Game Type](https://img.shields.io/badge/Genre-Puzzle%20%7C%20Match--3%20Style-orange.svg)

*Blast connected cubes, destroy obstacles, and clear your way to victory!*

</div>

## 🚀 Game Overview

**Rock The Obstacles** is an engaging 2D puzzle game that combines match-3 mechanics with strategic obstacle destruction. Players must clear various obstacles from the grid by strategically blasting connected colored cubes and utilizing powerful rocket mechanics.

### 🎯 Core Objective
Clear all obstacles (Boxes, Stones, Vases) from the grid within the given number of moves to complete each level.

## ✨ Key Features

### 🎮 **Gameplay Mechanics**
- **Cube Blasting**: Tap groups of 2+ connected same-colored cubes to blast them
- **Rocket Creation**: Create powerful rockets by blasting 4+ connected cubes
- **Obstacle Destruction**: Different obstacles require different strategies to destroy
- **Physics System**: Gravity-based block dropping for dynamic gameplay
- **10 Challenging Levels**: Progressive difficulty with unique layouts

### 🧱 **Block Types**

| Type | Description | Properties |
|------|-------------|------------|
| **🟦 Colored Cubes** | Main gameplay elements (Red, Blue, Green, Yellow) | Can be blasted in groups of 2+ |
| **📦 Box** | Basic obstacle | 1 hit to destroy|
| **🗿 Stone** | Sturdy obstacle | Immune to cube blasts, requires rockets |
| **🏺 Vase** | Fragile obstacle | 2 hits to destroy, can fall with gravity |
| **🚀 Rockets** | Power-ups | Blast entire rows/columns |

### 🚀 **Rocket System**
- **Horizontal Rockets**: Clear entire rows
- **Vertical Rockets**: Clear entire columns
- **Auto-Creation**: Generated when blasting 4+ connected cubes
- **Chain Reactions**: Rockets can trigger other rockets for massive combos

### 🎨 **Visual Effects**
- **Particle Systems**: Explosive destruction effects for each obstacle type
- **Camera Shake**: Dynamic feedback for rocket blasts
- **Smooth Animations**: Falling blocks and blast sequences
- **UI Polish**: Clean, modern interface with goal tracking

### ⭐ **Scoring & Star System**
- **Star Rating**: Earn 1-3 stars based on performance efficiency
- **Score Calculation**: Points awarded for obstacle destruction and move efficiency
- **Progress Tracking**: Star collection tracked across all levels
- **Performance Metrics**: Optimal move count determines star rating

## 🎮 How to Play

1. **📱 Tap Connected Cubes**: Find groups of 2+ same-colored cubes and tap to blast them
2. **🎯 Target Obstacles**: Use cube blasts to destroy Boxes and damage Vases
3. **🚀 Use Rockets Strategically**: Tap rockets to clear entire rows/columns and destroy Stones
4. **⏱️ Manage Moves**: Complete objectives within the move limit
5. **🏆 Win Condition**: Clear ALL obstacles to advance to the next level

### 💡 Pro Tips
- Plan your moves carefully - you have limited attempts!
- Create rockets by blasting 4+ cubes for maximum impact
- Stones can ONLY be destroyed by rockets
- Vases will fall with gravity, use this to your advantage
- Chain rocket explosions for spectacular combos

## 🛠️ Technical Features

### 🏗️ **Architecture**
- **Clean Code Structure**: Modular C# scripts with clear separation of concerns
- **Manager Pattern**: Dedicated managers for Grid, Level, Goals, and Effects
- **Event-Driven**: Smooth communication between game systems
- **Scalable Design**: Easy to add new levels, obstacles, and mechanics

### 🎨 **Graphics & Animation**
- **2D Sprite-Based**: High-quality pixel art graphics
- **Universal Render Pipeline (URP)**: Optimized rendering for all platforms
- **Particle Effects**: Custom explosion and destruction effects
- **Responsive UI**: Adapts to different screen sizes and resolutions

### 🔧 **Systems**
- **Level System**: JSON-based level configuration for easy editing
- **Save System**: Automatic progress saving using PlayerPrefs
- **Input Handling**: Mouse/touch input with precise hit detection
- **Audio Ready**: Structured for easy sound integration

## 📁 Project Structure

```
Assets/
├── 🎮 Scripts/
│   ├── Blocks/          # Cube, Obstacle, and Rocket logic
│   ├── Managers/        # Core game systems
│   ├── Data/            # Level data structures
│   └── Factories/       # Debugging utilities
├── 🎨 Resources/
│   ├── Cubes/           # Colored cube sprites & particles
│   ├── Obstacles/       # Box, Stone, Vase graphics
│   ├── Rocket/          # Rocket sprites & effects
│   ├── Levels/          # JSON level configurations
│   └── UI/              # Interface graphics
├── 🏗️ Prefabs/          # Reusable game objects
├── 🌅 Scenes/           # Main menu and game scenes
└── ⚙️ Settings/         # Unity project configurations
```

## 🚀 Getting Started

### 📋 Prerequisites
- **Unity Hub** (Latest version)
- **Unity 6000.0.32f1** or compatible version
- **Git** for cloning the repository

### 💾 Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/bmangir/DREAM_GAMES_CASE_STUDY.git
   ```

2. **Open with Unity**
   - Launch Unity Hub
   - Click "Open Project"
   - Select the cloned folder
   - Wait for Unity to import assets (1-2 minutes on first open)

3. **Play the Game**
   - Open `MainScene` to start from the menu
   - Press Play ▶️ in Unity Editor

### 🏗️ Building the Game

1. **Configure Build Settings**
   - Go to `File > Build Settings`
   - Add scenes: `MainScene` and `LevelScene`
   - Select your target platform

2. **Build**
   - Click "Build" and choose output folder
   - Unity will generate executable files

## 🎯 Game Design Highlights

### 🧩 **Puzzle Design Philosophy**
- **Progressive Complexity**: Each level introduces new challenges gradually
- **Multiple Solutions**: Various strategies possible for each level
- **Risk vs Reward**: Players must balance powerful rockets vs precise cube blasts
- **Satisfying Feedback**: Visual and tactile rewards for successful moves

### 🎨 **Visual Design**
- **Clean Aesthetic**: Minimalist design focusing on gameplay clarity
- **Color Psychology**: Intuitive color coding for different game elements
- **Particle Polish**: Satisfying destruction effects enhance player satisfaction
- **Responsive Layout**: UI adapts seamlessly to different screen sizes

## 🛡️ Code Quality Features

- **🧹 Clean Architecture**: SOLID principles and modular design
- **📝 Comprehensive Comments**: Well-documented code for easy understanding
- **🔧 Easy Configuration**: JSON-based level editing without code changes
- **🐛 Error Handling**: Robust error checking and user-friendly debugging
- **⚡ Performance Optimized**: Efficient algorithms and memory management

### 🔧 **Code Quality Deficiencies**

#### 🏗️ **OOP Structure Issues**
- [ ] **Missing Base Block Class**: No inheritance hierarchy for CubeBlock, ObstacleBlock, and RocketBlock
- [ ] **Interface Segregation**: Missing interfaces like IDestroyable, IFallable, IBlastable for better abstraction
- [ ] **Factory Pattern**: "Factories" folder contains debug classes instead of actual factory implementations
- [ ] **Tight Coupling**: Direct FindObjectOfType calls create dependencies instead of using dependency injection

#### 📏 **Single Responsibility Violations**
- [ ] **GridManager Monolith**: 2020+ lines handling grid logic, animations, physics, input, and effects
- [ ] **Manager Splitting**: GridManager should be split into GridAnimationManager, GridPhysicsManager, GridInputHandler

#### 🔗 **Architecture Improvements**
- [ ] **Event System**: Replace direct manager references with event-driven communication
- [ ] **Dependency Injection**: Implement proper dependency injection instead of FindObjectOfType patterns
- [ ] **Component Composition**: Break down large monolithic classes into smaller, focused components

## 🤝 Contributing

This project was developed as a case study for Dream Games. The codebase is designed to be:
- **Readable**: Clear naming conventions and documentation
- **Extensible**: Easy to add new features and content
- **Maintainable**: Modular structure for easy updates

## 🎮 Development Notes

### 🔍 **Key Technical Decisions**
- **Grid-Based System**: Enables precise positioning and collision detection
- **Coroutine-Based Animations**: Smooth, non-blocking visual effects
- **Component Architecture**: Reusable, testable game object behaviors
- **Manager Singleton Pattern**: Centralized system management

### 🎯 **Future Enhancement Ideas**
- 🎵 Sound effects and background music
- 🏆 Achievement system
- 💫 More particle effects and animations
- 📱 Mobile-specific touch controls
- 🌟 Power-up system expansion

## 📝 Deficiencies

### 🎨 **UI Improvements**
- [ ] **Dynamic Goal Panel Scaling**: Implement dynamic sizing for obstacle icons in the goal panel based on obstacle types and counts
- [ ] **Grid Background Alignment**: Fix grid background positioning to perfectly align with the game grid

### 🎮 **Gameplay Enhancements**
- [ ] **Stone Physics Fix**: Implement proper cube dropping behavior when stones don't fall - existing cubes above should drop down instead of spawning new cubes below

### 🎊 **Easter Eggs**
- **Dream Games Logo**: Create "DG" (Dream Games) letters using box obstacles in the final level as a special Easter egg :)

---

<div align="center">

**🎮 Ready to Rock The Obstacles? Clone and Play Now! 🚀**

*Developed with ❤️ using Unity 6*

</div>
