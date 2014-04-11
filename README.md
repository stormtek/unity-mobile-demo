unity-mobile-demo
=================

A demo game for on mobile built using Unity 4.3. This has been tested to work on Android using a Samsung Galaxy Note 2. It should work on iOS easily enough, but this has not been tried.

The idea is to build up a basic turn-based deathmatch. This will be single player with just the one computer player initially (though adding more should be easy). I would like to get 2 types of units working: melee and ranged. A unit will only be able to move / attack within a certain range -> 'line of sight'.

There will be a basic AI working for the computer too (especially since the target is single player). Each action it can take per turn will be given a ranking and it will pick the 'best' action when it needs to choose what to do. The basic ranking algorithm will be along the lines of: attack that kills a unit is great (with stronger units as better kills), attack that damages a unit is good (with stronger units as better targets), move as close to a unit as possible is ok (ideally close enough to attack but not be attacked). This should at least make things challenging. We can allow the AI to query units for their strength / range / hitpoints since that information will be present for a player when they make their turn.

Please note that this is under developement, so it is not guaranteed to be feature complete at any time.
