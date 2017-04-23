module ComputerPlayer

open Cards
open GinRummy

type Move = Gin | Knock | Continue
        
// Total pip values of a hand
let rec AverageScore (computer:Hand) (possible:Hand) = 
    if Seq.isEmpty possible
    then 0
    else
        let score = Deadwood (Seq.append computer (Seq.singleton (Seq.last possible)))
        score + AverageScore computer (Seq.truncate (Seq.length possible - 1) possible)

let ComputerPickupDiscard computerHand topDiscard possibleDeck =
    let pickupDiscard = Deadwood (Seq.append computerHand (Seq.singleton topDiscard))
    let pickupRemaining = (AverageScore computerHand possibleDeck) / (Seq.length possibleDeck)

    if pickupDiscard < pickupRemaining
    then true
    else false

let WithoutCard (hand:Hand) (card:Card) =
    let length = Seq.length hand
    let findIndex = fun x -> x = card
    let index = Seq.findIndex findIndex hand

    // Gather all cards in the sequence before the card
    let first =
        if index < 0
        then Seq.empty
        else Seq.truncate index hand
    
    // Gather all cards in the sequence after the card
    let last =
        if index >= length
        then Seq.empty
        else Seq.skip (index + 1) hand

    // Append first and last sequences
    Seq.append first last

let ComputerMove newHand =
    let deadwoodScore = Deadwood newHand

    // Check if deadwood score is 0
    if deadwoodScore = 0
    then (Gin, None)
    else
        // Find scores if each card is removed from the hand
        let deadwoodScores hand = fun x -> Deadwood (WithoutCard hand x)
        let possibleScores = Seq.map (deadwoodScores newHand) newHand

        // Find the card to remove which results in the lowest deadwood score
        let findMinimum options = fun x -> x = (Seq.min options)
        let bestDiscard = Seq.findIndex (findMinimum possibleScores) possibleScores
        let discard = Seq.nth bestDiscard newHand

        // Calculate new deadwood score and pick action to take
        let newScore = Deadwood (WithoutCard newHand discard)
        if newScore = 0
        then (Gin, Some discard)
        elif newScore <= 10
        then (Knock, Some discard)
        else (Continue, Some discard)

