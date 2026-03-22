# ItemPoser

A tool for posing and taking screenshot of items in their UI icon pose

## What does this do? ##

This mod is a handy little tool to help modders generate UI icons using the in-game rendering pipeline.

To use this tool, you'll first need to get a text console (like [AdvancedConsole](https://thunderstore.io/c/peak/p/keklick1337/AdvancedConsole/) for example)

You'll also need to find the `Item ID` of the item you want to pose.

If you're using `PEAKLib Items` to manage your custom item, the lib will automatically provide a free and valid ID, so you'll need to inspect your item in-game (using [Unity Explorer](https://thunderstore.io/c/peak/p/PEAK_Modding/UnityExplorer/)) to get their ID.

Otherwise, every vanilla item should have static IDs. If you ever used an item spawner, the items are order based on their item ID. You can also just decide to check their vanila item IDs too.

## How do I pose an item ##

As said before, you'll need an OPENABLE and INTERACTIVE console. But before entering the command, it's wise to do a couple of things...

1. Inspect your item and copy/note down their `Item ID`.
2. Using Unity Explorer, enable the freecam.

As a side note, I also recommand using [PeakCinema](https://thunderstore.io/c/peak/p/megalon2d/PeakCinema/) for
   getting rid of the GUI. (I'm sure there's possibly something that can be done in Unity Explorer to hide the UI in
   freecam mode, but this is faster imo)

You just need to enable the cinema camera BEFORE enableing the Freecam. This way, the GUI will stays hidden no mater what.

But once everyting is done, Open your console and type `ItemPoser.PoseItem <itemID>` (using the item id you just got) and
press enter.

This should pose your item using its UI data and lock the freecam directly to it.

You can then take a screenshot and use that to your's heart content.

## I'm a Mod Creator and my items don't pose correctly. What can I do? ##

For your item to pose nicely, you'll need to provide  `Icon Position Offset`, `Icon Rotation Offset` and
`Icon Scale Offset` in their UI Data's.

For quick results, you can copy the values off a vanila item with an Item Icon you like. It usualy give great looking icons, especially if your custom item has the same gurth that the item you're basing yourself on.

But if you want someting a bit more unique, you can always pose your item in the Unity Editor by hands with the "2D view" enabled. 

### Filling Out the UI Data In the Unity Editor ###

First, Before posing your item, you'll need to create an empty parent game object that will hold your item while posing (right click on your item > `Create Empty Parent`, or `Ctrl+Shift+G` with your item selected);

Then, just rotate/scale/move your item around until you're satisfied (NOTE: DO NOT ROTATE THE NEWLY CREATED PARENT. MAKE SURE IT'S YOUR ORIGINAL ITEM (THE ONE WITH THE ITEM COMPONENT ATTACHED TO IT) THAT'S SELECTED IN YOUR HIERARCHY!)

Also, because this mod doesn't use the EXACT same pipeline for generating Item icons, your main focus should be on finding a good icon angle instead of moving/scaling.  

But after you found a satisfying pose, the next step is to:
1. copy the `Position` (by right clicking on `Position`) of your item to the UI Data's `Icon Position Offset`
1. copy the `Rotation` (by right clicking on `Rotation`, then `Copy Euler Angle`) of your item to the UI Data's `Icon Rotation Offset`
1. copy the `Scale X` of your item to the UI Data's `Icon Scale Offset`

Then save the `Icon Position Offset`, `Icon Rotation Offset` and `Icon Scale Offset` to your prefab by right-clicking on "UI Data" and clicking on "Apply to Prefab"

You can then rebuild and replace your `.peakbundle` as usual and your item will be ready to pose.

## Configuration ##

To help you get a good and easy to work with shot, the mod comes with a couple of configs options:

- `Red Value`, `Green Value`, `Blue Value`: The RGB values of the item's backdrop (ranges from 0-255). Leave as default for a perfect green.
- "Setup Rotation": The Y angle (yaw) of the whole pose setup. Usefull for tweaking the lighting.

You can change these configs in the mod's config file.

You'll need to run the mod first for it to generate or use a mod like [ModConfig](https://thunderstore.io/c/peak/p/PEAKModding/ModConfig/) to change them in game. You can use your mod manager too!

## Issues ##

If you find any bugs with the mod or have any suggestion, please add a ticket to the git repo. We'll take a look and
respond as soon as possible!