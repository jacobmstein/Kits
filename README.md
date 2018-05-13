# Kits
Rewrite of the popular Kits plugin for Rust.

### Contents
* [Commands](#commands)
* [Configuration](#configuration)
* [Installation](#installation)
* [Migrating](#migrating)
* [Permissions](#permissions)
* [API](#api)
* [Contributions](#contributions)

## Commands
The following commands are currently available.
* [List](#list)
* [Redeem](#redeem)
* [Create](#create)
* [Cooldown](#cooldown)
* [Duplicate](#duplicate)
* [Give](#give)
* [Limit](#limit)
* [Remove](#remove)
* [Rename](#rename)
* [Update](#update)

### List
The list command allows players to view kits available to them.
#### Required Permissions
The kits which the user has permission for are displayed.
#### Syntax
`/kit`

---
### Redeem
The redeem command allows players to redeem a kit, if they're eligible.
#### Arguments
| Argument Name | Description          |
| ------------- | -------------------- |
| `name`        | The name of the kit. |
#### Required Permissions
The permission for the kit is required.
#### Syntax
`/kit <name>`
#### Example
`/kit example`

---
### Create
The create command creates a new kit based off the items in your inventory.
#### Arguments
> Note, the `name` may only contain letters.

| Argument Name | Description           |
| ------------- | --------------------- |
| `name`        | The name for the kit. |
#### Required Permissions
The permission `kits.admin` is required.
#### Syntax
`/kit create <name>`
#### Example
`/kit create example`

---
### Cooldown
The cooldown command sets the minimum time between redemptions for a kit.
#### Arguments
| Argument Name | Description          |
| ------------- | -------------------- |
| `name`        | The name of the kit. |
| `time`        | The minimum time.    |
#### Required Permissions
The permission `kits.admin` is required.
#### Syntax
`/kit cooldown <name> <time>`
#### Example
> Note, the `time` argument is parsed in a versatile way, if it's human readable it should be fine.

`/kit cooldown example 3 days 1 hour 4 minutes 1 second`

---
### Duplicate
The duplicate command duplicates a kit.
#### Arguments
| Argument Name | Description              |
| ------------- | ------------------------ |
| `name`        | The name of the kit.     |
| `newName`     | The name of the new kit. |
#### Required Permissions
The permission `kits.admin` is required.
#### Syntax
`/kit duplicate <name> <newName>`
#### Example
`/kit duplicate example test`

---
### Give
The give command gives a kit to the `player` or all players if no `player` argument is provided.
| Argument Name | Description          | Required |
| ------------- | -------------------- | -------- |
| `name`        | The name of the kit. | True     |
| `player`      | The player.          | False    |
#### Required Permissions
The permission `kits.admin` is required.
#### Syntax
`/kit give <name> <player>`
#### Example
`/kit give example Jacob`

`/kit give example`

---
### Limit
The limit command sets the maximum amount of redemptions, per player.
#### Arguments
| Argument Name | Description           |
| ------------- | --------------------- |
| `name`        | The name of the kit.  |
| `amount`      | The redemption limit. |
#### Required Permissions
The permission `kits.admin` is required.
#### Syntax
`/kit limit <name> <amount>`
#### Example
`/kit limit example 3`

---
### Remove
The remove command removes a kit.
#### Arguments
| Argument Name | Description          |
| ------------- | -------------------- |
| `name`        | The name of the kit. |
#### Required Permissions
The permission `kits.admin` is required.
#### Syntax
`/kit remove <name>`
#### Example
`/kit remove example`

---
### Rename
The rename command renames a kit.
#### Arguments
| Argument Name | Description              |
| ------------- | ------------------------ |
| `name`        | The name of the kit.     |
| `newName`     | The new name of the kit. |
#### Required Permissions
The permission `kits.admin` is required.
#### Syntax
`/kit rename <name> <newName>`
#### Example
`/kit rename example test`

---
### Update
The update command sets a kit's items to those in your inventory.
#### Arguments
| Argument Name | Description          |
| ------------- | -------------------- |
| `name`        | The name of the kit. |
#### Required Permissions
The permission `kits.admin` is required.
#### Syntax
`/kit update <name>`
#### Example
`/kit update example`

## Configuration
### Default Configuration
```javascript
{
  "Default kits (lowest to highest priority)": [],
  "Wipe player data on new save (true/false)": true
}
```
#### Default kits
Simply enter the names of kits to spawn with. When a player respawns, the last one they have permission to use will be given.

## Installation
Head over to the [releases](https://github.com/jacobmstein/Kits/releases) and download the latest version, then simply follow [this guide](https://oxidemod.org/threads/installing-and-configuring-plugins-for-oxide.24298/).

## Migrating
If found in the data directory, data from the previous version of Kits is automatically migrated to the new schema. It's important to note that [permissions](#permissions) and [default kits](#configuration) have been updated. To ensure a good user experience for players it's suggested you read the corresponding sections after updating and act accordingly.

## Permissions
The permission `kits.admin` is required to use most commands. For every kit that's created a corresponding permission is registered, following the pattern `kits.<name>`, where `name` is the name of the kit. A guide on using Oxide's permission system can be found [here](https://oxidemod.org/threads/using-the-oxide-permission-system.24291/).

## API
The following API methods are currently available.
* [`GiveKit`](#givekit)
* [`IsKit`](#iskit)
* [`IsKitRedeemable`](#iskitredeemable)

The following hooks are currently called by Kits.
* [`CanGiveDefaultKit`](#cangivedefaultkit)
* [`CanRedeemKit`](#canredeemkit)

### `GiveKit`
`GiveKit` gives a kit to the `player`.
#### Arguments
| Argument Name | Type         | Description          |
| ------------- | ------------ | -------------------- |
| `player`      | `BasePlayer` | The player.          |
| `name`        | `string`     | The name of the kit. |
#### Syntax
`GiveKit(BasePlayer player, string name)`
#### Example
```csharp
Kits.Call("GiveKit", player, "example");
```

---
### `IsKit`
`IsKit` returns whether a kit exists by the `name`.
#### Arguments
| Argument Name | Type     | Description            |
| ------------- | -------- | ---------------------- |
| `name`        | `string` | The name of the kit.   |
#### Syntax
`IsKit(string name)`
#### Example
```csharp
var isKit = Kits.Call<bool>("IsKit", "example");
```

---
### `IsKitRedeemable`
`IsKitRedeemable` returns whether the `player` can use the kit, taking into account permissions, limits, and cooldowns.
#### Arguments
| Argument Name | Type         | Description          |
| ------------- | ------------ | -------------------- |
| `player`      | `BasePlayer` | The player.          |
| `name`        | `string`     | The name of the kit. |
#### Syntax
`IsKitRedeemable(BasePlayer player, string name)`
#### Example
```csharp
var isRedeemable = Kits.Call<bool>("IsKitRedeemable", player, "example");
```

### `CanGiveDefaultKit`
`CanGiveDefaultKit` allows other plugins to intercept Kits giving [default kits](#default-kits).
#### Parameters
| Parameter Name | Type         | Description          | Required |
| -------------- | ------------ | -------------------- | -------- |
| `player`       | `BasePlayer` | The player.          | True     | 
| `name`         | `string`     | The name of the kit. | False    |
#### Return Behavior
Return a non-null value to override default behavior.
#### Examples
```csharp
private object CanGiveDefaultKit(BasePlayer player) => blockAllKits 
    ? false
    : (object)null;
```
> Note, the overload with the `name` parameter will override the one without, as it's more specific.

```csharp
private object CanGiveDefaultKit(BasePlayer player, string name) => name == "blocked" 
    ? false
    : (object)null;
```

---
### `CanRedeemKit`
`CanRedeemKit` allows other plugins to intercept kit redemption.
#### Parameters
| Parameter Name | Type         | Description          | Required |
| -------------- | ------------ | -------------------- | -------- |
| `player`       | `BasePlayer` | The player.          | True     | 
| `name`         | `string`     | The name of the kit. | False    |
#### Return Behavior
Return a non-null value to override default behavior, specifically a string if you'd like to send a message to the `player`.
#### Examples
```csharp
private object CanRedeemKit(BasePlayer player) => blockAllKits
    ? $"{Title} is preventing you from using that kit."
    : null;
```
> Note, the overload with the `name` parameter will override the one without, as it's more specific.

```csharp
private object CanRedeemKit(BasePlayer player, string name) => name == "blocked" 
    ? $"{Title} is preventing you from using that kit."
    : null;
```

See [ApiExample.cs](Kits/ApiExample.cs) for more context.

## Contributions
Kits is free and open-source software, simply [open a pull request](https://github.com/jacobmstein/Kits/pulls).
