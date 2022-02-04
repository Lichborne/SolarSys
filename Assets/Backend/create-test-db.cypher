CREATE 
// ================================== user 
(foo_user :USER {name: "Foo Bar", email: "foo.bar@doc.ic.ac.uk"}),

// ================================== project root 
(project_root :PROJECT_ROOT {title: "Test Project", guid: "61f51a64-2121-404a-9a4f-99d869e63aad"}),

// ================================== adding project to user
(foo_user) -[:OWNS_PROJECT]->(project_root),

// ================================== create log history linked list
(project_root) -[:LOG_HISTORY]-> (a :LOG_NODE {guid: '2f6e4433-6aeb-4072-8949-1b2da6e9eb59', change: 'INSERT', body: 'description', timestamp : '123'}),
(a) -[:LOG_LINK]-> (b :LOG_NODE {guid: 'fd22adc2-45eb-48f7-a4e7-e1836af257cf', change: 'INSERT', body: 'description', timestamp : '123'}),

// ================================== nodes in project_root
(project_root) -[:CONTAINS]-> (ro:NODE {
    title: 'Rowen', 
    description: 'In a hole in the ground there lived a hobbit. Not a nasty, dirty, wet hole, filled with the ends of worms and an oozy smell, nor yet a dry, bare, sandy hole with nothing in it to sit down on or to eat: it was a hobbit-hole, and that means comfort.', 
    guid: 'b22f0d72-cf45-481d-b697-80c0350341b9', 
    coordinates: [7, 7, 0]
}),

(project_root) -[:CONTAINS]-> (ju:NODE {
    title: 'Julius', 
    description: 'It had a perfectly round door like a porthole, painted green, with a shiny yellow brass knob in the exact middle. The door opened on to a tube-shaped hall like a tunnel: a very comfortable tunnel without smoke, with panelled walls, and floors tiled and carpeted, provided with polished chairs, and lots and lots of pegs for hats and coats-the hobbit was fond of visitors. The tunnel wound on and on, going fairly but not quite straight into the side of the hill-The Hill, as all the people for many miles round called it-and many little round doors opened out of it, first on one side and then on another. No going upstairs for the hobbit: bedrooms, bathrooms, cellars, pantries (lots of these), wardrobes (he had whole rooms devoted to clothes), kitchens, dining-rooms, all were on the same floor, and indeed on the same passage. The best rooms were all on the left-hand side (going in), for these were the only ones to have windows, deep-set round windows looking over his garden, and meadows beyond, sloping down to the river.',
    guid: '0db4d262-6f58-41ff-8d1c-24741ff70c0f', 
    coordinates: [-5.16, 4.67, 4.73]
}),

(project_root) -[:CONTAINS]-> (ni:NODE {
    title: 'Nick', 
    description: 'This hobbit was a very well-to-do hobbit, and his name was Baggins. The Bagginses had lived in the neighbourhood of The Hill for time out of mind, and people considered them very respectable, not only because most of them were rich, but also because they never had any adventures or did anything unexpected: you could tell what a Baggins would say on any question without the bother of asking him. This is a story of how a Baggins had an adventure, and found himself doing and saying things altogether unexpected. He may have lost the neighbours respect, but he gained-well, you will see whether he gained anything in the end.',
    guid: '3e8bb21f-e69d-4001-90ec-94b8448346e3', 
    coordinates: [0.61, 2.33, -6.97]
}),

(project_root) -[:CONTAINS]-> (jo:NODE {
    title: 'Josh', 
    description: 'The mother of our particular hobbit-what is a hobbit? I suppose hobbits need some description nowadays, since they have become rare and shy of the Big People, as they call us. They are (or were) a little people, about half our height, and smaller than the bearded Dwarves. Hobbits have no beards. There is little or no magic about them, except the ordinary everyday sort which helps them to disappear quietly and quickly when large stupid folk like you and me come blundering along, making a noise like elephants which they can hear a mile off. They are inclined to be fat in the stomach; they dress in bright colours (chiefly green and yellow); wear no shoes, because their feet grow natural leathery soles and thick warm brown hair like the stuff on their heads (which is curly); have long clever brown fingers, good-natured faces, and laugh deep fruity laughs (especially after dinner, which they have twice a day when they can get it). Now you know enough to go on with. As I was saying, the mother of this hobbit-of Bilbo Baggins, that is-was the famous Belladonna Took, one of the three remarkable daughters of the Old Took, head of the hobbits who lived across The Water, the small river that ran at the foot of The Hill. It was often said (in other families) that long ago one of the Took ancestors must have taken a fairy wife. That was, of course, absurd, but certainly there was still something not entirely hobbitlike about them, and once in a while members of the Took-clan would go and have adventures. They discreetly disappeared, and the family hushed it up; but the fact remained that the Tooks were not as respectable as the Bagginses, though they were undoubtedly richer.',
    guid: '80dc35fb-13b8-4231-9fd4-d37824bdf1ee', 
    coordinates: [4.26, 0, 5.56]
}),

(project_root) -[:CONTAINS]-> (ba:NODE {
    title: 'Balazs', 
    description: 'Not that Belladonna Took ever had any adventures after she became Mrs. Bungo Baggins. Bungo, that was Bilbos father, built the most luxurious hobbit-hole for her (and partly with her money) that was to be found either under The Hill or over The Hill or across The Water, and there they remained to the end of their days. Still it is probable that Bilbo, her only son, although he looked and behaved exactly like a second edition of his solid and comfortable father, got something a bit queer in his make-up from the Took side, something that only waited for a chance to come out. The chance never arrived, until Bilbo Baggins was grown up, being about fifty years old or so, and living in the beautiful hobbit-hole built by his father, which I have just described for you, until he had in fact apparently settled down immovably.',
    guid: 'd9391f3a-a35b-442c-8f5e-255b0d7df5d8', 
    coordinates: [-6.89, -2.33, -1.22]
}),

(project_root) -[:CONTAINS]-> (ol:NODE {
    title: 'Olivia', 
    description: 'By some curious chance one morning long ago in the quiet of the world, when there was less noise and more green, and the hobbits were still numerous and prosperous, and Bilbo Baggins was standing at his door after breakfast smoking an enormous long wooden pipe that reached nearly down to his woolly toes (neatly brushed)-Gandalf came by. Gandalf! If you had heard only a quarter of what I have heard about him, and I have only heard very little of all there is to hear, you would be prepared for any sort of remarkable tale. Tales and adventures sprouted up all over the place wherever he went, in the most extraordinary fashion. He had not been down that way under The Hill for ages and ages, not since his friend the Old Took died, in fact, and the hobbits had almost forgotten what he looked like. He had been away over The Hill and across The Water on businesses of his own since they were all small hobbit-boys and hobbit-girls.',
    guid: '20d39f6b-8662-4328-8dc5-df57eb3c4a3a', 
    coordinates: [5.91, -4.67, -3.76]
}),

(project_root) -[:CONTAINS]-> (ko:NODE {
    title: 'Konstantinos', 
    description: 'All that the unsuspecting Bilbo saw that morning was an old man with a staff. He had a tall pointed blue hat, a long grey cloak, a silver scarf over which his long white beard hung down below his waist, and immense black boots.',
    guid: '199640fc-3605-4368-827c-a4e66551c0b5', 
    coordinates: [-1.82, -7, 6.76]
}),

// =================================== edges between nodes
(ro)-[:LINK {
    title: 'messages', 
    description: '"Good Morning!" said Bilbo, and he meant it. The sun was shining, and the grass was very green. But Gandalf looked at him from under long bushy eyebrows that stuck out further than the brim of his shady hat.',
    guid: '180d1ece-d50b-4bbc-9507-a2af6812b22c'
}]->(ni),

(ni)-[:LINK {
    title: 'calls', 
    description: '"What do you mean?" he said. "Do you wish me a good morning, or mean that it is a good morning whether I want it or not; or that you feel good this morning; or that it is a morning to be good on?"',
    guid: 'c997c2d1-3c9c-4ea1-bd30-3a098fa92b15'
}]->(ro),

(ju)-[:LINK {
    title: 'calls', 
    description: '"All of them at once," said Bilbo. "And a very fine morning for a pipe of tobacco out of doors, into the bargain. If you have a pipe about you, sit down and have a fill of mine! Theres no hurry, we have all the day before us!" Then Bilbo sat down on a seat by his door, crossed his legs, and blew out a beautiful grey ring of smoke that sailed up into the air without breaking and floated away over The Hill.',
    guid: 'e73b6cb0-99f9-4179-a372-79b4062eb680'
}]->(ro),

(ju)-[:LINK {
    title: 'calls', 
    description: '"Very pretty!" said Gandalf. "But I have no time to blow smoke-rings this morning. I am looking for someone to share in an adventure that I am arranging, and its very difficult to find anyone."',
    guid: '43cf5e02-a874-4207-8727-00fdd0df3786'
}]->(ni),

(jo)-[:LINK {
    title: 'messages', 
    description: '"I should think so-in these parts! We are plain quiet folk and have no use for adventures. Nasty disturbing uncomfortable things! Make you late for dinner! I cant think what anybody sees in them," said our Mr. Baggins, and stuck one thumb behind his braces, and blew out another even bigger smokering. Then he took out his morning letters, and began to read, pretending to take no more notice of the old man. He had decided that he was not quite his sort, and wanted him to go away. But the old man did not move. He stood leaning on his stick and gazing at the hobbit without saying anything, till Bilbo got quite uncomfortable and even a little cross.',
    guid: '9813edc1-dcb4-4d13-b57f-ee954f81e89f'
}]->(ju),

(ju)-[:LINK {
    title: 'messages', 
    description: '"Good morning!" he said at last. "We dont want any adventures here, thank you! You might try over The Hill or across The Water." By this he meant that the conversation was at an end.',
    guid: 'd70c33ef-7db1-4cf5-9ece-f4a6301b8ef4'
}]->(jo),

(jo)-[:LINK {
    title: 'follows', 
    description: '"What a lot of things you do use Good morning for!" said Gandalf. "Now you mean that you want to get rid of me, and that it wont be good till I move off."',
    guid: 'f4835b6f-1489-4bb5-9105-69931e43c462'
}]->(ju),

(ol)-[:LINK {
    title: 'follows', 
    description: '"Not at all, not at all, my dear sir! Let me see, I dont think I know your name?"',
    guid: '32bfb7b2-8485-4b0d-b794-acee5395f32d'
}]->(jo),

(ol)-[:LINK {
    title: 'follows', 
    description: '"Yes, yes, my dear sir-and I do know your name, Mr. Bilbo Baggins. And you do know my name, though you dont remember that I belong to it. I am Gandalf, and Gandalf means me! To think that I should have lived to be good-morninged by Belladonna Tooks son, as if I was selling buttons at the door!"',
    guid: '8ab798e8-421a-425d-b9a2-3232dfcdd13d'
}]->(ba),

(ba)-[:LINK {
    title: 'messages', 
    description: '"Gandalf, Gandalf! Good gracious me! Not the wandering wizard that gave Old Took a pair of magic diamond studs that fastened themselves and never came undone till ordered? Not the fellow who used to tell such wonderful tales at parties, about dragons and goblins and giants and the rescue of princesses and the unexpected luck of widows sons? Not the man that used to make such particularly excellent fireworks! I remember those! Old Took used to have them on Midsummers Eve. Splendid! They used to go up like great lilies and snapdragons and laburnums of fire and hang in the twilight all evening!" You will notice already that Mr. Baggins was not quite so prosy as he liked to believe, also that he was very fond of flowers. "Dear me!" he went on. "Not the Gandalf who was responsible for so many quiet lads and lasses going off into the Blue for mad adventures? Anything from climbing trees to visiting elves-or sailing in ships, sailing to other shores! Bless me, life used to be quite inter-I mean, you used to upset things badly in these parts once upon a time. I beg your pardon, but I had no idea you were still in business."',
    guid: 'dfad166e-d4cf-4cb1-b09b-740d542c59af'
}]->(ba);