   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:fitness_club.Pages.AdminPagesfitness_club.Pages.AdminPages  5   	   8    2fitness_club.Pages.AdminPages.EditGroupClassWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   =local+clr-namespace:fitness_club.Pages.AdminPages     5   	     � Title$  EditGroupClassWindow��5   	   $	��450��6&   $	��800��63      GlobalPageBackground#��[  5	   	   .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $	��Top=�6"   $��Stretch=�6:   $��0 0 0 20q�6X   .+�5      �� $F��@Редактирование группового занятия��6   $��22(�5      $
��Bold"�6&   
�������$��0 0 0 20q�6K   $��Center=�5      �� 5       �� Orientation$ 
Horizontal=�6   $
��0 10q�62   $��Center=�6@   .+�5      �� $��Тренер:��6   $	��120��6+   $��16(�67   
�������$��Center=�6X   �� 5      -   5         ��Name$ TrainerComboBox��5      $	��250��64    �� DisplayMemberPath$ FullName��5       �� SelectedValuePath$ 	TrainerId��68   5      �� 5      $ 
Horizontal=�6   $
��0 10q�62   $��Center=�6@   .+�5       �� $��	Дата:��6   $	��120��6)   $��16(�65   
�������$��Center=�6V   (  "System.Windows.Controls.DatePicker  5!      -   5          Name$ 
DatePicker��5!      $	��180��61   5"      �� 5$      $ 
Horizontal=�6   $
��0 10q�62   $��Center=�6@   .+�5%      �� $��Начало:��6   $	��120��6+   $��16(�67   
�������$��Center=�6X   �� 5&      -   5         ��Name$ StartTimeBox��5&      $	��100��60   $��Center=�6<    � ToolTip$ Формат: HH:mm��6`   �� 5(      $��Конец:��6   $��20 0 5 0q�6*   $��16(�6<   
�������$��Center=�6]   �� 5)      -   5        $ 
EndTimeBox��5)      $	��100��6.   $��Center=�6:   $ Формат: HH:mm��6^   5*      5,      �� 5/   
   -   5        $��1��5/      $+��%Сохранить изменения��6   $	��200��50      $��40��6   $��0 20 0 0q�51      $��Center=�52      $��Center=�53      54      55      56      