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

        public MainWindow() {
            InitializeComponent();
            this.initializeSharedMemoryReader();

            telemetryTimer = new Timer();
            telemetryTimer.AutoReset = true;
            telemetryTimer.Elapsed += telemetryController_elapsedAsync;
            telemetryTimer.Interval = 200;
            telemetryTimer.Stop();

            //this.GoogleSheetIDTextBox.SetBinding

            //this.GoogleSheetIDTextBox.SetBinding(running, new Binding() {
            //    Path = "Enabled",
            //    Source = running
            //});

            //Binding bind = new Binding {
            //    Source = running,
            //    ElementName
            //};
        }


        public async void doSomething(object sender, EventArgs e) {
            var stateLabel = (Label)this.FindName("StateText");
            var startButton = (Button)this.FindName("StartButton");
            var googleSheetURLTextBox = (TextBox)this.FindName("GoogleSheetIDTextBox");

            if (this.running) {
                stateLabel.Content = "Stopped.";
                startButton.Content = "Start";
                googleSheetURLTextBox.IsEnabled = true;
                ac.Stop();
            } else {
                List<String> errorStrings = new List<String>();
                if (errorStrings.Count > 0) {
                    MessageBox.Show(String.Join("\n", errorStrings));
                    return;
                }

                string pattern = @"https:\/\/docs\.google\.com\/spreadsheets\/d\/(.*)\/";
                RegexOptions options = RegexOptions.Multiline;

                var googleSheetId = Regex.Matches(googleSheetURLTextBox.Text, pattern, options)[0].Groups[1].Value;

                googleSheetURLTextBox.IsEnabled = false;
                stateLabel.Content = "Running...";
                startButton.Content = "Stop";
                this.gsController.initializeGoogleApi(
                    "SharedMemoryReader",
                    googleSheetId
                );

                await sheetController.init(gsController);

                ac.Start();
            }

            this.running = !running;
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
