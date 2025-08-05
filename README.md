# Aerial Ace: Combat Fighter 🚀

## Dominate the Skies, Become the Aerial Ace\!

Aerial Ace: Combat Fighter is a fast-paced 2D vertically scrolling "Shoot 'em up" game where you pilot a fighter jet on a critical mission to save the world from an unknown threat. Hone your reflexes, master aerial combat, and experience high-flying mayhem in this action-packed arcade-inspired shooter\!

-----

## 🎮 Gameplay

Navigate your fighter jet through intense dogfights against waves of enemy aircraft. Dodge incoming fire, strategically collect power-ups, and unleash a barrage of bullets to clear the skies. The game features progressive difficulty, starting with training stages and escalating to challenging encounters requiring quick reflexes and optimal execution. Prepare for climactic boss battles that test your true piloting skills\!

-----

## ✨ Features

  * **Dynamic Difficulty Progression:** Levels scale in intensity, from initial training to high-octane combat.
  * **Intense Boss Battles:** Confront powerful bosses with unique attack patterns, multiple phases, and destructible gun turrets with individual health pools.
  * **Diverse Enemy Types:** Engage with various enemy aircraft, each with distinct movement and firing behaviors.
  * **Power-Up System:** Collect a variety of power-ups dropped by defeated enemies to gain temporary advantages:
      * ⚡ **Fire Rate Boost:** Increase your firing speed.
      * ↔️ **Bullet Spread:** Widen your shot pattern for broader coverage.
      * ❤️ **Extra Life:** Gain an additional life to keep fighting.
      * 💰 **Extra Score:** Boost your points for quick progression.
      * 🛡️ **Shield:** Absorb incoming damage, with visual shields stacking on your aircraft.
      * 💨 **Speed Boost:** Temporarily increase your movement speed.
      * 💣 **Bomb:** Clear all enemies currently on screen.
      * 🔫 **Extra Gun:** Equip additional firing turrets for massive firepower.
      * *Fire Rate increases automatically if both Bullet Spread and Extra Gun are active.*
  * **Health System:** Player has 1 hitpoint, protected by stackable shields. Losing health/shields triggers a brief immunity period.
  * **Score & High Score Tracking:** Track your current score and compete for the highest score, saved locally using PlayerPrefs.
  * **Dynamic Audio:** Seamless crossfading between intense boss music and regular background music based on boss presence.
  * **Pause Menu:** Press `Esc` to pause the game, allowing players to take a break and adjust settings.
  * **Adaptive Controls:** Automatically switches between keyboard/mouse controls (for PC) and Touch Controls Kit (TCK) for mobile WebGL builds.
  * **Explosive Visuals:** Satisfying explosion effects for defeated enemies, guns, and bosses.
  * **Konami Code Easter Egg:** Enter the classic Konami Code on the main menu for a special bonus in the next game: 100 lives, all power-ups, and 3 shields\!
  * **Secret Level/Video Playback:** A hidden level accessible via the Konami Code, featuring MP4 video playback.

-----

## 🚀 How to Play

  * **Movement:** Use **Arrow Keys** or **WASD** (Keyboard) / **Virtual Joystick** (Touch) to maneuver your aircraft.
  * **Firing:** Hold **Spacebar** (Keyboard) / **Fire Button** (Touch) for continuous fire, or tap rapidly for faster shots.
  * **Pause:** Press **Esc** (Keyboard) to pause/resume the game.
  * **Quit (WebGL):** For WebGL builds, a prompt will guide you to use the browser's close button.

-----

## 🛠️ Installation

To get a local copy up and running, follow these simple steps:

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/tempest918/AerialAce.git
    ```
2.  **Open in Unity:** Open the cloned project in Unity Editor (tested with Unity 2022.3.x LTS).
3.  **Build Settings:** Ensure your scenes are added to `File > Build Settings` in the correct order (e.g., MainMenu, GameScene, SecretLevel).
4.  **StreamingAssets:** Verify `Assets/StreamingAssets/EasterEgg.mp4` exists for the secret level video.

-----

## 🤝 Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1.  Fork the Project
2.  Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3.  Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push to the Branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request
