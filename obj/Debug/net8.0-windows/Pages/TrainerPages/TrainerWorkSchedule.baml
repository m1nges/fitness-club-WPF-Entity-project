   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:fitness_club.Pages.TrainerPagesfitness_club.Pages.TrainerPages  5      9    3fitness_club.Pages.TrainerPages.TrainerWorkSchedule   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:fitness_club.Pages.TrainerPages     5        N� Title$.  (График работы тренера��5	         GlobalPageBackground N� 
Background# [  5
       � 	Resources 5      &   System.Windows.Controls.DataGrid �� 
TargetType �� Property �� Value��%(  '        �� 5   
   "  6   .�5      ��  � �� �#E0E0F86+   �� 5       � �� �#8A2BE26,   �� 5       � ��$ 2q�60   �� 5       � ��$ 14(�6)   �� 5       � ��
 �   �6+   �� 5       �@ RowBackground �#D8BFD86.   �� 5        �@ AlternatingRowBackground �#E6E6FA69   �� 5       �@ HeadersVisibility$ Column=�62   �� 5        �@ HorizontalGridLinesBrush �#8A2BE269   �� 5       �@ VerticalGridLinesBrush �#8A2BE267   5      5      .��5      �� $��20q�6   .+�5   
   �� $@��:Ваш график на ближайшие 2 недели��6   $��20(�5      $
��Bold"�6"   $��0 0 0 15q�5      
�������5         5"   
   -   5          Name$ TrainerScheduleGrid��5"         AutoGenerateColumns$ False��5#         
IsReadOnly$
 True��5$      	   CanUserAddRows$	 False��5%      $	��400��5&      
   Columns
 5(      0  *System.Windows.Controls.DataGridTextColumn  5)      ,  &System.Windows.Controls.DataGridColumn   Header$ Дата��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$	 150 6i   1  +System.Windows.Controls.DataGridBoundColumn   Binding 63   �� *
WorkDate+ �� StringFormat$ 
dd.MM.yyyy��6i     5*      $ Начало��6%   $	 100 6S    65   �� *	StartTime+6S     5+      $ 
Конец��6%   $	 100 6P    64   �� *	EndTime+6P   5-      5.      5/      