#  Colony Game 
This project was an attempt to learn some of the most difficult and interesting things about creating a game like [_Rimworld_](https://store.steampowered.com/app/294100/RimWorld/)

## Goal 
The goal of this project was to take a look over the game _Rimworld_ and attempt to create a faithful recreation in unity game engine. Ultimately, I would like to use the experience as a method of hands-on learning to support my book knowledge on coding and game development. 

## Challenges 

### Grid System 
In _Rimworld_ the world is based on a grid like system where each box in the grid indicates a space with properties like walkability. In my implementation it was imperative to replicate this. To achieve something similar I used Unity's tilemap system for the visual 
aspects and created my own data structure where each space was called a "spot." These tiles held information about themselves like their 2-D position and neighbors. This then allowed me to create a grid where if a spot was walkable, it was registered into a dictionary 
for lookup later and had connections to each walkable neighbor. If a spot was not walkable, no neighbors would register that spot and it would not be included in pathfinding. 

### Pathfinding 
Pathfinding on the grid system above was achieved with an A-Star algorithm based on the [Manhattan Heuristic](https://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html). As the grid manager had the ability to locate a spot in the grid based off the dictionary
composed at runtime, it was now possible to use A-Star to create the best path. At first I was using a [Flood Fill Search](https://en.wikipedia.org/wiki/Flood_fill#:~:text=The%20traditional%20flood%2Dfill%20algorithm,them%20to%20the%20replacement%20color.) but, 
This method used an incredible amount of resources, causing lag and freezing with just one character path finding. Though this one a very difficult algorithm to implement, it was credibly rewarding to see many characters pathfinding all at the same time in real time. 

### Closest Task
Now that my characters could find a path to get to their destination, it was time to determine where they should be headed. When given a set of tasks, like cutting down 5 trees, how would an individual character know which tree to cut first or next. Originally 
I had the characters cut in order of which they were added. However, that meant that each character, no matter how far away, might walk through a whole forest of trees in order to start cutting the "first" tree. This is when I started to research and implement 
a [k-d tree](https://en.wikipedia.org/wiki/K-d_tree) in order to determine for each character which tree would be closest to them. This means if 2 characters stand on opposite sides of a forest they are ordered to cut down, they will work from each side respectively to
meet in the middle. 

### Inventory 
The last challenge I faced was that I wanted each character to deposit their inventories into a centralized chest. However, this chest should only be able to hold so much material. This means that we need to check the each characters inventory to see if the chest can 
accommodate the characters inventory before they spend the time going to deposit. For my implementation I had each chest log 2 inventories, the currently held inventory and the queued inventory. This way when a character needs to empty their pockets, they can test what they
have against the queued inventory and see if after everyone has deposited their pockets they can also add theirs. To address race conditions, only one character can be modifying the queued inventory at a time. Once a character arrives at the chest, their pockets have 
materials removed and the items transition from the queued inventory to the current inventory. This way, we can cancel the action of a character adding to the chest if needed. 

## Conclusion
Though this project will never be as great as the game _Rimworld_ the hands-on experience and difficulty of creating/learning solutions to these surprisingly in-depth issues was great! It was engaging, challenging, and fun! There are even more challenges in this project
documented here so, feel free to ask about this project for even more! 
