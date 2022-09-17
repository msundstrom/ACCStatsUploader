using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ACCStatsUploader {
    


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public bool running = false;
        AssettoCorsa ac = new AssettoCorsa();
        SheetsAPIController gsController = new SheetsAPIController();
        SheetController sheetController = new SheetController();
        private Timer telemetryTimer;

        TelemetryController telemetryController;

        public string googleSheetUrl { get; set; }

        public void test(object sender, TextChangedEventArgs e) {
            var tb = (TextBox)sender;

            var connectToGSButton = (Button)this.FindName("ConnectToGSButton");
            string pattern = @"https:\/\/docs\.google\.com\/spreadsheets\/d\/(.*)\/";
            RegexOptions options = RegexOptions.Multiline;

            if (Regex.Matches(tb.Text, pattern, options).Count == 0) {
                connectToGSButton.IsEnabled = false;
            } else {
                connectToGSButton.IsEnabled = true;
            }
        }

        public MainWindow() {
            InitializeComponent();
            this.initializeSharedMemoryReader();

            telemetryTimer = new Timer();
            telemetryTimer.AutoReset = true;
            telemetryTimer.Elapsed += telemetryController_elapsedAsync;
            telemetryTimer.Interval = 200;
            telemetryTimer.Stop();

            this.DataContext = this;

            googleSheetUrl = "";
        }

        public async void ToggleGSConnectionButton_click(object sender, EventArgs e) {
            var googleSheetURLTextBox = (TextBox)this.FindName("GoogleSheetIDTextBox");
            var connectionLabel = (Label)this.FindName("GSStateText");
            var connectionButton = (Button)this.FindName("ConnectToGSButton");

            connectionLabel.Content = "Setting up...";
            connectionButton.IsEnabled = false;

            string pattern = @"https:\/\/docs\.google\.com\/spreadsheets\/d\/(.*)\/";
            RegexOptions options = RegexOptions.Multiline;

            var googleSheetId = Regex.Matches(googleSheetURLTextBox.Text, pattern, options)[0].Groups[1].Value;

            googleSheetURLTextBox.IsEnabled = false;
            this.gsController.initializeGoogleApi(
                "SharedMemoryReader",
                googleSheetId
            );

            var status = await sheetController.init(gsController);

            GSStateText.Content = status ? "Ready!" : "Disconnected";
            connectionButton.IsEnabled = status ? false : true;
        }

        public async void ToggleSharedMemoryReader(object sender, EventArgs e) {
            var stateLabel = (Label)this.FindName("ACStateText");
            var startButton = (Button)this.FindName("SharedMemoryToggleButton");

            if (ac.IsRunning) {
                stateLabel.Content = "Stopped.";
                startButton.Content = "Start";

                ac.Stop();
            } else {
                stateLabel.Content = "Started!";
                startButton.Content = "Stop";

                ac.Start();
            }
        }

        void initializeSharedMemoryReader() {
            this.ac.StaticInfoInterval = 200; // Get StaticInfo updates ever 5 seconds
            this.ac.PhysicsInterval = 200;
            this.ac.GraphicsInterval = 200;
            //this.ac.StaticInfoUpdated += ac_StaticInfoUpdated; // Add event listener for StaticInfo
            //this.ac.PhysicsUpdated += ac_PhysicsInfoUpdated;
            this.ac.GameStatusChanged += ac_GameStatusChanged;
        }

        void ac_GameStatusChanged(object sender, GameStatusEventArgs e) {
            Console.WriteLine("Game status changed: ", e.ToString());

            if (e.GameStatus == ACC_STATUS.ACC_LIVE) {
                telemetryController = new TelemetryController(sheetController);
                telemetryTimer.Start();
            }
        }

        private async void telemetryController_elapsedAsync(object sender, ElapsedEventArgs e) {
            Timer timer = sender as Timer;
            timer.Stop();

            telemetryController.newGraphics(ac.ReadGraphics());
            telemetryController.newPhysics(ac.ReadPhysics());
            telemetryController.newStaticInfo(ac.ReadStaticInfo());

            await telemetryController.update();

            timer.Start();
        }
    }
}
