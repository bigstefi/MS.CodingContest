﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MS.CodingContest.Helpers.Java
{
    public unsafe class JavaVM : IDisposable
    {
        [DllImport("jvm.dll", CallingConvention = JavaVM.CC)]
        internal static extern int JNI_CreateJavaVM(out IntPtr pVM, out IntPtr pEnv, JavaVMInitArgs* Args);

        [DllImport("jvm.dll", CallingConvention = JavaVM.CC)]
        internal static extern int JNI_GetDefaultJavaVMInitArgs(JavaVMInitArgs* args);

        [DllImport("jvm.dll", CallingConvention = JavaVM.CC)]
        internal static extern int JNI_GetCreatedJavaVMs(out IntPtr pVM, int jSize1, [Out] out int jSize2);



        // We need to have delegates for each function pointer for the methods 
        // in the JavaVM structure in the DLL
        public struct JNIInvokeInterface_
        {
            [UnmanagedFunctionPointer(JavaVM.CC)]
            [SuppressUnmanagedCodeSecurity]
            internal delegate int DestroyJavaVM(IntPtr pVM);

            [UnmanagedFunctionPointer(JavaVM.CC)]
            [SuppressUnmanagedCodeSecurity]
            internal delegate int AttachCurrentThread(IntPtr pVM, out IntPtr pEnv, JavaVMInitArgs* Args);

            [UnmanagedFunctionPointer(JavaVM.CC)]
            [SuppressUnmanagedCodeSecurity]
            internal delegate int DetachCurrentThread(IntPtr pVM);

            [UnmanagedFunctionPointer(JavaVM.CC)]
            [SuppressUnmanagedCodeSecurity]
            internal delegate int GetEnv(IntPtr pVM, out IntPtr pEnv, int Version);
            // J2SDK1_4
            [UnmanagedFunctionPointer(JavaVM.CC)]
            [SuppressUnmanagedCodeSecurity]
            internal delegate int AttachCurrentThreadAsDaemon(IntPtr pVM, out IntPtr pEnv, JavaVMInitArgs* Args);
        }

        // Have a structure that mimic the same structure of all the methods and offsets of each of the methods
        // in the JavaVM structure in the DLL
        [StructLayout(LayoutKind.Sequential), NativeCppClass]
        public struct JNIInvokeInterface
        {
            public IntPtr reserved0;
            public IntPtr reserved1;
            public IntPtr reserved2;

            public IntPtr DestroyJavaVM;
            public IntPtr AttachCurrentThread;
            public IntPtr DetachCurrentThread;
            public IntPtr GetEnv;
            public IntPtr AttachCurrentThreadAsDaemon;
        }

        [StructLayout(LayoutKind.Sequential, Size = 4), NativeCppClass]
        private struct JNIInvokeInterfacePtr
        {
            public readonly JNIInvokeInterface* functions;
        }

        private JNIInvokeInterface_.AttachCurrentThread _attachCurrentThread;
        private JNIInvokeInterface_.AttachCurrentThreadAsDaemon _attachCurrentThreadAsDaemon;
        private JNIInvokeInterface_.DestroyJavaVM _destroyJavaVm;
        private JNIInvokeInterface_.DetachCurrentThread _detachCurrentThread;
        private JNIInvokeInterface_.GetEnv _getEnv;

        private IntPtr _jvm;
        private JNIInvokeInterface _functions;

        public const CallingConvention CC = CallingConvention.Winapi;

        public JavaVM(IntPtr pointer)
        {
            this._jvm = pointer;
            _functions = *(*(JNIInvokeInterfacePtr*)_jvm.ToPointer()).functions;
        }

        public static bool ByteToBoolean(byte b)
        {
            return b != JNIBooleanValue.JNI_FALSE ? true : false;
        }

        public static byte BooleanToByte(bool value)
        {
            return value ? (byte)JNIBooleanValue.JNI_TRUE : (byte)JNIBooleanValue.JNI_FALSE;
        }

        public static void GetDelegateForFunctionPointer<T>(IntPtr ptr, ref T res)
        {  // Converts an unmanaged function pointer to a delegate.
            res = (T)(object)Marshal.GetDelegateForFunctionPointer(ptr, typeof(T));
        }

        internal int AttachCurrentThread(out JNIEnv penv, JavaVMInitArgs? args)
        {
            if (_attachCurrentThread == null)
            {
                //attachCurrentThread = (JNIInvokeInterface_.AttachCurrentThread)Marshal.GetDelegateForFunctionPointer(functions.AttachCurrentThread, typeof(JNIInvokeInterface_.AttachCurrentThread));
                GetDelegateForFunctionPointer(_functions.AttachCurrentThread, ref _attachCurrentThread);
            }
            IntPtr env;
            int result;
            if (args.HasValue)
            {
                JavaVMInitArgs initArgs = args.Value;
                result = _attachCurrentThread.Invoke(_jvm, out env, &initArgs);
            }
            else
            {
                result = _attachCurrentThread.Invoke(_jvm, out env, null);
            }
            penv = new JNIEnv(env);
            return result;
        }

        // This is only available in JNI_VERSION_1_4 or higher.
        [SuppressUnmanagedCodeSecurity]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public int AttachCurrentThreadAsDaemon(out JNIEnv penv, JavaVMInitArgs? args)
        {
            if (_attachCurrentThreadAsDaemon == null)
            {
                GetDelegateForFunctionPointer(_functions.AttachCurrentThreadAsDaemon,
                                                            ref _attachCurrentThreadAsDaemon);
            }
            IntPtr env;
            int result;
            if (!args.Equals(null))
            {
                JavaVMInitArgs value = args.Value;
                result = _attachCurrentThreadAsDaemon.Invoke(_jvm, out env, &value);
            }
            else
            {
                result = _attachCurrentThreadAsDaemon.Invoke(_jvm, out env, null);
            }
            if (result == JNIReturnValue.JNI_OK)
            {
                penv = new JNIEnv(env);
            }
            else
            {
                penv = null;
            }
            return result;
        }

        public int DestroyJavaVM()
        {
            if (_destroyJavaVm == null)
            {
                GetDelegateForFunctionPointer(_functions.DestroyJavaVM, ref _destroyJavaVm);
            }
            return _destroyJavaVm.Invoke(_jvm);
        }

        public int DetachCurrentThread()
        {
            if (_detachCurrentThread == null)
            {
                GetDelegateForFunctionPointer(_functions.DetachCurrentThread, ref _detachCurrentThread);
            }
            return _detachCurrentThread.Invoke(_jvm);
        }

        public int GetEnv(out JNIEnv penv, int version)
        {
            if (_getEnv == null)
            {
                GetDelegateForFunctionPointer(_functions.GetEnv, ref _getEnv);
            }
            IntPtr env;
            int result = _getEnv.Invoke(_jvm, out env, version);
            penv = new JNIEnv(env);
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~JavaVM() { Dispose(false); }

        protected virtual void Dispose(bool disposing)
        {
            if (_jvm != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(_jvm);
                _jvm = IntPtr.Zero;
            }
        }
    }
}
