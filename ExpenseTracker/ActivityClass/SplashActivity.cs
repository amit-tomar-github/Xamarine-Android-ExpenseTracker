using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ExpenseTracker.ActivityClass
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/Theme.SplashNew", NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_splash);

            Context context = this.ApplicationContext;
            var versionName = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
            TextView txtAppVersion = FindViewById<TextView>(Resource.Id.txtAppVersion);
            txtAppVersion.Text = "App Version: " + versionName;
            //Display Splash Screen for 4 Sec  
            //Thread.Sleep(3000);
            ////Start Activity1 Activity  
            //StartActivity(typeof(MainActivity));
        }
        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Prevent the back button from canceling the startup process
        public override void OnBackPressed() { }

        // Simulates background work that happens behind the splash screen
        async void SimulateStartup()
        {
            await Task.Delay(4000); // Simulate a bit of startup work.
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}