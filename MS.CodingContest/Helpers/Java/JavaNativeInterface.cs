using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MS.CodingContest.Helpers.Java
{
    public class JavaNativeInterface
    {
        private static readonly string JRE_REGISTRY_KEY = Registry.LocalMachine.Name + @"\SOFTWARE\JavaSoft\Java Runtime Environment";
        private static readonly string JRE_DEFAULT_VERSION = "1.8";

        private IntPtr javaClass;
        private IntPtr javaObject;
        private string javaClassName;
        private JavaVM jvm;
        private JNIEnv env;

        public bool AttachToCurrentJVMThread { get; set; }

        public void LoadVM(Dictionary<string, string> options, bool addToExistingJVM)
        {
            string jreVersion = (string)Registry.GetValue(JRE_REGISTRY_KEY, "CurrentVersion", JRE_DEFAULT_VERSION);
            string keyName = Path.Combine(JRE_REGISTRY_KEY, jreVersion);

            string jvmDir = (string)Registry.GetValue(keyName, "RuntimeLib", null);

            if ((jvmDir.Length == 0) || (!File.Exists(jvmDir)))
                throw new Exception("Error determining the location of the Java Runtime Environment");

            // Set the directory to the location of the JVM.dll. 
            // This will ensure that the API call JNI_CreateJavaVM will work
            Directory.SetCurrentDirectory(Path.GetDirectoryName(jvmDir));

            var args = new JavaVMInitArgs();

            //switch (Convert.ToInt32((decimal.Parse(jreVersion.Substring(0, 3)) - 1)/2*10))
            switch (Convert.ToInt32((decimal.Parse(jreVersion.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0]) / 2 * 10)))
            {
                case 0:
                    throw new Exception("Unsupported java version. Please upgrade your version of the JRE.");

                case 1:
                    args.version = JNIVersion.JNI_VERSION_1_2;
                    break;
                case 2:
                    args.version = JNIVersion.JNI_VERSION_1_4;
                    break;
                default:
                    args.version = JNIVersion.JNI_VERSION_1_8;
                    break;
            }

            args.ignoreUnrecognized = JavaVM.BooleanToByte(true); // True

            if (options.Count > 0)
            {
                args.nOptions = options.Count;
                var opt = new JavaVMOption[options.Count];
                int i = 0;
                foreach (KeyValuePair<string, string> kvp in options)
                {
                    opt[i++].optionString = Marshal.StringToHGlobalAnsi(kvp.Key.ToString() + "=" + kvp.Value.ToString());
                }

                unsafe
                {
                    fixed (JavaVMOption* a = &opt[0])
                    {
                        // prevents the garbage collector from relocating the opt variable as this is used in unmanaged code that the gc does not know about
                        args.options = a;
                    }
                }
            }

            if (!AttachToCurrentJVMThread)
            {
                IntPtr environment;
                IntPtr javaVirtualMachine;

                unsafe
                {
                    int result = JavaVM.JNI_CreateJavaVM(out javaVirtualMachine, out environment, &args);
                    if (result != JNIReturnValue.JNI_OK)
                    {
                        throw new Exception("Cannot create JVM " + result.ToString());
                    }
                }

                jvm = new JavaVM(javaVirtualMachine);
                env = new JNIEnv(environment);
            }
            else AttachToCurrentJVM(args);
        }

        private void AttachToCurrentJVM(JavaVMInitArgs args)
        {
            // This is only required if you want to reuse the same instance of the JVM
            // This is especially useful if you are using JNI in a webservice. see page 89 of the
            // Java Native Interface: Programmer's Guide and Specification by Sheng Liang
            if (AttachToCurrentJVMThread)
            {
                int nVMs;

                IntPtr javaVirtualMachine;
                int res = JavaVM.JNI_GetCreatedJavaVMs(out javaVirtualMachine, 1, out nVMs);
                if (res != JNIReturnValue.JNI_OK)
                {
                    throw new Exception("JNI_GetCreatedJavaVMs failed (" + res.ToString() + ")");
                }
                if (nVMs > 0)
                {
                    jvm = new JavaVM(javaVirtualMachine);
                    res = jvm.AttachCurrentThread(out env, args);
                    if (res != JNIReturnValue.JNI_OK)
                    {
                        throw new Exception("AttachCurrentThread failed (" + res.ToString() + ")");
                    }
                }
            }
        }
    }
}
