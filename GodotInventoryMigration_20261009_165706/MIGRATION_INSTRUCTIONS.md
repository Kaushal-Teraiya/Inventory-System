# Godot to Unity Inventory System Migration

Godot source: C:\Users\kaushal\GodotProjects\Inventory-System
Unity destination: C:\Users\kaushal\Inventory System
Collected source: C:\Users\kaushal\Inventory System\GodotInventoryMigration_20261009_165706\GodotSource
Unity backup: C:\Users\kaushal\Inventory System\GodotInventoryMigration_20261009_165706\UnityBackup

Translate the COMPLETE collected inventory system, not only its UI.

Include:
- Inventory domain, slots, stacks, capacity and weight limits
- Item definitions, instances, IDs and item effects
- Add, remove, move, split, stack and lock operations
- Inventory rules, transactions, snapshots and serialization
- Inventory events and state refresh
- Item use, player health and healing/damage effects
- Slot selection, hover, drag/drop and split interactions
- Context menus, item details, capacity labels and health HUD
- Starting items and UI setup
- Relevant scenes, resources, input actions and dependencies

Requirements:
1. Preserve existing domain behavior and public API intent.
2. Translate Godot Node, Control, signals, resources, input and scene
   concepts to appropriate Unity equivalents.
3. Use Unity C#, MonoBehaviour/ScriptableObject where appropriate,
   Unity UI, EventSystem and Input System.
4. Reuse existing Unity domain classes when equivalent functionality
   already exists; do not create duplicate competing implementations.
5. Do not invent missing Godot behavior. Flag unresolved dependencies.
6. Do not overwrite the existing Unity implementation until the translated
   files have been reviewed and compile-checked.
7. Provide a source-to-destination mapping and migration report.
8. Include tests for behavior translated from existing Godot tests.

The collected source is input material, not proof that translation is complete.
