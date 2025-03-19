using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sedna.Service.Requirements.DTO;

namespace Senda.Requirements.Capture.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static public IConfiguration Config { get; set; }

        //https://riptutorial.com/wpf/example/25400/creating-splash-screen-window-with-progress-reporting
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Current.ShutdownMode = ShutdownMode.OnLastWindowClose;

            Popups.SplashScreen splashScreen = new Popups.SplashScreen();
            splashScreen.ShowInTaskbar = true;
            this.MainWindow = splashScreen;
            //splashScreen.Topmost = true;
            splashScreen.Show();

            //in order to ensure the UI stays responsive, we need to
            //do the work on a different thread
            Task.Factory.StartNew(() =>
            {
                IHostEnvironment env = null;

                var hostBuilder = new HostBuilder()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .ConfigureHostConfiguration(configurationBuilder => {
                        configurationBuilder.AddCommandLine(e.Args);
                    })
                    .ConfigureAppConfiguration((hostingContext, cfg) =>
                    {
                        env = hostingContext.HostingEnvironment;                       
                    });
                hostBuilder.Build();

                splashScreen.Dispatcher.Invoke(() => splashScreen.StatusMessage = "Gathering configuration info");

                var configurationBuilder = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.Staging.json", optional: true)
                    .AddEnvironmentVariables();

                Config = configurationBuilder.Build();
                Sedna.API.Client.Instance.SetBaseAddress(Config["Sedna:ApiBaseAddress"]);
                Sedna.API.Client.Instance.SetClient(Config["Sedna:ClientId"], Config["Sedna:ClientSecret"]);

                string api_client_id = Config["RequirementAPI:ClientAccessId"];
                string api_client_secret = Config["RequirementAPI:ClientAccessSecret"];
                string api_base_url = Config["RequirementAPI:BaseUrl"];


                splashScreen.Dispatcher.Invoke(() => splashScreen.StatusMessage = "Launching application");

                splashScreen.Dispatcher.Invoke(() => splashScreen.StatusMessage = "Preparing access to Sedna data");
                Instance.InitializeApiClient(api_client_id, api_client_secret, api_base_url);
                Instance.SetSlackChannelName(Config["Messaging:SlackChannelName"]);

                string sednaLoginToken = string.Empty;
                string launchExternalIdentifier = string.Empty;

                /* If we have arguments they ought to be
                 * 1 - the Sedna API token and
                 * 2 - the "OPP12345678" number for the desired opportunity
                 * We test the token to see if it's valid.  If it is, we 
                 * apply both, and the user goes right to work on that opportunity.
                 * If not, we show them the login screen.
                 */

                if (e.Args.Length >= 2)
                {
                    sednaLoginToken = e.Args[0];
                    launchExternalIdentifier = e.Args[1];

                    splashScreen.Dispatcher.Invoke(() => splashScreen.StatusMessage = "Testing Sedna token");

                    if (Instance.TestSednaApiToken(sednaLoginToken) == false)
                    {
                        // Failed so blank this variable out.
                        // User will get a new one via the login screen.
                        sednaLoginToken = string.Empty;
                    }
                }

                if (string.IsNullOrWhiteSpace(sednaLoginToken))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        splashScreen.Dispatcher.Invoke(() => splashScreen.StatusMessage = "Opening Sedna Login window");
                        System.Threading.Thread.Sleep(1);

                        Login login = new Login();
                        login.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        splashScreen.Topmost = false;
                        login.Topmost = true;
                        this.MainWindow = login;
                        if (login.ShowDialog() != true)
                        {
                            this.Shutdown();
                        }
                        else
                        {
                            splashScreen.Topmost = true;
                            sednaLoginToken = login.SednaLoginToken;
                        }
                    });
                }

                splashScreen.Dispatcher.Invoke(() => splashScreen.StatusMessage = "Storing Sedna token");
                Instance.SetSednaApiToken(sednaLoginToken);

                splashScreen.Dispatcher.Invoke(() => splashScreen.StatusMessage = "Loading requirements data");
                RequirementCaptureVM vm = new RequirementCaptureVM();
                vm.LoadReferenceData(launchExternalIdentifier);
                splashScreen.Dispatcher.Invoke(() => splashScreen.StatusMessage = "Preparing main window");


                //once we're done we need to use the Dispatcher
                //to create and show the main window
                this.Dispatcher.Invoke(() =>
                {
                    //initialize the main window, set it as the application main window
                    //and close the splash screen
                    var mainWindow = new MainWindow();
                    mainWindow.LoadSubjectDataAndWindow(launchExternalIdentifier, vm);
                    this.MainWindow = mainWindow;
                    mainWindow.WindowState = WindowState.Maximized;
                    mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    mainWindow.Show();

                    splashScreen.Close();
                });
            });



        }


        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Instance.SlackChannelName) == false)
                {
                    var baseExc = e.Exception.GetBaseException();
                    string stackTrace = baseExc.StackTrace;
                    string message = baseExc.Message;
                    string wholeException = baseExc.ToString();

                    Common.Slack.Send(Instance.SlackChannelName, "Requirements Capture Tool Exception", $"Message:\n{message}\n\nStackTrace:\n{stackTrace} \n\nWhole Exception:\n{wholeException}", Common.Slack.SlackEmoji.collision);
                    MessageBox.Show("Dang!  Something went wrong.  Sorry about that.\n\nWe sent the details to Tom Kelleher already, but feel free to call\\email\\pester him. It's his job.", "Confounded errors...", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
                }
            }
            catch (Exception ex)
            {
                // I don't like hiding errors, but if we hit one here, not much we can do
            }

            // Prevent default unhandled exception processing
            e.Handled = true;
        }
    }
}
