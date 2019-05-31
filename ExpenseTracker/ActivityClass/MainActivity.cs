using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ExpenseTracker.ActivityClass;
using ExpenseTracker.DAL;
using ExpenseTracker.Models;
using System;
using System.IO;

namespace ExpenseTracker
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;
        clsGlobal clsGLB;
        CreditDebitDb creditDebitDb;
        static readonly int REQUEST_STORAGE = 1;
        public MainActivity()
        {
            try
            {
                clsGLB = new clsGlobal();
                creditDebitDb = new CreditDebitDb();
                //Set DBFilePath
                string folderPath = Path.Combine(clsGlobal.FilePath, clsGlobal.FileFolder);
                clsGlobal.DBFilePath = Path.Combine(folderPath, clsGlobal.DBFileName);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.activity_main);

                textMessage = FindViewById<TextView>(Resource.Id.message);
                BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
                navigation.SetOnNavigationItemSelectedListener(this);

                //Check Permission
                if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                {
                    // We have permission, go ahead and use the storage.
                    //Create Database if not exist
                    creditDebitDb.CreateDatabase();
                    LoadFragment(Resource.Id.navigation_home);
                }
                else
                {
                   // Storage permission is not granted. If necessary display rationale &request.

                    if (Android.Support.V4.App.ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.WriteExternalStorage))
                    {
                        // Provide an additional rationale to the user if the permission was not granted
                        // and the user would benefit from additional context for the use of the permission.
                        // For example if the user has previously denied the permission.
                        clsGLB.ShowMessage("Bina Permission ke kuch nhi krna chahiye...Application require storage permission to run,Go to Setting->Apps->ExpenseTracker->Permissions. Give all permission and restart the app", this, MessageTitle.INFORMATION);
                    }
                    else
                    {
                        Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission_group.Storage }, REQUEST_STORAGE);
                    }
                }
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }
       
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            try
            {
                LoadFragment(item.ItemId);
                return true;
            }
            catch (Exception ex) { throw ex; }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == REQUEST_STORAGE)
            {
                // Check if the only required permission has been granted
                if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                {
                    // Location permission has been granted, okay to retrieve the location of the device.
                    Toast.MakeText(this, "Storage permission has now been granted.", ToastLength.Long).Show();
                }
                else
                {
                    clsGLB.ShowMessage("Bina Permission ke kuch nhi krna chahiye...Application require storage permission to run,Go to Setting->Apps->ExpenseTracker->Permissions. Give all permission and restart the app", this, MessageTitle.INFORMATION);
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        void LoadFragment(int id)
        {
            try
            {
                Android.Support.V4.App.Fragment fragment = null;
                switch (id)
                {
                    case Resource.Id.navigation_home:
                        fragment = HomeFragment.NewInstance();
                        break;
                    case Resource.Id.navigation_credit:
                        fragment = CreditFragment.NewInstance();
                        break;
                    case Resource.Id.navigation_debit:
                        fragment = DebitFragment.NewInstance();
                        break;
                    case Resource.Id.navigation_notifications:
                        fragment = NotificationFragment.NewInstance();
                        break;
                }

                if (fragment == null)
                    return;

                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, fragment)
                    .Commit();
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }
    }
}

