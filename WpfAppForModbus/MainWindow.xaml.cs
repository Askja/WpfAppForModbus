using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfAppForModbus.Const;
using WpfAppForModbus.Domain.Interfaces;
using WpfAppForModbus.Domain.Models;
using WpfAppForModbus.Handlers;
using WpfAppForModbus.Hooks;
using WpfAppForModbus.Models.Analyzers;
using WpfAppForModbus.Models.Core;
using WpfAppForModbus.Models.Helpers;
using WpfAppForModbus.Models.Views;

namespace WpfAppForModbus {
    public partial class MainWindow : Window {
        private Point startPoint;

        public ComPort? ActivePort { get; set; }
        public bool IsLaunched { get; set; }
        public Logger? AppLog { get; set; } = null;
        public Logger? PortsLog { get; set; } = null;
        private Settings? AppSettings { get; set; } = null;
        private ISensorDataList SensorDataListDb { get; set; } = null!;
        private SensorHandlers Sensors { get; set; } = null!;
        private int SendCount { get; set; } = 0;
        private int GetCount { get; set; } = 0;
        private int UnreadNotifications { get; set; } = 0;
        private string AppSettingsFile { get; set; } = "settings.json";
        private Dispatcher GetCurrentDispatcher() => Application.Current.Dispatcher;

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
            BindComboBox(comboBoxBaudRate);
            BindComboBox(comboBoxDataBits);
            BindComboBox(comboBoxHandshake);
            BindComboBox(comboBoxParity);
            BindComboBox(comboBoxStopBit);

            BindCheckBox(SensorBar);
            BindCheckBox(SensorTemperature);
            BindCheckBox(SensorWater);
            BindCheckBox(SensorLight);

            BindCheckBox(SaveLogsToFile);
        }

        private void BindComboBox(ComboBox comboBox) {
            comboBox.SelectionChanged += SaveSettings;
        }

        private void BindCheckBox(ToggleButton checkBox) {
            checkBox.Checked += SaveSettings;
            checkBox.Unchecked += SaveSettings;
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
            SendInterval.Text = DictionaryHelper.GetValueOrDefault(AppSettings.TextBoxValues, "SendInterval", "500");

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

            StartDate.SelectedDate = DateTime.Now;
            EndDate.SelectedDate = DateTime.Now;
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

            if (!SensorDataListDb.SensorExist(sensorHandler.Id)) {
                SensorDataListDb.AddSensor(sensorHandler.Id, sensorHandler.Name);
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
            NotificationsContent.Visibility = Visibility.Collapsed;

            TextBlock? selectedMenuItem = sender as TextBlock;

            if (selectedMenuItem == PortsMenuItemText) {
                PortsContent.Visibility = Visibility.Visible;
            } else if (selectedMenuItem == LogMenuItemText) {
                LogContent.Visibility = Visibility.Visible;
            } else if (selectedMenuItem == AnalyzeMenuItemText) {
                AnalyzeContent.Visibility = Visibility.Visible;
            } else if (selectedMenuItem == SettingsMenuItemText) {
                SettingsContent.Visibility = Visibility.Visible;
            } else if (selectedMenuItem == NotificationsMenuItemText) {
                UnreadNotifications = 0;
                NotificationsMenuItemLabel.Foreground = Brushes.White;
                NotificationsMenuItemLabel.Text = "Уведомления";
                NotificationsContent.Visibility = Visibility.Visible;
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

                    double MaxTemperature = double.MaxValue;
                    GetCurrentDispatcher().Invoke(() => MaxTemperature = double.Parse(TemperatureLimit.Text));

                    double MaxBar = double.MaxValue;
                    GetCurrentDispatcher().Invoke(() => MaxBar = double.Parse(BarLimit.Text));

                    double MaxWater = double.MaxValue;
                    GetCurrentDispatcher().Invoke(() => MaxWater = double.Parse(WaterLimit.Text));

                    double MaxVoltage = double.MaxValue;
                    GetCurrentDispatcher().Invoke(() => MaxVoltage = double.Parse(VoltageLimit.Text));

                    AddSensor(SensorBar, new SensorData {
                        Id = 1,
                        Command = "01 04 00 03 00 01 C1 CA",
                        Name = "Датчик давления",
                        Handler = SensorHandler.CountBar,
                        Max = MaxBar
                    });

                    AddSensor(SensorLight, new SensorData {
                        Id = 2,
                        Command = "01 04 00 03 00 01 C1 CA",
                        Name = "Датчик напряжения",
                        Handler = SensorHandler.CountVoltage,
                        Max = MaxVoltage
                    });

                    AddSensor(SensorTemperature, new SensorData {
                        Id = 3,
                        Command = "01 04 00 03 00 01 C1 CA",
                        Name = "Датчик температуры",
                        Handler = SensorHandler.CountTemperature,
                        Max = MaxTemperature
                    });

                    AddSensor(SensorWater, new SensorData {
                        Id = 4,
                        Command = "01 04 00 03 00 01 C1 CA",
                        Name = "Датчик влажности",
                        Handler = SensorHandler.CountWater,
                        Max = MaxWater
                    });

                    if (Sensors.Any()) {
                        SensorDataListDb.SaveAll();

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

        private async void ReceivedData(object sender, SerialDataReceivedEventArgs e) {
            try {
                string? Answer = ActivePort?.Read();

                if (!string.IsNullOrEmpty(Answer)) {
                    double Result = Sensors.Current().Handler(Answer);

                    AppAndPortsLog(LoadResource("DataHandling") + ": " + Answer);
                    AppAndPortsLog(LoadResource("InDecryptedView") + ": " + Result);

                    double Difference = Result - Sensors.Current().Max;

                    if (Difference > 0) {
                        AddWarningNotification("Превышение допустимой нормы на " + Difference.ToString());
                    }

                    SensorDataListDb.AddSensorData(Sensors.Current().Id, Result.ToString());
                    SensorDataListDb.SaveAll();

                    IncrementGetStat();
                } else {
                    AppAndPortsLog(LoadResource("EmptyAnswer"));
                }

                if (IsLaunched) {
                    int Interval = 500;

                    GetCurrentDispatcher().Invoke(() => Interval = int.Parse(SendInterval.Text));

                    if (Interval > int.MaxValue) {
                        Interval = int.MaxValue;
                    }

                    if (Interval < 100) {
                        Interval = 100;
                    }

                    AppAndPortsLog(LoadResource("IntervalWaiting") + ": " + Interval);

                    await Task.Delay(Interval);

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

        private void SaveSettings(object sender, RoutedEventArgs e) {
            SaveSettings();
        }

        private void SaveSettings() {
            DictionaryHelper.UpdateDictionaryValue<string, string>(AppSettings.TextBoxValues, "TemperatureLimit", TemperatureLimit.Text);
            DictionaryHelper.UpdateDictionaryValue<string, string>(AppSettings.TextBoxValues, "WaterLimit", WaterLimit.Text);
            DictionaryHelper.UpdateDictionaryValue<string, string>(AppSettings.TextBoxValues, "VoltageLimit", VoltageLimit.Text);
            DictionaryHelper.UpdateDictionaryValue<string, string>(AppSettings.TextBoxValues, "BarLimit", BarLimit.Text);
            DictionaryHelper.UpdateDictionaryValue<string, string>(AppSettings.TextBoxValues, "SendInterval", SendInterval.Text);

            DictionaryHelper.UpdateDictionaryValue<string, bool>(AppSettings.CheckBoxValues, "SaveLogsToFile", SaveLogsToFile.IsChecked ?? false);

            DictionaryHelper.UpdateDictionaryValue<string, int>(AppSettings.ComboBoxValues, "comboBoxParity", comboBoxParity.SelectedIndex);
            DictionaryHelper.UpdateDictionaryValue<string, int>(AppSettings.ComboBoxValues, "comboBoxDataBits", comboBoxDataBits.SelectedIndex);
            DictionaryHelper.UpdateDictionaryValue<string, int>(AppSettings.ComboBoxValues, "comboBoxStopBit", comboBoxStopBit.SelectedIndex);
            DictionaryHelper.UpdateDictionaryValue<string, int>(AppSettings.ComboBoxValues, "comboBoxHandshake", comboBoxHandshake.SelectedIndex);
            DictionaryHelper.UpdateDictionaryValue<string, int>(AppSettings.ComboBoxValues, "comboBoxBaudRate", comboBoxBaudRate.SelectedIndex);

            DictionaryHelper.UpdateDictionaryValue<string, bool>(AppSettings.CheckBoxValues, "SensorTemperature", SensorTemperature.IsChecked ?? false);
            DictionaryHelper.UpdateDictionaryValue<string, bool>(AppSettings.CheckBoxValues, "SensorWater", SensorWater.IsChecked ?? false);
            DictionaryHelper.UpdateDictionaryValue<string, bool>(AppSettings.CheckBoxValues, "SensorBar", SensorBar.IsChecked ?? false);
            DictionaryHelper.UpdateDictionaryValue<string, bool>(AppSettings.CheckBoxValues, "SensorLight", SensorLight.IsChecked ?? false);

            AppSettings.Save(AppSettingsFile);

            OnlyAppLog(LoadResource("SettingsUpdated"));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            SaveSettings();
        }

        private void AppClose(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void FakeNoty(object sender, RoutedEventArgs e) {
            AddErrorNotification("Test error");
            AddWarningNotification("Test warning");
        }

        private void AppHide(object sender, RoutedEventArgs e) {
            Window window = GetWindow(this);

            if (window != null) {
                window.WindowState = WindowState.Minimized;
            }
        }

        private void CustomBorder_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                startPoint = e.GetPosition(null);
            }
        }

        private void CustomBorder_MouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Point mousePos = e.GetPosition(null);
                Vector diff = startPoint - mousePos;

                Window window = GetWindow(this);

                if (window != null) {
                    window.Left -= diff.X;
                    window.Top -= diff.Y;
                }
            }
        }

        private void CustomBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Window window = GetWindow(this);

            window?.DragMove();
        }

        private void ClearNotifications(object sender, RoutedEventArgs e) {
            UnreadNotifications = 0;
            NotificationsMenuItemLabel.Foreground = Brushes.White;
            NotificationsMenuItemLabel.Text = "Уведомления";

            NotificationsList.Children.Clear();
        }


        private void AddNotification(string Title, string Text, Color Type) {
            GetCurrentDispatcher().Invoke(() => {
                UnreadNotifications++;

                NotificationsList.Children.Insert(0, Notification.Show(Title, Text, Type));

                CheckNotifications();
            });
        }

        private void AddErrorNotification(string Text) {
            GetCurrentDispatcher().Invoke(() => {
                UnreadNotifications++;

                NotificationsList.Children.Insert(0, Notification.ShowError("Ошибка", Text));

                CheckNotifications();
            });
        }

        private void AddWarningNotification(string Text) {
            GetCurrentDispatcher().Invoke(() => {
                UnreadNotifications++;

                NotificationsList.Children.Insert(0, Notification.ShowWarning("Предупреждение", Text));

                CheckNotifications();
            });
        }

        private void CheckNotifications() {
            if (UnreadNotifications > 0) {
                NotificationsMenuItemLabel.Foreground = Brushes.IndianRed;
                NotificationsMenuItemLabel.Text = "Уведомления [+" + UnreadNotifications.ToString() + "]";
            }
        }

        private void StartAnalyze(object sender, RoutedEventArgs e) {
            IEnumerable<SensorView> Data = SensorDataListDb.GetByDate(StartDate.SelectedDate ?? new(), EndDate.SelectedDate ?? DateTime.Now);

            AnalyzeResults.Children.Clear();

            if (!Data.Any()) {
                Label label = new() {
                    Content = "Нет результатов для отображения",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,

                    Width = double.NaN,
                    Height = double.NaN
                };

                AnalyzeResults.Children.Add(label);

                return;
            }

            IEnumerable<int> Ids = Data.Select(Row => Row.SensorId).AsEnumerable().Distinct();

            if (Ids.Any()) {
                foreach (int Id in Ids) {
                    AnalyzerView AnalyzeResult = Analyzer.AnalyzeData(Data.Where(Row => Row.SensorId == Id).Select(Row => double.Parse(Row.SensorData)));

                    if (AnalyzeResult != null) {
                        string Name = Data.Where(Row => Row.SensorId == Id).Select(Row => Row.SensorName).FirstOrDefault() ?? "Датчик";

                        Label label = new() {
                            Content = Name,
                            FontSize = 16.0,
                            FontWeight = FontWeights.Bold,
                            Margin = new(5.0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,

                            Width = double.NaN
                        };

                        AnalyzeResults.Children.Add(label);

                        Label mean = new() {
                            Content = "Среднее значение = " + AnalyzeResult.Mean,
                            FontSize = 14.0,
                            FontWeight = FontWeights.SemiBold,
                            Margin = new(5.0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,

                            Width = double.NaN
                        };

                        AnalyzeResults.Children.Add(mean);

                        Label min = new() {
                            Content = "Минимальное значение = " + AnalyzeResult.Min,
                            FontSize = 14.0,
                            FontWeight = FontWeights.SemiBold,
                            Margin = new(5.0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,

                            Width = double.NaN
                        };

                        AnalyzeResults.Children.Add(min);

                        Label max = new() {
                            Content = "Максимальное значение = " + AnalyzeResult.Max,
                            FontSize = 14.0,
                            FontWeight = FontWeights.SemiBold,
                            Margin = new(5.0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,

                            Width = double.NaN
                        };

                        AnalyzeResults.Children.Add(max);

                        Label deviation = new() {
                            Content = "Средне-квадратичное отклонение = " + AnalyzeResult.StandardDeviation,
                            FontSize = 14.0,
                            FontWeight = FontWeights.SemiBold,
                            Margin = new(5.0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,

                            Width = double.NaN
                        };

                        AnalyzeResults.Children.Add(deviation);

                        AnalyzeResults.Children.Add(new Border() {
                            Margin = new(10.0)
                        });
                    }
                }
            }
        }
    }
}
