MOSSBACK VILLAGE - FREE 16x16 PIXEL ART PLATFORMER PACK
========================================================

Price: pay what you want, $0 minimum.

An overgrown fishing village built on the shell of a sleeping giant turtle.
The pack is designed for 2D platformers and Metroidvanias.

CONTENTS
--------
107 PNG exports across terrain, backgrounds, characters, props, effects and UI.
280 animation frames.
164 terrain tiles:
  tiles_shell_ground: 64 (47 blob masks, 7 variations, a 2x2 large-scute stamp,
  and 6 T2 slopes)
  tiles_shell_rim: 15
  tiles_platforms: 25
  tiles_buildings: 30
  tiles_water: 30
51 prop and interactable images:
  29 static props, 7 animated props, and 15 interactable states.
28 character, enemy, NPC and critter animations.
32-colour Mossback palette.

FILES AND IMPORT
----------------
The PNGs are the ready-to-import game assets. Editable layered Aseprite sources
are in source/. File names follow <stem>[_stripN]_WxH.png; sheet exports include
their dimensions in the filename (for example,
tilesets/tiles_shell_ground_192x96.png). Animation strips are horizontal, use
nearest-neighbour filtering, and have frame timing in the included Aseprite
source.

palette/ contains mossback32.hex, mossback32.gpl, and two palette swatch
previews. All game art uses Mossback 32.

examples/demo_map.tmx is the Tiled showcase map. Its top rows show every tile
cell; the playable village area demonstrates the shell ground, T2 slopes,
stilted buildings, platforms, water and characters; the lower rows show all
51 props and interactables. The large scute stamp is four adjacent tiles in a
2x2 block, placed about once per 6x4 tiles of shell fill. The map uses the
tiles_water_grid.tsx helper for 16x16 water cells and the four-frame animated
surface; the standard generated tiles_water_64x128.tsx remains available for
the raw atlas.

Page media in examples/preview/:
  cover_630x500.png
  village_run.gif (8-second loop, 80 frames at 100 ms, exported 2x nearest)
  full_mock_320x180.png (reference resolution)
  full_mock_640x360.png
  tilesets_3x_labeled.png
  characters_enemies_labeled.png
  props_lineup_labeled.png
  fx_ui_labeled.png
  demo_map.png

The Godot 4 validation project is in the project workspace under godot_test/
and is not part of this release folder. The included PNGs and Aseprite sources
can also be imported into Unity, GameMaker, or any engine that reads PNG.

PARALLAX
--------
Draw in this order, back to front:
  bg0_sky                 speed 0.00 (static)
  bg1_far_sea             speed 0.10
  bg_landmark_turtle_head speed 0.15
  bg2_shell_curve         speed 0.25
  bg3_village_mid         speed 0.50

The bg_landmark_turtle_head filename is retained for compatibility; its current
artwork shows the distant village skyline and harbor light only, with no turtle
silhouette.

CHARACTER PIVOTS
----------------
Characters, critters and FX use the centre of the canvas for horizontal
flipping. Grounded character feet sit at the centre x on the bottom row.
player_attack is 80x48 with its feet at (40,47); fx_slash_hook is 80x32 and
should be spawned with its centre at the player's feet (40,31). Flipping these
assets needs no horizontal offset.

PLAYER TEST METRICS
-------------------
Collision box: 10x22 pixels.
Measured jump: 48.06 px rise and 80.06 px horizontal travel (3 tiles high,
5 tiles wide). The test controller exposes all 11 T1/T2 player animations.

LICENSE
-------
Free for personal and commercial games. Modification is allowed. Credit is
appreciated but not required. Do not resell or redistribute the asset files
themselves, whether alone or bundled into another asset pack. See LICENSE.txt.

TAGS
----
pixel-art, 16x16, tileset, platformer, metroidvania, 2d, sprites, fantasy,
ocean, cozy, asset-pack, free

COMING NEXT
-----------
1. Mossback: The Underbelly - caves inside the shell and beneath her, with
   dream-purple bioluminescence.
2. Night & Storm - palette-swapped tiles and backgrounds, rain, lightning,
   and lit windows.
3. Village Life - 10+ NPCs, market stalls, and interiors.
4. Things From the Deep - an enemy pack plus a boss.
5. She Wakes - set pieces of the turtle stirring.
6. A bundle of everything.
