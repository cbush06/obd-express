   M S B A M L   `   `   `  ���� E  AObdExpress, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:ObdExpress.Ui.UserControlsObdExpress.Ui.UserControls  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem M ISystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089A-clr-namespace:System.IO.Ports;assembly=SystemSystem.IO.Ports L    FObdExpress.Ui.UserControls.ConfigurationPanels.ConnectionSettingsPanel   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     7uc(clr-namespace:ObdExpress.Ui.UserControls     6sys&clr-namespace:System;assembly=mscorlib     ?ports-clr-namespace:System.IO.Ports;assembly=System       � 	Resources     parities S� 
MethodName S� 
ObjectType  System.Enum S� MethodParameters M� TypeName  stopbits��%�   &	        &	 5     S� $ GetNames��"   M� $ ports:Parity��S� $ GetNames��"   M� $ ports:StopBits������  �� Color$ White��.���� 
���   �$	��1.0q�
������  J� SnapsToDevicePixels . ._�� -    �Name$ MainPanelGrid��$
��Auto��$��Stretch=� � RowDefinitions �� $
��Auto ��� $��* �.+��� 
���   �$��0,0,0,1q�$��0�� . ���� 	 �� 
StartPoint$	 0.5,0&�
 �� EndPoint$
 0.5,1&�.���  � Color$ 	#FF838383�� � Offset$ 0Q�� $ 	#FF666666��$ .5Q�� $ 	#FF474747��$ 1Q�._�]�  � DataContext ��  �� Mode$ OneWay=� �� RelativeSource �� *FindAncestor+ �� AncestorType" >� �� AncestorLevel$ 1��.+��� 
�������$
��Left=�$��5,4,0,4q�$��Center=����� *Title+o� $��Right=�$��Right=�
������ .@�m� $��0q�����  �� Orientation$ 
Horizontal=�.+��� $��Options��
�������$	��5,4q�$��Center=��� $N��Hpack://application:,,,/ObdExpress;component/UI/Images/down_arrow_512.png��$��16��$��16��$
��Fill=�.@�m� -   $��Remove���� 
������ $��0q�
������ $��1�� . ._�]� $	��Top=���.  �� $ OneWay=� �� *FindAncestor+" >�$ 1������ # � ��.+��� $��Stretch=�$
��Wrap=� �� Padding$ 5q�$	��Top=�$����These settings configure the way in which OBD Express attempts to connect to an ELM327. Unless you have specifications for the device you wish to connect to, it is recommended that you leave these set to their defaults.���� $��0,1,0,0q����	#FFC5C5C5$��5,0,5,0q�$��2��$��Stretch=�
������ $	��Top=�� $
��Left=�$	��Top=�$��
20,10,20,5q�$	��200��$	��Top=� � ColumnDefinitions �� �� $	��200 � �� �� �� �� �� .+��� $��	Baud Rate��$��0��$��0��$��0,0,5,5q��� -    ��Name$ 
cbBaudRate��$��1��$��0��$��0,0,0,5q����� *	BaudRates+ �� SelectedItem �� *SelectedBaudRate+$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� $��	Data Bits��$��0��$��1��$��0,0,5,5q��� -   $ 
cbDataBits��$��1��$��1��$��0,0,0,5q����� *
DataBits+ �� *SelectedDataBits+$ TwoWay=�$ PropertyChanged=��� $��Parity��$��0��$��2��$��0,0,5,5q��� -   $ cbParity��$��1��$��2��$��0,0,0,5q�����  �� Source# [   �� *SelectedParity+$ TwoWay=�$ PropertyChanged=��� $��	Stop Bits��$��0��$��3��$��0,0,5,5q��� -   $ 
cbStopBits��$��1��$��3��$��0,0,0,5q����� # [  �� *SelectedStopBits+$ TwoWay=�$ PropertyChanged=��� $��Device Description��$��0��$��4��$��0,0,5,5q��� -    ��Name$ txtDeviceDescription��$��1��$��4��$��0,0,0,5q����� *DeviceDescription+$ TwoWay=�$ PropertyChanged=�]� 
�������$	��Top=�$��Bottom=�.+��� -   $
��Save��$��60��$
��Left=�$��	10,5,10,5q��� -	   $��Reset to Defaults��$	��120��$
��Left=�$��0,5,10,5q��� $��Stretch=�$	��Top=�$��1��$��Bottom=����	#FF9D9D9D