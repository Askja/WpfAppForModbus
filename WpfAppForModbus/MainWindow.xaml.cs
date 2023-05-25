using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfAppForModbus.Const;
using WpfAppForModbus.Domain;
using WpfAppForModbus.Domain.Interfaces;
using WpfAppForModbus.Domain.Models;
using WpfAppForModbus.Enums;
using WpfAppForModbus.Handlers;
using WpfAppForModbus.Hooks;
using WpfAppForModbus.Models;
using WpfAppForModbus.Models.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WpfAppForModbus {
    public partial class MainWindow : Window {
        public ComPort? ActivePort { get; set; }
        public bool IsLaunched { get; set; }
        public Logger? AppLog { get; set; } = null;
        public Logger? PortsLog { get; set; } = null;
        private Settings? AppSettings { get; set; } = null;
        private ISensorDataList SensorDataListDb { get; set; } = null!;
        private SensorHandlers Sensors { get; set; } = null!;
        private int SendCount { get; set; } = 0;
        private int GetCount { get; set; } = 0;
        private string AppSettingsFile { get; set; } = "settings.json";
        private Dispatcher GetCurrentDispatcher () => Application.Current.Dispatcher;

        public MainWindow() {
            InitializeComponent();
            InitializeUI();
            InitializeSettings();
            InitializeBinders();
            InitializeContexts();
        }

        public void InitializeContexts() {
            SensorDataListDb = new SensorDataList(new());
        }

        public void InitializeBinders() {
            comboBoxParity.SelectionChanged += SaveSettings;
            comboBoxDataBits.SelectionChanged += SaveSettings;
            comboBoxStopBit.SelectionChanged += SaveSettings;
            comboBoxHandshake.SelectionChanged += SaveSettings;
            comboBoxBaudRate.SelectionChanged += SaveSettings;
        }

        public void InitializeSettings() {
            AppLog = new(GetCurrentDispatcher(), Log, SaveLogsToFile);
            PortsLog = new(GetCurrentDispatcher(), PortsLogBox);

            AppLog?.AddDatedLog(LoadResource("SettingsLoading"));
            AppSettings = Settings.Load(AppSettingsFile);

            TemperatureLimit.Text = DictionaryHelper.GetValueOrDefault(AppSettings.TextBoxValues, "TemperatureLimit", "55");
            WaterLimit.Text = DictionaryHelper.GetValueOrDefault(AppSettings.TextBoxValues, "WaterLimit", "50");
            VoltageLimit.Text = DictionaryHelper.GetValueOrDefault(AppSettings.TextBoxValues, "VoltageLimit", "500");
            BarLimit.Text = DictionaryHelper.GetValueOrDefault(AppSettings.TextBoxValues, "BarLimit", "700");
            SendInterval.Text = DictionaryHelper.GetValueOrDefault(AppSettings.TextBoxValues, "SendInterval", "5000");

            SaveLogsToFile.IsChecked = DictionaryHelper.GetValueOrDefault(AppSettings.CheckBoxValues, "SaveLogsToFile", false);

            comboBoxParity.SelectedIndex = DictionaryHelper.GetValueOrDefault(AppSettings.ComboBoxValues, "comboBoxParity", -1);
            comboBoxDataBits.SelectedIndex = DictionaryHelper.GetValueOrDefault(AppSettings.ComboBoxValues, "comboBoxDataBits", -1);
            comboBoxStopBit.SelectedIndex = DictionaryHelper.GetValueOrDefault(AppSettings.ComboBoxValues, "comboBoxStopBit", -1);
            comboBoxHandshake.SelectedIndex = DictionaryHelper.GetValueOrDefault(AppSettings.ComboBoxValues, "comboBoxHandshake", -1);
            comboBoxBaudRate.SelectedIndex = DictionaryHelper.GetValueOrDefault(AppSettings.ComboBoxValues, "comboBoxBaudRate", -1);

            SensorTemperature.IsChecked = DictionaryHelper.GetValueOrDefault(AppSettings.CheckBoxValues, "SensorTemperature", false);
            SensorWater.IsChecked = DictionaryHelper.GetValueOrDefault(AppSettings.CheckBoxValues, "SensorWater", false);
            SensorBar.IsChecked = DictionaryHelper.GetValueOrDefault(AppSettings.CheckBoxValues, "SensorBar", false);
            SensorLight.IsChecked = DictionaryHelper.GetValueOrDefault(AppSettings.CheckBoxValues, "SensorLight", false);

            AppLog?.AddDatedLog(LoadResource("SettingsLoaded"));
        }

        public void InitializeUI() {
            FillBoxes();

            UIHooks.ClickElement(PortsMenuItemText);
        }

        public void FillBoxes() {
            AppLog?.AddDatedLog(LoadResource("StartLoadingData"));

            ComboBoxHelper.AddRange<string>(comboBoxParity, Shared.GetNames(ParityList.Parities));
            ComboBoxHelper.AddRange<string>(comboBoxHandshake, Shared.GetNames(HandshakeList.Handshakes));
            ComboBoxHelper.AddRange<string>(comboBoxStopBit, Shared.GetNames(StopBitsList.StopBitsTypes));

            AppLog?.AddDatedLog(LoadResource("StartLoadingPorts"));

            ComboBoxHelper.AddRange<string>(comboBoxPorts, Shared.GetAvailablePorts());
            ComboBoxHelper.AddRange(comboBoxBaudRate, BaudRateList.BaudRate);
            ComboBoxHelper.AddRange(comboBoxDataBits, DataBitsList.DataBits);

            AppLog?.AddDatedLog(LoadResource("LoadedData"));
        }

        private void AddSensor(ToggleButton Sensor, SensorData sensorHandler) {
            if (Sensor != null && Sensor.IsChecked == true) {
                Sensors.AddSensor(sensorHandler);
            }
        }

        private string LoadResource(string Key) {
            return Shared.GetString(this, Key);
        }

        public bool IsConnected(ComPort? Port) => (Port != null && Port.IsOpened());

        public void ShowMessage(string message) => MessageBox.Show(message);

        private void MenuItem_Click(object sender, MouseButtonEventArgs e) {
            PortsContent.Visibility = Visibility.Collapsed;
            LogContent.Visibility = Visibility.Collapsed;
            AnalyzeContent.Visibility = Visibility.Collapsed;
            SettingsContent.Visibility = Visibility.Collapsed;

            TextBlock? selectedMenuItem = sender as TextBlock;

            if (selectedMenuItem == PortsMenuItemText) {
                PortsContent.Visibility = Visibility.Visible;
            } else if (selectedMenuItem == LogMenuItemText) {
                LogContent.Visibility = Visibility.Visible;
            } else if (selectedMenuItem == AnalyzeMenuItemText) {
                AnalyzeContent.Visibility = Visibility.Visible;
            } else if (selectedMenuItem == SettingsMenuItemText) {
                SettingsContent.Visibility = Visibility.Visible;
            }

            foreach (var menuItem in LeftMenuStackPanel.Children.OfType<TextBlock>()) {
                menuItem.FontWeight = menuItem == selectedMenuItem ? FontWeights.Bold : FontWeights.Normal;
                menuItem.Background = menuItem == selectedMenuItem ? Brushes.Indigo : Brushes.Transparent;
            }
        }

        private void UpdateStats() {
            Dispatcher.Invoke(() => {
                StatCounterGet.Content = GetCount;
                StatCounterSend.Content = SendCount;
            });
        }

        private void IncrementGetStat() {
            Dispatcher.Invoke(() => {
                GetCount++;
            });
        }

        private void IncrementSendStat() {
            Dispatcher.Invoke(() => {
                SendCount++;
            });
        }

        private void AppAndPortsLog(string Text) {
            PortsLog?.AddDatedLog(Text);
            AppLog?.AddDatedLog(Text);
        }

        private void OnlyAppLog(string Text) {
            AppLog?.AddDatedLog(Text);
        }

        private void OnlyPortsLog(string Text) {
            PortsLog?.AddDatedLog(Text);
        }

        private void Button_Connect(object sender, RoutedEventArgs e) {
            try {
                if (IsConnected(ActivePort)) {
                    AppAndPortsLog(LoadResource("AlreadyConnected"));
                    
                    return;
                }

                ActivePort = new ComPort();

                ComPortOptions Options = new() {
                    SelectedParity = ComboBoxHelper.GetSelectedItem(comboBoxParity, ParityList.Parities),
                    SelectedHandshake = ComboBoxHelper.GetSelectedItem(comboBoxHandshake, HandshakeList.Handshakes),
                    SelectedBaudRate = ComboBoxHelper.GetSelectedItem(comboBoxBaudRate, BaudRateList.BaudRate),
                    SelectedDataBits = ComboBoxHelper.GetSelectedItem(comboBoxDataBits, DataBitsList.DataBits),
                    SelectedPort = ComboBoxHelper.GetSelectedItem(comboBoxPorts, Shared.GetAvailablePorts()),
                    SelectedStopBits = ComboBoxHelper.GetSelectedItem(comboBoxStopBit, StopBitsList.StopBitsTypes),
                    Handler = ReceivedData
                };

                ActivePort?.Open(Options);

                if (IsConnected(ActivePort)) {
                    AppAndPortsLog(LoadResource("SuccessfulConnected"));

                    Sensors = new();

                    AddSensor(SensorBar, new SensorData {
                        Command = "01 0F 00 10 00 1F 00 00 8E C2",
                        Name = "Датчик давления",
                        Handler = SensorHandler.CountBar
                    });

                    AddSensor(SensorLight, new SensorData {
                        Command = "01 04 00 03 00 01 C1 CA",
                        Name = "Датчик напряжения",
                        Handler = SensorHandler.CountVoltage
                    });

                    AddSensor(SensorTemperature, new SensorData {
                        Command = "01 03 00 02 00 0A 64 0D",
                        Name = "Датчик температуры",
                        Handler = SensorHandler.CountTemperature
                    });

                    AddSensor(SensorWater, new SensorData {
                        Command = "01 0F 00 10 00 1F 00 00 8E C2",
                        Name = "Датчик влажности",
                        Handler = SensorHandler.CountWater
                    });

                    if (Sensors.Any()) {
                        IsLaunched = true;

                        AppAndPortsLog(LoadResource("StartConnection"));

                        SendData();

                        StartHandle.IsEnabled = false;
                        StopHandle.IsEnabled = true;
                    } else {
                        IsLaunched = false;

                        ActivePort?.Close();
                        ActivePort = null;

                        throw new ArgumentException(LoadResource("NoSensors"));
                    }
                }
            } catch (ArgumentException) {
                AppAndPortsLog(LoadResource("IncorrectConnectionData"));
            } catch (Exception ex) {
                AppAndPortsLog("Exception connect: " + ex.Message);
                OnlyAppLog(!string.IsNullOrEmpty(ex.StackTrace) ? ex.StackTrace : "No StackTrace");
            }
        }

        public void SendData() {
            UpdateStats();

            SensorData CurrentSensor = Sensors.Current();

            AppAndPortsLog(LoadResource("CurrentSensor") + ": " + CurrentSensor.Name);
            AppAndPortsLog(LoadResource("SendingData") + ": " + CurrentSensor.Command);

            ActivePort?.Write(CurrentSensor.Command);

            IncrementSendStat();
        }

        private void ReceivedData(object sender, SerialDataReceivedEventArgs e) {
            try {
                string? Answer = ActivePort?.Read();

                if (!string.IsNullOrEmpty(Answer)) {
                    AppAndPortsLog(LoadResource("DataHandling") + ": " + Answer);
                    AppAndPortsLog(LoadResource("InDecryptedView") + ": " + Sensors.Current().Handler(Answer));

                    IncrementGetStat();
                } else {
                    AppAndPortsLog(LoadResource("EmptyAnswer"));
                }

                if (IsLaunched) {
                    int Interval = int.Parse(SendInterval.Text);

                    if (Interval > int.MaxValue) {
                        Interval = int.MaxValue;
                    }

                    if (Interval < 100) {
                        Interval = 100;
                    }

                    AppAndPortsLog(LoadResource("IntervalWaiting") + ": " + Interval);

                    Task.Delay(Interval);

                    Sensors.Next();

                    SendData();
                }
            } catch (Exception ex) {
                AppAndPortsLog("Exception in StopHandle: " + ex.Message);
                OnlyPortsLog(!string.IsNullOrEmpty(ex.StackTrace) ? ex.StackTrace : "No StackTrace");
            }
        }

        private void StopHandle_Click(object sender, RoutedEventArgs e) {
            try {
                ActivePort?.Close();

                ActivePort = null;

                IsLaunched = false;

                if (!IsConnected(ActivePort)) {
                    AppAndPortsLog(LoadResource("SuccessfulStopped"));

                    StartHandle.IsEnabled = true;
                    StopHandle.IsEnabled = false;
                }
            } catch (Exception ex) {
                AppAndPortsLog("Exception in StopHandle: " + ex.Message);
                OnlyPortsLog(!string.IsNullOrEmpty(ex.StackTrace) ? ex.StackTrace : "No StackTrace");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            AppLog?.ClearLog();
        }

        private void SaveLogsToFile_Checked(object sender, RoutedEventArgs e) {
            SaveSettings();

            OnlyAppLog(LoadResource("SettingsUpdated"));
        }

        private void SaveSettings(object sender, RoutedEventArgs e) {
            SaveSettings();
        }

        private void SaveSettings() {
            AppSettings.TextBoxValues["TemperatureLimit"] = TemperatureLimit.Text;
            AppSettings.TextBoxValues["WaterLimit"] = WaterLimit.Text;
            AppSettings.TextBoxValues["VoltageLimit"] = VoltageLimit.Text;
            AppSettings.TextBoxValues["BarLimit"] = BarLimit.Text;
            AppSettings.TextBoxValues["SendInterval"] = SendInterval.Text;

            AppSettings.CheckBoxValues["SaveLogsToFile"] = SaveLogsToFile.IsChecked ?? false;

            AppSettings.ComboBoxValues["comboBoxParity"] = comboBoxParity.SelectedIndex;
            AppSettings.ComboBoxValues["comboBoxDataBits"] = comboBoxDataBits.SelectedIndex;
            AppSettings.ComboBoxValues["comboBoxStopBit"] = comboBoxStopBit.SelectedIndex;
            AppSettings.ComboBoxValues["comboBoxHandshake"] = comboBoxHandshake.SelectedIndex;
            AppSettings.ComboBoxValues["comboBoxBaudRate"] = comboBoxBaudRate.SelectedIndex;

            AppSettings.CheckBoxValues["SensorTemperature"] = SensorTemperature.IsChecked ?? false;
            AppSettings.CheckBoxValues["SensorWater"] = SensorWater.IsChecked ?? false;
            AppSettings.CheckBoxValues["SensorBar"] = SensorBar.IsChecked ?? false;
            AppSettings.CheckBoxValues["SensorLight"] = SensorLight.IsChecked ?? false;

            AppSettings.Save(AppSettingsFile);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            SaveSettings();
        }
    }
}
