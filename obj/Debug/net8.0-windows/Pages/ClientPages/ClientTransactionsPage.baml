   M S B A M L   `   `   `  ����  fitness-club;    5fitness_club.Pages.ClientPages.ClientTransactionsPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5        N� Title$  ClientTransactions��5         GlobalPageBackground N� 
Background# [  5      .��5      �� $
��Auto=�6   .��5   
   �� $��20q�6   .+�5      �� $)��#История транзакций��6   $��24(�62   $
��Bold"�6@   
�������$��0 0 0 20q�6e   �� 5      -   5         ��Name$ TransactionsListView��5      $��Stretch=�65   ��5      �� 5       �� 
TargetType" ��6   .�5      ��  �� Property � �� �� Value$ Stretch=�6G   �� 5       � ��$ 0q�64   �� 5       � ��$	 0 5q�63   �� 5       � ��
 ���� 67    �� Triggers 5      P� 5       P� Property �@��
IsSelected P� Value$
 True��6<   . �5   "   ��  � �� �#7B68EE6?   �� 5   "    � ��
 �   �6?   5      5      ��5      �� 5       .R�5!      �� ���#8A2BE2$��2q�67   ���#E0E0F8	 �� CornerRadius$	 8��6`   
 �� Padding$
 10q�6q   $��0 2 0 10q�6~   ._�5"      �� .+�5#   "   �� $��16(�6R   $
��Bold"�6`   
���   ���6,   �� *OperationDescription+6r   �� 5%   "    �� Orientation$ 
Horizontal=�6-   $��0 5 0 0q�6F   .+�5&   &   �� $!��Способ оплаты: ��60   $
��Bold"�6G   
���   �6Y   �� 5'   &   
���   ���60   �� *
PaymentWay+6L   5(   #   �� 5*   "   $ 
Horizontal=�6-   .+�5+   &   �� $��Сумма: ��60   $
��Bold"�6?   
���   �6Q   �� 5,   &   
���   ���60   �� *Amount+ �� StringFormat$ {0} руб.��6a   5-   #   �� 5/   "   $ 
Horizontal=�6-   .+�50   &   �� $��Тип операции: ��60   $
��Bold"�6F   
���   �6X   �� 51   &   
���   ���60   �� *TransactionType+6Q   52   #   �� 54   "   $ 
Horizontal=�6-   .+�55   &   �� $��
Дата: ��60   $
��Bold"�6>   
���   �6P   �� 56   &   
���   ���60   �� *TransactionDate+$ {0:dd.MM.yyyy HH:mm}��6v   57   #   58      59      5:      5<      5=      5>      5?      