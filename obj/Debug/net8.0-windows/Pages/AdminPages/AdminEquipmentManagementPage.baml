   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:fitness_club.Pages.AdminPagesfitness_club.Pages.AdminPages  5      @    :fitness_club.Pages.AdminPages.AdminEquipmentManagementPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =local+clr-namespace:fitness_club.Pages.AdminPages     5        N� Title$"  AdminEquipmentManagementPage��5	         GlobalPageBackground N� 
Background# [  5
      .��5      �� $��20q�6    �� Orientation$ Vertical=�6   .+�5   
   �� $5��/Управление оборудованием��6   $��24(�64   $
��Bold"�6B   
�������$��0 0 0 20q�6g   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ EquipmentGrid��5         AutoGenerateColumns$ False��5      $	��350��5         CanUserAddRows$ False��5         
IsReadOnly$ False��5      
�������   GridLinesVisibility$ 
Horizontal=�5      ���#8A2BE2$��1q�5      $��0 0 0 20q�5         Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn	   Header$	 Название��6%   
   
IsReadOnly$

 True��6Y      Width5  /System.Windows.Controls.DataGridLengthConverter$	 180 6k   1  +System.Windows.Controls.DataGridBoundColumn   Binding 67   �� *EquipmentName+6k   4  .System.Windows.Controls.DataGridComboBoxColumn  5      $	 Состояние��6)      SelectedValuePath$ EquipmentConditionId��5   )      DisplayMemberPath$# EquipmentConditionDescription��5   )   $	 150 5    )   -   5        ConditionColumn��   SelectedValueBinding 5   )   �� *EquipmentConditionId+5!   )     5"      $	 Поставка��6%   $

 True��6X   $	 120 6j    67   �� *DeliveryDate+6j     5#      $
	 ТО��6%   $	 120 6Y    61   �� *LastMaintenanceDate+6Y     5$      $	 Кол-во��6%   $ 80 6R    65   �� *
Quantity+6R   4  .System.Windows.Controls.DataGridTemplateColumn  5&      $ 	 Распределение��6)   $	 180 6@      CellTemplate 5'      �� 5(      .R�5)      �� -   5        $��Открыть окно��5)   %   $��4q�6<   $��Center=�6H   
 � Tag 5*   C   �� *EquipmentId+5+      5-        5/      $	 Удалить��6)   $	 100 6:    50      �� 51      .R�52      �� -   5        $	��❌��52   %   $��4q�61   $��Center=�6=    53   C   �� *EquipmentId+54      56      59      �� 5;   
   -   5        $+��%Сохранить изменения��5;      $	��200��6/   $��8q�6;   $
��Left=�6a   �� 5=   
   $��
20 20 0 20q�6   �� 5?   
   $��0 10 0 0q�6   $ Vertical=�6'   .+�5@      �� $:��4Добавить новое оборудование��6   $��20(�6;   $
��Bold"�6I   
�������$��0 0 0 20q�6n   �� 5B      $ 
Horizontal=�6   $��0 0 0 10q�62   .+�5C      �� $��Название:��6   $��Center=�6-   
�������$��0 0 5 0q�6[   �� 5D      -   5         ��Name$ NewEquipmentNameTb��5D      $	��200��66   $��0 0 20 0q�6B   $��30��6T   �� 5F      $��Количество:��6   $��Center=�6/   
�������$��0 0 5 0q�6]   �� 5G      -   5        $ NewEquipmentQtyTb��5G      $	��100��65   $��0 0 20 0q�6A   $��30��6S   �� 5I      -   5        $/��)Добавить оборудование��5I      $��8q�69   6E   5J      5K      5M      5N      