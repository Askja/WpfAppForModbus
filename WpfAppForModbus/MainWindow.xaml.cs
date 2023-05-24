using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfAppForModbus.Const;
using WpfAppForModbus.Domain;
using WpfAppForModbus.Domain.Interfaces;
using WpfAppForModbus.Domain.Models;
using WpfAppForModbus.Hooks;
using WpfAppForModbus.Models;
using WpfAppForModbus.Models.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WpfAppForModbus {
    public partial class MainWindow : Window {
        public ComPort? ActivePort {
            get; set;
        }

        private AsyncTimer? Timer { get; set; } = null;

        private Logger? AppLog { get; set; } = null;
        private Logger? PortsLog { get; set; } = null;

        private Settings? AppSettings { get; set; } = null;

        private ISensorDataList SensorDataListDb { get; set; } = null!;

        private string AppSettingsFile { get; set; } = "settings.xml";

        public MainWindow() {
            InitializeComponent();
            InitializeSettings();
            InitializeUI();

            ApplicationContext Context = new();
            SensorDataListDb = new SensorDataList(Context);
        }

        public void InitializeSettings() {
            AppLog = new(Log, SaveLogsToFile);
            PortsLog = new(PortsLogBox);

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
                menuItem.TextDecorations = menuItem == selectedMenuItem ? TextDecorations.Underline : null;
                menuItem.Background = menuItem == selectedMenuItem ? Brushes.Indigo : Brushes.Transparent;
            }
        }

        private void Button_Connect(object sender, RoutedEventArgs e) {
            try {
                if (IsConnected(ActivePort)) {
                    PortsLog?.AddDatedLog(LoadResource("AlreadyConnected"));
                    AppLog?.AddDatedLog(LoadResource("AlreadyConnected"));

                    return;
                }

                ActivePort = new ComPort();

                ComPortOptions Options = new() {
                    SelectedParity = ComboBoxHelper.GetSelectedItem(comboBoxParity, ParityList.Parities),
                    SelectedHandshake = ComboBoxHelper.GetSelectedItem(comboBoxHandshake, HandshakeList.Handshakes),
                    SelectedBaudRate = ComboBoxHelper.GetSelectedItem(comboBoxBaudRate, BaudRateList.BaudRate),
                    SelectedDataBits = ComboBoxHelper.GetSelectedItem(comboBoxDataBits, DataBitsList.DataBits),
                    SelectedPort = ComboBoxHelper.GetSelectedItem(comboBoxPorts, Shared.GetAvailablePorts()),
                    SelectedStopBits = ComboBoxHelper.GetSelectedItem(comboBoxStopBit, StopBitsList.StopBits),
                    Handler = ReceivedData
                };

                ActivePort?.Open(Options);

                if (IsConnected(ActivePort)) {
                    PortsLog?.AddDatedLog(LoadResource("SuccessfulConnected"));
                    AppLog?.AddDatedLog(LoadResource("SuccessfulConnected"));

                    Timer = new(5000, SendData);
                    Timer.Start();
                }

                StartHandle.IsEnabled = false;
                StopHandle.IsEnabled = true;
            } catch (ArgumentException) {
                PortsLog?.AddDatedLog(LoadResource("IncorrectConnectionData"));
                AppLog?.AddDatedLog(LoadResource("IncorrectConnectionData"));
            } catch (Exception ex) {
                AppLog?.AddDatedLog("Exception connect: " + ex.Message);
                PortsLog?.AddDatedLog("Exception connect: " + ex.Message);
            }
        }

        public Task SendData() {
            string[] Senders = Array.Empty<string>();

            /*if (SensorBar != null && SensorBar.IsChecked == true) {
                Senders.Append("01 0F 00 10 00 1F FF FF 8F 72");
            }*/

            if (SensorLight != null && SensorLight.IsChecked == true) {
                Senders.Append("01 0F 00 10 00 1F FF FF 8F 72");
            }

            /*if (SensorTemperature != null && SensorTemperature.IsChecked == true) {
                Senders.Append("01 0F 00 10 00 1F FF FF 8F 72");
            }

            if (SensorWater != null && SensorWater.IsChecked == true) {
                Senders.Append("01 0F 00 10 00 1F FF FF 8F 72");
            }*/

            if (Senders.Any()) {
                foreach (string Command in Senders) {
                    ActivePort?.Write(Command);

                    PortsLog?.AddDatedLog(LoadResource("SendingData") + ": " + Command);
                    AppLog?.AddDatedLog(LoadResource("SendingData") + ": " + Command);
                }
            } else {
                Timer?.StopAsync();

                throw new ArgumentException("Не выбрано ни единого датчика");
            }

            return Task.CompletedTask;
        }

        private void ReceivedData(object sender, SerialDataReceivedEventArgs e) {
            lock (this) {
                PortsLog?.AddDatedLog("Получено: " + ActivePort?.Read());
                AppLog?.AddDatedLog("Получено: " + ActivePort?.Read());
            }
        }

        private async void StopHandle_Click(object sender, RoutedEventArgs e) {
            try {
                ActivePort?.Close();

                ActivePort = null;

                if (Timer != null) {
                    await Timer.StopAsync();
                }

                Timer = null;

                if (!IsConnected(ActivePort)) {
                    PortsLog?.AddDatedLog(LoadResource("SuccessfulStopped"));
                    AppLog?.AddDatedLog(LoadResource("SuccessfulStopped"));

                    StartHandle.IsEnabled = true;
                    StopHandle.IsEnabled = false;
                }
            } catch (Exception ex) {
                AppLog?.AddDatedLog("Exception in StopHandle: " + ex.Message);
                AppLog?.AddDatedLog(ex.StackTrace);
                PortsLog?.AddDatedLog("Exception in StopHandle: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            AppLog?.ClearLog();
        }

        private void SaveLogsToFile_Checked(object sender, RoutedEventArgs e) {
            AppSettings?.UpdateFromControls(SaveLogsToFile);
            AppSettings?.SaveSettings(AppSettingsFile);

            AppLog?.AddDatedLog(LoadResource("SettingsUpdated"));
        }
    }
}
