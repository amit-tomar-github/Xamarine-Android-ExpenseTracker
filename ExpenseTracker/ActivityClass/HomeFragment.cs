using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using ExpenseTracker.DAL;
using ExpenseTracker.Models;

namespace ExpenseTracker.ActivityClass
{
    public class HomeFragment : Android.Support.V4.App.Fragment
    {
        clsGlobal clsGLB;
        CreditDebitDb creditDebitDb;

        TextView txtCurrentMonth;
        Button btnCredit, btnDebit, btnViewHistory, btnBal;
        public HomeFragment()
        {
            try
            {
                clsGLB = new clsGlobal();
                creditDebitDb = new CreditDebitDb();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Activity, ex.Message, ToastLength.Long).Show();
            }
        }
        public static HomeFragment NewInstance()
        {
            return new HomeFragment();
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var FragmentView = inflater.Inflate(Resource.Layout.fragment_home, container, false);
            try
            {
                txtCurrentMonth = FragmentView.FindViewById<TextView>(Resource.Id.txtCurrentMonth);
                txtCurrentMonth.Text = DateTime.Now.ToString("MMM-yyyy");

                btnCredit = FragmentView.FindViewById<Button>(Resource.Id.btnCredit);
                btnCredit.Click += btnCredit_Click;

                btnDebit = FragmentView.FindViewById<Button>(Resource.Id.btnDebit);
                btnDebit.Click += btnDebit_Click;

                btnBal = FragmentView.FindViewById<Button>(Resource.Id.btnBalance);
                btnBal.Click += BtnBal_Click;

                btnViewHistory = FragmentView.FindViewById<Button>(Resource.Id.btnViewHistory);
                btnViewHistory.Click += btnViewHisotry_Click;

                GetCurrentMonthCreditDebitAsync();
            }
            catch (Exception ex1)
            {
                clsGLB.ShowMessage(ex1.Message, Activity, MessageTitle.ERROR);
            }
            return FragmentView;
        }

        private void BtnBal_Click(object sender, EventArgs e)
        {
            clsGLB.ShowMessage("Bas itna hi bacha hai", Activity, MessageTitle.INFORMATION);
        }

        #region Events
        private void btnCredit_Click(object sender, EventArgs e)
        {
            try
            {
                clsGLB.ShowMessage("Garibo ke pass paise nhi hote", Activity, MessageTitle.INFORMATION);
                OpenActivity(false, eType.CREDIT.ToString());
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, Activity, MessageTitle.ERROR);
            }
        }

        private void btnDebit_Click(object sender, EventArgs e)
        {
            try
            {
                clsGLB.ShowMessage("Jab paise hi nhi toh debit kya hoga", Activity, MessageTitle.INFORMATION);
                OpenActivity(false, eType.DEBIT.ToString());
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, Activity, MessageTitle.ERROR);
            }
        }

        private void btnViewHisotry_Click(object sender, EventArgs e)
        {
            try
            {
                clsGLB.ShowMessage("History bhoot weak hai", Activity, MessageTitle.INFORMATION);
                OpenActivity(true, eType.CREDIT.ToString());
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, Activity, MessageTitle.ERROR);
            }
        }

        #endregion

        #region Methods

        private void OpenActivity(bool IsFilter, string Type)
        {
            Intent MenuIntent = new Intent(Activity, typeof(CreditDebitHistoryActivity));
            MenuIntent.PutExtra("IsFilter", IsFilter);
            MenuIntent.PutExtra("Type", Type);
            StartActivity(MenuIntent);
        }

        private async void GetCurrentMonthCreditDebitAsync()
        {
            var progressDialog = ProgressDialog.Show(Activity, "", "Loading...", true);
            try
            {
                string FromDate = DateTime.Now.ToString("yyyy-MM-") + "01";
                string ToDate = DateTime.Now.ToString("yyyy-MM-dd");
                // var listTotalCreditAmount = await Task.Run(() => creditDebitDb.GetMonthyCreditDebit(FromDate, ToDate));

                var listTotalCreditAmount = creditDebitDb.GetMonthyCreditDebit(FromDate, ToDate);

                decimal CreditAmount = 0, DebitAmount = 0;
                foreach (var Item in listTotalCreditAmount)
                {
                    if (Item.Type == eType.CREDIT.ToString())
                        CreditAmount = Convert.ToDecimal(Item.Amount);
                    else if (Item.Type == eType.DEBIT.ToString())
                        DebitAmount = Convert.ToDecimal(Item.Amount);
                }

                btnCredit.Text = Math.Round(CreditAmount, 2).ToString();
                btnDebit.Text = Math.Round(DebitAmount, 2).ToString();

                btnBal.Text = Math.Round((CreditAmount - DebitAmount), 2).ToString();

                progressDialog.Hide();

            }
            catch (Exception ex)
            {
                progressDialog.Hide();
                clsGLB.ShowMessage(ex.Message, Activity, MessageTitle.ERROR);
            }
        }

        #endregion
    }
}