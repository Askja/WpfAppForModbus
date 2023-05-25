using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
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
using WpfAppForModbus.Hooks;
using WpfAppForModbus.Models;
using WpfAppForModbus.Models.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WpfAppForModbus {
    public partial class MainWindow : Window {
        public ComPort? ActivePort { get; set; }
        public Task? AsyncTimer { get; set; }
        public CancellationTokenSource? AsyncTimerToken { get; set; }
        public Logger? AppLog { get; set; } = null;
        public Logger? PortsLog { get; set; } = null;
        private Settings? appSettings { get; set; } = null;
        private ISensorDataList SensorDataListDb { get; set; } = null!;
        private Dictionary<string, string> Senders { get; set; } = null!;
        private string? CurrentSensor { get; set; } = null;
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
            appSettings = Settings.Load(AppSettingsFile);

            TemperatureLimit.Text = DictionaryHelper.GetValueOrDefault(appSettings.TextBoxValues, "TemperatureLimit", "55");
            WaterLimit.Text = DictionaryHelper.GetValueOrDefault(appSettings.TextBoxValues, "WaterLimit", "50");
            VoltageLimit.Text = DictionaryHelper.GetValueOrDefault(appSettings.TextBoxValues, "VoltageLimit", "500");
            BarLimit.Text = DictionaryHelper.GetValueOrDefault(appSettings.TextBoxValues, "BarLimit", "700");
            SendInterval.Text = DictionaryHelper.GetValueOrDefault(appSettings.TextBoxValues, "SendInterval", "5000");

            SaveLogsToFile.IsChecked = DictionaryHelper.GetValueOrDefault(appSettings.CheckBoxValues, "SaveLogsToFile", false);

            comboBoxParity.SelectedIndex = DictionaryHelper.GetValueOrDefault(appSettings.ComboBoxValues, "comboBoxParity", -1);
            comboBoxDataBits.SelectedIndex = DictionaryHelper.GetValueOrDefault(appSettings.ComboBoxValues, "comboBoxDataBits", -1);
            comboBoxStopBit.SelectedIndex = DictionaryHelper.GetValueOrDefault(appSettings.ComboBoxValues, "comboBoxStopBit", -1);
            comboBoxHandshake.SelectedIndex = DictionaryHelper.GetValueOrDefault(appSettings.ComboBoxValues, "comboBoxHandshake", -1);
            comboBoxBaudRate.SelectedIndex = DictionaryHelper.GetValueOrDefault(appSettings.ComboBoxValues, "comboBoxBaudRate", -1);

            SensorTemperature.IsChecked = DictionaryHelper.GetValueOrDefault(appSettings.CheckBoxValues, "SensorTemperature", false);
            SensorWater.IsChecked = DictionaryHelper.GetValueOrDefault(appSettings.CheckBoxValues, "SensorWater", false);
            SensorBar.IsChecked = DictionaryHelper.GetValueOrDefault(appSettings.CheckBoxValues, "SensorBar", false);
            SensorLight.IsChecked = DictionaryHelper.GetValueOrDefault(appSettings.CheckBoxValues, "SensorLight", false);

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

        private void AddCommand(ToggleButton Sensor, string Key, string Command) {
            if (Sensor != null && Sensor.IsChecked == true) {
                Senders.Add(Key, Command);
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

                    Senders = new();

                    AddCommand(SensorBar, "Bar", "01 0F 00 10 00 1F 00 00 8E C2");
                    AddCommand(SensorLight, "Light", "01 04 00 03 00 01 C1 CA");
                    AddCommand(SensorTemperature, "Temperature", "01 03 00 02 00 0A 64 0D");
                    AddCommand(SensorWater, "Water", "01 0F 00 10 00 1F 00 00 8E C2");

                    if (Senders.Any()) {
                        AsyncTimerToken = new();

                        int Interval = int.Parse(SendInterval.Text);

                        if (Interval < 100) {
                            Interval = 100;
                        }

                        if (Interval > int.MaxValue) {
                            Interval = int.MaxValue;
                        }

                        AppAndPortsLog(LoadResource("StartConnectionWithInterval") + Interval.ToString() + " мс");

                        AsyncTimer ??= RunPeriodicallyAsync(() => {
                            SendData();

                            return Task.CompletedTask;
                        }, TimeSpan.FromMilliseconds(Interval), AsyncTimerToken.Token);

                        StartHandle.IsEnabled = false;
                        StopHandle.IsEnabled = true;
                    } else {
                        AsyncTimer = null;
                        AsyncTimerToken?.Cancel();

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

            foreach (KeyValuePair<string, string> Sender in Senders) {
                CurrentSensor = Sender.Key;

                ActivePort?.Write(Sender.Value);

                OnlyPortsLog(LoadResource("SendingData") + ": " + Sender.Value);

                IncrementSendStat();

                Task.Delay(200);
            }
        }

        private void ReceivedData(object sender, SerialDataReceivedEventArgs e) {
            try {
                AppAndPortsLog("Sensor: " + CurrentSensor + ". Получено: " + ActivePort?.Read());
                
                IncrementGetStat();
            } catch (Exception ex) {
                AppAndPortsLog("Exception in StopHandle: " + ex.Message);
                OnlyPortsLog(!string.IsNullOrEmpty(ex.StackTrace) ? ex.StackTrace : "No StackTrace");
            }
        }

        private void StopHandle_Click(object sender, RoutedEventArgs e) {
            try {
                ActivePort?.Close();

                ActivePort = null;

                AsyncTimer = null;
                AsyncTimerToken?.Cancel();

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

        public async Task RunPeriodicallyAsync(
            Func<Task> Callback,
            TimeSpan interval,
            CancellationToken cancellationToken
        ) {
            while (!cancellationToken.IsCancellationRequested) {
                await Task.Delay(interval, cancellationToken);
                await Callback();
            }
        }

        private void SaveSettings() {
            appSettings.TextBoxValues["TemperatureLimit"] = TemperatureLimit.Text;
            appSettings.TextBoxValues["WaterLimit"] = WaterLimit.Text;
            appSettings.TextBoxValues["VoltageLimit"] = VoltageLimit.Text;
            appSettings.TextBoxValues["BarLimit"] = BarLimit.Text;
            appSettings.TextBoxValues["SendInterval"] = SendInterval.Text;

            appSettings.CheckBoxValues["SaveLogsToFile"] = SaveLogsToFile.IsChecked ?? false;

            appSettings.ComboBoxValues["comboBoxParity"] = comboBoxParity.SelectedIndex;
            appSettings.ComboBoxValues["comboBoxDataBits"] = comboBoxDataBits.SelectedIndex;
            appSettings.ComboBoxValues["comboBoxStopBit"] = comboBoxStopBit.SelectedIndex;
            appSettings.ComboBoxValues["comboBoxHandshake"] = comboBoxHandshake.SelectedIndex;
            appSettings.ComboBoxValues["comboBoxBaudRate"] = comboBoxBaudRate.SelectedIndex;

            appSettings.CheckBoxValues["SensorTemperature"] = SensorTemperature.IsChecked ?? false;
            appSettings.CheckBoxValues["SensorWater"] = SensorWater.IsChecked ?? false;
            appSettings.CheckBoxValues["SensorBar"] = SensorBar.IsChecked ?? false;
            appSettings.CheckBoxValues["SensorLight"] = SensorLight.IsChecked ?? false;

            appSettings.Save(AppSettingsFile);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            SaveSettings();
        }
    }
}
