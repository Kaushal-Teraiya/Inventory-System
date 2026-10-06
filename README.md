# Inventory System

A scalable and extensible inventory system for Unity, built in C# with a focus on clean separation of responsibilities, predictable inventory operations, validation, testability, and persistence.

## Core Architecture

```text
ItemDefinition
      │
      ▼
ItemInstance
      │
      ▼
ItemStack
      │
      ▼
InventorySlot
      │
      ▼
_Inventory
```

### ItemDefinition

Defines the static properties of an item:

- Item ID
- Display name
- Icon
- Maximum stack size
- Weight

Implemented as a Unity `ScriptableObject`.

### ItemInstance

Represents a runtime instance of an item and references its `ItemDefinition`.

### ItemStack

Represents an item together with its quantity.

```text
ItemStack
├── ItemInstance
└── Quantity
```

The stack enforces valid quantities and provides controlled quantity modification.

### InventorySlot

Represents a single inventory position.

```text
InventorySlot
├── Index
└── ItemStack
```

A slot can either contain an `ItemStack` or be empty.

### _Inventory

The main inventory domain class.

Responsibilities include:

- Managing inventory slots
- Adding items
- Removing items
- Moving items
- Merging stacks
- Swapping items
- Splitting stacks
- Removing items from specific slots
- Clearing the inventory
- Tracking capacity
- Tracking maximum weight
- Tracking current weight
- Transferring items between inventories

The inventory is implemented as a plain C# class rather than a `MonoBehaviour`, keeping the core system independent from Unity scene behaviour.

## Inventory Rules

`InventoryRules` contains reusable inventory rules such as:

- Determining whether two items can stack together
- Calculating remaining capacity in an existing stack

This keeps inventory rules separate from the inventory implementation.

## Operation Results

Inventory operations return an `InventoryOperationResult` rather than relying on exceptions for normal gameplay failures.

```text
InventoryOperationResult
├── Succeeded
├── Failure
└── AffectedQuantity
```

Supported failure states include:

- `None`
- `InvalidItem`
- `InventoryFull`
- `InvalidSlot`
- `InvalidQuantity`
- `OperationNotAllowed`

This allows systems using the inventory to determine exactly why an operation failed.

## Inventory Operations

### Add Items

Items are added to compatible existing stacks first, then placed into empty slots when required.

Stack limits are respected.

### Remove Items

Items can be removed by item instance or directly from a specific inventory slot.

### Move Items

Moving supports:

- Moving into an empty slot
- Merging compatible stacks
- Swapping incompatible occupied slots

### Split Stacks

A quantity can be moved from one stack into an empty target slot while preserving both stacks.

### Transfer

Items can be transferred between inventories through the transaction layer.

## Transactions

`InventoryTransaction` provides a higher-level boundary for multi-inventory operations.

This separates transactional behaviour from the basic slot and stack operations.

## Persistence

The system includes:

- `InventorySerializer`
- `InventorySaveData`

Serialization is kept separate from the inventory domain so persistence concerns do not become part of the core inventory implementation.

## Events

The inventory provides change notifications:

```csharp
SlotChanged
InventoryChanged
```

These allow external systems to react to inventory changes without continuously polling the inventory.

## Validation

The system validates:

- Invalid items
- Invalid quantities
- Invalid slot indices
- Stack compatibility
- Maximum stack sizes
- Inventory capacity
- Maximum inventory weight
- Invalid inventory operations

## Testing

The inventory system contains a comprehensive NUnit test suite covering:

- Inventory construction
- Slot creation
- Adding items
- Removing items
- Stack behaviour
- Splitting
- Moving
- Merging
- Swapping
- Capacity limits
- Weight limits
- Item validation
- Inventory events
- Serialization
- Snapshots
- Transactions
- Failure scenarios

Current test suite:

**102 test cases across 15 test files.**

## Design Goals

The inventory system is designed so additional gameplay systems can build on top of it without modifying the core inventory responsibilities.

Possible extensions include:

- Equipment systems
- Loadouts
- Hotbars
- Loot containers
- Crafting
- Trading
- Consumables
- Item durability
- Item rarity
- Equipment slots
