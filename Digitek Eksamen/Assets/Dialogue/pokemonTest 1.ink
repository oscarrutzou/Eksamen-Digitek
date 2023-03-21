INCLUDE globals.ink

#audio:celeste_high
{ pokemon_name == "": -> main | -> already_chose }

=== main ===
Which pokemon do you choose? #speaker:Mr.Bum #portrait:dr_green_neutral #layout:right
    + [Charmander]
        -> chosen("Charmander")
    + [Bulbasaur]
        -> chosen("Bulbasaur")
    + [Squirtle]
        -> chosen("Squirtle")
        
=== chosen(pokemon) ===
~ pokemon_name = pokemon
You chose {pokemon}!
-> END

=== already_chose ===
You already chose {pokemon_name}!
-> END