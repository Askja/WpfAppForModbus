﻿using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfAppForModbus.Const;
using WpfAppForModbus.Hooks;
using WpfAppForModbus.Models;
using WpfAppForModbus.Models.Helpers;

namespace WpfAppForModbus {
    public partial class MainWindow : Window {
        public ComPort? ActivePort {
            get; set;
        }

        private AsyncTimer? Timer { get; set; } = null;

        private Logger? AppLog { get; set; } = null;

        private Settings? AppSettings { get; set; } = null;

        private string AppSettingsFile { get; set; } = "settings.xml";

        public MainWindow() {
            InitializeComponent();
            InitializeSettings();
            InitializeUI();
        }

        public void InitializeSettings() {
            AppLog = new(Log, SaveLogsToFile);
            AppSettings = Settings.LoadSettings("settings.xml");

            AppSettings?.ApplyToControls(SaveLogsToFile);
        }

        public void InitializeUI() {
            FillBoxes();

            UIHooks.ClickElement(PortsMenuItemText);
        }

        public void FillBoxes() {
            AppLog?.AddDatedLog(LoadResource("StartLoadingData"));

            ComboBoxHelper.AddRange<string>(comboBoxParity, Shared.GetNames(ParityList.Parities));
            ComboBoxHelper.AddRange<string>(comboBoxHandshake, Shared.GetNames(HandshakeList.Handshakes));
            ComboBoxHelper.AddRange<string>(comboBoxStopBit, Shared.GetNames(StopBitsList.StopBits));

            AppLog?.AddDatedLog(LoadResource("StartLoadingPorts"));

            ComboBoxHelper.AddRange<string>(comboBoxPorts, Shared.GetAvailablePorts());
            ComboBoxHelper.AddRange(comboBoxBaudRate, BaudRateList.BaudRate);
            ComboBoxHelper.AddRange(comboBoxDataBits, DataBitsList.DataBits);

            AppLog?.AddDatedLog(LoadResource("LoadedData"));
        }

        private string LoadResource(string Key) {
            return Shared.GetString(this, Key);
        }

        public bool IsConnected(ComPort? Port) => (Port != null && Port.IsOpened());

        public void ShowMessage(string message) => MessageBox.Show(message);

        public void SuccessfullyConnected() => ShowMessage(LoadResource("SuccessfulConnected"));

        public void SuccessfullyStopped() => ShowMessage(LoadResource("SuccessfulStopped"));

        public void AlreadyConnected() => ShowMessage(LoadResource("AlreadyConnected"));

        private void MenuItem_Click(object sender, MouseButtonEventArgs e) {
            PortsContent.Visibility = Visibility.Collapsed;
            LogContent.Visibility = Visibility.Collapsed;
            AnalyzeContent.Visibility = Visibility.Collapsed;

            TextBlock? selectedMenuItem = sender as TextBlock;

            if (selectedMenuItem == PortsMenuItemText) {
                PortsContent.Visibility = Visibility.Visible;
            } else if (selectedMenuItem == LogMenuItemText) {
                LogContent.Visibility = Visibility.Visible;
            } else if (selectedMenuItem == AnalyzeMenuItemText) {
                AnalyzeContent.Visibility = Visibility.Visible;
            }

            foreach (var menuItem in LeftMenuStackPanel.Children.OfType<TextBlock>()) {
                menuItem.FontWeight = menuItem == selectedMenuItem ? FontWeights.Bold : FontWeights.Normal;
                menuItem.TextDecorations = menuItem == selectedMenuItem ? TextDecorations.Underline : null;
            }
        }

        private void Button_Connect(object sender, RoutedEventArgs e) {
            try {
                if (IsConnected(ActivePort)) {
                    AlreadyConnected();

                    return;
                }

                ActivePort = new ComPort();

                ComPortOptions Options = new() {
                    SelectedParity = ComboBoxHelper.GetSelectedItem(comboBoxParity, ParityList.Parities),
                    SelectedHandshake = ComboBoxHelper.GetSelectedItem(comboBoxHandshake, HandshakeList.Handshakes),
                    SelectedBaudRate = ComboBoxHelper.GetSelectedItem(comboBoxBaudRate, BaudRateList.BaudRate),
                    SelectedDataBits = ComboBoxHelper.GetSelectedItem(comboBoxDataBits, DataBitsList.DataBits),
                    SelectedPort = ComboBoxHelper.GetSelectedItem(comboBoxPorts, Shared.GetAvailablePorts()),
                    SelectedStopBits = ComboBoxHelper.GetSelectedItem(comboBoxStopBit, StopBitsList.StopBits)
                };

                ActivePort?.Open(Options);

                if (IsConnected(ActivePort)) {
                    SuccessfullyConnected();
                }
            } catch (ArgumentException) {
                ShowMessage(LoadResource("IncorrectConnectionData"));
            } catch (Exception ex) {
                AppLog?.AddDatedLog("Exception: " + ex.Message);
            }
        }

        private void Button_CloseConnect(object sender, RoutedEventArgs e) {
            try {
                ActivePort?.Close();

                ActivePort = null;

                MessageBox.Show("Подключение закрыто");

            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonSend(object sender, RoutedEventArgs e) {
            Thread threadSending = new(ButtonSendSecondThread);

            threadSending?.Start();
        }
        public void ButtonSendSecondThread() {
            try {
                /*bool funcChanger = true;

                while (true) {
                    if (funcChanger) {
                        Dispatcher.Invoke(() => {
                            richTextBoxDataTime.AppendText("Отправка посылки - 01 0F 00 10 00 1F FF FF 8F 72" + DateTime.Now + "\r\n");
                        });

                        ActivePort?.Write("01 0F 00 10 00 1F FF FF 8F 72"); //выполнение команды во твором потоке для включение всех ламп


                        Dispatcher.Invoke(() =>         //выход из потока и вывод в текст бокс
                        {
                            richTextBoxDataTime?.AppendText("Ответ от устройства - " + ActivePort?.Read() + DateTime.Now + "\r\n");
                        });
                    } else {
                        Dispatcher.Invoke(() => {
                            richTextBoxDataTime.AppendText("Отправка посылки - 01 0F 00 10 00 1F 00 00 8E C2" + DateTime.Now + "\r\n");
                        });

                        ActivePort?.Write("01 0F 00 10 00 1F 00 00 8E C2"); //выполнение команды для выключение лампы од регистром 11

                        string? ResaultFunc2 = ActivePort?.Read();

                        Dispatcher.Invoke(() =>          //выход из потока и вывод в текст бокс
                        {
                            richTextBoxDataTime?.AppendText("Ответ от устройства - " + ResaultFunc2 + DateTime.Now + "\r\n");
                        });
                    }

                    funcChanger = !funcChanger; //флаг для смены выполняеом функции

                    Thread.Sleep(3000);
                }*/

            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void StopHandle_Click(object sender, RoutedEventArgs e) {

        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            AppLog?.ClearLog();
        }

        private void SaveLogsToFile_Checked(object sender, RoutedEventArgs e) {
            AppSettings?.UpdateFromControls(SaveLogsToFile);
            AppSettings?.SaveSettings(AppSettingsFile);
        }
    }
}
