   M S B A M L   `   `   `  ���� p  lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35pTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls  B >TimeTag, Version=1.0.0.4, Culture=neutral, PublicKeyToken=null/clr-namespace:TimeTag.LayoutTimeTag.Layout !clr-namespace:TimeTagTimeTag (   "TimeTag.Layout.TimeReportStopWatch   \my1Tclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit  W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     .localclr-namespace:TimeTag.Layout     $myclr-namespace:TimeTag     .���   � RowDefinitions  �� $
��137* ��� $
��103* �.+�  TimeTag.TimeInput  -     Name$ txtHours_st��$��30��$
��Left=�$��	12,37,0,0q�$�� ��$	��Top=�$��58��   ValueChanged txtHoursValueChanged  -   $ txtHours_sl��$��30��$
��Left=�$��	85,37,0,0q�$�� ��$	��Top=�$��58�� txtHoursValueChanged�� -   $��txtHours��$��30��$
��Left=�$��
157,37,0,0q�$��0��$	��Top=�$��37�� �� TextAlignment$ Center=� J� RenderTransformOrigin$ 	0.517,0.8&��� -   $��stsl_del��$��23��$
��Left=�$��	75,44,0,0q�$��-��$	��Top=����#464646$ 
0.75,-1.13&�$��5���� -   $��titleComments��$��23��$
��Left=�$��
226,74,0,0q�   Comment#��[  $	��Top=����#464646$��79���� -   $��titleCustomer��$��23��$
��Left=�$��	226,9,0,0q�  CustomerJob#��[ $	��Top=����#464646$��79���� -   $��titleActivity��$��23��$
��Left=�$��	463,9,0,0q�  Activity#��[ $	��Top=����#464646$��44���� -    ��Name$ 
titleHours��$��23��$
��Left=�$��
157,10,0,0q�$ Center=�$	��Top=����#464646$��37��  Time#��[ �� -	   $��
titleStart��$��23��$
��Left=�$��12,9,0,0q� 	 Start#��[ $	��Top=����#464646$��73���� -
   $��	titleSlut��$��23��$
��Left=�$��	85,10,0,0q�  Stop#��[ $	��Top=����#464646$ -0.25,0.435&�$��58��-   'System.Windows.Controls.AutoCompleteBox  -     Name$ autoCustomerJob��$��2��$��
226,37,0,0q�$��30��$	��223��$	��Top=�$
��Left=�   
FilterMode$ Contains=�   ItemsSource �� 	   ItemTemplate	 �� .R��� ���� ���� *	ItemColor+  -   $ autoActivity��$��3��$��
463,37,0,0q�$��30��$	��180��$	��Top=�$
��Left=�$ Contains=� �� 	 �� .R��� 
 �� Orientation$
 Vertical=�.+��� ����  �� Path$
 Name��� $��9(�
�������$��12������ $ Description�y��� *DescriptionVisibility+ � ToolTip �� $ Description��� -   $��txtComments��$��82�� �� TextWrapping$
 Wrap=� �� AcceptsReturn . $��4��$
��Left=�$��
226,98,0,0q�$	��Top=�$	��417��$��2���� -   $��btnOpdjobliste��$��20��$
��Left=�$��	421,9,0,0q�$	��Top=�$��28��$��10(�
�������$��1��.���� $0��*/TimeTag;component/Images/Opdater20x20.png��$��Center=��� -   $��btnStart��  	StartStop#��[ $��82��$��1��$
��Left=�$��	11,98,0,0q�$	��Top=�
�������$	��186��$��20(����#5cb85c$��2���� -   $��	btnSubmit��  SendTime#��[ $��30��$��5��$
��Left=�$��
503,74,0,0q�$	��Top=�$	��140�����#428bca
�������$��1���� -   $��titleOnline��$��23��$
��Left=�$��	27,85,0,0q� 
 Online#��[ $	��Top=����#464646$��1��$��58��A� -   $��ellipseOnline��$��10��$
��Left=�$��	11,89,0,0q�
��� � �$	��Top=�$��10�����	#FF6BAE58 J� OpacityMask �	#FF6BAE58$��1���� -   $��txtSubmitStatus��$��23��$��Right=�$��
0,76,201,0q�$#��Timer er sendt til ugeseddel.��$	��Top=����	#FF00A000 �	#FF00B700$y�	Collapsed=�$��1���� -   $ 
hoursToday��$��30��$
��Wrap=�$
��Left=�$��	12,42,0,0q� 	 
HoursToday#��[	 $	��Top=����#464646$��68��$��1��