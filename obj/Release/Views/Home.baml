   M S B A M L   `   `   `  ���� 	  SJAPP    SJAPP.Views.Home   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008       N� Title$
  Home�� N� 	KeepAlive .  � 	Resources    MyTextBlockStyle �� 
TargetType �� Property �� Value  MyButtonStyle  StartButtonStyle �� BasedOn �� Triggers �� Binding	 �� Value  StopButtonStyle  MyCheckBoxStyle
 �� ScaleX �� ScaleY��%�  &	        &	 l     &	     7  &	 �    7  &	 �    �� " ��.���  � ��$ 20(���  � ��$ SemiBold"���  � ��$ 2q���  � ��
 �   ��� " ��.���  � ��$ 20(���  � ��$
 Bold"���  � ��$ 70����  � ��$ 35����  � ��$ 5,0,5,0q���  � ��
 �������  � ��
 �OO/��� " ��8   .���  � �� �#4CAF50��  � {� .  ��  �� *Status+	 	運行中.a���  � {� .  ��  �� *IsOperational+		 False.a���  � {� .  �� " ��8   .���  � �� �#F44336��  � {� .  ��  �� *Status+
	 閒置.a���  � {� .  ��  �� *IsOperational+		 False.a���  � {� .  �� " ��.���  � ��$ 20(���  � ��$ SemiBold"���  � ��$ 5,0,0,0q���  � ��
 �   ���  � ��$ Center=���  �@�LayoutTransform �� $
 2Q�$ 2Q�.���� $
��Auto=�$
��Auto=�.���� ���� *	Devices+���� .R�A�  A� Columns$ 6������ .R��� $��5q� �� Padding$ 10q� �� CornerRadius$ 8��
�������$��1q����� " ��.���  � ��
 ����� ��  �� *Status+
	 閒置.a���  � ��
 �������  �� *Status+	 	運行中.a���  � ��
 ������  �� *Status+
	 故障.a���  � ��
 �������  �� *Status+	 通訊失敗.a���  � ��
 �� ��._��� .+���  �� Orientation$ 
Horizontal=�$��0,0,0,5q�.+��� $��是否啟用:��#��[  �� #��[  d� 	IsChecked �� *IsOperational+ �� UpdateSourceTrigger$ PropertyChanged=��� $ 
Horizontal=�$��0,0,0,10q�.+��� $��	名稱：��#��[  $��Center=��� $��20(�$	��120������ *Name+$ PropertyChanged=��� #��[  ���� *	IpAddress+ �� StringFormat$ ESP32 IP：{0}���� #��[  ���� *	SlaveId+$ RS485從站 ID：{0}���� #��[  ���� *
RunCount+$ 跑合次數：{0}���� #��[  $
��Bold"�
����=H����� *Status+$ 狀態：{0}���� $ 
Horizontal=�$��0,15,0,0q�$��Center=�.+��� $��啟動��#��[ ���� *StartCommand+�� $��停止��#��[ ���� *StopCommand+