using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using Logictracker.DatabaseTracer.Core;
using NHibernate;

namespace Logictracker.DAL.NHibernate
{
    public class SmartTransaction : IDisposable
    {
        private ITransaction _actualTransaction;
        private ISession _actualSession;
        private bool _newTransaction = false;

        public static SmartTransaction BeginTransaction()
        {
            var st = new SmartTransaction();
            return st;
        }

        public static SmartTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            var st = new SmartTransaction(isolationLevel);
            return st;
        }

        private void initSmartTransaction()
        {
            _actualSession = SessionHelper.Current;
            _actualTransaction = _actualSession.Transaction;

            _newTransaction = (_actualTransaction == null || !_actualTransaction.IsActive);

        }

        public SmartTransaction()
        {
            initSmartTransaction();
            if (_newTransaction)
                _actualTransaction = _actualSession.BeginTransaction();
        }

        public SmartTransaction(IsolationLevel isolationLevel)
        {
            initSmartTransaction();
            if (_newTransaction)
                _actualTransaction = _actualSession.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            if (_newTransaction)
            {
                try
                {
                    _actualTransaction.Commit();
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof(SmartTransaction).FullName, ex, "Exception doing CommitTransaction();");
                    throw ex;
                }
            }
        }

        public void Rollback()
        {
            if (_newTransaction)
            {
                try
                {
                    if (!_actualTransaction.IsActive)
                    {
                        throw new Exception("Transaction is not Active in RollbackTransaction();");
                    }
                    _actualTransaction.Rollback();
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof(SmartTransaction).FullName, ex, "Exception doing RollbackTransaction();");
                    throw ex;
                }
            }
        }

        public void Dispose()
        {
            _actualTransaction = null;
            _actualSession = null;
            _newTransaction = false;
        }
    }
}
