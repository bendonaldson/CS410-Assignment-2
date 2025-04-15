# Enhanced John Lemon's Haunted Jaunt - Project README

## Overview

This project is an extended version of the Unity Learn Beginner Tutorial "John Lemon's Haunted Jaunt". Building upon the original stealth gameplay, this version incorporates several new features demonstrating key game programming concepts:

* **Ghost Rear Detection** using Dot Product.
* **Ghost Fading Effect** using Linear Interpolation (Lerp).
* **Ghost Trail** using Particle Effects.
* **Alert Sound** using AudioSource.

These additions enhance the gameplay challenge and visual feedback.

## Added Features Details

Here's how each new feature works and how it was implemented:

### 1. Ghost Rear Detection (Dot Product)

* **Purpose:** Adds a new way for the player to be caught by approaching a ghost directly from behind, increasing the stealth challenge.
* **Technical Concept:** This feature utilizes the **Dot Product**. In the `WaypointPatrol.cs` script attached to each ghost, we calculate `Vector3.Dot(transform.forward, directionToPlayer.normalized)`. This compares the direction the ghost is facing (`transform.forward`) with the direction towards the player. A value close to -1 means the player is directly behind the ghost.

### 2. Ghost Fading Effect (Linear Interpolation - Lerp)

* **Purpose:** Makes ghosts visually fade in and out while patrolling, making them potentially harder to track constantly.
* **Technical Concept:** This uses **Linear Interpolation** (`Mathf.Lerp`). The `WaypointPatrol.cs` script modifies the alpha (transparency) value of the ghost material's `color` property over time.

### 3. Ghost Trail (Particle Effect)

* **Purpose:** Adds a visual effect to help the player see where ghosts have recently moved.
* **Technical Concept:** This uses a standard Unity `Particle System`. A GameObject containing the configured Particle System component is added as a **child** of the main Ghost prefab.

### 4. Rear Detection Alert (Sound Effect)

* **Purpose:** Provides clear audio feedback to the player when they have been detected from behind by a ghost.
* **Technical Concept:** An `AudioSource` component is attached to the Ghost prefab. The `WaypointPatrol.cs` script has a public variable `rearDetectAudio` which holds a reference to this AudioSource and plays when the player is detected.
