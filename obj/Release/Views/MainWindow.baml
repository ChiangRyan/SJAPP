   M S B A M L   `   `   `  ���� 	  SJAPP    SJAPP.Views.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006       � WindowStartupLocation$  Manual=� � Left$ 0��
 � Top$ 0�� � Title$ SanJetMotion��$
��1080��$
��1920�� � WindowStyle$
 None=� � WindowState$ 	Maximized=� � 	Resources    BackgroundImage �� ImageSource j� Stretch��%M   &	        �� $2 ,pack://application:,,,/Images/background.png��$
 Fill=�.��� #��[  	 � RowDefinitions	 �� $	��120 ��� $��* �.+��� $��0�����#555555$��1q����� $8 2pack://application:,,,/Images/Borderbackground.png��._�� .+��� 
 �� Orientation$
 
Horizontal=�$
��Left=�   ToolBarRadioButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType ��Name �� CornerRadius �� Triggers P� Property P� Value �� 
TargetName �� ScaleX �� ScaleY��%�  &	       �� " 	�.���  � ��$	 160����  � ��$	 100����  � ��
 ���� ��  � �� �� " 	�.R��� $ border��#��z��$ 20��._���  P�  �@d�	IsChecked$
 TrueY�. ��� $ border�� � �� �#FF8C00P�  �@J�IsMouseOver . . ��� $ border�� � ��
 �����P�  � �� . . ��� $ border�� � z� �� $
 0.95Q�$
 0.95Q�.+�	� -    	�Name$ 
HomeButton�� 	� 	GroupName$ 
Navigation��#��[  � ToolTip$ 首頁��$��	152,2,0,2q�.��F� 
������� �� Stretch$
 Fill=�$	��110��$��85��
������ ����� �J���� �J�� �J��_��r{���}�)P@�s�)�� �o���(\���*h	�sА�Tg{���W�b@&́��W�b@&́����������e����eDL� �kSP?<`TR'�[@ͪ��VX^@SW>��[@p=
ףV^@����o9W@:M�U^@��\7��S@����`UUR@R��%�[@��`UUR@b��	�rX@��`UUR@U'���0�T@U'����U'� �oU'� �o�d� �o� �J �o �J �o �J���		� -   $ 
Navigation��#��[ $ 手動操作��$��13,2,0,2q�.��F� 
�������$
 Fill=�$��90��$��85��
������ ��������l����������X�L�Y�&��R� \N�Ԑ�
�fZ��H	����	`n��h,��@#�	�@#zj��@#���L� (� ' (��ε (���Hy!���H�#����Hd~��X�l��S�ʥAa��c[@�	�g��5D	�ȓ�k�\@�,z	��
��Bs��c@����]@��?�ߒd@���{��]@�uT5A�e@:̗h]@�t���f@4ı.n�^@��9#J�f@û\�w\`@�����g@l����9#g@*$�n��J��g@aTR'��`@��-ʎi@/���ǹ`@}'f��i@x�Z�3���i@M
�g~5j@P\���3���h@��� �4���� �4��� �4 @B�`0� @B���� @Bp��U[rS`@�4��W��g.~KzR{[���/�@�b_]	4� ��-�K� HL��b�
����, �
.'h�F�
2�Z�9X
J'L0�,
��G��NSd@�7�0"Z@ȱ	W���&Z@��Q��d@������
��WV�8
�1X�riS��a�(hI6���F�r���f�@P@͡��o� �>���� �>��i� �>(�^\7����U[rS`@���	+��,*	 A���a�QP@���\�.-��	%�?w���0>�z��|����U@`���*ϼv~U@�n�JK�qo {�qXu#�� �}��d�Y{
l-��z.	?�}��Y	���		� -   $ 
Navigation��#��[ $ 	顯示器��$��14,2,0,2q�.��F� 
�������$
 Fill=�$	��110��$��85��
������ ���	$
��Left=�	� -   $ 
Navigation��#��[ $ 警告��$��14,2,0,2q�.��F� 
�������$
 Fill=�$	��110��$��85��
������ ���	$
��Left=�$��18,2,0,2q�	� -   $ 
Navigation��#��[ $ 設置��$��12,2,0,2q�.��F� 
�������$
 Fill=�$	��110��$��85��
������ ���	$
��Left=�$��18,2,0,2q��� $
 
Horizontal=�$��Right=�$	��Top=�$��	0,10,20,0q�.+��� -   $��40��$��40��
������ $ 	最小化��.���� $	��一��$��24(�
�������$��Center=�$��Center=��� -   $��40��$��40��
������ $ 關閉��.���� $��X��$��24(�
�������$��Center=�$��Center=�� $��1��$��2�� � ColumnDefinitions �� $	��73* ��� $
��567* �.+� � -     �Name$ 	MainFrame��  � NavigationUIVisibility$ Hidden=�$��2��