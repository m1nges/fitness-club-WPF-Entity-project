   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:fitness_club.Pages.TrainerPagesfitness_club.Pages.TrainerPages  5      ;    5fitness_club.Pages.TrainerPages.ClassVisitorsListPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:fitness_club.Pages.TrainerPages     5        N� Title$'  !Участники занятия��5	         GlobalPageBackground N� 
Background# [  5
       � 	Resources 5       �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Padding	 �� Triggers
 P� Property P� Value&   System.Windows.Controls.DataGrid��%  '��       '  �    �� 5   
   " ��6   .�5      ��  � �� �#7B68EE6+   �� 5       � �� �#8A2BE26,   �� 5       � ��$ 2q�60   �� 5       � ��
 �����6+   �� 5       � ��$ 14(�6)   �� 5       � ��$
 Bold"�6+   �� 5       �@�Cursor$
 Hand��6'   �� 5       � ��$ 5q�6'   �� 5       � ��$ 6q�6(   �� 5       � ��$ 35��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   #��z��5   !   #��z��5   !   $ 8��5   !   # z��5    !   ._�5!      �� $��Center=�6/   $��Center=�6L   5"      	 5#      P� 5$      
 �@J�IsMouseOver$
 True��6=   . �5%   "   ��  � �� �#6A5ACD6?   5&      P� 5'      
 � ��$
 True��6;   . �5(   "   ��  � �� �#5A4ABE6?   5)      P� 5*      
 � {�$ False��6;   . �5+   "   ��  � �� �#FFB6B66?   �� 5,   "    � ��
 �����6?   5-      5/      51      52      �� 55   
   "  6   .�56      ��  � �� �#E0E0F86+   �� 57       � �� �#8A2BE26,   �� 58       � ��$ 2q�60   �� 59       � ��$ 14(�6)   �� 5:       � ��
 �   �6+   �� 5;       �@ RowBackground �#D8BFD86.   �� 5<        �@ AlternatingRowBackground �#E6E6FA69   �� 5=       �@ HeadersVisibility$ Column=�62   �� 5>        �@ HorizontalGridLinesBrush �#8A2BE269   �� 5?       �@ VerticalGridLinesBrush �#8A2BE267   5@      5A      .��5C      �� $��20q�6    �� Orientation$ Vertical=�6   .+�5D   
   �� -   5         ��Name$ classNameTb��5E      $'��!Участники занятия��5D      $��20(�5F      $
��Bold"�6"   
�������$��0 0 0 10q�5H        5J   
   -   5          Name$ VisitorsDataGrid��5J         AutoGenerateColumns$ False��5K         CanUserAddRows$ False��5L         	RowHeight$ 30Q�5M      $��0 0 0 10q�5N         Columns 5O      0  *System.Windows.Controls.DataGridTextColumn  5P      ,  &System.Windows.Controls.DataGridColumn   Header$ ФИО��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 5R   %      
IsReadOnly$
 True��5S   %   1  +System.Windows.Controls.DataGridBoundColumn   Binding 5Q   %   �� *
FullName+5S   %   4  .System.Windows.Controls.DataGridCheckBoxColumn  5T      $ Пришел��6)   $ * 5U   R    6)   �� *	Visited+ �� Mode$ TwoWay=�6R   5W      �� 5Y   
   $ 
Horizontal=�6   $��Center=�6.   $��0 20 0 0q�6K   .+�5Z      �� -   5        $��Сохранить��5Z      $	��140��5\      �� 5^      -   5        $��
Назад��5^      $	��140��5`      5a      5b      5c      