# Trebuchet MVP — Editor Setup Checklist

After Unity finishes recompiling all the new scripts, perform these one-time
steps in the Editor to bring the launch system to life.

## 1. Generate the input C# class
1. Select `Assets/InputSystem_Actions.inputactions`.
2. In the Inspector, tick **Generate C# Class**.
3. Class Name: `InputSystem_Actions` (default). Namespace: leave empty.
4. Click **Apply**.

This creates `InputSystem_Actions.cs` next to the asset and is what
`InputBridge.cs` references.

## 2. Cut the FBX animation clips
1. Select `Assets/_3Dmodels/Trebuchet/Trebuchet_LODG.fbx`.
2. **Rig** tab → Animation Type: **Generic** (already set), Avatar: **Create From This Model**.
3. **Animation** tab → in the Clips list, define at minimum:
   - `Trebuchet_Idle` — short loop (or 1 frame), Loop Time = ON.
   - `Trebuchet_Fire` — full release sequence, Loop Time = OFF.
4. For both clips: Root Transform Position (Y) = Bake Into Pose, Root Transform Rotation = Bake Into Pose.
5. Apply.

## 3. Add the `OnProjectileRelease` animation event
1. Open `Trebuchet_Fire` clip in the Animation window.
2. Scrub to the frame where the sling whips around and the stone should leave.
3. Click the **Add Event** button on the timeline.
4. Function: `OnProjectileRelease` (no arguments). Save.

## 4. Create ScriptableObject assets
1. Project window → right-click `Assets/ScriptableObjects` (create the folder).
2. **Create → Counterweight → Trebuchet Config** → name `TrebuchetConfig_Default`.
3. **Create → Counterweight → Projectile Config** → name `ProjectileConfig_Stone`. Leave `prefab` empty for now.

## 5. Build the projectile prefab
1. Create folder `Assets/Prefabs/Projectiles`.
2. GameObject → 3D Object → Sphere. Scale ~0.4.
3. Add components: `Rigidbody` (Use Gravity ON), `SphereCollider`, `Counterweight.Trebuchet.Projectile`.
4. Drag the GameObject into `Assets/Prefabs/Projectiles` to make a prefab. Name it `StoneProjectile`.
5. Delete the scene instance.
6. Open `ProjectileConfig_Stone` and drop the new prefab into its `Prefab` slot.

## 6. Build the Animator Controller
1. Create folder `Assets/Animation`.
2. **Create → Animator Controller** → name `TrebuchetAnimator`.
3. Open it. Add:
   - Parameter: Trigger named `Fire`.
   - State `Idle` (default) using clip `Trebuchet_Idle` (drag from FBX).
   - State `Firing` using clip `Trebuchet_Fire`.
   - Transition `Idle → Firing` with condition `Fire`, **Has Exit Time = OFF**, transition duration ~0.05.
   - Transition `Firing → Idle` with **Has Exit Time = ON**, exit time = 0.95.

## 7. Build the Trebuchet prefab
1. Drag `Trebuchet_LODG.fbx` from `Assets/_3Dmodels/Trebuchet` into a new empty scene.
2. Add components on the root:
   - `Animator` → assign Controller `TrebuchetAnimator`, Avatar from the FBX.
   - `Counterweight.Trebuchet.TrebuchetAnimationRelay`.
   - `Counterweight.Trebuchet.ProjectileSpawner` → assign `ProjectileConfig_Stone`.
   - `Counterweight.Trebuchet.TrebuchetController` → assign:
     - Config = `TrebuchetConfig_Default`
     - Animator = the Animator on this GameObject
     - Spawner = the ProjectileSpawner on this GameObject
     - Animation Relay = the TrebuchetAnimationRelay on this GameObject
3. Create an empty child named `ReleasePoint`. Position it where the sling tip is at the moment of release (scrub the Fire clip to find this frame, or just place it roughly above the long arm tip for now). Make sure its forward (blue Z arrow) points in the desired launch direction (usually the trebuchet's forward).
4. Drag `ReleasePoint` into the `Release Point` slot on `TrebuchetController`.
5. Drag the root into `Assets/Prefabs` → name `Trebuchet`. Delete the scene instance.

## 8. Build the FiringRange test scene
1. **File → New Scene** (Basic 3D, URP).
2. GameObject → 3D Object → Plane. Scale (10, 1, 10). Add a Material (any URP-Lit) so it's visible.
3. Drag `Trebuchet.prefab` into the scene at origin.
4. Move the Main Camera back and up; add component `Counterweight.Trebuchet.DebugFlyCamera`.
5. Create empty GameObject `Input`, add component `Counterweight.Input.InputBridge`.
6. Save scene as `Assets/Scenes/FiringRange.unity`.

## 9. Run the EditMode tests
1. **Window → General → Test Runner**.
2. Switch to **EditMode** tab.
3. Click **Run All**. The 6 `BallisticsSolverTests` should be green.

## 10. Smoke test
1. Open `FiringRange.unity`.
2. Press Play.
3. Trebuchet plays Idle.
4. Click left mouse button (the existing `Player/Attack` action).
5. Fire animation plays. At the release frame the stone spawns at `ReleasePoint`, flies forward in an arc, and hits the ground (debug log appears in Console).
6. Clicking again before the trebuchet returns to Idle does nothing — by design.

## Tuning later
Once it works end-to-end, iterate purely on the `TrebuchetConfig_Default`
asset values: bigger `counterweightMass` and `armLength` → farther shot;
`releaseAngleDeg` controls arc; `launchEfficiency` is the global feel knob.
