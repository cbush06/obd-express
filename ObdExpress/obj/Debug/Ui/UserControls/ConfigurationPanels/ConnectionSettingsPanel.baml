   M S B A M L   `   `   `  ����E  AObdExpress, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:ObdExpress.Ui.UserControlsObdExpress.Ui.UserControls  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      M ISystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089A-clr-namespace:System.IO.Ports;assembly=SystemSystem.IO.Ports 5      L    FObdExpress.Ui.UserControls.ConfigurationPanels.ConnectionSettingsPanel   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7uc(clr-namespace:ObdExpress.Ui.UserControls     5      6sys&clr-namespace:System;assembly=mscorlib     5      ?ports-clr-namespace:System.IO.Ports;assembly=System     5        � 	Resources  5         parities S� 
MethodName S� 
ObjectType  System.Enum S� MethodParameters M� TypeName  stopbits��%�   &	        &	 h     S� 5   
   $ GetNames��6.   "  6D    5      M� 5      $ ports:Parity��6   5      S� 5   
   $ GetNames��6.   "  6D    5      M� 5      $ ports:StopBits��6   5      5      ��5      �� 5   
    �� Color$ White��6   .��5      �� 
���   �$	��1.0q�6!   
������  J� SnapsToDevicePixels . ._�5   
   � -   5         �Name$ MainPanelGrid��5      $
��Auto��6&   $��Stretch=�64    � RowDefinitions 5      �� 5       $
��Auto �6    �� 5!      $��* �6    .+�5$      �� 
���   �$��0,0,0,1q�6)   $��0��6C    . ��5%      �� 5&      	 �� 
StartPoint$	 0.5,0&�6*   
 �� EndPoint$
 0.5,1&�6=   .��5'      �  � Color$ 	#FF838383��6'    � Offset$ 0Q�69   � 5(      $ 	#FF666666��6'   $ .5Q�69   � 5)      $ 	#FF474747��6'   $ 1Q�69   5*      ._�5,      ]�  � DataContext 6   ��  �� Mode$ OneWay=� �� RelativeSource �� *FindAncestor+ �� AncestorType" >� �� AncestorLevel$ 1��.+�5-      �� 
�������$
��Left=�6I   $��5,4,0,4q�6_   $��Center=�6q   ��6   �� *Title+6q   o� 5.      $��Right=�6   $��Right=�62   
������ .@�5/      m� $��0q�6#   ��50      �� 51   "    �� Orientation$ 
Horizontal=�6-   .+�52   &   �� $��Options��6,   
�������$	��5,4q�6Q   $��Center=�6_   �� 53   &   $N��Hpack://application:,,,/ObdExpress;component/UI/Images/down_arrow_512.png��6,   $��16��6~   $��16��6�   $
��Fill=�6�   54   #   .@�57      m� -   5        $��Remove��57   '   67   58      59      5:      5;      �� 5=      
������ $��0q�6/   
������ $��1��6\    . ._�5>      ]� $	��Top=�6   ��.  6I   �� $ OneWay=� �� *FindAncestor+" >�$ 1����5?      �� 5@      # � ��6*   .+�5C      �� $��Stretch=�6    $
��Wrap=�6>    �� Padding$ 5q�6R   $	��Top=�6^   $����These settings configure the way in which OBD Express attempts to connect to an ELM327. Unless you have specifications for the device you wish to connect to, it is recommended that you leave these set to their defaults.��5G   *   5H      �� 5J      $��0,1,0,0q�6   ���	#FFC5C5C5$��5,0,5,0q�6O   $��2��6`   $��Stretch=�6k   
������ $	��Top=�6�   � 5L      $
��Left=�6   $	��Top=�66   $��
20,10,20,5q�6N   $	��200��6b   $	��Top=�6r    � ColumnDefinitions 5M      �� 5N      �� 5O      $	��200 �6/    5R      �� 5S      �� 5T      �� 5U      �� 5V      �� 5W      .+�5Z      �� $��	Baud Rate��6$   $��0��65   $��0��6E   $��0,0,5,5q�6R   �� 5[      -   5         ��Name$ 
cbBaudRate��5[   #   $��1��6�   $��0��6�   $��0,0,0,5q�6�   ��6Q   �� *	BaudRates+ �� SelectedItem 6s   �� *SelectedBaudRate+$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5]      $��	Data Bits��6$   $��0��65   $��1��6E   $��0,0,5,5q�6R   �� 5^      -   5        $ 
cbDataBits��5^   #   $��1��6�   $��1��6�   $��0,0,0,5q�6�   ��6Q   �� *
DataBits+ 6r   �� *SelectedDataBits+$ TwoWay=�$ PropertyChanged=�6�   �� 5`      $��Parity��6$   $��0��62   $��2��6B   $��0,0,5,5q�6O   �� 5a      -   5        $ cbParity��5a   #   $��1��6�   $��2��6�   $��0,0,0,5q�6�   ��6O   ��  �� Source# [   6�   �� *SelectedParity+$ TwoWay=�$ PropertyChanged=�6�   �� 5c      $��	Stop Bits��6$   $��0��65   $��3��6E   $��0,0,5,5q�6R   �� 5d      -   5        $ 
cbStopBits��5d   #   $��1��6�   $��3��6�   $��0,0,0,5q�6  ��6Q   �� # [  6�   �� *SelectedStopBits+$ TwoWay=�$ PropertyChanged=�6  �� 5f      $��Device Description��6$   $��0��6>   $��4��6N   $��0,0,5,5q�6[   �� 5g      -   5         ��Name$ txtDeviceDescription��5g   "   $��1��6�   $��4��6�   $��0,0,0,5q�6�   ��6Z   �� *DeviceDescription+$ TwoWay=�$ PropertyChanged=�6�   5h      ]� 5j      
�������$	��Top=�63   $��Bottom=�6K   .+�5k      �� -   5        $
��Save��5k   !   $��60��60   $
��Left=�6;   $��	10,5,10,5q�6V   6i   �� 5l      -	   5        $��Reset to Defaults��5l   !   $	��120��6=   $
��Left=�6I   $��0,5,10,5q�6d   6v   5m      �� 5o      $��Stretch=�6   $	��Top=�6;   $��1��6S   $��Bottom=�6^   ���	#FF9D9D9D6v   5p      5q      5r      5s      5t      