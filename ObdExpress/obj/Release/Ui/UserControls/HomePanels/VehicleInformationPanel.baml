   M S B A M L   `   `   `  ���� E  AObdExpress, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:ObdExpress.Ui.UserControlsObdExpress.Ui.UserControls  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem M ISystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089A-clr-namespace:System.IO.Ports;assembly=SystemSystem.IO.Ports C    =ObdExpress.Ui.UserControls.HomePanels.VehicleInformationPanel   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     7uc(clr-namespace:ObdExpress.Ui.UserControls     6sys&clr-namespace:System;assembly=mscorlib     ?ports-clr-namespace:System.IO.Ports;assembly=System       � 	Resources     parities S� 
MethodName S� 
ObjectType  System.Enum S� MethodParameters M� TypeName  stopbits��%�   &	        &	 5     S� $ GetNames��"   M� $ ports:Parity��S� $ GetNames��"   M� $ ports:StopBits������  �� Color$ White������ 
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
��Fill=�.@�m� -   $��Properties...��m� -   $��Remove���� 
������ $��0q�
������ $��1�� . ._�&   System.Windows.Controls.DataGrid  $��Stretch=�$��Stretch=�   HeadersVisibility$
 None=�   AutoGenerateColumns .     CanUserAddRows .     CanUserReorderColumns .     
IsReadOnly .  �� $ OneWay=� �� *FindAncestor+" >�$ 1������ *	DataItems+  )  #System.Windows.Controls.DataGridRow �� 
TargetType �� Property �� Value��%<   '        �� "  .���  �@J�IsHitTestVisible .     Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 0.3* 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *	Measure+  $
 0.7*  �� *Value+