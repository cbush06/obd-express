   M S B A M L   `   `   `  ����   
ObdExpress)    #ObdExpress.Ui.Windows.PluginManager   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Manage Plugins��$	��310��$	��500�� � WindowStartupLocation$ CenterOwner=� � Icon$3 -/ObdExpress;component/Ui/Images/48x48icon.png�� � ShowInTaskbar .   � 
ResizeMode$ NoResize=�����  �� Color# � ��.��]� ��.  � DataContext ��  �� Mode$ OneWay=� �� RelativeSource �� *FindAncestor+	 �� AncestorType"	 �.+�]� ��.  $��0��$��5q�$	��Top=�.+��� -   $��btnAdd��$��Add...��$��Right=�$
��10,1q��� -   $��	btnRemove��$��Remove��$��0,0,5,0q�$��Right=�$
��10,1q�]� $��Bottom=�
�������.+��� -   
 ��Name$
 btnDone��$
��Done��$��60��$��Right=�$��Right=�$��0,5,10,5q��� $��Stretch=�$��1��$��Bottom=����	#FF9D9D9D&   System.Windows.Controls.DataGrid  $��Stretch=�$��5,0,5,5q�$	��Top=�   CanUserReorderColumns .     HeadersVisibility$ Column=�   SelectionMode$ Single=�   CanUserResizeRows .     AutoGenerateColumns .     CanUserAddRows .  ���� *
PluginList+ �� SelectedItem �� *SelectedPlugin+���� $ White��   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Exists��   Width5  /System.Windows.Controls.DataGridLengthConverter$
 0.1*    CellTemplate �� .R��� $��ExistsIndicator��$��16��$��16�� �� Triggers ��  �� Value$
 true�� �� Binding �� *PluginExists+.a���  �� 
TargetName$ ExistsIndicator�� �� Property � �� �� Value$I Cpack://application:,,,/ObdExpress;component/UI/Images/success32.png���� $ false�� �� *PluginExists+.a��� $ ExistsIndicator�� � ��$H Bpack://application:,,,/ObdExpress;component/UI/Images/delete32.png��0  *System.Windows.Controls.DataGridTextColumn  $
 Name��$
 0.3*    
IsReadOnly . 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *PluginAssemblyName+  $ 	File Path��$
 0.6*  .  �� *
PluginPath+