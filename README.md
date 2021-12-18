# Randomizer 3.5

Randomizer 3.5 is a quick port of the Randomizer 3 mod for Hollow Knight 1.5.

# Randomizer 3
Randomizer 3 expands on previous versions of the Hollow Knight randomizer by allowing you to randomize more items than ever before and, for the first time, to randomize area or room transitions.

- Randomizer 3 requires SereCore.dll and Modding Api version 53 or greater to run. These are both automatically downloaded by the ModInstaller.

- There is a map tracker for area and room randomizer available at https://github.com/homothetyhk/RandomizerTracker/releases
- There is a community wiki published here: https://hollow-knight-randomizer.fandom.com/
- There is an old community randomizer guide published here (no longer updated): https://tinyurl.com/HollowKnightRandomizerGuide

Details on all of the various settings follow:

## Restrictions

The skip settings control which difficult skips the randomizer may require. If you are not familiar with these skips, especially as they are used in speedrunning, you are advised to turn them off.

With skips allowed, the player is advised to take care to not get locked out of certain required pogos. Obtain:
- No more than 1 nail upgrade before claw or wings
- No more than 3 nail upgrades before claw

In Split Claw mode, obtain:
- No more than 2 nail upgrades with only a single claw piece
- Any number of nail upgrades with a single claw piece as well as wings

## Quality of Life

- Reduce Rock/Totem Preloads: Only preload a single geo rock/soul totem type; randomized geo rocks/soul totems will all look like KP rocks/generic totems instead of the correct original rock type. The game must be restarted for changes to this setting to take effect.
- Early Geo: start the game with between 300 and 600 geo.
- Extra Platforms: platforms in various places that prevent softlocks. For example, there are several platforms added to Ancient Basin to prevent having to quit-out after checking certain locations without vertical movement.
- Recent Items: Display the last 5 randomized items collected
- NPC Item Dialogue: certain checks (Cornifer maps, Nailmasters' nail arts, Nailmaster's Glory, Vengeful Spirit, Shade Soul, Descending Dark) will be given by the relevant NPCs instead of a shiny item on the ground.

## Randomization

These settings control which items are randomized.
- Dreamers: Lurien, Monomon, Herrah, and World Sense. World Sense is the Black Egg Temple pickup to view your completion percentage
- Skills: all spells, nail arts, and movement abilities
- Charms
- Keys: all key objects, as well as King's Brand, Godtuner, and Collector's Map
- Mask Shards
- Vessel Fragments
- Pale Ore
- Charm Notches: all charm notches, except those sold by Salubra
- Geo Chests: all geo chests, except the one above Baldur Shell and those in the Junk Pit
- Relics: all wanderers journals, hallownest seals, king's idols, and arcane eggs.
- Rancid Eggs
- Stags
- Maps
- Whispering Roots
- Grubs
- Lifeblood Cocoons
- Soul Totems
- Lore Tablets 
- Grimmkin Flames
- Geo Rocks
- Boss Essence: essence drops from Dream Warriors and Dream Bosses
- Boss Geo: geo drops from Gruz Mother, Massive Moss Charger, Vengefly King, Gorgeous Husk, Crystal/Enraged Guardian and both Soul Warriors
- Journal Entries: The Hunter's Journal, along with the Goam, Garpede, Charged Lumafly and Void Tendrils and Seal of Binding locations.
- Junk Pit Chests
- Palace Totems/Tablets/Entries: Disable this to remove the totems and tablets in White Palace from the randomization pool, along with the Seal of Binding
- Egg Shop: instead of summoning your shade, Jiji will now let you buy items for rancid eggs. If you've given her enough rancid eggs, any items you've reached the cost for will be summoned.
- Mimics: Add the four non-colo mimic locations to the randomization pool (some grubs may turn into mimics). With grubs unrandomized, instead randomize the mimics and grubs among themselves (some grubs may turn into mimics).

Note: several items are randomized progressively, meaning that collecting any item in a given family always gives the first upgrade, collecting another gives the second upgrade, etc. The families this pertains to are:
- Dream Nail, Dream Gate, Awoken Dream Nail
- Mothwing Cloak, Shade Cloak
- Vengeful Spirit, Shade Soul
- Desolate Dive, Descending Dark
- Howling Wraiths, Abyss Shriek
Any of the above pickups may be forced by randomizer logic.

Note: the following items can be used to kill baldurs:
- All difficulties: Vengeful Spirit (or upgrades), Desolate Dive (or upgrades), Grubberfly's Elegy, Glowing Womb, Dash Slash with Dash
- With Mild skips: Weaversong, Spore Shroom
- With Spicy skips: Cyclone Slash, Mark of Pride
- Not in logic, but feel free to try if you have time on your hands: Longnail, Sprintmaster+Dashmaster
Baldur hp is reduced to 5 to make slower baldur kills less tedious, and to reduce rng.

Note: the lifeblood door in Abyss opens if you enter the room with a single lifeblood mask. In logic, it requires a lifeblood charm.

Note: while lifeblood cocoons and soul totems normally reset after sitting on a bench, this is no longer true if you randomize them. Instead, picking up a randomized lifeblood cocoon will give a large number of lifeblood masks immediately, and picking up a randomized soul totem will completely fill up all soul.

- Duplicate Major Items: adds second copies of the following important items:
		Dreamer (interchangeable for any of the three dreamers in opening black egg temple), Void Heart,
		Mothwing Cloak, Shade Cloak, Mantis Claw, Monarch Wings, 
		Crystal Heart, Isma's Tear, Dream Nail, 
		Vengeful Spirit, Desolate Dive, Howling Wraiths
	Picking up an excess copy after collecting all other copies of an item gives 100 geo.
	Duplicate items are *not* placed using randomizer logic.
- Randomize Starting Items: begin the game with several items already in the inventory, including at least one vertical movement upgrade.
- Starting Location: you may select a starting location from the menu, or set it to be random. Some locations may be unavailable depending on your settings.
- Create spoiler log: creates a file in the save directory with all item/transition placements

## Additional features

There are four logs created in the save directory to help you with your playthrough.
- Tracker Log: this log continuously records item locations, transition connections, and hints as you discover them.
- Helper Log: this log computes which locations/transitions are accessible with your current equipment.
- Spoiler Log: this log lists the exact locations of every randomized item and/or transition.
- Condensed Spoiler Log: this log condenses the spoiler log to only list the locations of major progression items.

The "cursed" option is a special hard mode for randomizer veterans. Features include:
- Masks, Vessels, Ore, Notches, Geo Chests, Eggs, and Relics are replaced by 1 geo pickups, if randomized.
- Shade Soul, Descending Dark, and Abyss Shriek are removed.
- Major items are less likely to be placed as early progression items.
- Focus (the ability to heal) is randomized, and is no longer available from the start of the game.

Activating the "split claw" option replaces Mantis Claw with two items, a "Left Mantis Claw" that works only on left walls, and a "Right Mantis Claw" that works only on the right.
- There are now two checks in Mantis Village to replace the one Mantis Claw check.
- Neither claw piece is duped when "duplicate major items" is activated.
- With Randomized Start items, it is impossible to start with exactly one claw piece.
- With this setting activated but skills unrandomized, the Mantis Claw pickup will still be replaced with the two claw pickups, which will not be added to the randomization algorithm.

Activating the "split cloak" option replaces Mothwing Cloak with two items, a "Left Mothwing Cloak" that lets you dash to the left, and a "Right Mothwing Cloak" that dashes to the right.
- There is now an additional check dropped by Hornet (in Greenpath).
- Shade Cloak is *not* split (so if you can shadowdash to the left and dash to the right, you can also shadowdash to the right)
- The cloak pieces *are* duped with "duplicate major items" activated.
- This setting is unavailable with skills unrandomized.

Activating the "cursed nail" option adds three extra items to the pool, that let you swing your nail left, right and up; by default in this mode, the nail can only be swung down.

Activating the Randomize Swim option causes all pools of water to be dangerous, until the Swim item is found.

Activating the Elevator Pass option prevents you from being able to use the two large city elevators until the elevator pass item is found; a new location is added in city storerooms where the toll used to be.

## Area/Room randomizer

- Area randomizer randomizes items and connections between areas, which are understood to be any region of the game with a name which appears as onscreen text, excluding dream areas, trams, and elevators.
- Room randomizer randomzies items and nearly every transition between different rooms. Not included are:
    - Warps of any kind, including those entering dream areas
	- Trams and elevators
	- Transitions within Godhome and the Shrine of Believers
	- The transitions leading to Sly's storeroom, Bretta's basement, or to any trial of the colosseum
- The Connected-Area Room randomizer works similarly to Room randomizer, with the additional constraint that it attempts to keep rooms from the same area connected, up to a certain extent, and not affecting single entrance rooms.
Also, note the following:
- Due to an imbalance in the number of left and right transitions, the Divine and Grimm tents are included in the randomization, but their vanilla entrances have been removed, and will not spawn in Dirtmouth
- The nightmare lantern must be lit to check Grimmchild
- Sly must be rescued to use his shop

## New game mechanics
- Collecting Grimmchild activates the Nightmare Lantern. If Grimmkin Flames are not randomized, Grimmchild is also given with the first 6 flames already collected.
- You can preview the items at Colosseum, Grey Mourner, and King Fragment by interaction.
- You can preview the item at Grey Prince Zote by reading Bretta's diary.

## Known issues
- Using Benchwarp may cause some room changes to fail to occur. This can be fixed by exiting and reentering the room.
