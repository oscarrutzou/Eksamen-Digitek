INCLUDE ../globals.ink

-> Start

=== Start ===
//Switch statement for at se hvor lang i historien spilleren er
{brothers_firstTalkCalled: |-> FirstTalk}
{brothers_secondTalkCalled: |-> SecondTalk}
-> END
//Start block end


=== FirstTalk ===

Halohoj, her kommer jeg! #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

Ha! Se bror det er den yngste. #speaker: Bror 1 #portrait:dr_green_neutral #layout:right #audio:animal_crossing_mid

Klods Hans, hvor er du paa vej hen? #speaker: Bror 2 #portrait:dr_green_neutral #layout:right #audio:animal_crossing_mid

Jeg vil tage hen til prinsessen, da jeg faar sådan en lyst til at gifte mig. Ta'r hun mig, så ta'r hun mig! og ta'r hun mig ikke, så ta'r jeg hende alligevel! #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

Haha! Tror du virkelig, at du kan vinde hendes hjerte? Du er ikke laert, og kan ingen gang få en ordentlig hest at ride på. #speaker: Bror 1 #portrait:dr_green_neutral #layout:right #audio:animal_crossing_mid

Jeg har fundet gaver jeg vil foraere til kongedatteren, en doed krage, en gammel traesko, og noget pludder. Det kommer hun til at blive glad for. #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

Ja, goer du det! Men vejen frem til slottet er farligt, så hvad med du ridder forreste på din smarte ged Klods Hans. #speaker: Bror 2 #portrait:dr_green_neutral #layout:right #audio:animal_crossing_mid

Det lyder fint. Halohoj her kommer jeg! #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

~ brothers_firstTalkCalled = true
-> DONE


=== SecondTalk ===
Oemh. Hej.. Det er da en meget varm dag i dag. #speaker: Bror 1 #portrait:dr_green_neutral #layout:left #audio:animal_crossing_mid

Du er simpelthen for kedelig. Dur ikke! Vaek! #speaker: Kongedatter #portrait:dr_green_neutral #layout:right #audio:animal_crossing_high



Hvad er det for en lugt? #speaker: Bror 2 #portrait:dr_green_neutral #layout:left #audio:animal_crossing_mid

Det er fordi min fader i dag steger hanekyllinger! #speaker: Kongedatter #portrait:dr_green_neutral #layout:right #audio:animal_crossing_high

Hvad be - hvad? #speaker: Bror 2 #portrait:dr_green_neutral #layout:left #audio:animal_crossing_mid

Dur ikke! Vaek! #speaker: Kongedatter #portrait:dr_green_neutral #layout:right #audio:animal_crossing_high



Sikke en dejlig lugt herhenne! #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

Det er fordi jeg steger hanekyllinger! #speaker: Kongedatter #portrait:dr_green_neutral #layout:right #audio:animal_crossing_high

Naah det lyder rart. Kan jeg faa en krage stegt? #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

Det kan De meget godt, men har De noget at stege den i, for jeg har hverken potte eller pande. #speaker: Kongedatter #portrait:dr_green_neutral #layout:right #audio:animal_crossing_high

Det har jeg! Jeg fandt denne gamle traesko og dette er lige ledligheden at bruge den i. #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

Det er jo et helt maaltid, men hvor faar vi dyppelsen fra? #speaker: Kongedatter #portrait:dr_green_neutral #layout:right #audio:animal_crossing_high

Det har jeg skam i lommen, jeg ville ikke spilde noget af det laekre pludder. #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

Ha det kan jeg lide! Du kan da svare! Jeg vil have dig til mand! #speaker: Kongedatter #portrait:dr_green_neutral #layout:right #audio:animal_crossing_high

~ brothers_secondTalkCalled = true
-> DONE








