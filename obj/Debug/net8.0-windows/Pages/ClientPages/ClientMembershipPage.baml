   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullO,clr-namespace:fitness_club.Pages.ClientPagesfitness_club.Pages.ClientPages  5      9    3fitness_club.Pages.ClientPages.ClientMembershipPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      >local,clr-namespace:fitness_club.Pages.ClientPages     5         GlobalPageBackground  N� 
Background#  [  5       N� Title$ ClientMembershipPage��5
      .��5      �  � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   �� -   5         ��Name$ membershipSp��5   6   $��0��6   $��2��6"   $��10q�6L   .+�5      �� $#��Ваши абонементы��6   $��20(�6/   $
��Bold"�6=   $��0 0 0 10q�6O   &   System.Windows.Controls.DataGrid  5      -   5          Name$ MembershipGrid��5         AutoGenerateColumns$ False��5         
IsReadOnly$
 True��5      $��0 0 0 0q�5         HeadersVisibility$ Column=�5      	   CanUserAddRows$	 False��5      $	��250��5      
   RowBackground
 �#E0E0F8   AlternatingRowBackground �#D6D6F5   GridLinesVisibility$
 None=�5!      ���#8A2BE2$��1q�5#         RowHeaderWidth$ 0Q�5$       � 	Resources 5%      =  7System.Windows.Controls.Primitives.DataGridColumnHeader �� 
TargetType �� Property �� Value��%�   '        �� 5&      "  6   .�5'      ��  � �� �#7B68EE67   �� 5(       � ��
 �   �67   �� 5)       � ��$
 Bold"�67   �� 5*       � ��$ 5q�64   5+      5,         Columns 5-      0  *System.Windows.Controls.DataGridTextColumn  5.      ,  &System.Windows.Controls.DataGridColumn   Header$ Название��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6^   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6;   �� *MembershipName+6^     5/      $ Описание��6)   $ * 6e    6;   �� *MembershipDescription+6e     50      $ Тип��6)   $	 100 6Y    66   �� *MembershipType+6Y     51      $ Дата начала��6)   $	 120 6>    6J   �� *	StartDate+ �� StringFormat$ 
dd.MM.yyyy��  52      $! Дата окончания��6)   $	 120 6A    6M   �� *	EndDate+$ 
dd.MM.yyyy��  53      $ Цена��6)   $ 80 6Q    67   �� *Price+6Q     54      $ Статус��6)   $	 100 6T    69   �� *Status+6T   56      57      �� 5:   
   -   5        $ buyMembershipSp��5:   ,    �� Orientation$ Vertical=�6   $��0��6E   $��1��6U   $��10 0 0 0q�6b   .+�5;      �� -   5         ��Name$ buyMembershipHeaderTb��5;      $%��Купить абонемент��67   $��0 0 0 10q�6O   $
��Bold"�6a   $��20(�6s   �� 5<      $D��>Выберите длительность абонемента��6   $��0 0 0 10q�6@   $
��Bold"�6R   $��14(�6d   �� 5=      -   5         ��Name$ membershipDurationCmb��5=      $	��300��6p   $
��Left=�6|   �� 5>      $)��#Выберите абонемент��6   $��	0 10 0 10q�62   $
��Bold"�6E   $��14(�6W   �� 5@      $
��Auto=�6   $��Disabled=�6>   $	��220��6g   .��5A      �� -   5         ��Name$ membershipList��5A       �� SelectionMode$ Single=�6f   $��Disabled=�6}   ��5B      �� 5C      " ��6    .�5D      ��  � ��$ Stretch=�6K   �� 5E       � ��$ 0q�68   �� 5F       � ��$	 0 5q�67   �� 5G       � ��
 ���� 6;    �� Triggers 5H      P� 5I   "    P� Property �@��
IsSelected P� Value$
 True��6@   . �5J   &   ��  � �� �#7B68EE6C   �� 5K   &    � ��
 �   �6C   5L   #   5N      ��5Q      �� 5R      .R�5S      �� ���#8A2BE2$��2q�6;    �� CornerRadius$ 8��6O   ���#E0E0F8  �� Padding$  10q�6u   $	��5 2q�6�   ._�5T   "   �  5U   &   �� 5V   *   $��* �6;   �� 5W   *   $
��0.3* �6;   .+�5Y   &   �� $��0��61   $��0 0 10 0q�6A   .+�5Z   *   �� $
��Bold"�6T   $��14(�6f   
���   ���64   �� *MembershipName+6t   �� 5[   *   $
��Wrap=�6y   
���   ���64   �� *MembershipDescription+$ Описание: {0}��6�   5\   '   �� 5]   &   $��1��61   $��Center=�6A   $��Right=�6\   .+�5^   *   �� $
��Bold"�6f   
���   ���64   �� *Price+$ {0} руб.��6x   5_   '   5`   #   5a      5b      5d      5e      �� 5g      -   5        $ costTb��5g      $ ��К оплате: 0 руб.��6(   $��0 10 0 0q�6@   $y�	Collapsed=�6R   �� 5h      -   5        ! ��Name$! buyMemembershipBtn��5h      $��Оплатить��61   $	��250��6D   $��0 20 0 0q�6q   5i      5j      5k      