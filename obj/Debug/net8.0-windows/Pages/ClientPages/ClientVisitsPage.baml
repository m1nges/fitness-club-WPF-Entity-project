   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullO,clr-namespace:fitness_club.Pages.ClientPagesfitness_club.Pages.ClientPages  5      5    /fitness_club.Pages.ClientPages.ClientVisitsPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      >local,clr-namespace:fitness_club.Pages.ClientPages     5         GlobalPageBackground  N� 
Background#  [  5       N� Title$6 0История посещений занятий��5	      .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $4��.Мои посещения тренировок��6   $��20(�64   $
��Bold"�6B   
�������$��0 0 0 10q�6g   �� 5   
   $��1��6   $
��Auto=�6$   $��Disabled=�6G   $	��850��6p   .��5      �� -   5         ��Name$ ClientVisitsList��5       �� SelectionMode$ Single=�6f   ��5      �� 5       �� 
TargetType" ��6   .�5      ��  �� Property � �� �� Value$ Stretch=�6G   �� 5       � ��$ 0q�64   �� 5       � ��$	 0 5q�63   �� 5       � ��
 ���� 67    �� Triggers 5      P� 5      	 P� Property	 �@��
IsSelected
 P� Value$

 True��6<   . �5   "   ��  � �� �#7B68EE6?   �� 5   "    � ��
 �   �6?   5      5!      ��5$      �� 5%      .R�5&      �� ���#8A2BE2$��2q�67    �� CornerRadius$ 8��6K   ���#E0E0F8 �� Padding$ 10q�6q   $	��5 2q�6~    � ContextMenu 5'      �� 5(   "   .@�5)   &   m� -   5        $%��Вернуть средства��5)   /    m� CommandParameter 6j   �� *	ClassId+5*   #   ._�5-      �  � ColumnDefinitions 5.   "   �� 5/   &   $��* �67   �� 50   &   $
��Auto �67   .+�53   "   �� $��0��6-   $��0 0 10 0q�6=   .+�54   &   �� $
��Bold"�6K   $��14(�6]   
���   ���60   �� *	ClassName+6k   �� 56   &    �� Orientation$ 
Horizontal=�61   $��0,5,0,0q�6J   .+�57   *   �� 
���   ���64   �� *Date+ �� StringFormat$! Дата: {0:dd.MM.yyyy} | ��6r   �� 58   *   
���   ���64   �� *Time+$ Время: {0} | ��6h   �� 59   *   
���   ���64   �� *
HallName+$ Зал: {0}��6g   5:   '   �� 5<   &   $��0,5,0,0q�6i   
���   ���60   �� *TrainerName+$ Тренер: {0}��6z   5=   #   �� 5?   "   $��1��6-   $	��Top=�6=   $��Right=�6U   .+�5@   &   �� $
��Bold"�6g   
���   ���60   �� *	Visited+$ Посещено: {0}��6y   �� 5A   &   $
��Bold"�6b   
���   ���60   �� *Price+$ {0} руб.��6t   5B   #   5C      5D      5E      5G      5H      5I      5J      