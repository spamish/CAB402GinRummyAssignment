module GinRummy

open Cards

// Check if higher cards exist
let rec NextCard (hand:Hand) (ranking:Cards.Rank) =
    let nextRank =
        match ranking with
            | Ace -> Some Cards.Two
            | Two -> Some Cards.Three
            | Three -> Some Cards.Four
            | Four -> Some Cards.Five
            | Five -> Some Cards.Six
            | Six -> Some Cards.Seven
            | Seven -> Some Cards.Eight
            | Eight -> Some Cards.Nine
            | Nine -> Some Cards.Ten
            | Ten -> Some Cards.Jack
            | Jack -> Some Cards.Queen
            | Queen -> Some Cards.King
            | _ -> None

    let testRank = fun x -> x.rank = nextRank.Value

    // Return 0 if None or if next rank not in hand, increment and recurse otherwise
    if Option.isNone nextRank
    then 0
    elif Seq.isEmpty (Seq.filter testRank hand)
    then 0
    else 1 + (NextCard hand nextRank.Value)

// Check if lower cards exist
let rec PreviousCard (hand:Hand) (ranking:Cards.Rank) =
    
    let previousRank =
        match ranking with
            | Two -> Some Cards.Ace
            | Three -> Some Cards.Two
            | Four -> Some Cards.Three
            | Five -> Some Cards.Four
            | Six -> Some Cards.Five
            | Seven -> Some Cards.Six
            | Eight -> Some Cards.Seven
            | Nine -> Some Cards.Eight
            | Ten -> Some Cards.Nine
            | Jack -> Some Cards.Ten
            | Queen -> Some Cards.Jack
            | King -> Some Cards.Queen
            | _ -> None

    let testRank = fun x -> x.rank = previousRank.Value

    // Return 0 if None or if previous rank not in hand, increment and recurse otherwise
    if Option.isNone previousRank
    then 0
    elif Seq.isEmpty (Seq.filter testRank hand)
    then 0
    else 1 + (PreviousCard hand previousRank.Value)

// Check if card is part of a run
let InRun (hand:Hand) (card:Card) =
    let sameSuit = Seq.filter (fun x -> card.suit = x.suit) hand

    let counter = 1 + (NextCard sameSuit card.rank) + (PreviousCard sameSuit card.rank)

    if counter > 2
    then true
    else false

// Check if card is part of a set
let InSet (hand:Hand) (card:Card) =
    let sameRank = Seq.filter (fun x -> card.rank = x.rank) hand
    
    if (Seq.length sameRank) > 2
    then true
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
        
// Total pip values of a hand
let rec PipScore (hand:Hand) = 
    if Seq.isEmpty hand
    then 0
    else
        let score = PipValue (Seq.last hand)
        score + PipScore (Seq.truncate (Seq.length hand - 1) hand)

// Get cards in both hands
let GetIntersect (firstSeq:Hand) (secondSeq:Hand) =
    let firstSet = Set.ofSeq firstSeq
    let secondSet = Set.ofSeq secondSeq
    Set.intersect firstSet secondSet |> Set.toSeq

// Get cards from first hand but not in second hand
let GetDifference (firstSeq:Hand) (secondSeq:Hand) =
    let firstSet = Set.ofSeq firstSeq
    let secondSet = Set.ofSeq secondSeq
    Set.difference firstSet secondSet |> Set.toSeq

// Get cards that form runs
let GetInRun (sequence:Hand) =
    let searchFunction = fun x -> InRun sequence x
    Seq.filter searchFunction sequence

// Get cards that don't form runs
// REMOVE IN FUTURE
let GetNotInRun (sequence:Hand) =
    let searchFunction = fun x -> not (InRun sequence x)
    Seq.filter searchFunction sequence

// Get cards that form sets
let GetInSet (sequence:Hand) =
    let searchFunction = fun x ->  InSet sequence x
    Seq.filter searchFunction sequence

// Get cards that don't form sets
// REMOVE IN FUTURE
let GetNotInSet (sequence:Hand) =
    let searchFunction = fun x -> not (InSet sequence x)
    Seq.filter searchFunction sequence

// Take last card in both a set and run, then see which combination is better
let rec BestRun (both:Hand) (run:Hand) (set:Hand) (none:Hand) =
    // If there are no more conflicting cards, retun the cards from run that still form runs
    if Seq.isEmpty both
    then GetInRun run
    else
        // Create new runs and sets with the last card from both
        let lastBoth = Seq.singleton (Seq.last both)
        let newRun = Seq.append run lastBoth
        let newSet = Seq.append set lastBoth
        let newBoth = Seq.truncate (Seq.length both - 1) both

        // Comments for how the sets are manipulated are provided in the deadwood function

        // Recurse this function
        let alphaRun = BestRun newBoth newRun set none
        let betaRun = BestRun newBoth run newSet none

        // Create new sets
        let alphaSet = GetInSet (Seq.append set (GetDifference both alphaRun))
        let betaSet = GetInSet (Seq.append set (GetDifference both betaRun))
        
        // Find new deadwood cards
        let alphaNone = Seq.concat [ none; GetDifference run alphaRun; GetDifference set alphaSet ]
        let betaNone = Seq.concat [ none; GetDifference run betaRun; GetDifference set betaSet ]

        // Calculate deadwood scores
        let alphaScore = PipScore alphaNone
        let betaScore = PipScore betaNone

        // Return runs for best hand
        if alphaScore < betaScore
        then alphaRun
        else betaRun

let Deadwood (hand:Hand) =
    // Get cards in both runs and sets, cards in runs only, cards in sets only and cards in neither
    // REWRITE USING DIFFERENCE
    let inBoth = GetIntersect (GetInRun hand) (GetInSet hand)
    let inRun = GetIntersect (GetInRun hand) (GetNotInSet hand)
    let inSet = GetIntersect (GetNotInRun hand) (GetInSet hand)
    let inNone = GetIntersect (GetNotInRun hand) (GetNotInSet hand)

    // Find best possible deadwood score
    let finishedRun = BestRun inBoth inRun inSet inNone
    // final sets = cards that form sets from (cards in sets only + cards in both that are not in final runs)
    let finishedSet = GetInSet (Seq.append inSet (GetDifference inBoth finishedRun))
    // final deadwood cards = cards in nothing + cards not in final runs + cards not in final sets
    let finishedNone = Seq.concat [ inNone; GetDifference inRun finishedRun; GetDifference inSet finishedSet ]
    PipScore finishedNone
    
// Calculate the score of a game where a call for gin or undercut gives the winner a 25 point boost
// and the deadwood 
let Score (firstOut:Hand) (secondOut:Hand) =
    let boostScore = 25
    let firstScore = Deadwood firstOut
    let secondScore = Deadwood secondOut

    if firstScore = 0
    then boostScore + secondScore
    elif firstScore < secondScore
    then abs (firstScore - secondScore)
    else - abs (secondScore - firstScore) - boostScore

// Add other functions related to Gin Rummy here ...