# Kits
Rewrite of the popular Kits plugin for Rust.

### Contents
* [Commands](#commands)
* [Configuration](#configuration)
* [Installation](#installation)
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
* [Limit](#limit)
* [Remove](#remove)
* [Rename](#rename)
* [Update](#update)

### List
The list command allows players to view kits available to them.
#### Permissions
The kits which the user has permission for are displayed.
#### Syntax
`/kit`

---
### Redeem
The redeem command allows players to redeem a chosen kit, assuming they're eligible.
#### Arguments
| Argument Name | Description          |
| ------------- | -------------------- |
| `name`        | The name of the kit. |
#### Permissions
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
#### Permissions
The permission `kits.admin` is required.
#### Syntax
`/kit create <name>`
#### Example
`/kit create example`

---
### Cooldown
The cooldown command sets the minimum time between redemptions.
#### Arguments
| Argument Name | Description          |
| ------------- | -------------------- |
| `name`        | The name of the kit. |
| `time`        | The minimum time.    |
#### Permissions
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
| ------------- | --------------------     |
| `name`        | The name of the kit.     |
| `newName`     | The name of the new kit. |
#### Permissions
The permission `kits.admin` is required.
#### Syntax
`/kit duplicate <name> <newName>`
#### Example
`/kit duplicate example test`

---
### Limit
The limit command sets the maximum amount of redemptions, per player.
#### Arguments
| Argument Name | Description           |
| ------------- | --------------------  |
| `name`        | The name of the kit.  |
| `amount`      | The redemption limit. |
#### Permissions
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
#### Permissions
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
| ------------- | --------------------     |
| `name`        | The name of the kit.     |
| `newName`     | The new name of the kit. |
#### Permissions
The permission `kits.admin` is required.
#### Syntax
`/kit rename <name> <newName>`
#### Example
`/kit rename example test`

---
### Update
The update command sets the kit's items to those in your inventory.
#### Arguments
| Argument Name | Description          |
| ------------- | -------------------- |
| `name`        | The name of the kit. |
#### Permissions
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
### Example Configuration
```javascript
{
  "Default kits (lowest to highest priority)": [
    "default",
    "donator"
  ],
  "Wipe player data on new save (true/false)": true
}
```

## Installation
Head over to the [releases](https://github.com/jacobmstein/Kits/releases) and download the latest version, then simply follow [this guide](https://oxidemod.org/threads/installing-and-configuring-plugins-for-oxide.24298/).

## Permissions
The permission `kits.admin` is required to use most commands. For every kit that's created a corresponding permission is registered, following the pattern `kits.<name>`, where `name` is the name of the kit. A guide on using Oxide's permission system can be found [here](https://oxidemod.org/threads/using-the-oxide-permission-system.24291/).

## API
The following API method is currently available.
* [`IsKitRedeemable`](#iskitredeemable)

The following hook is currently called by Kits.
* [`CanRedeemKit`](#canredeemkit)

### `IsKitRedeemable`
`IsKitRedeemable` returns whether or not the player can use the kit, taking into account permissions, limits, and cooldowns.
#### Arguments
| Argument Name | Type     | Description            |
| ------------- | -------- | ---------------------- |
| `userId`      | `ulong`  | The player's Steam ID. |
| `name`        | `string` |The name of the kit.    |
#### Syntax
`IsKitRedeemable(ulong userId, string name)`
#### Example
```csharp
var canUse = Kits.Call<bool>("IsKitRedeemable", player.userID, "example");
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
Return a non-null value to override default behavior, specifically a string if you'd like to send a message to the player.
#### Examples
```csharp
private object CanRedeemKit(BasePlayer player) => blockAllKits 
    ? $"{Title} is preventing you from using that kit." 
    : null;
```
> Note, the overload with the `name` argument will override the one without, as it's more specific.

```csharp
private object CanRedeemKit(BasePlayer player, string name) => name == "blocked" 
    ? $"{Title} is preventing you from using that kit." 
    : null;
```

See [ApiExample.cs](Kits/ApiExample.cs) for more context.

## Contributions
Kits is free and open-source software, simply [open a pull request](https://github.com/jacobmstein/Kits/pulls).
