   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:fitness_club.Pages.TrainerPagesfitness_club.Pages.TrainerPages  5      8    2fitness_club.Pages.TrainerPages.TrainerReviewsPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:fitness_club.Pages.TrainerPages     5        N� Title$#  Отзывы клиентов��5	         GlobalPageBackground N� 
Background# [  5
       � 	Resources 5       �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value��%�  '��       �� 5   
   " ��6   .�5      ��  � ��$ 0 5 0 5q�6'   �� 5       � ��$ 10q�6(   �� 5       � �� �#E0E0F86+   �� 5       � �� �#8A2BE26,   �� 5       � ��$ 1q�60   �� 5       � ��$ Stretch=�6;   �� 5       � ��$ Center=�69    5      P� 5       �@��
IsSelected$
 True��60   . �5      ��  � �� �#7B68EE63   �� 5       � ��
 �   �63   5      5      5      .��5      � $��20q�6   .+�5    
   �� -   5        	 ��Name$	 	reviewsSP��5       $��0q�6(   .+�5!      �� $-��'Отзывы клиентов о вас��6   $��24(�5"      $
��Bold"�6&   $��0 0 0 10q�5#      
�������6*   �� 5%      -   5        
 ��Name$
 avgTrainerRating��5%      $,��&Ваша средняя оценка: ��5&      $��20(�5'      $��SemiBold"�6&   $��0 0 0 20q�5(      
�������6*   �� 5*      ���#5E03FC �� CornerRadius$ 10��6*    �� Padding$ 10q�6<   ._�5+      �� $
��Auto=�6   $	��400��6B   .��5,      �� -   5         ��Name$ reviewsListView��5,      $��0 5 0 0q�68   ��5-      �� 5.      .R�5/   "   �� 
�������$ 8��6<   $ 10q�6M   $��0 5 0 5q�6Z   ���#8A2BE2$��1q�6�   ._�50   &   ��  �� Orientation$ Vertical=�61   .+�51   *   �� $
��Bold"�6i   $��14(�6{   $
��Wrap=�6�   ��64   �� *
ClientName+ �� StringFormat$ Автор: {0}��6�   �� 52   *   $��0 5 0 0q�6l   $
��Wrap=�6}   ��64   �� *ReviewContent+$ Отзыв: {0}��6}   �� 53   *   $��0 5 0 0q�6k   $��SemiBold"�6|   ��64   �� *ReviewGrade+$ Оценка: {0}��6|   54   '   55   #   56      58      59      5:      5;      5<      5=      