using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public class Trace
    {
        #region Properties

        #region Properties - Static

        public static TraceDeviceCollection Devices
        {
            get { return instance.listDevice; }
        }

        #endregion

        #endregion

        #region Fields

        #region Fields - System

        private static readonly object objSync;
        private static Trace instance;
        private const int intMaxLogCount = 100;

        #endregion

        #region Fields - Function

        private TraceDeviceCollection listDevice;
        private LogCollection listErrors;

        #endregion

        #endregion

        #region Ctor

        static Trace()
        {
            objSync = new object();
            instance = new Trace();
        }

        public Trace()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        #region Methods - Static

        #region Methods - Static - System

        #endregion

        #region Methods - Static - Common

        #region Methods - Static - Common - Write

        public static void Write(int id, string description)
        {
            Log log = new Log(id, description);
            Write(log);
        }

        public static void WriteLine(int id, string description)
        {
            Log log = new Log(id, description);
            WriteLine(log);
        }

        public static void Write(string description)
        {
            Log log = new Log(-1, description);
            Write(log);
        }

        public static void WriteLine(string description)
        {
            Log log = new Log(-1, description);
            WriteLine(log);
        }

        public static void Write(Log log)
        {
            if (log == null)
                return;

            lock (objSync)
            {
                if (instance.listErrors.Count >= intMaxLogCount)
                    instance.listErrors.RemoveAt(0);

                instance.listErrors.Add(log);
            }

            WriteToDevices(log);
        }

        public static void WriteLine(Log log)
        {
            Write(log);
        }

        public static void WriteDebugOnly(int id, string description)
        {
#if DEBUG
            Log log = new Log(id, description);
            Write(log);
#endif
        }

        public static void WriteLineDebugOnly(int id, string description)
        {
#if DEBUG
            Log log = new Log(id, description);
            WriteLine(log);
#endif
        }

        public static void WriteDebugOnly(string description)
        {
#if DEBUG
            Log log = new Log(-1, description);
            Write(log);
#endif
        }

        public static void WriteLineDebugOnly(string description)
        {
#if DEBUG
            Log log = new Log(-1, description);
            WriteLine(log);
#endif
        }

        public static void WriteDebugOnly(Log log)
        {
#if DEBUG
            Write(log);
#endif
        }

        public static void WriteLineDebugOnly(Log log)
        {
#if DEBUG
            WriteLine(log);
#endif
        }

        #endregion

        #region Methods - Static - Common - Get

        public static Log GetLast()
        {
            if (instance.listErrors.Count == 0)
                return null;

            return instance.listErrors[instance.listErrors.Count - 1];
        }

        #endregion

        #region Methods - Static - Common - Other

        public static void Clear()
        {
            lock (objSync)
            {
                instance.listErrors.Clear();
            }

            ClearDevices();
        }

        #endregion

        #endregion

        #region Methods - Static - Helper

        private static void WriteToDevices(Log log)
        {
            foreach (ITraceDevice device in instance.listDevice)
                device.WriteLine(log);
        }

        private static void ClearDevices()
        {
            foreach (ITraceDevice device in instance.listDevice)
                device.Clear();
        }

        #endregion

        #region Methods - Initialize

        private void InitializeComponent()
        {
            listErrors = new LogCollection();
            listDevice = new TraceDeviceCollection();
        }

        #endregion

        #endregion

        #endregion
    }
}