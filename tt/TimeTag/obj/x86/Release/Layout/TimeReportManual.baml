   M S B A M L   `   `   `  ���� p  lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35pTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls  B >TimeTag, Version=1.0.0.4, Culture=neutral, PublicKeyToken=null/clr-namespace:TimeTag.LayoutTimeTag.Layout !clr-namespace:TimeTagTimeTag %   TimeTag.Layout.TimeReportManual   \my1Tclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit  W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     .localclr-namespace:TimeTag.Layout     $myclr-namespace:TimeTag     .���   � RowDefinitions  �� $
��137* ��� $
��103* �.+��� -    ��Name$ txtHoursReported��$��30��$
��Left=� �� TextAlignment$
 Left=�$��
244,29,0,0q�$	��Top=�$	��186���� -   $��titleComments��$��23��$
��Left=�$��	7,123,0,0q�   Comment#��[  $	��Top=����#464646$��85���� -   $��titleCustomer��$��23��$
��Left=�$��7,71,0,0q�  CustomerJob#��[ $	��Top=����#464646$��85���� -   $��titleActivity��$��23��$
��Left=�$��
244,71,0,0q�  Activity#��[ $	��Top=����#464646$��44���� -   $��
titleStart��$��23��$
��Left=�$��	244,8,0,0q�  
HoursToday#��[ $	��Top=����#464646$	��118��-   'System.Windows.Controls.AutoCompleteBox  -     Name$ autoCustomerJob��$��2��$��7,92,0,0q�$��30��$	��223��$	��Top=�$
��Left=�   
FilterMode$ Contains=�   ItemsSource ��    ItemTemplate �� .R��� ���� ���� *	ItemColor+�� -    ��Name$ autoActivityList��$��3��{�.   �� 
IsEditable . $y�Hidden=�$��
244,92,0,0q�$��30��$	��136��$	��Top=�$
��Left=�����   -   $ autoActivity��$��3��$��
244,92,0,0q�$��30��$	��136��$	��Top=�$
��Left=�$ Contains=� ��  �� .R��� 	 �� Orientation$	 Vertical=�.+��� ���� 
 �� Path$

 Name��� $��9(�
�������$��12������ $
 Description�y��� *DescriptionVisibility+ � ToolTip �� $
 Description��� -	   $��txtComments��$��82�� �� TextWrapping$
 Wrap=� �� AcceptsReturn . $��4��$
��Left=�$��	7,144,0,0q�$	��Top=�$	��423��$��2���� -
   $��btnOpdjobliste��$��20��$
��Left=�$��
202,64,0,0q�$	��Top=�$��28��$��10(�
�������$��1��.���� $0��*/TimeTag;component/Images/Opdater20x20.png��$��Center=��� -   $��	btnSubmit��  SendTime#��[ $��30��$��5��$
��Left=�$��
290,82,0,0q�$	��Top=�$	��140�����#428bca
�������$��1���� -   $��titleOnline��$��23��$
��Left=�$��	22,89,0,0q� 
 Online#��[ $	��Top=����#464646$��1��$��70��A� -   $��ellipseOnline��$��10��$
��Left=�$��7,93,0,0q�
��� � �$	��Top=�$��10�����	#FF6BAE58 J� OpacityMask �	#FF6BAE58$��1���� -   $��txtSubmitStatus��$��23��$��Right=�$��
0,85,182,0q�$#��Timer er sendt til ugeseddel.��$	��Top=����	#FF00A000 �	#FF00B700$y�	Collapsed=�$��1���� -   $ titleStart_Copy��$��23��$
��Left=�$��7,6,0,0q�  Date#��[ $	��Top=����#464646$	��106���� -    ��Name$ txtHours��$��30��$
��Left=�$��
395,92,0,0q�$�� ��$	��Top=�$��35���� -   $ titleStart_Copy1��$��23��$
��Left=�$��
395,71,0,0q�  Time#��[ $	��Top=����#464646$��32��(  "System.Windows.Controls.DatePicker  -     Name$ selectedDate��$��30��$
��Left=�$��7,29,0,0q�$	��Top=�$	��223��