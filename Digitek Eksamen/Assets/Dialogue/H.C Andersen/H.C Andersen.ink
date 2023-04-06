INCLUDE ../globals.ink

//Start Block
->Start

=== Start ===
//Switch statement for at se hvor lang i historien spilleren er

{questItemsCollected:
- 0: {hc_firstTalkCalled: -> FirstTalk.FirstWaitStich | -> FirstTalk}
- 1: -> FirstTalk.FirstWaitStich
- 2: {hc_secondTalkCalled: -> SecondTalk.SecondWaitStich | -> SecondTalk}
- 3: -> SecondTalk.ThirdWaitStich
}

-> END
//Start block end


=== FirstTalk ===
Hej Klods Hans! Jeg hedder H.C. Andersen. #speaker: H.C Andersen #portrait:dr_green_neutral #layout:right #audio:animal_crossing_mid

Jeg har hørt fra dine brødre at de ikke tror du har en chance med kongedatteren, men det tror jeg ikke på det. 

Jeg ved du er en speciel person og kan gøre det hvis du prøver. 

Halohoj Hr. Andersen! #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

Har du da nogen ide omkring hvad jeg skal gøre for at få hende? #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

Ja, det er jo en god ide at have en gave med. Prøv derfor at kig rundt og se om du kan finde noget hun ville kunne lide. #speaker: H.C Andersen #portrait:dr_green_neutral #layout:right #audio:animal_crossing_mid

Kom tilbage til mig når du har fundet to gaver. 

Oki dokki! #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

~ hc_firstTalkCalled = true
-> DONE

= FirstWaitStich
Led efter de lysende markører, de vil lede dig på rette vej. #speaker: H.C Andersen #portrait:dr_green_neutral #layout:right #audio:animal_crossing_mid
-> DONE


=== SecondTalk ===
Godt gået, Klods Hans! Du har fundet både mudder og en gammel træsko. #speaker: H.C Andersen #portrait:dr_green_neutral #layout:right #audio:animal_crossing_mid

Tak skal du have! Var der mere jeg har brug for? #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

Ja. Den sidste ting, du skal finde, ligger på toppen af en klippe. #speaker: H.C Andersen #portrait:dr_green_neutral #layout:right #audio:animal_crossing_mid

Men du har brug for noget at ride på for at komme op på toppen af klippen og til den lange tur hen til kongedatteren. 

Hmm. Hvad kan jeg mon bruge? #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

Jeg har en overraskelse til dig! Du kan få denne ged, som du kan bruge. #speaker: H.C Andersen #portrait:dr_green_neutral #layout:right #audio:animal_crossing_mid

En ged? Ej hvor en pragtfuld gave! Tusind tak! #speaker: Klods Hans #portrait:dr_green_neutral #layout:left #audio:animal_crossing_high

Haha det var så lidt. Held og lykke med at få kongedatteren! #speaker: H.C Andersen #portrait:dr_green_neutral #layout:right #audio:animal_crossing_mid

~ hc_secondTalkCalled = true
-> DONE

= SecondWaitStich
Oppe på toppen af en klippe, finder du den sidste ting. #speaker: H.C Andersen #portrait:dr_green_neutral #layout:right #audio:animal_crossing_mid
-> DONE

= ThirdWaitStich
Held og lykke med at få kongedatteren! #speaker: H.C Andersen #portrait:dr_green_neutral #layout:right #audio:animal_crossing_mid
-> DONE





