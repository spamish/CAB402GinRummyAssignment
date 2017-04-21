module GinRummy

open Cards

// Check if higher cards exist
let rec NextCard (hand:Hand) (ranking:Cards.Rank) =
    let nextRank =
        match ranking with
            | Ace -> Cards.Two
            | Two -> Cards.Three
            | Three -> Cards.Four
            | Four -> Cards.Five
            | Five -> Cards.Six
            | Six -> Cards.Seven
            | Seven -> Cards.Eight
            | Eight -> Cards.Nine
            | Nine -> Cards.Ten
            | Ten -> Cards.Jack
            | Jack -> Cards.Queen
            | Queen -> Cards.King
            | _ -> Cards.Ace // NEED TO FIX THIS DIRTY DIRTY HACK

    if (nextRank.Equals Cards.Ace) then 0
    else
        let search = Seq.filter (fun x -> nextRank.Equals x.rank) hand

        if (Seq.isEmpty search) then 0
        else 1 + (NextCard hand nextRank)

// Check if lower cards exist
let rec PreviousCard (hand:Hand) (ranking:Cards.Rank) =
    let previousRank =
        match ranking with
            | Two -> Cards.Ace
            | Three -> Cards.Two
            | Four -> Cards.Three
            | Five -> Cards.Four
            | Six -> Cards.Five
            | Seven -> Cards.Six
            | Eight -> Cards.Seven
            | Nine -> Cards.Eight
            | Ten -> Cards.Nine
            | Jack -> Cards.Ten
            | Queen -> Cards.Jack
            | King -> Cards.Queen
            | _ -> Cards.King // NEED TO FIX THIS DIRTY DIRTY HACK TOO

    if (previousRank.Equals Cards.King) then 0
    else
        let search = Seq.filter (fun x -> previousRank.Equals x.rank) hand

        if (Seq.isEmpty search) then 0
        else 1 + (PreviousCard hand previousRank)

// Check if card is part of a run
let InRun (hand:Hand) (card:Card) =
    let sameSuit = Seq.filter (fun x -> card.suit.Equals x.suit) hand
    let counter = 1 + (NextCard sameSuit card.rank) + (PreviousCard sameSuit card.rank)

    if counter > 2 then true
    else false

// Check if card is part of a set
let InSet (hand:Hand) (card:Card) =
    let sameRank = Seq.filter (fun x -> card.rank.Equals x.rank) hand
    
    if (Seq.length sameRank) > 2 then true
    else false

// Return Pip value for a card
let PipValue (card:Card) =
    match card.rank with
        | Ace -> 1
        | Two -> 2
        | Three -> 3
        | Four -> 4
        | Five -> 5
        | Six -> 6
        | Seven -> 7
        | Eight -> 8
        | Nine -> 9
        | _ -> 10

let Deadwood (hand:Hand) = 
    let inRun = Seq.filter (fun x -> InSet hand x) hand
    let inSet = Seq.filter (fun x -> InRun hand x) hand


    0

let Score (firstOut:Hand) (secondOut:Hand) =
    0
    // Fixme change so that it computes how many points should be scored by the firstOut hand
    // (score should be negative if the secondOut hand is the winner)

// Add other functions related to Gin Rummy here ...