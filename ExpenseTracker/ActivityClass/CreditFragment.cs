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
    public class CreditFragment : Android.Support.V4.App.Fragment
    {
        clsGlobal clsGLB;
        CreditDebitDb creditDebitDb;

        EditText editDate, editAmount, editRemarks;
        TextView txtTotalCredit;
        Button btnAddAmount;
        public CreditFragment()
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
        public static CreditFragment NewInstance()
        {
            return new CreditFragment();
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var FragmentView = inflater.Inflate(Resource.Layout.fragment_credit, container, false);
            try
            {
                editDate = FragmentView.FindViewById<EditText>(Resource.Id.editDate);
                editDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                ImageButton imgbtnDate = FragmentView.FindViewById<ImageButton>(Resource.Id.imgbtnDate);
                imgbtnDate.Click += (s, e) =>
                {
                    try
                    {
                        DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                        {
                            editDate.Text = time.ToString("dd-MMM-yyyy");
                        });
                        frag.Show(FragmentManager, DatePickerFragment.TAG);
                    }
                    catch (Exception ex)
                    {
                        clsGLB.ShowMessage(ex.Message, Activity, MessageTitle.ERROR);
                    }
                };

                txtTotalCredit = FragmentView.FindViewById<TextView>(Resource.Id.txtTotalCredit);
                editAmount = FragmentView.FindViewById<EditText>(Resource.Id.editAmount);
                editRemarks = FragmentView.FindViewById<EditText>(Resource.Id.editRemarks);
                btnAddAmount = FragmentView.FindViewById<Button>(Resource.Id.btnAddAmount);
                btnAddAmount.Click += BtnAddAmount_Click;

                GetTotalCreditAmountAsync();
            }
            catch (Exception ex1)
            {
                clsGLB.ShowMessage(ex1.Message, Activity, MessageTitle.ERROR);
            }
            return FragmentView;
        }

        #region Events
        private void BtnAddAmount_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(editDate.Text))
                {
                    Toast.MakeText(Activity, "Select Date", ToastLength.Long).Show();
                    return;
                }
                if (string.IsNullOrEmpty(editAmount.Text) || Convert.ToDecimal(editAmount.Text) <= 0)
                {
                    Toast.MakeText(Activity, "Input amount", ToastLength.Long).Show();
                    editAmount.RequestFocus();
                    return;
                }
                AddCreditAsync();
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, Activity, MessageTitle.ERROR);
            }
        }

        #endregion

        #region Methods

        private async void GetTotalCreditAmountAsync()
        {
            var progressDialog = ProgressDialog.Show(Activity, "", "Loading...", true);
            try
            {
                string TotalCreditAmount = await Task.Run(() => creditDebitDb.GetTotalCreditAmount());
                txtTotalCredit.Text = TotalCreditAmount;
                progressDialog.Hide();
                Clear();
            }
            catch (Exception ex)
            {
                progressDialog.Hide();
                clsGLB.ShowMessage(ex.Message, Activity, MessageTitle.ERROR);
            }
        }

        private async void AddCreditAsync()
        {
            var progressDialog = ProgressDialog.Show(Activity, "", "Adding...", true);
            try
            {
                CreditDebitHistory creditDebitHistory = new CreditDebitHistory
                {
                    TypeDate = Convert.ToDateTime(editDate.Text).ToString("yyyy-MM-dd"),
                    Amount = Convert.ToDecimal(editAmount.Text),
                    OperationDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Remarks = editRemarks.Text,
                    Type = eType.CREDIT.ToString(),
                    UserId = clsGlobal.UserId
                };
                string TotalCreditAmount=await Task.Run(() => creditDebitDb.AddCredit(creditDebitHistory));
                txtTotalCredit.Text = TotalCreditAmount;
                progressDialog.Hide();
                Clear();
                Toast.MakeText(Activity, "Amount added successfully!!", ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                progressDialog.Hide();
                clsGLB.ShowMessage(ex.Message, Activity, MessageTitle.ERROR);
            }
        }

        private void Clear()
        {
            try
            {
                editDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                editAmount.Text = "";
                editRemarks.Text = "";
            }
            catch (Exception ex)
            { throw ex; }
        }

        #endregion
    }
}