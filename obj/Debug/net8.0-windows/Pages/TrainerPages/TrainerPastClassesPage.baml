   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:fitness_club.Pages.TrainerPagesfitness_club.Pages.TrainerPages  5      <    6fitness_club.Pages.TrainerPages.TrainerPastClassesPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:fitness_club.Pages.TrainerPages     5        N� Title$+  %Проведённые занятия��5	         GlobalPageBackground N� 
Background# [  5
       � 	Resources 5       �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value��%�  '��       '�� 2    �� 5   
   " ��6   .�5      ��  � �� �#5E03FC6+   �� 5       � �� �#8A2BE26,   �� 5       � ��$ 2q�60   �� 5       � ��
 �����6+   �� 5       � ��$ 14(�6)   �� 5       � ��$ 5q�6(   �� 5       � ��$ 0 0 0 10q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$ 0 5 0 5q�6'   �� 5       � ��$ 5q�6(   �� 5       � �� �#E0E0F86+   �� 5       � �� �#8A2BE26,   �� 5       � ��$ 1q�60   �� 5       � ��$ Stretch=�6;    5       P� 5!       �@��
IsSelected$
 True��60   . �5"      ��  � �� �#7B68EE63   �� 5#       � ��
 �   �63   5$      5&      5'      .��5)      �� $��Hidden=�6   .��5*   
   �� $��20q�6   .+�5+      �� $+��%Проведенные занятия��6   $��20(�5,      $
��Bold"�6&   $��0 0 0 10q�5-      
�������5.      �� 50      -   5        	 ��Name$	 FilterComboBox��50      $	��200��52      .@�53      �� $��Все��6   �� 54      $'��!Только отмеченные��6   �� 55      $+��%Только неотмеченные��6   56      �� 58      -   5        
 ��Name$
 PastClassesListView��58      $��Stretch=�5:      ��5;      �� 5<      .R�5=      �� ���#8A2BE2$��1q�67    �� CornerRadius$ 8��6K    �� Padding$ 8q�6\   
�������$	��0,5q�6{   ._�5>      �� .+�5?   "   �� $
��Bold"�6G   $��14(�6Y   $
��Wrap=�6g   ��6,   �� *	ClassName+6g   �� 5@   "   $��0 2 0 0q�6g   ��6,   �� *Date+ �� StringFormat$ Дата: {0:dd.MM.yyyy}��6g   �� 5A   "   $��0 2 0 0q�6k   ��6,   �� *	StartTime+$ Начало: {0:hh\:mm}��6k   �� 5B   "   $��0 2 0 0q�6h   ��6,   �� *	EndTime+$ Конец: {0:hh\:mm}��6h   �� 5C   "   $��0 2 0 0q�6_   ��6,   �� *
HallName+$ Зал: {0}��6_   �� 5D   "   $��0 2 0 0q�6b   ��6,   �� *Price+$ Цена: {0} руб.��6b   5E      5F      5G      5I      5J      5K      5L      