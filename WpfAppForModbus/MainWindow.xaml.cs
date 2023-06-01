namespace WpfAppForModBus;

public partial class MainWindow : System.Windows.Window {
    private System.Windows.Point _startPoint;

    public MainWindow() {
        InitializeComponent();
        InitializeUi();
        InitializeSettings();
        InitializeBinders();
        InitializeContexts();
    }

    public WpfAppForModBus.Models.Core.ComPort? ActivePort { get; set; }
    public bool IsLaunched { get; set; }
    public WpfAppForModBus.Models.Core.Logger? AppLog { get; set; }
    public WpfAppForModBus.Models.Core.Logger? PortsLog { get; set; }
    private WpfAppForModBus.Models.Core.Settings AppSettings { get; set; } = null!;
    private WpfAppForModBus.Domain.Interfaces.ISensorDataList SensorDataListDb { get; set; } = null!;
    private WpfAppForModBus.Models.Core.SensorHandlers Sensors { get; set; } = null!;
    private int SendCount { get; set; }
    private int GetCount { get; set; }
    private int UnreadNotifications { get; set; }
    private string AppSettingsFile { get; } = "settings.json";

    private static System.Windows.Threading.Dispatcher CurrentDispatcher =>
        System.Windows.Application.Current.Dispatcher;

    public void InitializeContexts() {
        SensorDataListDb = new WpfAppForModBus.Domain.Models.SensorDataList(context: new());

        LoadSensors();
    }

    private void LoadSensors() {
        ReviewSensor.Items.Clear();

        WpfAppForModBus.Models.Helpers.ComboBoxHelper.AddRange(comboBox: ReviewSensor,
            items: SensorDataListDb.GetSensors());
    }

    private void LoadSensors(object sender, System.Windows.RoutedEventArgs e) {
        LoadSensors();
    }

    public void InitializeBinders() {
        BindComboBox(comboBox: ComboBoxBaudRate);
        BindComboBox(comboBox: ComboBoxDataBits);
        BindComboBox(comboBox: ComboBoxHandshake);
        BindComboBox(comboBox: ComboBoxParity);
        BindComboBox(comboBox: ComboBoxStopBit);
        BindComboBox(comboBox: RoundNumbersCount);

        BindCheckBox(checkBox: SensorBar);
        BindCheckBox(checkBox: SensorTemperature);
        BindCheckBox(checkBox: SensorWater);
        BindCheckBox(checkBox: SensorLight);

        BindCheckBox(checkBox: SaveLogsToFile);

        BindCheckBox(checkBox: RoundNumbers);
    }

    private void BindComboBox(System.Windows.Controls.ComboBox comboBox) {
        comboBox.SelectionChanged += SaveSettings;
    }

    private void BindCheckBox(System.Windows.Controls.Primitives.ToggleButton checkBox) {
        checkBox.Checked += SaveSettings;
        checkBox.Unchecked += SaveSettings;
    }

    public void InitializeSettings() {
        AppLog = new(currentDispatcher: CurrentDispatcher, element: Log, saveToFile: SaveLogsToFile);
        PortsLog = new(currentDispatcher: CurrentDispatcher, element: PortsLogBox);

        AppLog?.AddDatedLog(text: LoadResource(key: "SettingsLoading"));

        AppSettings = WpfAppForModBus.Models.Core.Settings.Load(filePath: AppSettingsFile);

        TemperatureLimit.Text =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.TextBoxValues,
                key: "TemperatureLimit", defaultValue: "55");
        WaterLimit.Text =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.TextBoxValues,
                key: "WaterLimit", defaultValue: "50");
        VoltageLimit.Text =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.TextBoxValues,
                key: "VoltageLimit", defaultValue: "500");
        BarLimit.Text =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.TextBoxValues,
                key: "BarLimit", defaultValue: "700");
        SendInterval.Text =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.TextBoxValues,
                key: "SendInterval", defaultValue: "500");

        SaveLogsToFile.IsChecked =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.CheckBoxValues,
                key: "SaveLogsToFile", defaultValue: false);

        string lastPort =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault<string, string>(
                dictionary: AppSettings.TextBoxValues, key: "comboBoxPorts",
                defaultValue: string.Empty);

        if (ComboBoxPorts.Items.Contains(containItem: lastPort)) {
            ComboBoxPorts.SelectedValue = lastPort;
        }

        ComboBoxParity.SelectedIndex =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.ComboBoxValues,
                key: "comboBoxParity", defaultValue: -1);
        ComboBoxDataBits.SelectedIndex =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.ComboBoxValues,
                key: "comboBoxDataBits", defaultValue: -1);
        ComboBoxStopBit.SelectedIndex =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.ComboBoxValues,
                key: "comboBoxStopBit", defaultValue: -1);
        ComboBoxHandshake.SelectedIndex =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.ComboBoxValues,
                key: "comboBoxHandshake", defaultValue: -1);
        ComboBoxBaudRate.SelectedIndex =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.ComboBoxValues,
                key: "comboBoxBaudRate", defaultValue: -1);
        RoundNumbersCount.SelectedIndex =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.ComboBoxValues,
                key: "RoundNumbersCount", defaultValue: -1);

        SensorTemperature.IsChecked =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.CheckBoxValues,
                key: "SensorTemperature", defaultValue: false);
        SensorWater.IsChecked =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.CheckBoxValues,
                key: "SensorWater", defaultValue: false);
        SensorBar.IsChecked =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.CheckBoxValues,
                key: "SensorBar", defaultValue: false);
        SensorLight.IsChecked =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.CheckBoxValues,
                key: "SensorLight", defaultValue: false);
        RoundNumbers.IsChecked =
            WpfAppForModBus.Models.Helpers.DictionaryHelper.GetValueOrDefault(dictionary: AppSettings.CheckBoxValues,
                key: "RoundNumbers", defaultValue: false);

        AppLog?.AddDatedLog(text: LoadResource(key: "SettingsLoaded"));
    }

    public void InitializeUi() {
        FillBoxes();

        WpfAppForModBus.Hooks.UiHooks.ClickElement(element: PortsMenuItemText);

        StartDate.SelectedDate = System.DateTime.Now;
        EndDate.SelectedDate = System.DateTime.Now;
    }

    public void FillBoxes() {
        AppLog?.AddDatedLog(text: LoadResource(key: "StartLoadingData"));

        WpfAppForModBus.Models.Helpers.ComboBoxHelper.AddRange(comboBox: ComboBoxParity,
            items: WpfAppForModBus.Models.Helpers.Shared.GetNames(list: WpfAppForModBus.Const.ParityList.Parities));
        WpfAppForModBus.Models.Helpers.ComboBoxHelper.AddRange(comboBox: ComboBoxHandshake,
            items: WpfAppForModBus.Models.Helpers.Shared.GetNames(list: WpfAppForModBus.Const.HandshakeList
                .Handshakes));
        WpfAppForModBus.Models.Helpers.ComboBoxHelper.AddRange(comboBox: ComboBoxStopBit,
            items: WpfAppForModBus.Models.Helpers.Shared.GetNames(
                list: WpfAppForModBus.Const.StopBitsList.StopBitsTypes));

        AppLog?.AddDatedLog(text: LoadResource(key: "StartLoadingPorts"));

        WpfAppForModBus.Models.Helpers.ComboBoxHelper.AddRange(comboBox: ComboBoxBaudRate,
            items: WpfAppForModBus.Const.BaudRateList.BaudRate);
        WpfAppForModBus.Models.Helpers.ComboBoxHelper.AddRange(comboBox: ComboBoxDataBits,
            items: WpfAppForModBus.Const.DataBitsList.DataBits);

        AppLog?.AddDatedLog(text: LoadResource(key: "LoadedData"));
    }

    private void LoadPorts(object sender, System.Windows.RoutedEventArgs e) {
        AppAndPortsLog(text: LoadResource(key: "RefreshedPorts"));
        LoadPorts();
    }

    private void LoadPorts() {
        ComboBoxPorts.Items.Clear();

        System.Collections.Generic.IEnumerable<string>
            ports = WpfAppForModBus.Models.Helpers.Shared.GetAvailablePorts();

        if (System.Linq.Enumerable.Any(source: ports)) {
            WpfAppForModBus.Models.Helpers.ComboBoxHelper.AddRange(comboBox: ComboBoxPorts, items: ports);
        }
        else {
            WpfAppForModBus.Models.Helpers.ModalDialogHelper.OpenModalWindow(windowTitle: "Внимание",
                contentText: "Нет ни одного доступного порта для подключения",
                iconKind: MaterialDesignThemes.Wpf.PackIconKind.Information, ownerWindow: this);
        }
    }

    private void AddSensor(System.Windows.Controls.Primitives.ToggleButton sensor,
        WpfAppForModBus.Models.Views.SensorData sensorHandler) {
        if (sensor is { IsChecked: true }) {
            Sensors.AddSensor(sensor: sensorHandler);
        }

        if (!SensorDataListDb.SensorExist(sensorId: sensorHandler.Id)) {
            SensorDataListDb.AddSensor(sensorId: sensorHandler.Id, sensorName: sensorHandler.Name);
        }
    }

    private string LoadResource(string key) => WpfAppForModBus.Models.Helpers.Shared.GetString(window: this, key: key);

    public bool IsConnected(WpfAppForModBus.Models.Core.ComPort? port) => port != null && port.IsOpened();

    private void MenuItem_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) {
        PortsContent.Visibility = System.Windows.Visibility.Collapsed;
        ReviewContent.Visibility = System.Windows.Visibility.Collapsed;
        LogContent.Visibility = System.Windows.Visibility.Collapsed;
        AnalyzeContent.Visibility = System.Windows.Visibility.Collapsed;
        SettingsContent.Visibility = System.Windows.Visibility.Collapsed;
        NotificationsContent.Visibility = System.Windows.Visibility.Collapsed;

        System.Windows.Controls.TextBlock? selectedMenuItem = sender as System.Windows.Controls.TextBlock;

        if (selectedMenuItem == PortsMenuItemText) {
            PortsContent.Visibility = System.Windows.Visibility.Visible;
        }
        else if (selectedMenuItem == ReviewMenuItemText) {
            ReviewContent.Visibility = System.Windows.Visibility.Visible;
        }
        else if (selectedMenuItem == LogMenuItemText) {
            LogContent.Visibility = System.Windows.Visibility.Visible;
        }
        else if (selectedMenuItem == AnalyzeMenuItemText) {
            AnalyzeContent.Visibility = System.Windows.Visibility.Visible;
        }
        else if (selectedMenuItem == SettingsMenuItemText) {
            SettingsContent.Visibility = System.Windows.Visibility.Visible;
        }
        else if (selectedMenuItem == NotificationsMenuItemText) {
            UnreadNotifications = 0;
            NotificationsMenuItemLabel.Foreground = System.Windows.Media.Brushes.White;
            NotificationsMenuItemLabel.Text = "Уведомления";
            NotificationsContent.Visibility = System.Windows.Visibility.Visible;
        }

        foreach (System.Windows.Controls.TextBlock menuItem in System.Linq.Enumerable
                     .OfType<System.Windows.Controls.TextBlock>(
                         source: LeftMenuStackPanel.Children)) {
            menuItem.FontWeight = menuItem == selectedMenuItem
                ? System.Windows.FontWeights.Bold
                : System.Windows.FontWeights.Normal;
            menuItem.Background = menuItem == selectedMenuItem
                ? System.Windows.Media.Brushes.Indigo
                : System.Windows.Media.Brushes.Transparent;
        }
    }

    private void UpdateStats() {
        Dispatcher.Invoke(callback: () => {
            StatCounterGet.Content = GetCount;
            StatCounterSend.Content = SendCount;
        });
    }

    private void IncrementGetStat() {
        Dispatcher.Invoke(callback: () => { GetCount++; });
    }

    private void IncrementSendStat() {
        Dispatcher.Invoke(callback: () => { SendCount++; });
    }

    private void AppAndPortsLog(string text) {
        PortsLog?.AddDatedLog(text: text);
        AppLog?.AddDatedLog(text: text);
    }

    private void OnlyAppLog(string text) {
        AppLog?.AddDatedLog(text: text);
    }

    private void OnlyPortsLog(string text) {
        PortsLog?.AddDatedLog(text: text);
    }

    private void Button_Connect(object sender, System.Windows.RoutedEventArgs e) {
        try {
            if (IsConnected(port: ActivePort)) {
                AppAndPortsLog(text: LoadResource(key: "AlreadyConnected"));

                return;
            }

            ActivePort = new();

            WpfAppForModBus.Models.Views.ComPortOptions options = new() {
                SelectedParity = WpfAppForModBus.Models.Helpers.ComboBoxHelper.GetSelectedItem(comboBox: ComboBoxParity,
                    items: WpfAppForModBus.Const.ParityList.Parities),
                SelectedHandshake =
                    WpfAppForModBus.Models.Helpers.ComboBoxHelper.GetSelectedItem(comboBox: ComboBoxHandshake,
                        items: WpfAppForModBus.Const.HandshakeList.Handshakes),
                SelectedBaudRate =
                    WpfAppForModBus.Models.Helpers.ComboBoxHelper.GetSelectedItem(comboBox: ComboBoxBaudRate,
                        items: WpfAppForModBus.Const.BaudRateList.BaudRate),
                SelectedDataBits =
                    WpfAppForModBus.Models.Helpers.ComboBoxHelper.GetSelectedItem(comboBox: ComboBoxDataBits,
                        items: WpfAppForModBus.Const.DataBitsList.DataBits),
                SelectedPort = WpfAppForModBus.Models.Helpers.ComboBoxHelper.GetSelectedItem(comboBox: ComboBoxPorts,
                    items: WpfAppForModBus.Models.Helpers.Shared.GetAvailablePorts()),
                SelectedStopBits =
                    WpfAppForModBus.Models.Helpers.ComboBoxHelper.GetSelectedItem(comboBox: ComboBoxStopBit,
                        items: WpfAppForModBus.Const.StopBitsList.StopBitsTypes),
                Handler = ReceivedData
            };

            ActivePort?.Open(options: options);

            if (IsConnected(port: ActivePort)) {
                AppAndPortsLog(text: LoadResource(key: "SuccessfulConnected"));

                Sensors = new();

                double maxTemperature = double.MaxValue;
                CurrentDispatcher.Invoke(callback: () => maxTemperature = double.Parse(s: TemperatureLimit.Text));

                double maxBar = double.MaxValue;
                CurrentDispatcher.Invoke(callback: () => maxBar = double.Parse(s: BarLimit.Text));

                double maxWater = double.MaxValue;
                CurrentDispatcher.Invoke(callback: () => maxWater = double.Parse(s: WaterLimit.Text));

                double maxVoltage = double.MaxValue;
                CurrentDispatcher.Invoke(callback: () => maxVoltage = double.Parse(s: VoltageLimit.Text));

                AddSensor(sensor: SensorBar, sensorHandler: new() {
                    Id = 1,
                    Command = "01 04 00 03 00 01 C1 CA",
                    Name = "Датчик давления",
                    Handler = WpfAppForModBus.Handlers.SensorHandler.CountBar,
                    Max = maxBar,
                    Recommendations = "Рекомендуется наполнить бак"
                });

                AddSensor(sensor: SensorLight, sensorHandler: new() {
                    Id = 2,
                    Command = "01 04 00 03 00 01 C1 CA",
                    Name = "Датчик напряжения",
                    Handler = WpfAppForModBus.Handlers.SensorHandler.CountVoltage,
                    Max = maxVoltage,
                    Recommendations = "Рекомендуется включить лампу"
                });

                AddSensor(sensor: SensorTemperature, sensorHandler: new() {
                    Id = 3,
                    Command = "01 04 00 03 00 01 C1 CA",
                    Name = "Датчик температуры",
                    Handler = WpfAppForModBus.Handlers.SensorHandler.CountTemperature,
                    Max = maxTemperature,
                    Recommendations = "Рекомендуется открыть фрамугу"
                });

                AddSensor(sensor: SensorWater, sensorHandler: new() {
                    Id = 4,
                    Command = "01 04 00 03 00 01 C1 CA",
                    Name = "Датчик влажности",
                    Handler = WpfAppForModBus.Handlers.SensorHandler.CountWater,
                    Max = maxWater,
                    Recommendations = "Рекомендуется включить полив"
                });

                if (Sensors.Any()) {
                    SensorDataListDb.SaveAll();

                    IsLaunched = true;

                    AppAndPortsLog(text: LoadResource(key: "StartConnection"));

                    SendData();

                    StartHandle.IsEnabled = false;
                    StopHandle.IsEnabled = true;
                }
                else {
                    IsLaunched = false;

                    ActivePort?.Close();
                    ActivePort = null;

                    throw new System.ArgumentException(message: LoadResource(key: "NoSensors"));
                }
            }
        }
        catch (System.ArgumentException) {
            AppAndPortsLog(text: LoadResource(key: "IncorrectConnectionData"));
        }
        catch (System.Exception ex) {
            AppAndPortsLog(text: "Exception connect: " + ex.Message);
            OnlyAppLog(text: !string.IsNullOrEmpty(value: ex.StackTrace) ? ex.StackTrace : "No StackTrace");
        }
    }

    public void SendData() {
        UpdateStats();

        AppAndPortsLog(text: LoadResource(key: "CurrentSensor") + ": " + Sensors.GetCurrentName());
        AppAndPortsLog(text: LoadResource(key: "SendingData") + ": " + Sensors.GetCurrentCommand());

        ActivePort?.Write(buffer: Sensors.GetCurrentCommand());

        IncrementSendStat();
    }

    private async void ReceivedData(object sender, System.IO.Ports.SerialDataReceivedEventArgs e) {
        try {
            string? answer = ActivePort?.Read();

            if (!string.IsNullOrEmpty(value: answer)) {
                double result = Sensors.Current().Handler(arg: answer);

                CurrentDispatcher.Invoke(callback: () => {
                    if (RoundNumbers.IsChecked == true) {
                        int numbersAfterPoint = (int)RoundNumbersCount.SelectedValue;

                        result = System.Math.Round(value: result, digits: numbersAfterPoint);
                    }
                });

                AppAndPortsLog(text: LoadResource(key: "DataHandling") + ": " + answer);
                AppAndPortsLog(text: LoadResource(key: "InDecryptedView") + ": " + result);

                double difference = result - Sensors.Current().Max;

                if (difference > 0) {
                    AddWarningNotification(text: Sensors.GetCurrentName() + ": превышение допустимой нормы на " +
                                                 difference +
                                                 System.Environment.NewLine + Sensors.GetCurrentRecommendations());
                }

                SensorDataListDb.AddSensorData(sensorId: Sensors.GetCurrentId(),
                    sensorData: result.ToString(provider: System.Globalization.CultureInfo.CurrentCulture));
                SensorDataListDb.SaveAll();

                IncrementGetStat();
            }
            else {
                AppAndPortsLog(text: LoadResource(key: "EmptyAnswer"));
            }

            if (!IsLaunched) {
                return;
            }

            int interval = 500;

            CurrentDispatcher.Invoke(callback: () => interval = int.Parse(s: SendInterval.Text));

            if (interval < 100) {
                interval = 100;
            }

            AppAndPortsLog(text: LoadResource(key: "IntervalWaiting") + ": " + interval);

            await System.Threading.Tasks.Task.Delay(millisecondsDelay: interval);

            Sensors.Next();

            SendData();
        }
        catch (System.Exception ex) {
            AppAndPortsLog(text: "Exception in StopHandle: " + ex.Message);
            OnlyPortsLog(text: !string.IsNullOrEmpty(value: ex.StackTrace) ? ex.StackTrace : "No StackTrace");
        }
    }

    private void StopHandle_Click(object sender, System.Windows.RoutedEventArgs e) {
        try {
            ActivePort?.Close();

            ActivePort = null;

            IsLaunched = false;

            if (IsConnected(port: ActivePort)) {
                return;
            }

            AppAndPortsLog(text: LoadResource(key: "SuccessfulStopped"));

            StartHandle.IsEnabled = true;
            StopHandle.IsEnabled = false;
        }
        catch (System.Exception ex) {
            ActivePort = null;

            IsLaunched = false;
            AppAndPortsLog(text: "Exception in StopHandle: " + ex.Message);
            OnlyPortsLog(text: !string.IsNullOrEmpty(value: ex.StackTrace) ? ex.StackTrace : "No StackTrace");
        }
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e) {
        AppLog?.ClearLog();
    }

    private void SaveSettings(object sender, System.Windows.RoutedEventArgs e) {
        SaveSettings();
    }

    private void SaveSettings() {
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, string>(
            dictionary: AppSettings.TextBoxValues, key: "TemperatureLimit",
            value: TemperatureLimit.Text);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, string>(
            dictionary: AppSettings.TextBoxValues, key: "WaterLimit",
            value: WaterLimit.Text);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, string>(
            dictionary: AppSettings.TextBoxValues, key: "VoltageLimit",
            value: VoltageLimit.Text);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, string>(
            dictionary: AppSettings.TextBoxValues, key: "BarLimit", value: BarLimit.Text);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, string>(
            dictionary: AppSettings.TextBoxValues, key: "SendInterval",
            value: SendInterval.Text);

        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, bool>(
            dictionary: AppSettings.CheckBoxValues, key: "SaveLogsToFile",
            value: SaveLogsToFile.IsChecked ?? false);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, bool>(
            dictionary: AppSettings.CheckBoxValues, key: "RoundNumbers",
            value: RoundNumbers.IsChecked ?? false);

        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, int>(
            dictionary: AppSettings.ComboBoxValues, key: "comboBoxParity",
            value: ComboBoxParity.SelectedIndex);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, string>(
            dictionary: AppSettings.TextBoxValues, key: "comboBoxPorts",
            value: (string)ComboBoxPorts.SelectedValue);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, int>(
            dictionary: AppSettings.ComboBoxValues, key: "comboBoxDataBits",
            value: ComboBoxDataBits.SelectedIndex);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, int>(
            dictionary: AppSettings.ComboBoxValues, key: "comboBoxStopBit",
            value: ComboBoxStopBit.SelectedIndex);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, int>(
            dictionary: AppSettings.ComboBoxValues, key: "comboBoxHandshake",
            value: ComboBoxHandshake.SelectedIndex);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, int>(
            dictionary: AppSettings.ComboBoxValues, key: "comboBoxBaudRate",
            value: ComboBoxBaudRate.SelectedIndex);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, int>(
            dictionary: AppSettings.ComboBoxValues, key: "RoundNumbersCount",
            value: RoundNumbersCount.SelectedIndex);

        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, bool>(
            dictionary: AppSettings.CheckBoxValues, key: "SensorTemperature",
            value: SensorTemperature.IsChecked ?? false);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, bool>(
            dictionary: AppSettings.CheckBoxValues, key: "SensorWater",
            value: SensorWater.IsChecked ?? false);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, bool>(
            dictionary: AppSettings.CheckBoxValues, key: "SensorBar",
            value: SensorBar.IsChecked ?? false);
        WpfAppForModBus.Models.Helpers.DictionaryHelper.UpdateDictionaryValue<string, bool>(
            dictionary: AppSettings.CheckBoxValues, key: "SensorLight",
            value: SensorLight.IsChecked ?? false);

        AppSettings.Save(filePath: AppSettingsFile);

        OnlyAppLog(text: LoadResource(key: "SettingsUpdated"));
    }

    private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e) {
        SaveSettings();
    }

    private void AppClose(object sender, System.Windows.RoutedEventArgs e) {
        System.Windows.Application.Current.Shutdown();
    }

    private void AppHide(object sender, System.Windows.RoutedEventArgs e) {
        System.Windows.Window? window = GetWindow(dependencyObject: this);

        if (window != null) {
            window.WindowState = System.Windows.WindowState.Minimized;
        }
    }

    private void CustomBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
            _startPoint = e.GetPosition(relativeTo: null);
        }
    }

    private void CustomBorder_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
            System.Windows.Point mousePos = e.GetPosition(relativeTo: null);
            System.Windows.Vector diff = _startPoint - mousePos;

            System.Windows.Window? window = GetWindow(dependencyObject: this);

            if (window != null) {
                window.Left -= diff.X;
                window.Top -= diff.Y;
            }
        }
    }

    private void CustomBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
        System.Windows.Window? window = GetWindow(dependencyObject: this);

        window?.DragMove();
    }

    private void ClearNotifications(object sender, System.Windows.RoutedEventArgs e) {
        UnreadNotifications = 0;
        NotificationsMenuItemLabel.Foreground = System.Windows.Media.Brushes.White;
        NotificationsMenuItemLabel.Text = "Уведомления";

        NotificationsList.Children.Clear();

        CheckNotifications();
    }

    private void AddWarningNotification(string text) {
        CurrentDispatcher.Invoke(callback: () => {
            UnreadNotifications++;

            NotificationsList.Children.Insert(index: 0,
                element: WpfAppForModBus.Models.Views.Notification.ShowWarning(title: "Предупреждение", text: text,
                    handle: CheckNotifications));

            CheckNotifications();
        });
    }

    private bool CheckNotifications() {
        int notifications = NotificationsList.Children.Count;

        if (notifications > 0) {
            NotificationCount.Content = "Количество уведомлений: " + notifications;
        }
        else {
            NotificationCount.Content = "Новых уведомлений нет";
        }

        if (UnreadNotifications > 0) {
            NotificationsMenuItemLabel.Foreground = System.Windows.Media.Brushes.IndianRed;
            NotificationsMenuItemLabel.Text = "Уведомления [+" + UnreadNotifications + "]";
        }

        return true;
    }

    private void AddAnalyzeRow(string text, double size = 14.0, double margin = 1.0) {
        AnalyzeResults.Children.Add(element: new System.Windows.Controls.Label {
            Content = text,
            FontSize = size,
            FontWeight = System.Windows.FontWeights.Bold,
            Margin = new(uniformLength: margin),
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
            VerticalAlignment = System.Windows.VerticalAlignment.Center,

            Width = double.NaN
        });
    }

    private void StartAnalyze(object sender, System.Windows.RoutedEventArgs e) {
        System.Collections.Generic.IEnumerable<WpfAppForModBus.Models.Views.SensorView> data =
            SensorDataListDb.GetByDate(start: StartDate.SelectedDate ?? new System.DateTime(),
                end: (EndDate.SelectedDate ?? System.DateTime.Now).AddHours(value: 23).AddMinutes(value: 59));

        AnalyzeResults.Children.Clear();

        System.Collections.Generic.IEnumerable<WpfAppForModBus.Models.Views.SensorView> sensorViews =
            data as WpfAppForModBus.Models.Views.SensorView[] ?? System.Linq.Enumerable.ToArray(source: data);

        if (!System.Linq.Enumerable.Any(source: sensorViews)) {
            System.Windows.Controls.Label label = new() {
                Content = "Нет результатов для отображения",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,

                Width = double.NaN,
                Height = double.NaN
            };

            AnalyzeResults.Children.Add(element: label);

            return;
        }

        System.Collections.Generic.IEnumerable<int> ids =
            System.Linq.Enumerable.Distinct(source: System.Linq.Enumerable.AsEnumerable(
                source: System.Linq.Enumerable.Select(source: sensorViews, selector: row => row.SensorId)));

        System.Collections.Generic.IEnumerable<int> enumerable =
            ids as int[] ?? System.Linq.Enumerable.ToArray(source: ids);

        if (!System.Linq.Enumerable.Any(source: enumerable)) {
            return;
        }

        {
            foreach (int id in enumerable) {
                WpfAppForModBus.Models.Views.AnalyzerView? analyzeResult =
                    WpfAppForModBus.Models.Analyzers.Analyzer.AnalyzeData(data: System.Linq.Enumerable.Select(
                        source: System.Linq.Enumerable.Where(source: sensorViews, predicate: row => row.SensorId == id),
                        selector: row => double.Parse(s: row.SensorData)));

                {
                    string name = System.Linq.Enumerable.FirstOrDefault(source: System.Linq.Enumerable.Select(
                                      source: System.Linq.Enumerable.Where(source: sensorViews,
                                          predicate: row => row.SensorId == id),
                                      selector: row => row.SensorName)) ??
                                  "Датчик";

                    AddAnalyzeRow(text: name, size: 16.0, margin: 5.0);
                    AddAnalyzeRow(text: "Среднее значение = " + analyzeResult.Mean);
                    AddAnalyzeRow(text: "Минимальное значение = " + analyzeResult.Min);
                    AddAnalyzeRow(text: "Максимальное значение = " + analyzeResult.Max);
                    AddAnalyzeRow(text: "Средне-квадратичное отклонение = " + analyzeResult.StandardDeviation);
                    AddAnalyzeRow(text: "Медиана = " + analyzeResult.Median);
                    AddAnalyzeRow(text: "Мода = " + analyzeResult.Mode);
                    AddAnalyzeRow(text: "Интерквартильный размах = " + analyzeResult.InterquartileRange);
                    AddAnalyzeRow(text: "Коэффициент вариации = " + analyzeResult.CoefficientOfVariation);
                    AddAnalyzeRow(text: "Дисперсия = " + analyzeResult.Dispersion);

                    AnalyzeResults.Children.Add(element: new System.Windows.Controls.Border {
                        Margin = new(uniformLength: 10.0)
                    });
                }
            }
        }
    }

    private void LoadSensorDataClick(object sender, System.Windows.RoutedEventArgs e) {
        LoadReviewData();
    }

    private void LoadReviewData() {
        ReviewResult.ItemsSource = null;
        ReviewResult.Columns.Clear();

        if (ReviewSensor.SelectedIndex > -1 && ReviewSensor.HasItems) {
            System.Collections.Generic.IEnumerable<WpfAppForModBus.Models.Views.SensorDataGridView> reviewList =
                SensorDataListDb.GetSensorData(sensorName: (string)ReviewSensor.SelectedValue);

            if (System.Linq.Enumerable.Any(source: reviewList)) {
                ReviewResult.Columns.Add(item: new MaterialDesignThemes.Wpf.DataGridTextColumn {
                    Header = "Данные",
                    Binding = new System.Windows.Data.Binding(path: "SensorData"),
                    Width = 200
                });

                ReviewResult.Columns.Add(item: new MaterialDesignThemes.Wpf.DataGridTextColumn {
                    Header = "Дата записи",
                    Binding = new System.Windows.Data.Binding(path: "RowDate")
                });

                ReviewResult.ItemsSource =
                    new System.Collections.ObjectModel.ObservableCollection<
                        WpfAppForModBus.Models.Views.SensorDataGridView>(collection: reviewList);
            }
        }
    }

    private void DeleteDataClick(object sender, System.Windows.RoutedEventArgs e) {
        if (ReviewSensor.SelectedIndex > -1 && ReviewSensor.HasItems) {
            System.DateTime? start = StartDeleteDate.SelectedDate;
            System.DateTime? end = EndDeleteDate.SelectedDate;

            int sensorId = SensorDataListDb.GetSensorId(sensorName: (string)ReviewSensor.SelectedValue);

            if (sensorId > 0 && start != null && end != null) {
                SensorDataListDb.DeleteByDate(sensorId: sensorId, start: (System.DateTime)start,
                    end: (System.DateTime)end);

                AppAndPortsLog(text: LoadResource(key: "DeletedData"));

                LoadReviewData();

                return;
            }
        }

        WpfAppForModBus.Models.Helpers.ModalDialogHelper.OpenModalWindow(windowTitle: "Ошибка",
            contentText: "Неверные параметры, возможно не выбран датчик, или не установлен период",
            iconKind: MaterialDesignThemes.Wpf.PackIconKind.Error);
    }
}