   M S B A M L   `   `   `  ����E  AObdExpress, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:ObdExpress.Ui.UserControlsObdExpress.Ui.UserControls  5      =#clr-namespace:ObdExpress.Ui.WindowsObdExpress.Ui.Windows  5      2    ,ObdExpress.Ui.UserControls.SideNavigationBar   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7uc(clr-namespace:ObdExpress.Ui.UserControls     5      7Windows#clr-namespace:ObdExpress.Ui.Windows     5      -   5          >�Name$  userControl��5   B    � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $ ../StyleDictionary.xaml?�6%   %    5      .��5      �� 
���   �$��1,1,0,0q�5   	    J� SnapsToDevicePixels . ._�5   
   � $��Stretch=�6    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $��* �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    $��25��6.    � ColumnDefinitions 5       �� 5!      $��* �6#   .+�5$      ]�    SectionHeaderDock#��[  6   $��0��6C   $��Stretch=�6P   .+�5%      �� $��0,0,0,1q�6   
���   �._�5&      ��   SectionHeaderLabel#��[ 6   $��Center=�6j   ��6H   �� *SectionHeader+ � DataContext 6�   ��  �� ElementName$ userControl��	 �� Mode$	 OneWay=�5'      5(      �� 5*      $��1��6   $
��Auto=�6(   $��Stretch=�6K   .��5+      �� -   5        
 ��Name$
 navigationViewer��5+      
���������6S   �� *	MenuItems+ �� RelativeSource �� *FindAncestor+ �� AncestorType&    ObdExpress.Ui.Windows.MainWindow"  ��5,      �� 5-      .R�5.      1   +ObdExpress.Ui.UserControls.NavigationButton     Label 62   �� *Label+   ImageSource 6J   �� *
IconPath+   Click 6k   �� *Click+   
IsSelected 6�   �� *
IsSelected+ �� UpdateSourceTrigger$ PropertyChanged=�$	 TwoWay=�5/      51      52      �� 54      -   5         ��Name$ 	gSplitter��54      $��2��6.   $��2��6;   $��Stretch=�6F   
���   �6d   �� 56      -   5        $
 menuSelectViewer��56      $��3��6�   ��6O   �� *TopMenuItems+ �� *FindAncestor+"  ��57      �� 58      .R�59      �� $��0,0,0,1q�6!   
���   �._�5:        $��0q�6J      NavButtonType$
 MENU=�6v   $
��Auto��6�      ImageHeight$ 0��6�      
ImageWidth$ 0��6�      ImageMargin$ 0q�6   62   �� *Label+ 6^   �� *Click+ 6�   �� *
IsSelected+$ PropertyChanged=�$	 TwoWay=�6  5;      5<      5>      5?      5@      5A      