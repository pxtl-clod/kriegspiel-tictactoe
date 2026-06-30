# PLAN

This document contains incomplete plans for new features.  When ready, they will be moved to TODO.

## GUI
Use Uno or Avalonia for a GUI. Google recommends Avalonia for sliding animations.

## Server
Currently the game is served by a simple JSON file, which is obviously a dumb
hack. Need a proper API.

 - Need a way to extract a player-view of the gamestate, that state can be sent
   to clients and renderers. KriegspielTicTacToe.PlayerViewModel?

## Incomplete Information Tic-Tac-Toe

https://www.smbc-comics.com/comic/incomplete

https://mastodon.social/@ZachWeinersmith/111896904870106666


## Drop Mode
A mode where pieces are placed in from the top, Drop-4 style.

## Emoji
Currently player spaces are 1 char long.  Emoji are 2 char long in most
monospaced fonts. Allow the players to pick an emoji or a 2-char-long
Mark.  Custom boardwidth?
- Will have to block Marks that are a substring of other Marks.

## BSky bot?  Chat Bot?
- Simple emoji mode without borders?  Render a black square for empty spaces? Or U+3000 space?
- API for chatbots?  Is there a standard one?  Needs standard API including @s,
  DMs, Likes (for voting-based games)
    - There is not.  Have to DIY if I want to support multiple platforms.
- Chatbot could be stateless if there's bidirectional serialization of gamestate.