   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:fitness_club.Pages.AdminPagesfitness_club.Pages.AdminPages  5      >    8fitness_club.Pages.AdminPages.AdminServiceManagementPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =local+clr-namespace:fitness_club.Pages.AdminPages     5        N� Title$   AdminServiceManagementPage��5	         GlobalPageBackground N� 
Background# [  5
      .��5      �� $��20q�6   .+�5   
   �� $+��%Управление услугами��6   $��22(�6/   $
��Bold"�6=   $��0 0 0 15q�6O   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ ServiceGrid��5         AutoGenerateColumns$ False��6(   $	��300��6D      CanUserAddRows$ False��5      $��0 0 0 20q�6*      
IsReadOnly$ False��6<      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$# Название услуги��6%      
IsReadOnly$
 True��6^   	   Width5  /System.Windows.Controls.DataGridLengthConverter$		 200 6p   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 6>   �� *ServiceName+6p     5      $ Тип��6%   $
 True��6V   $		 150 6h   
 62   �� *ServiceTypeName+6h     5      $ Описание��6%   $		 250 6W   
 67   �� *Description+6W     5      $ Цена (₽)��6%   $	 80 6Q   
 67   �� *Price+6Q     5      $ 
Лимит��6%   $	 80 6W   
 64   �� *FreeUsageLimit+6W   5      �� 5   
   -   5        $+��%Сохранить изменения��5      $	��200��6/   $��0 0 0 20q�6U   �� 5   
   $��10q�6   � 5!   
    � ColumnDefinitions 5"      �� 5#      $��* �6#   �� 5$      $��* �6#   .+�5(      �� $��0��6   $��0 0 10 0q�6)   .+�5)      �� $,��&Создать новую услугу��6   $
��Bold"�68   $��16(�6J   $��0 0 0 10q�6X   �� 5*      $��Название:��6   �� 5+      -   5         ��Name$ NewServiceNameBox��5+      $��0 5 0 10q�65   �� 5-      $��Описание:��6   �� 5.      -   5        $ NewServiceDescriptionBox��5.      $��0 5 0 10q�6<   �� 50      $��Тип:��6   �� 51      -   5         ��Name$ ServiceTypeComboBox��51      $��0 5 0 10q�68    �� DisplayMemberPath$ TypeName��52       �� SelectedValuePath$ ServiceTypeId��68   �� 54      $��	Цена:��6   �� 55      -   5        $ NewPriceBox��55      $��0 5 0 10q�6/   �� 57      $C��=Лимит бесплатного использования:��6   �� 58      -   5        $ NewFreeLimitBox��58      $��0 5 0 10q�63   �� 5:      -   5        $#��Добавить услугу��5:      $	��180��63   6?   5;      �� 5>      $��1��6   $��10 0 0 0q�6)   .+�5?      �� $*��$Добавить тип услуги��6   $
��Bold"�67   $��16(�6I   $��0 0 0 10q�6W   �� 5@      $ ��Название типа:��6   �� 5A      -	   5        $ NewServiceTypeBox��5A      $��0 5 0 10q�65   �� 5B      -
   5        $��Добавить тип��5B      $	��180��60   6<   5C      5D      5E      5F      