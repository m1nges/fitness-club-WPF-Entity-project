   M S B A M L   `   `   `  ����G  Cfitness-club, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:fitness_club.Pages.AdminPagesfitness_club.Pages.AdminPages  5      7    1fitness_club.Pages.AdminPages.AdminModerationPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =local+clr-namespace:fitness_club.Pages.AdminPages     5        N� Title$  AdminModerationPage��5         GlobalPageBackground N� 
Background# [  5	      .��5      �� $��20q�6   .+�5   
   �� $'��!Модерация отзывов��6   $��24(�6-   $
��Bold"�6;   
�������$��0 0 0 20q�6`   �� 5   
   $
��Auto=�6   $	��600��6:   .��5      �� -   5         ��Name$ ReviewsListView��5      $	��800��5      $��Stretch=�5      
������ $��0q�5      ��5      �� 5       �� 
TargetType" ��6   .�5      ��  �� Property � �� �� Value$ 0q�64   �� 5       � ��$ 0 0 0 10q�63   �� 5       � ��
 ���� 67   �� 5       � ��$ 0q�6<   �� 5       � ��$ Stretch=�6G   5      ��5      �� 5       .R�5!      �� ���#8A2BE2$��2q�67    �� CornerRadius$ 8��6K   ���#E0E0F8 �� Padding$ 15q�6q   ._�5"      �� .+�5$   "   �� $��18(�5%   ,   $
��Bold"�5&   ,   $
��Wrap=�5'   ,   
���   ���5$   ,   �� *
ObjectName+5(   ,   �� 5*   "   $��Italic$�5+   ,   ���#555$��0 8 0 0q�5-   ,   $
��Wrap=�5.   ,   ��5*   ,   �� *
AuthorName+5.   ,   �� 50   "   $
��Wrap=�51   ,   $��0 8 0 0q�52   ,   $��14(�53   ,   
���   ���50   ,   �� *ReviewContent+54   ,   �� 56   "   $��SemiBold"�57   ,   $��0 8 0 0q�58   ,   
����=H�$��14(�5:   ,   ��56   ,   �� *Grade+ �� StringFormat$ Оценка: {0}��5:   ,   �� 5<   "   	 �� Orientation$	 
Horizontal=�6-   $��Right=�6F   $��0 15 0 0q�6b   .+�5=   &   �� -   5        $	��5 0q�5=   Y   $	��8 4q�6f   $	��100��6t   ���#4CAF50
�������

 � Tag
 6I   �� .��5>   *   �� $��Одобрить��64    �� TextAlignment$ Center=�6D   $
��Wrap=�6[   5?   '   �� 5A   &   -   5        $	��5 0q�5A   X   $	��8 4q�6e   $	��100��6s   ���#F44336
�������
 6H   �� .��5B   *   �� $��Отклонить��64   $ Center=�6E   $
��Wrap=�6\   5C   '   5D   #   5F      5G      5H      5J      5K      5M      5N      