using MS.CodingContest.Helpers.Fence;
using MS.CodingContest.Helpers.Java;
using MS.CodingContest.Interfaces.Fence;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MS.CodingContest.FantasyName.Fence
{
    public class JavaWrapperParticipant : IFenceBuilderParticipant, IDisposable
    {
        private bool _disposed = false;
        private string _fantasyName = "Java";
        private double _fenceLength = 4; // dummy initial value
        private List<Cell> _trees = null;
        private static JavaNativeInterface _jni = null; // IDisposable
        private static string _javaClassName = "FenceBuilderParticipantSample";
        private static Dictionary<string, string> _pairs = new Dictionary<string, string>();

        #region Constructor
        public /*static*/ JavaWrapperParticipant()
        {
            _jni = new JavaNativeInterface();

            // Set the path where the java class is located
            AddJVMOption("-Djava.class.path", Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "JavaAssemblies"));

            _jni.LoadVM(_pairs, false);
            _jni.InstantiateJavaObject(_javaClassName);
        }

        public void Dispose()
        {
            if(_disposed)
            {
                return;
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // free native resources if there are any.
            // nothing to be done here

            if(disposing)
            {
                // free managed resources
                if (_jni != null)
                {
                    _jni.Dispose();
                    _jni = null;
                }
            }

            _disposed = true;
        }
        #endregion

        #region IFenceBuilderParticipant
        public string FantasyName
        {
            get
            {
                return _fantasyName;
            }
        }

        public double Encircle(IForestProvider forestProvider)
        {
            _trees = new List<Cell>(forestProvider.Trees);

            // ToDo: add your implementation here...
            List<object> olParameters = new List<object>();

            //string msg = Java.CallMethod<int>("AddTwoNumbers", "(IILjava/lang/String;)I", olParameters).ToString();

            Point[] trees = _trees.Select(item => new Point(item.Row, item.Column)).ToArray();

            olParameters.Clear();
            olParameters.Add(trees);

            double _fenceLength = _jni.CallMethod<double>("Encircle", "([ILjava/lang/Integer;)I", olParameters);

            return _fenceLength;
        }

        public List<int> ShowSolution()
        {
            int[] treeIndexes = _jni.CallMethod<int[]>("ShowSolution", "([ILjava/lang/Void;)I", null);

            return new List<int>(treeIndexes);
        }
        #endregion

        #region Helpers
        private static void AddJVMOption(string name, string value)
        {
            if (!_pairs.ContainsKey(name))
                _pairs.Add(name, value);

            //   jvmOptions.Add(name + "=" + value);
        }
        #endregion
    }
}
