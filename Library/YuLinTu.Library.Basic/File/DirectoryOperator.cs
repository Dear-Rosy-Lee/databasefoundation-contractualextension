using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLinTu.Library.Basic
{
    public abstract class DirectoryOperator
    {
        #region Events

        public event DirectoryOperationEventHandler OperationProcessing;
        public event DirectoryOperationEventHandler OperationCompleted;
        public event DirectoryOperationExceptionEventHandler OperationException;

        #endregion

        #region Fields

        private string _source;
        private string _destination;

        #endregion

        #region Ctor

        public DirectoryOperator()
        {
        }

        #endregion

        #region Methods

        #region Methods - Public

        public void Do(string source, string destination)
        {
            _source = source;
            _destination = destination;

            ToolFile.TraversalDirectoryContent(HandleDirectoryContent, source, OnGetPriorityChildValue());

            AlertOperationCompleted(source, destination, eDirectoryContentType.Directory);
        }

        #endregion

        #region Methods - Override

        protected virtual bool OnGetPriorityChildValue()
        {
            return false;
        }

        protected virtual void OnApplyContent(string source, string dest, eDirectoryContentType typeContent)
        {

        }

        #endregion

        #region Methods - Private

        private bool HandleDirectoryContent(string source, eDirectoryContentType typeContent)
        {
            string offset = source.Replace(_source, string.Empty);
            if (offset.StartsWith("\\"))
                offset = offset.Remove(0, 1);

            string dest = Path.Combine(_destination, offset);
            bool isCancel = false;
            try
            {
                AlertOperationProcessing(source, dest, typeContent);

                OnApplyContent(source, dest, typeContent);
            }
            catch (Exception ex)
            {
                isCancel = AlertOperationException(source, dest, typeContent, ex);
            }

            return !isCancel;
        }

        private void AlertOperationProcessing(string source, string dest, eDirectoryContentType typeContent)
        {
            if (OperationProcessing != null)
                OperationProcessing(this, new DirectoryOperationEventArgs(source, dest, typeContent));
        }

        private void AlertOperationCompleted(string source, string dest, eDirectoryContentType typeContent)
        {
            if (OperationCompleted != null)
                OperationCompleted(this, new DirectoryOperationEventArgs(source, dest, typeContent));
        }

        private bool AlertOperationException(string source, string dest, eDirectoryContentType typeContent, Exception ex)
        {
            bool isCancel = false;

            if (OperationException != null)
            {
                DirectoryOperationExceptionEventArgs e = new DirectoryOperationExceptionEventArgs(source, dest, typeContent, ex);
                OperationException(this, e);
                isCancel = e.IsCancel;
            }

            return isCancel;
        }

        #endregion

        #endregion
    }
}
