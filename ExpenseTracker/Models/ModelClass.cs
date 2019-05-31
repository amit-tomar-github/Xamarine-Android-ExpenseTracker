using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SQLite;

namespace ExpenseTracker.Models
{
    public enum eType { CREDIT, DEBIT,ALL }

    #region CreditDebitHistory
    public class CreditDebitHistory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string TypeDate { get; set; }
        public string OperationDateTime { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }

    }

    #endregion

    #region TotalCreditAmount
    public class TotalCreditAmount
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
    }

    #endregion
}