   M S B A M L   `   `   `  ����E  AObdExpress, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:ObdExpress.Ui.UserControlsObdExpress.Ui.UserControls  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      M ISystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089A-clr-namespace:System.IO.Ports;assembly=SystemSystem.IO.Ports 5      :    4ObdExpress.Ui.UserControls.HomePanels.DashboardPanel   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7uc(clr-namespace:ObdExpress.Ui.UserControls     5      6sys&clr-namespace:System;assembly=mscorlib     5      ?ports-clr-namespace:System.IO.Ports;assembly=System     5      ��5      �� 5   
     �� Color$  White��6   ��5      �� 5   
   
���   �$	��1.0q�6%   
������  J� SnapsToDevicePixels . ._�5      � -   5         �Name$ MainPanelGrid��5      $
��Auto��6*   $��Stretch=�68    � RowDefinitions 5      �� 5      $
��Auto �6$   �� 5      $��* �6$   .+�5      �� 
���   �$��0,0,0,1q�6-   $��0��6G    . ��5      �� 5       �� 
StartPoint$ 0.5,0&�6.    �� EndPoint$ 0.5,1&�6A   .��5      �  � Color$ 	#FF838383��6+    � Offset$ 0Q�6=   � 5      $ 	#FF666666��6+   $ .5Q�6=   � 5      $ 	#FF474747��6+   $ 1Q�6=   5      ._�5       ]�  � DataContext 6    �� 	 �� Mode$	 OneWay=�
 �� RelativeSource
 �� *FindAncestor+ �� AncestorType" >� �� AncestorLevel$ 1��.+�5!      �� 
�������$
��Left=�6M   $��5,4,0,4q�6c   $��Center=�6u   ��6    �� *Title+6u   o� 5"      $��Right=�6   $��Right=�66   
������ .@�5#      m� $��0q�6'   ��5$   "   �� 5%   &    �� Orientation$ 
Horizontal=�61   .+�5&   *   �� $��Options��60   
�������$	��5,4q�6U   $��Center=�6c   �� 5'   *   $N��Hpack://application:,,,/ObdExpress;component/UI/Images/down_arrow_512.png��60   $��16��6�   $��16��6�   $
��Fill=�6�   5(   '   .@�5*   "   m� -   5        $��Properties...��5*   +   6B   �� 5+   "   m� 5,   "   -   5        $��Remove��5,   +   6;   5-      5.      5/      50      �� 52      
������ $��0q�63   
������ $��1��6`    . ._�53      � $��Center=�6   $	��Top=�68   $��0q�6P    � ColumnDefinitions 54      �� 55      .+�57      (�  (� Stretch$
 None=�6"   $��0��61   .��58      �� $	��Top=�6�    6+   �� $	 OneWay=�
 �� *FindAncestor+" >�$ 1����6�   �� *DashboardItems+��59   "   �� 5:   &   .R�5;   *   A� $	��Top=�6�    66   �� $	 OneWay=�
 �� *FindAncestor+" >�$ 1�� A� Columns 6�   �� *	Columns+ A� Rows 6�   �� *Rows+6�   5<   '   ��5?   "   �� 5@   &   .R�5A   *   �� $��5q�61    �� CornerRadius$ 3��6<    �� Padding$ 10q�6M   ��5B   .   �� 5C   2   $  Black��6B   ��5E   .   �� 5F   2   $ 0.5,0&�6F   $ 0.5,1&�6Y   .��5G   6   � $ 	#FF838383��6C   $ 0Q�6U   � 5H   6   $ 	#FF666666��6C   $ .5Q�6U   � 5I   6   $ 	#FF474747��6C   $ 1Q�6U   5J   3   ._�5L   .   �� $ Vertical=�69   $��Center=�6P   $��Stretch=�6m   .+�5M   2   �� $��18(�6R   
���������6<   �� *Unit+6`   �� 5N   2   $��28(�6S   
���������6<   �� *Value+6a   �� 5O   2   $��12(�6U   
���������6<   �� *	Measure+6c   5P   /   5Q   +   5R   '   5T      5U      5V      5W      5X      5Y      5[      