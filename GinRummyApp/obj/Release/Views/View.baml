   M S B A M L   `   `   `  ���� c  _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  C ?GinRummy, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:QUTQUT � \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8$prismhttp://prismlibrary.com/  XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity  localclr-namespace:QUT    � Title$  	Gin Rummy��$	��800��$
��1200�� � 	Resources   QUT.ViewModel   	ViewModel QUT.HiddenCardConverter  HiddenCardConverter QUT.VisibleCardConverter  VisibleCardConverter QUT.DeckPanel  	DeckPanel��%<   &	        &	      &	 
     &	                � DataContext ��  �� ResourceKey$ 	ViewModel��.��� ����  �� ImageSource$ ../Resources/green.jpg�� j� TileMode$
 Tile=� j� ViewportUnits$ Absolute=� j� Viewport$ 0,0,507,338� � RowDefinitions �� $��50 ��� $��* ��� $��* ��� $��* ��� $��50 �	 � ColumnDefinitions	 �� $��* �.   (System.Windows.Interactivity.Interaction
   Triggers
 F  @Prism.Interactivity.InteractionRequest.InteractionRequestTrigger  3   -System.Windows.Interactivity.EventTriggerBase   SourceObject �� *NotificationRequest+ �� Mode$ OneWay=�.   (System.Windows.Interactivity.TriggerBase   Actions. +  %Prism.Interactivity.PopupWindowAction  !   CenterOverAssociatedObject .    IsModal . .+���  �� Orientation$ 
Horizontal=�$��Center=�$��0���� $��1������  �� Path$ ComputerCards����� .R��� $��80��$��5q�����  �� 	Converter# [ ���� .R��� $ 
Horizontal=��� $ 
Horizontal=�$
��Left=�$��2��.+��� $	��100��$��20(�$	��100��$��100,10,20,10q����� *ButtonEndCommand+���� *ButtonEndContent+{��� *ButtonEndEnabled+�� $��Continue��$	��100��$��20(�$	��100��$��20,10,100,10q����� *ButtonContinueCommand+{��� *ButtonContinueEnabled+�� ���� $ Discards����� .R��� $��80������ # [ 
 /	   )System.Windows.Interactivity.EventTrigger	   	  	EventName$ 	MouseDown��. 6
   0System.Windows.Interactivity.InvokeCommandAction
   
  Command ��  �� Source# [  $$ TakeCardFromDiscardPileCommand� 
  CommandParameter �� ���� .R�  �� ���� $ RemainingDeck����� .R��� $��80������ # [ 
 	  $ 	MouseDown��. 
   �� # [  $ TakeCardFromDeckCommand� �� ���� .R�  �� $��3������ $ 
HumanCards����� .R��� $��80��$��5q����� # [ 
 	  $ 	MouseDown��. 
   �� # [  $  DiscardCardFromHandCommand� �� ���� .R��� $ 
Horizontal=��� $ 
Horizontal=�$��Center=�$��4��.+��� 
�������$��20(�$��Center=����� *HumanDeadwood+