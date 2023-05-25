using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.IO.Ports;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        private Settings? AppSettings { get; set; } = null;
        private ISensorDataList SensorDataListDb { get; set; } = null!;
        private string[] Senders { get; set; } = null!;
        private int SendCount { get; set; } = 0;
        private int GetCount { get; set; } = 0;
        private string AppSettingsFile { get; set; } = "settings.xml";
        private Dispatcher GetCurrentDispatcher () => Application.Current.Dispatcher;

        public MainWindow() {
            InitializeComponent();
            InitializeSettings();
            InitializeUI();

            ApplicationContext Context = new();
            SensorDataListDb = new SensorDataList(Context);
        }

        public void InitializeSettings() {
            AppLog = new(GetCurrentDispatcher(), Log, SaveLogsToFile);
            PortsLog = new(GetCurrentDispatcher(), PortsLogBox);

            AppLog?.AddDatedLog(LoadResource("SettingsLoading"));
            AppSettings = Settings.LoadSettings("settings.xml");

            AppSettings?.ApplyToControls(SaveLogsToFile);
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

        private void AddCommand(CheckBox Sensor, string Command) {
            if (Sensor != null && Sensor.IsChecked == true) {
                Senders = Senders.Append(Command).ToArray();
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

                    Senders = Array.Empty<string>();

                    AddCommand(SensorBar, "01 0F 00 10 00 1F 00 00 8E C2");
                    AddCommand(SensorLight, "01 0F 00 10 00 1F FF FF 8F 72");
                    AddCommand(SensorTemperature, "01 0F 00 10 00 1F 00 00 8E C2");
                    AddCommand(SensorWater, "01 05 00 11 00 00 0E 07");

                    if (Senders.Any()) {
                        AsyncTimerToken = new();

                        AsyncTimer ??= RunPeriodicallyAsync(() => {
                            SendData();

                            return Task.CompletedTask;
                        }, TimeSpan.FromMilliseconds(5000), AsyncTimerToken.Token);

                        StartHandle.IsEnabled = false;
                        StopHandle.IsEnabled = true;
                    } else {
                        AsyncTimer = null;
                        AsyncTimerToken?.Cancel();

                        throw new ArgumentException("Не выбрано ни единого датчика");
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

            OnlyPortsLog("Timer interval");

            OnlyPortsLog("Before foreach statement");

            foreach (string Command in Senders) {
                ActivePort?.Write(Command);

                Task.Delay(200);

                OnlyPortsLog(LoadResource("SendingData") + ": " + Command);

                IncrementSendStat();
            }
            

            OnlyPortsLog("after foreach statement");
        }

        private void ReceivedData(object sender, SerialDataReceivedEventArgs e) {
            //lock (this) {
            try {
                AppAndPortsLog("Получено: " + ActivePort?.Read());

                IncrementGetStat();
            } catch (Exception ex) {
                AppAndPortsLog("Exception in StopHandle: " + ex.Message);
                OnlyPortsLog(!string.IsNullOrEmpty(ex.StackTrace) ? ex.StackTrace : "No StackTrace");
            }
            //}
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
            AppSettings?.UpdateFromControls(SaveLogsToFile);
            AppSettings?.SaveSettings(AppSettingsFile);

            OnlyAppLog(LoadResource("SettingsUpdated"));
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
    }
}
