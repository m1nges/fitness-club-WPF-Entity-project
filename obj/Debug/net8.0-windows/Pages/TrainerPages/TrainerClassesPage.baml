   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:fitness_club.Pages.TrainerPagesfitness_club.Pages.TrainerPages  5      8    2fitness_club.Pages.TrainerPages.TrainerClassesPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:fitness_club.Pages.TrainerPages     5        N� Title$  TrainerClassesPage��5	         GlobalPageBackground N� 
Background# [  5
       � 	Resources 5       �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value��%e  '��       �� 5   
   " ��6   .�5      ��  � ��$ 0 4 0 4q�6'   �� 5       � ��$ 0q�6(   �� 5       � ��$ Stretch=�6;   �� 5       � ��$ Center=�69    5      P� 5       �@��
IsSelected$
 True��60   . �5      ��  � �� �#7B68EE63   �� 5       � ��
 �   �63   5      5      5      .��5      � 	 � ColumnDefinitions	 5   
   �� 5      $��* �6   �� 5      $��* �6   
 � RowDefinitions
 5!   
   �� 5"      $
��Auto �6   �� 5#      $��* �6   .+�5&   
   �� $)��#Расписание занятий��6   $��20(�5'      $
��Bold"�6"   $��10q�5(      $��2��6    
�������5)      �� 5,   
   $��1��6   $��0��6"   $��10q�62   .+�5-      �� $1��+Индивидуальные занятия��6   $
��Bold"�5.      $��0 0 0 5q�5/      
�������50      �� 52      $	��600��6   $
��Auto=�6+   .��53      �� -   5         ��Name$ IndividualSessionsList��53      $��Stretch=�54      ��55      �� 56      .R�57      �� ���#8A2BE2$��1q�6;   
������� �� CornerRadius$ 8��6b    �� Padding$ 8q�6s   $	��0,4q�6   ._�58   "   � 	 59   &   �� 5:   *   $��* �6;   �� 5;   *   $
��Auto �6;   .+�5>   &   �� $��0��61   .+�5?   *   �� $
��Bold"�6O   ��64   �� *	ClassName+6O   �� 5@   *   ��64   �� *Date+ �� StringFormat$ Дата: {0:dd.MM.yyyy}���� 5A   *   ��64   �� *	StartTime+$ Начало: {0:hh\:mm}���� 5B   *   ��64   �� *	EndTime+$ Конец: {0:hh\:mm}���� 5C   *   ��64   �� *
ClientName+$ Клиент: {0}���� 5D   *   ��64   �� *
HallName+$ Зал: {0}��5E   '   �� 5G   &   $��1��61   $��Center=�6A   $��10 0 0 0q�6\   .+�5H   *   �� $
��Bold"�6d   ��64   �� *Price+$ {0} руб.��6d   5I   '   5J   #   5K      5L      5N      5O      5P      �� 5S   
   $��1��6   $��1��6"   $��10q�62   .+�5T      �� $'��!Групповые занятия��6   $
��Bold"�5U      $��0 0 0 5q�5V      
�������5W      �� 5Y      $	��600��6   $
��Auto=�6+   .��5Z      �� -   5        $ GroupSessionsList��5Z      $��Stretch=�5[      ��5\      �� 5]      .R�5^      �� ���#8A2BE2$��1q�6;   
�������$ 8��6b   $ 8q�6s   $	��0,4q�6   ._�5_   "   � 	 5`   &   �� 5a   *   $��* �6;   �� 5b   *   $
��Auto �6;   .+�5e   &   �� $��0��61   .+�5f   *   �� $
��Bold"�6O   ��64   �� *	ClassName+6O   �� 5g   *   ��64   �� *Date+$ Дата: {0:dd.MM.yyyy}���� 5h   *   ��64   �� *	StartTime+$ Начало: {0:hh\:mm}���� 5i   *   ��64   �� *	EndTime+$ Конец: {0:hh\:mm}���� 5j   *   ��64   �� *
HallName+$ Зал: {0}���� 5k   *   ��64   �� *VisitorsCount+$! Посетителей: {0}��5l   '   �� 5n   &   $��1��61   $��Center=�6A   $��10 0 0 0q�6\   .+�5o   *   �� $
��Bold"�6d   ��64   �� *Price+$ {0} руб.��6d   5p   '   5q   #   5r      5s      5u      5v      5w      5x      5y      