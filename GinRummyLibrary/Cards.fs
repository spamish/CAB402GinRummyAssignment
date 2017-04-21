module Cards

type Suit = Spades | Clubs | Hearts | Diamonds
type Rank = Ace | Two | Three | Four | Five | Six | Seven | Eight | Nine | Ten | Jack | Queen | King
type Card = { suit: Suit; rank: Rank}

type Hand = Card seq
type Deck = Card seq

let AllSuits = [ Spades; Clubs; Hearts; Diamonds ]
let AllRanks = [ Ace; Two; Three; Four; Five; Six; Seven; Eight; Nine; Ten; Jack; Queen; King ]

let allCards = 
    seq { 
        for s in AllSuits do
            for r in AllRanks do
                yield {suit=s; rank=r}
    }

let FullDeck = 
    allCards

let random = System.Random()

let rec Shuffle (deck:Deck) = 
    // Exit if empty deck, if only one element will shuffle and return that
    if Seq.isEmpty deck
    then deck
    else
        // Get the length of the sequence and extract a random element
        let length = Seq.length deck
        let index = random.Next(0, length)
        let value = Seq.singleton (Seq.nth index deck)

        // Gather all elements in the sequence before the random element
        let first =
            if index < 0
            then Seq.empty
            else Seq.truncate index deck

        // Gather all elements in the sequence after the random element
        let last =
            if index >= length
            then Seq.empty
            else Seq.skip (index + 1) deck

        // Append random element to front and shuffle remaining sequence
        let remainder = Seq.append first last
        Seq.append value (Shuffle remainder)

// Add other functions here related to Card Games ...

let CheckDuplicates (cards:Deck) =
    let duplicates =
        cards
        |> Seq.groupBy id
        |> Seq.map snd
        |> Seq.exists (fun s -> (Seq.length s) > 1)
    if duplicates
    then raise (new System.Exception "duplicates found!");