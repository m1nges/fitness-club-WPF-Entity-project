   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:fitness_club.Pages.AdminPagesfitness_club.Pages.AdminPages  5      9    3fitness_club.Pages.AdminPages.AdminWorkSchedulePage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =local+clr-namespace:fitness_club.Pages.AdminPages     5        N� Title$  AdminWorkSchedulePage��5	         GlobalPageBackground N� 
Background# [  5
      .��5      �� $��20q�6   .+�5   
   �� $0��*График работы тренеров��6   $��24(�5      $
��Bold"�5      
�������$��0 0 0 20q�5      $��Center=�5      �� 5   
    �� Orientation$ 
Horizontal=�6   $��0 0 0 20q�6.   .+�5      �� $��Тренер:��6   $��Center=�6'   $��0 0 5 0q�6B   
�������6S   �� 5      -   5         ��Name$ TrainerComboBox��5      $	��200��60   $��0 0 10 0q�6<    �� DisplayMemberPath$ FullName��6N    �� SelectedValuePath$ 	TrainerId��6k   �� 5      $��	Дата:��6   $��Center=�6%   $��0 0 5 0q�6@   
�������6Q   (  "System.Windows.Controls.DatePicker  5      -   5          Name$ WorkDatePicker��5      $��0 0 10 0q�61   �� 5      $	��С:��6   $��Center=�6"   $��0 0 5 0q�6=   
�������6N   �� 5      -   5         ��Name$ StartTimeBox��5      $��60��6,   $��0 0 10 0q�67    � ToolTip$ Формат: HH:mm��6I   �� 5!      $��До:��6   $��Center=�6#   $��0 0 5 0q�6>   
�������6O   �� 5"      -   5        $ 
EndTimeBox��5"      $��60��6*   $��0 0 10 0q�65   $ Формат: HH:mm��6G   �� 5$      -   5        $��Добавить��5$      $	��120��5%      $��35��5&      $��20 0 0 0q�5(      5)      �� 5,   
   
�������$��1q�6)   	 �� CornerRadius$	 8��6=   
�������
 �� Padding$
 10q�6a   ._�5-      �� -   5         ��Name$ ScheduleListView��5-      $��0q�61   
�������$	��300��6O    � 	Resources 5.       �� 
TargetType �� Property �� Value��%b  '��       �� 5/      " ��6   .�50      ��  � ��$ Center=�6G   �� 51       � ��$
 Bold"�67   �� 52       � ��$ 14(�65   �� 53       � �� �#7B68EE67   �� 54       � ��
 �����67   �� 55       � ��$ 5q�64   �� 56       � ��
 �����68   �� 57       � ��$ 0,0,0,1q�6<   58      59       �� View 5:      �� 5;      .P�5<      �� $��Тренер��6)    �� Width$	 200��6f    �� DisplayMemberBinding 69   �� *TrainerName+6f   �� 5=      $��Дата��6)   $	 120��6]    67   �� *Date+6]   �� 5>      $��Начало��6)   $	 100��6d    69   �� *	StartTime+6d   �� 5?      $��
Конец��6)   $	 100��6a    68   �� *	EndTime+6a   �� 5@      $��Действия��6)   $	 120��6;    �� CellTemplate 5A      �� 5B   "   .R�5C   &   �� -   5        $��Удалить��5C   -   ���#E57373
�������$	��6 2q�5H   -   $	��5 0q�5I   -   $��80��5J   -   $��Center=�5K   -   
������  � Cursor$
 Hand��5M   -   
 � Tag 5E   -   �� *WorkScheduleId+5M   -   5N   #   5P      5Q      5S      5T      5V      5W      