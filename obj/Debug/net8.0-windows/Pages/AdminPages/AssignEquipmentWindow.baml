   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:fitness_club.Pages.AdminPagesfitness_club.Pages.AdminPages  5   	   9    3fitness_club.Pages.AdminPages.AssignEquipmentWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   =local+clr-namespace:fitness_club.Pages.AdminPages     5   	     � Title$9  3Распределение оборудования��5   	   $	��450��6,   $	��800��69      GlobalPageBackground#��[  6E   .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� -   5         ��Name$ RemainingTextBlock��5      $=��7Доступно к распределению: N шт.��5      $��18(�5      $
��Bold"�5      
�������$��0 0 0 20q�5      $
��Left=�5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ HallGrid��5      $��1��6%      AutoGenerateColumns$ False��5         CanUserAddRows$ False��5         HeadersVisibility$ Column=�5      $��0 0 0 20q�5       
�������   RowBackground �#F0F0F0   AlternatingRowBackground �#E8E8E8
�������$��1q�5%      	   SelectionMode$	 Single=�5&      
   GridLinesVisibility$
 
Horizontal=�5'         Columns 5)      0  *System.Windows.Controls.DataGridTextColumn  5*      ,  &System.Windows.Controls.DataGridColumn   Header$ Зал��6%      
IsReadOnly$
 True��6O      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6a   1  +System.Windows.Controls.DataGridBoundColumn   Binding 62   �� *
HallName+6a     5+      $ Кол-во��6%   $	 120 6R    65   �� *
Quantity+6R   5-      �� 50   
   -   5        $��Сохранить��50      $��2��6%   $	��180��51      $��40��6   $��Center=�52      $��0 10 0 0q�53      54      55      56      