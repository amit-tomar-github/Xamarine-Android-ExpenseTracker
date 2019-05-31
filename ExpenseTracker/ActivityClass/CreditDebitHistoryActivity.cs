using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ExpenseTracker.DAL;
using ExpenseTracker.Models;
using MiposAndroid.CodeFile;

namespace ExpenseTracker.ActivityClass
{
    [Activity(Label = "CreditDebitHistoryActivity", WindowSoftInputMode = SoftInput.StateHidden, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class CreditDebitHistoryActivity : Activity
    {
        clsGlobal clsGLB;
        CreditDebitDb creditDebitDb;
        bool IsFilter = true;
        string _Type = "";
        int _HistoryTblId = 0;
        CreditDebitHistory _SelectedHistoryItem;

        RecyclerView recylerview_history;
        CreditDebitHistoryAdapter creditDebitHistoryAdapter;
        RecyclerView.LayoutManager mLayoutManager;
        RadioGroup radio_group;
        RadioButton radio_credit, radio_debit, radio_all;
        EditText editFromDate, editToDate;
        LinearLayout layout_FromDate, layout_ToDate, layout_Btn;
        Button btnView, btnBack;
        TextView txtTotalCredit, txtTotalDebit, txtTotalBal;
        public CreditDebitHistoryActivity()
        {
            try
            {
                clsGLB = new clsGlobal();
                creditDebitDb = new CreditDebitDb();
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
                SetContentView(Resource.Layout.activity_credit_debit_history_first);

                btnBack = FindViewById<Button>(Resource.Id.btnBack);
                btnBack.Click += BtnBack_Click;
                //Retrieve the data using Intent.GetStringExtra method  
                IsFilter = Intent.GetBooleanExtra("IsFilter", true);
                _Type = Intent.GetStringExtra("Type");

                radio_group = FindViewById<RadioGroup>(Resource.Id.radio_group);
                layout_FromDate = FindViewById<LinearLayout>(Resource.Id.layout_FromDate);
                layout_ToDate = FindViewById<LinearLayout>(Resource.Id.layout_ToDate);
                layout_Btn = FindViewById<LinearLayout>(Resource.Id.layout_Btn);
                radio_credit = FindViewById<RadioButton>(Resource.Id.radio_credit);
                radio_debit = FindViewById<RadioButton>(Resource.Id.radio_debit);
                radio_all = FindViewById<RadioButton>(Resource.Id.radio_all);

                txtTotalCredit = FindViewById<TextView>(Resource.Id.txtTotalCredit);
                txtTotalDebit = FindViewById<TextView>(Resource.Id.txtTotalDebit);
                txtTotalBal = FindViewById<TextView>(Resource.Id.txtTotalBal);

                editFromDate = FindViewById<EditText>(Resource.Id.editFromDate);
                editFromDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                ImageButton imgbtnFromDate = FindViewById<ImageButton>(Resource.Id.imgbtnFromDate);
                imgbtnFromDate.Click += (s, e) =>
                {
                    try
                    {
                        DatePickerFragmentForActivity frag = DatePickerFragmentForActivity.NewInstance(delegate (DateTime time)
                        {
                            editFromDate.Text = time.ToString("dd-MMM-yyyy");
                        });
                        frag.Show(FragmentManager, DatePickerFragment.TAG);
                    }
                    catch (Exception ex)
                    {
                        clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
                    }
                };
                editToDate = FindViewById<EditText>(Resource.Id.editToDate);
                editToDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                ImageButton imgbtnToDate = FindViewById<ImageButton>(Resource.Id.imgbtnToDate);
                imgbtnToDate.Click += (s, e) =>
                {
                    try
                    {
                        DatePickerFragmentForActivity frag = DatePickerFragmentForActivity.NewInstance(delegate (DateTime time)
                        {
                            editToDate.Text = time.ToString("dd-MMM-yyyy");
                        });
                        frag.Show(FragmentManager, DatePickerFragment.TAG);
                    }
                    catch (Exception ex)
                    {
                        clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
                    }
                };
                btnView = FindViewById<Button>(Resource.Id.btnView);
                btnView.Click += BtnView_Click;


                if (IsFilter)
                {
                    radio_group.Visibility = ViewStates.Visible;
                    layout_FromDate.Visibility = ViewStates.Visible;
                    layout_ToDate.Visibility = ViewStates.Visible;
                    layout_Btn.Visibility = ViewStates.Visible;
                    btnView.Visibility = ViewStates.Visible;
                }
                else
                {
                    radio_group.Visibility = ViewStates.Gone;
                    layout_FromDate.Visibility = ViewStates.Gone;
                    layout_ToDate.Visibility = ViewStates.Gone;
                    layout_Btn.Visibility = ViewStates.Gone;
                    btnView.Visibility = ViewStates.Invisible;
                }

                recylerview_history = FindViewById<RecyclerView>(Resource.Id.recylerview_history);
                mLayoutManager = new LinearLayoutManager(this);
                recylerview_history.SetLayoutManager(mLayoutManager);

                if (IsFilter == false)
                {
                    GetHistoryAsync();
                }
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        private void BtnView_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(editFromDate.Text))
                {
                    Toast.MakeText(this, "Select From Date", ToastLength.Long).Show();
                    return;
                }
                if (string.IsNullOrEmpty(editToDate.Text))
                {
                    Toast.MakeText(this, "Select To Date", ToastLength.Long).Show();
                    return;
                }
                GetHistoryAsync();
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }
      
        private async void GetHistoryAsync()
        {
            var progressDialog = ProgressDialog.Show(this, "", "Loading...", true);
            try
            {
                string FromDate = "", ToDate = "", Type = "";
                decimal Credit = 0, Debit = 0, Bal = 0;
                if (IsFilter)
                {
                    FromDate = Convert.ToDateTime(editFromDate.Text).ToString("yyyy-MM-dd");
                    ToDate = Convert.ToDateTime(editToDate.Text).ToString("yyyy-MM-dd");
                    if (radio_credit.Checked)
                        Type = eType.CREDIT.ToString();
                    else if (radio_debit.Checked)
                        Type = eType.DEBIT.ToString();
                    else if (radio_all.Checked)
                    {
                        Type = eType.ALL.ToString();
                    }
                }
                else
                {
                    FromDate = DateTime.Now.ToString("yyyy-MM-")+"01";
                    ToDate = DateTime.Now.ToString("yyyy-MM-dd");
                    Type = _Type;
                }
                 var list = await Task.Run(() => creditDebitDb.GetHistory(FromDate, ToDate, Type));
             
                if (Type == eType.CREDIT.ToString())
                {
                    Credit = list.Sum(x => x.Amount);
                    ShowControl(true, false, false);
                }
                else if (Type == eType.DEBIT.ToString())
                {
                    Debit = list.Sum(x => x.Amount);
                    ShowControl(false, true, false);
                }
                else
                {
                    Credit = list.Sum(x => (x.Type == eType.CREDIT.ToString() ? x.Amount : 0));
                    Debit = list.Sum(x => (x.Type == eType.DEBIT.ToString() ? x.Amount : 0));
                    ShowControl(true, true, true);
                }

                Bal = Credit - Debit;

                txtTotalCredit.Text = "Credit : " + Math.Round(Credit, 2).ToString();
                txtTotalDebit.Text = "Debit : " + Math.Round(Debit, 2).ToString();
                txtTotalBal.Text = "Bal. : " + Math.Round(Bal, 2).ToString();

                SetListData(list);

                progressDialog.Hide();

            }
            catch (Exception ex)
            {
                progressDialog.Hide();
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }

        private void ShowControl(bool Credit, bool Debit, bool Bal)
        {
            try
            {
                txtTotalCredit.Visibility = Credit ? ViewStates.Visible : ViewStates.Gone;
                txtTotalDebit.Visibility = Debit ? ViewStates.Visible : ViewStates.Gone;
                txtTotalBal.Visibility = Bal ? ViewStates.Visible : ViewStates.Gone;
            }
            catch (Exception ex) { throw ex; }
        }
        private void SetListData(List<CreditDebitHistory> list)
        {
            try
            {
                creditDebitHistoryAdapter = new CreditDebitHistoryAdapter(list, this);
                creditDebitHistoryAdapter.ItemClick += CreditDebitHistoryAdapter_ItemClick;
                recylerview_history.SetAdapter(creditDebitHistoryAdapter);
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }

        private void CreditDebitHistoryAdapter_ItemClick(object sender, int e)
        {
            try
            {
                _SelectedHistoryItem = creditDebitHistoryAdapter.lst[e];
                ShowConfirmBox("Sach mein delete krna hai ya majak kar rhe ho ?",this);
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }
        public void ShowConfirmBox(string msg, Activity activity)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(activity);
            builder.SetTitle("Message");
            builder.SetMessage(msg);
            builder.SetCancelable(false);
            builder.SetPositiveButton("Haan", handllerOkButton);
            builder.SetNegativeButton("Nhi", handllerCancelButton);
            builder.Show();
        }
        void handllerOkButton(object sender, DialogClickEventArgs e)
        {
            try
            {
                creditDebitDb.DeleteCreditDebit(_SelectedHistoryItem);
                GetHistoryAsync();
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }
        void handllerCancelButton(object sender, DialogClickEventArgs e)
        {

        }
    }
    public class CreditDebitHistoryAdapter : RecyclerView.Adapter
    {
        public List<CreditDebitHistory> lst;
        Context context;
        public event EventHandler<int> ItemClick;
        public CreditDebitHistoryAdapter(List<CreditDebitHistory> para, Context cont)
        {
            this.lst = para;
            context = cont;
        }
        public override int ItemCount
        {
            get { return lst.Count; }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                CreditDebitHistoryViewHolder vh = holder as CreditDebitHistoryViewHolder;
                vh.txtDate.Text = lst[position].TypeDate;
                if (lst[position].Type == eType.CREDIT.ToString())
                    vh.txtAmount.SetTextColor(Android.Graphics.Color.ForestGreen);
                else
                    vh.txtAmount.SetTextColor(Android.Graphics.Color.Red);
                vh.txtAmount.Text = lst[position].Amount.ToString();
                vh.txtRemarks.Text = lst[position].Remarks;
            }
            catch (Exception ex) { Toast.MakeText(context, ex.Message, ToastLength.Long).Show(); }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            CreditDebitHistoryViewHolder vh = null;
            try
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.view_credit_debit_history, parent, false);
                vh = new CreditDebitHistoryViewHolder(itemView,OnClick);

            }
            catch (Exception ex) { Toast.MakeText(context, ex.Message, ToastLength.Long).Show(); }
            return vh;
        }

        private void OnClick(int obj)
        {
            try
            {
                if (ItemClick != null)
                    ItemClick(this, obj);
            }
            catch (Exception ex) { Toast.MakeText(context, ex.Message, ToastLength.Long).Show(); }
        }
    }

    public class CreditDebitHistoryViewHolder : RecyclerView.ViewHolder
    {
        public TextView txtDate { get; set; }
        public TextView txtAmt { get; set; }
        public TextView txtAmount { get; set; }
        public TextView txtRemarks { get; set; }
        public ImageButton imgbtnDelete { get; set; }
        public CreditDebitHistoryViewHolder(View itemview, Action<int> listener) : base(itemview)
        {
            try
            {
                txtDate = itemview.FindViewById<TextView>(Resource.Id.txtDate);
                txtAmt = itemview.FindViewById<TextView>(Resource.Id.txtAmt);
                txtAmount = itemview.FindViewById<TextView>(Resource.Id.txtAmount);
                txtRemarks = itemview.FindViewById<TextView>(Resource.Id.txtRemarks);
                imgbtnDelete = itemview.FindViewById<ImageButton>(Resource.Id.imgbtnDelete);
                imgbtnDelete.Click += (sender, e) => listener(base.Position);
            }
            catch (Exception ex) { throw ex; }
        }
    }
}