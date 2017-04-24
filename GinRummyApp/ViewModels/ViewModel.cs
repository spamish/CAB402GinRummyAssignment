using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.FSharp.Core;

namespace QUT
{
    class ViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<Cards.Card> HumanCards { get; private set; }
        public ObservableCollection<Cards.Card> ComputerCards { get; private set; }
        public ObservableCollection<Cards.Card> Discards { get; private set; }
        public ObservableCollection<Cards.Card> RemainingDeck { get; private set; }

        public InteractionRequest<INotification> NotificationRequest { get; private set; }
        public ICommand ButtonContinueCommand { get; set; }
        public ICommand ButtonEndCommand { get; set; }
        
        public ICommand DiscardCardFromHandCommand { get; set; }
        public ICommand TakeCardFromDiscardPileCommand { get; set; }
        public ICommand TakeCardFromDeckCommand { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private bool inGame;
        private bool humanTurn;
        private bool humanCardPickedUp;
        private int humanDeadwoodScore;
        private string humanDeadwood;

        private bool buttonContinueClickable;
        private bool buttonEndClickable;
        private string buttonEndText;

        private int humanScore;
        private int computerScore;

        public ViewModel()
        {
            TakeCardFromDiscardPileCommand = new DelegateCommand<Cards.Card>(HumanPickupFromDiscard, HumanCanPickup);
            TakeCardFromDeckCommand = new DelegateCommand<Cards.Card>(HumanPickupFromDeck, HumanCanPickup);
            DiscardCardFromHandCommand = new DelegateCommand<Cards.Card>(HumanDiscardFromHand, HumanCanDiscard);

            ButtonContinueCommand = new DelegateCommand(ButtonContinueClick);
            ButtonEndCommand = new DelegateCommand(ButtonEndClick);
            NotificationRequest = new InteractionRequest<INotification>();

            HumanCards = new ObservableCollection<Cards.Card>();
            ComputerCards = new ObservableCollection<Cards.Card>();
            Discards = new ObservableCollection<Cards.Card>();
            RemainingDeck = new ObservableCollection<Cards.Card>();

            HumanCards.CollectionChanged += HumanCards_CollectionChanged;

            inGame = false;
            ButtonContinueEnabled = false;
            ButtonEndEnabled = true;
            ButtonEndContent = "New Game";
        }

        // Can the human perform the actions?

        private bool HumanCanPickup(Cards.Card p)
        {
            if (humanTurn && !humanCardPickedUp)
            {
                humanCardPickedUp = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private bool HumanCanDiscard(Cards.Card arg)
        {
            if (humanTurn && humanCardPickedUp)
            {
                humanTurn = false;
                humanCardPickedUp = false;
                ButtonContinueEnabled = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        // Pickup and discard action for the human

        private void HumanPickupFromDiscard(Cards.Card p)
        {
            Discards.Remove(p);
            HumanCards.Add(p);
        }
        
        private void HumanPickupFromDeck(Cards.Card card)
        {
            RemainingDeck.Remove(card);
            HumanCards.Add(card);
        }

        private void HumanDiscardFromHand(Cards.Card p)
        {
            HumanCards.Remove(p);
            Discards.Add(p);
        }

        // Process the deadwood score display, button text and popup window
        
        async private void HumanCards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            HumanDeadwood = "Calculating ...";
            // this might take a while, so let's do it in the background
            int deadwood = await Task.Run(() => GinRummy.Deadwood(HumanCards));
            HumanDeadwood = "Deadwood: " + deadwood;
            humanDeadwoodScore = deadwood;

            if (inGame)
            {
                if (deadwood == 0)
                {
                    ButtonEndContent = "Gin";
                }
                else
                {
                    ButtonEndContent = "Knock";
                }
            }
        }

        public string HumanDeadwood
        {
            get
            {
                return humanDeadwood;
            }
            private set
            {
                humanDeadwood = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("HumanDeadwood"));
            }
        }

        public bool ButtonContinueEnabled
        {
            get
            {
                return buttonContinueClickable;
            }
            private set
            {
                buttonContinueClickable = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ButtonContinueEnabled"));
            }
        }

        public bool ButtonEndEnabled
        {
            get
            {
                return buttonEndClickable;
            }
            private set
            {
                buttonEndClickable = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ButtonEndEnabled"));
            }
        }

        public string ButtonEndContent
        {
            get
            {
                return buttonEndText;
            }
            private set
            {
                buttonEndText = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ButtonEndContent"));
            }
        }

        private void ButtonContinueClick()
        {
            humanTurn = false;
            ButtonContinueEnabled = false;
            ButtonEndEnabled = false;
            ComputerTurn();
        }

        private async void ButtonEndClick()
        {
            if (inGame)
            {
                int winnerScore = await Task.Run(() => GinRummy.Score(HumanCards, ComputerCards));

                if (humanDeadwoodScore == 0)
                {
                    VictoryConditions(winnerScore, true, true);
                }
                else
                {
                    VictoryConditions(winnerScore, true, false);
                }
            }
            else
            {
                humanScore = 0;
                computerScore = 0;

                Deal();
            }
        }

        private void RaiseNotification(string msg, string title)
        {
            NotificationRequest.Raise(new Notification { Content = msg, Title = title });
        }
        
        private void VictoryConditions(int score, bool humanCalled, bool ginCalled)
        {
            string message = "";
            string title = "";
            bool humanWins;
            bool continueGame = true;
            inGame = false;

            if (score > 0)
            {
                humanWins = humanCalled ? true : false;
                humanScore += humanCalled ? score : 0;
                computerScore += humanCalled ? 0 : score;
            }
            else
            {
                score = -score;
                humanWins = humanCalled ? false : true;
                humanScore += humanCalled ? 0 : score;
                computerScore += humanCalled ? score : 0;
            }

            if ((humanScore > 99) || (computerScore > 99))
            {
                continueGame = false;
            }

            title += (humanWins ? "Human " : "Computer ")
                + (continueGame ? "wins this round." : "claims victory!");

            message += (humanCalled ? "Human" : "Computer") + " called "
                + (ginCalled ? "gin" : "knock") + ".\n"
                + (humanWins ? "Human" : "Computer") + " scored " + score + " points."
                + "\nHuman on " + humanScore + " points."
                + "\nComputer on " + computerScore + " points."
                + (continueGame ? "" : "\nGame over!");

            RaiseNotification(message, title);

            if (continueGame)
            {
                Deal();
            }
            else
            {
                ButtonContinueEnabled = false;
                ButtonEndEnabled = true;
                ButtonEndContent = "New Game";
            }
        }

        // Reset buttons and deal cards for a new game
        
        private async void Deal()
        {
            inGame = true;
            ButtonContinueEnabled = false;
            ButtonEndEnabled = false;

            HumanCards.Clear();
            ComputerCards.Clear();
            Discards.Clear();
            RemainingDeck.Clear();
            
            var deck = Cards.Shuffle(Cards.FullDeck);

            foreach (var card in deck)
            {
                RemainingDeck.Add(card);
                await Task.Delay(1);
            }

            for (int i = 0; i < 10; i++)
            {
                ComputerCards.Add(DrawTopCardFromDeck());
                await Task.Delay(30);
                HumanCards.Add(DrawTopCardFromDeck());
                await Task.Delay(30);
            }

            Discards.Add(DrawTopCardFromDeck());

            humanTurn = true;
            humanCardPickedUp = false;
            ButtonContinueEnabled = false;
            ButtonEndEnabled = true;
        }

        private Cards.Card DrawTopCardFromDeck()
        {
            var top = RemainingDeck[RemainingDeck.Count - 1];
            RemainingDeck.Remove(top);
            return top;
        }

        // Process computer turn, pickup and discard cards

        private async void ComputerTurn()
        {
            await Task.Run(() => GinRummy.Deadwood(HumanCards));

            var topDiscard = Discards[Discards.Count - 1];
            var topDeck = RemainingDeck[RemainingDeck.Count - 1];
            bool pickupDiscard = await Task.Run(() => ComputerPlayer.ComputerPickupDiscard(ComputerCards, topDiscard, RemainingDeck));
            if (pickupDiscard)
            {
                ComputerPickupFromDiscard(topDiscard);
            }
            else
            {
                ComputerPickupFromDeck(topDeck);
            }

            await Task.Delay(30);
            var move = await Task.Run(() => ComputerPlayer.ComputerMove(ComputerCards));
            
            var discard = move.Item2;
            if (FSharpOption<Cards.Card>.get_IsSome(discard))
            {
                ComputerDiscardFromHand(discard.Value);
            }

            await Task.Delay(30);
            var action = move.Item1;
            if (action.IsGin)
            {
                int winnerScore = await Task.Run(() => GinRummy.Score(ComputerCards, HumanCards));
                VictoryConditions(winnerScore, false, true);
            }
            else if (action.IsKnock)
            {
                int winnerScore = await Task.Run(() => GinRummy.Score(ComputerCards, HumanCards));
                VictoryConditions(winnerScore, false, false);
            }
            else
            {
                humanTurn = true;
                ButtonContinueEnabled = false;
                ButtonEndEnabled = true;
            }
        }

        private void ComputerPickupFromDiscard(Cards.Card p)
        {
            Discards.Remove(p);
            ComputerCards.Add(p);
        }

        private void ComputerPickupFromDeck(Cards.Card card)
        {
            RemainingDeck.Remove(card);
            ComputerCards.Add(card);
        }

        private void ComputerDiscardFromHand(Cards.Card p)
        {
            ComputerCards.Remove(p);
            Discards.Add(p);
        }
    }
}
