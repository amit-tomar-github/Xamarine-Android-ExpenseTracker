using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ExpenseTracker.Models;
using SQLite;
namespace ExpenseTracker.DAL
{
    class CreditDebitDb
    {
        //DateFormat in sqllite is YYYY-MM-DD
        StringBuilder _SbQry;
        public bool CreateDatabase()
        {
            try
            {
                string folderPath = Path.Combine(clsGlobal.FilePath, clsGlobal.FileFolder);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                using (var connection = new SQLiteConnection(clsGlobal.DBFilePath))
                {
                    var tableName = connection.GetTableInfo("CreditDebitHistory");
                    if (tableName.Count == 0)
                        connection.CreateTable<CreditDebitHistory>();

                    return true;
                }
            }
            catch (SQLiteException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex) { throw ex; }
        }

        public string AddCredit(CreditDebitHistory item)
        {
            try
            {
                using (var connection = new SQLiteConnection(clsGlobal.DBFilePath))
                {
                    connection.Insert(item);
                    return GetTotalCreditAmount();
                }
            }
            catch (SQLiteException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex) { throw ex; }
        }

        public string GetTotalCreditAmount()
        {
            try
            {
                _SbQry = new StringBuilder();
                using (var connection = new SQLiteConnection(clsGlobal.DBFilePath))
                {
                    _SbQry.AppendLine("SELECT SUM(AMOUNT) Amount,Type FROM CreditDebitHistory group BY Type Order By Type");
                    var Item = connection.Query<TotalCreditAmount>(_SbQry.ToString());
                    decimal CreditAmount = 0, DebitAmount = 0;
                    foreach (var li in Item)
                    {
                        if (li.Type==eType.CREDIT.ToString())
                            CreditAmount = Convert.ToDecimal(li.Amount);
                        else if (li.Type == eType.DEBIT.ToString())
                            DebitAmount = Convert.ToDecimal(li.Amount);
                    }
                    string TotalAmount = Math.Round((CreditAmount - DebitAmount), 2).ToString();
                    return TotalAmount;
                }
            }
            catch (SQLiteException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex) { throw ex; }
        }

        public List<TotalCreditAmount> GetMonthyCreditDebit(string FromDate, string ToDate)
        {
            try
            {
                _SbQry = new StringBuilder();
                using (var connection = new SQLiteConnection(clsGlobal.DBFilePath))
                {
                    _SbQry.AppendLine("SELECT SUM(AMOUNT) Amount,Type FROM CreditDebitHistory Where TypeDate >='" + FromDate + "'");
                    _SbQry.AppendLine(" And TypeDate <= '" + ToDate + "' group BY Type Order By Type");
                    var Item = connection.Query<TotalCreditAmount>(_SbQry.ToString());
                    return Item;
                }
            }
            catch (SQLiteException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex) { throw ex; }
        }

        public List<CreditDebitHistory> GetHistory(string FromDate, string ToDate, string Type)
        {
            try
            {
                _SbQry = new StringBuilder();
                using (var connection = new SQLiteConnection(clsGlobal.DBFilePath))
                {
                    _SbQry.AppendLine("SELECT Id,Amount,Type,Remarks,TypeDate FROM CreditDebitHistory Where TypeDate >='" + FromDate + "'");
                    _SbQry.AppendLine("And TypeDate <= '" + ToDate + "'");
                    if (Type == eType.ALL.ToString())
                        _SbQry.AppendLine("And Type In ('" + eType.CREDIT.ToString() + "','" + eType.DEBIT.ToString() + "')");
                    else
                        _SbQry.AppendLine("And Type In ('" + Type + "')");
                    _SbQry.AppendLine("  Order By TypeDate");

                    var Item = connection.Query<CreditDebitHistory>(_SbQry.ToString());
                   
                    return Item;
                }
            }
            catch (SQLiteException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex) { throw ex; }
        }

        public bool AddDebit(CreditDebitHistory item)
        {
            try
            {
                using (var connection = new SQLiteConnection(clsGlobal.DBFilePath))
                {
                    connection.Insert(item);
                    return true;
                }
            }
            catch (SQLiteException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex) { throw ex; }
        }

        public int DeleteCreditDebit(CreditDebitHistory item)
        {
            try
            {
                using (var connection = new SQLiteConnection(clsGlobal.DBFilePath))
                {
                  return  connection.Delete(item);
                }
            }
            catch (SQLiteException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex) { throw ex; }
        }
    }
}