   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:fitness_club.Pages.TrainerPagesfitness_club.Pages.TrainerPages  5   	   5    /fitness_club.Pages.TrainerPages.WritePlanWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ?local-clr-namespace:fitness_club.Pages.TrainerPages     5   	     � Title$6  0Создание плана тренировок��5   	   $	��450��6+   $	��800��68      GlobalPageBackground#��[  5	   	    � 	Resources 5       �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Padding �� Triggers	 P� Property
 P� Value��%�  '��       �� 5   
   " ��6   .�5      ��  � �� �#7B68EE6+   �� 5       � �� �#8A2BE26,   �� 5       � ��$ 2q�60   �� 5       � ��
 �����6+   �� 5       � ��$ 14(�6)   �� 5       � ��$
 Bold"�6+   �� 5       �@�Cursor$
 Hand��6'   �� 5       � ��$ 5q�6'   �� 5       � ��$ 6q�6(   �� 5       � ��$ 35��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   #��z��5   !   #��z��5   !   $ 8��5   !   # z��5   !   ._�5       �� $��Center=�6/   $��Center=�6L   5!       5"      P� 5#      	 �@J�IsMouseOver$

 True��6=   . �5$   "   ��  � �� �#6A5ACD6?   5%      P� 5&      	 � ��$

 True��6;   . �5'   "   ��  � �� �#5A4ABE6?   5(      P� 5)      	 � {�$
 False��6;   . �5*   "   ��  � �� �#FFB6B66?   �� 5+   "    � ��
 �����6?   5,      5.      50      51      52      .��54      � $��15q�6    � RowDefinitions 55   
   �� 56      $
��Auto �6   �� 57      $��* �6   �� 58      $
��Auto �6   .+�5;   
   �� -   5         ��Name$ headerTb��5;      $P��JВведите индивидуальный план тренировок:��6&   $��16(�5<      $
��Bold"�6"   $��0,0,0,10q�64   �� 5>   
   -   5         ��Name$ PlanTextBox��5>      $��1��5?       �� TextWrapping$
 Wrap=�5@       �� AcceptsReturn$
 True��5A      " �� VerticalScrollBarVisibility$
 Auto=�5B      $��14(�5C      �� 5E   
   $��2��6    �� Orientation$ 
Horizontal=�6"   $��Right=�5F      $��0,15,0,0q�61   .+�5G      �� -   5        $��Сохранить��5G      $	��120��6)   65   �� 5H      -   5        $��Отмена��5H      $	��100��6&   62   5I      5J      5K      