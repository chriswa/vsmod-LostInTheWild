# vsmod-LostInTheWild

*1.1.0 fixes an issue where patching failed because of JIT optimizations*

Vintage Story mod which hides coordinates from players by disabling the `/waypoint` command and redacts coordinates in `/land` messages. Only needs to run on the server.

Also hides positions from the following server console messages, to help server owners avoid accidentally cheating:

- "Placing player at ..."
- "Teleporting player ... to ..."
- "Teleporting entity ... to ..."

If you, as the server owner, need a player's position, you can use a command added by this mod:

`/getplayerpos PlayerName`

The player's position will be reported to the server console (not in chat.)
