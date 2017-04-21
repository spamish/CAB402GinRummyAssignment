module ComputerPlayer

open Cards
open GinRummy

type Move = Gin | Knock | Continue
    
let ComputerPickupDiscard computerHand topDiscard possibleDeck =
    true
    // Fixme: change function so that it computes if Computer should pickup from Discard pile 
    //        or draw a fresh card from the deck

let ComputerMove newHand =
    let card = Seq.head newHand
    (Continue, Some card)
    // Fixme: change function so that it computes which action the Computer should take: Continue, Knock or Gin 
    //        and which card would be best to discard

