   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullO,clr-namespace:fitness_club.Pages.ClientPagesfitness_club.Pages.ClientPages  5      :    4fitness_club.Pages.ClientPages.ClientTrainerListPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      >local,clr-namespace:fitness_club.Pages.ClientPages     5         GlobalPageBackground  N� 
Background#  [  5       N� Title$ Наши тренеры��5	      .��5      �� $
��Auto=�6   .��5   
   �� $��20q�6   .+�5      �� $��Наши тренеры��6   $��24(�6,   $
��Bold"�6:   
�������$��0 0 0 20q�6_   $��Center=�6q   �� 5      -   5         ��Name$ TrainersItemsControl��5      ��5      �� 5      .R�5      �  K� IsItemsHost$
 True��6$   5      ��5      �� 5      .R�5      �� -   5        $��2q�5   !   ���#8A2BE2���#E0E0F8 �� CornerRadius$ 12��5   !   $��10q�5   !    �� Padding$ 10q�5   !   $	��220��5   !    � Cursor$
 Hand��5    !   ._�5"      �� .+�5#   "   �� $	��100��6A   $��0 0 0 10q�6N   $��UniformToFill=�6`   ��6(   �� *Photo+6`   �� 5$   "   $
��Wrap=�6e   $
��Bold"�6y   
���   ���6,   �� *
FullName+ �� StringFormat$  Имя тренера: {0}��6�   �� 5%   "   $��0 5 0 0q�6p   $
��Wrap=�6�   
���   ���6,   �� *AverageRating+$) #Средняя оценка: {0:F1}��6�   �� 5&   "   $
��Wrap=�6m   $��0 5 0 0q�6�   
���   ���6,   �� *Specialization+$% Специализация: {0}��6�   �� 5'   "   $
��Wrap=�6g   $��0 5 0 0q�6{   
���   ���6,   �� *
Experience+$  Стаж работы: {0}��6�   5(      5*      5+      5-      5.      5/      50      