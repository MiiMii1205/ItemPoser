# ItemPoser

A tool for posing items to screenshot clean GUI item icons using PEAK’s in-game rendering.

## Overview ##

ItemPoser is designed for custom item developers who want consistent, high-quality GUI item icons without recreating the game’s
rendering setup externally.

## Requirements ##

To use ItemPoser, you will need:

- A console (like [AdvancedConsole](https://thunderstore.io/c/peak/p/keklick1337/AdvancedConsole/) for example),
- A way to get Item IDs
- [Unity Explorer](https://thunderstore.io/c/peak/p/PEAK_Modding/UnityExplorer/)
- [PeakCinema](https://thunderstore.io/c/peak/p/megalon2d/PeakCinema/) (optionnal, to hide the HUD more easily)

## Getting Items IDs ##

Before posing an item, you must retrieve its Item ID.

You can do this by using [Unity Explorer](https://thunderstore.io/c/peak/p/PEAK_Modding/UnityExplorer/) to inspect your item's `ItemComponent`. 

Copy or note it down for later.

## Posing an item ##

Before running the command, you should:

1. Retrived the item's ID
2. (Optional) Enable the cinema camera in [PeakCinema](https://thunderstore.io/c/peak/p/megalon2d/PeakCinema/)
3. Enable the freecam in [Unity Explorer](https://thunderstore.io/c/peak/p/PEAK_Modding/UnityExplorer/)

### Command ###

Once ready, open your console and run:
```
ItemPoser.PoseItem <itemID>
```

This will snap and pose the item using it's UI Data, and locks the freecam to it.

You can then easily take a screenshot and import it to your editor of choice.

## Making Your Items Pose Correctly ##

If your custom item does not pose well, you'll need to configure these UI Data fields in your custom item's prefab:
- `Icon Position Offset` 
- `Icon Rotation Offset` 
- `Icon Scale Offset` 

These control how the item appears when rendered as an icon.

There are two methods for configuring icon offsets.

### The Easy way ###

For quick results, you can copy the values of a vanilla item with an Item Icon you like.

It usually give great looking icons, especially if both items have the same girth.

### The Custom Way ###

For better control, you can configure the offsets manually in the Unity Editor.

#### Step 1: Prepare the hierarchy ####

1. Drag'n'drop your custom item Prefab from the Project Window into your Scene View (doesn't matter which scene is loaded)
2. Enter the "2D view" mode by clicking the "2D" icon in your scene view's toolbar
3. Move your item so that it's fully visible
1. Select your item
1. Create an empty parent (<kbd>Ctrl+Shift+G</kbd>, or right-click it in the Hierarchy Window -> `Create Empty Parent`)

<p>
<mark>
❌ <b>Important</b>: Do NOT change the parent object's transform. Only change the original item's transform (the game object holding the <code>ItemComponent</code>)
</mark>
</p>

You can then hide/disable the player hand models and/or anything that's in the way.

#### Step 2: Pose the item ####

Adjust the position, rotation, and scale of your item visually and manually.

Once you're satisfied, copy/paste the transform values into the item's UI Data:
1. Copy the `Position` ( right-clicking `Position` -> `Copy`) to `Icon Position Offset`
2. Copy the `Rotation` ( right-clicking `Rotation` -> `Copy Euler Angle`) to `Icon Rotation Offset`
3. Copy the `Scale` of X to `Icon Scale Offset`

Then finaly in the Inspector, right-click on `UI Data` and select `Apply to Prefab` to save the offset to your prefab.

Rebuild your bundle and your item should now pose correctly.

## Configuration ##

To help you get a good and easy to work with shot, the mod comes with a couple of configs options:

- `Red Value`, `Green Value`, `Blue Value`: The RGB values of the item's backdrop (ranges from 0-255). Leave as default for a perfect green.
- `Setup Rotation`: The Y angle (yaw) of the whole pose setup. Use this to tweak the lighting.

You can change these configs in the mod's config file.

You'll need to run the mod first for it to generate or use a mod like [ModConfig](https://thunderstore.io/c/peak/p/PEAKModding/ModConfig/) to change them in game. You can use your mod manager too!

## Issues ##

If you find any bugs with the mod or have any suggestion, please add a ticket to the git repo. We'll take a look and
respond as soon as possible!