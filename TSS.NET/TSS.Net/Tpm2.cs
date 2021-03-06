﻿/*++

Copyright (c) 2010-2015 Microsoft Corporation
Microsoft Confidential

*/
using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TbsResult = Tpm2Lib.TbsWrapper.TBS_RESULT;

namespace Tpm2Lib
{
    /// <summary>
    /// All tpm devices must derive from Tpm2Device.  TPM devices must forward
    /// TPM commands and other actions (e.g. assertion of physical-presence) to their
    /// associated TPM.  Note that not all TPM devices will be able to support all
    /// of the actions here.  In some cases the caller can query whether an action 
    /// is supported (e.g. can the TPM power state be programmatically cycled
    /// </summary>
    public abstract class Tpm2Device : IDisposable
    {
        /// <summary>
        /// Send TPM-command buffer to device
        /// </summary>
        /// <param name="active">Locality and priority</param>
        /// <param name="inBuf">command buffer</param>
        /// <param name="outBuf">response buffer</param>
        public virtual void DispatchCommand(CommandModifier active, byte[] inBuf, out byte[] outBuf)
        {
            outBuf = new byte[4];
            throw new Exception("Should never be here");
        }

        /// <summary>
        /// Connect to TPM device
        /// </summary>
        public virtual void Connect()
        {
            throw new Exception("Should never be here");
        }

        /// <summary>
        /// Power-cycle TPM device
        /// </summary>
        public virtual void PowerCycle()
        {
        }

        /// <summary>
        /// Return whether the TPM device supports sending/emulation of platform signals,
        /// and if the platform hierarchy is enabled. In particular platform signals
        /// are required to power-cycle the TPM.
        /// </summary>
        /// <returns>whether PowerCycle is implemented</returns>
        public virtual bool PlatformAvailable()
        {
            return false;
        }

        /// <summary>
        /// Assert physical presence on underlying device
        /// </summary>
        /// <param name="assertPhysicalPresence">true to assert PP, false to cancel assertion</param>
        public virtual void AssertPhysicalPresence(bool assertPhysicalPresence)
        {
            throw new Exception("AssertPhysicalPresence: Should not be here");
        }

        /// <summary>
        /// Return whether physical presence can be asserted
        /// </summary>
        public virtual bool ImplementsPhysicalPresence()
        {
            return false;
        }

        /// <summary>
        /// Return whether the TPM device is accessed via TBS.
        /// </summary>
        public virtual bool UsesTbs()
        {
            return false;
        }

        /// <summary>
        /// Return whether the TPM device implements Resource Management.
        /// </summary>
        public virtual bool HasRM()
        {
            return _HasRM;
        }

        // ReSharper disable once InconsistentNaming
        public bool _HasRM = false;

        // ReSharper disable once InconsistentNaming
        public bool _NeedsHMAC = true;

        /// <summary>
        /// Return true if the device requires HMAC authorization sessions. A rule of
        /// thumb is that HMAC session should be used when communication to TPM occurs
        /// via an untrusted channel. Otherwise password session suffices. 
        /// </summary>
        public bool NeedsHMAC
        {
            get
            {
                return _NeedsHMAC;
            }
            set
            {
                _NeedsHMAC = value;
            }
        }

        /// <summary>
        /// attempt to cancel any outstanding command
        /// </summary>
        public virtual void CancelContext()
        {
            throw new Exception("Should never be here");
        }

        /// <summary>
        /// Clean up
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // nothing...
            }
        }

        /// <summary>
        /// Return underlying handle (not all devices)
        /// </summary>
        /// <param name="p">pointer to handle</param>
        /// <returns>pointer to handle</returns>
        public virtual UIntPtr GetHandle(UIntPtr p)
        {
            return UIntPtr.Zero;
        }

        /// <summary>
        /// Specify that an error should be translated before beng propagated to caller
        /// (not all devices)
        /// </summary>
        /// <param name="r">alternative result</param>
        public virtual void SetAlternativeResult(Results r)
        {
        }

        /// <summary>
        /// Send hash-start signal
        /// </summary>
        public virtual void SignalHashStart()
        {
            throw new Exception("Should never be here");
        }

        /// <summary>
        /// hash data
        /// </summary>
        /// <param name="data"></param>
        public virtual void SignalHashData(byte[] data)
        {
            throw new Exception("Should never be here");
        }

        /// <summary>
        /// Send hash-end signal
        /// </summary>
        public virtual void SignalHashEnd()
        {
            throw new Exception("Should never be here");
        }

        /// <summary>
        /// Send new Endorsement Primary Seed to TPM simulator
        /// </summary>
        public virtual void TestFailureMode()
        {
            throw new Exception("Should never be here");
        }

        /// <summary>
        /// Return whether cancel is implemented
        /// </summary>
        /// <returns></returns>
        public virtual bool ImplementsCancel()
        {
            return false;
        }

        /// <summary>
        /// Send cancel-on signal
        /// </summary>
        public virtual void SignalCancelOn()
        {
            throw new Exception("Should never be here");
        }

        /// <summary>
        ///  Send cancel-off signal
        /// </summary>
        public virtual void SignalCancelOff()
        {
            throw new Exception("Should never be here");
        }

        /// <summary>
        /// Switch NV On
        /// </summary>
        public virtual void SignalNvOn()
        {
            throw new Exception("Should never be here");
        }

        /// <summary>
        /// Switch NV Off
        /// </summary>
        public virtual void SignalNvOff()
        {
            throw new Exception("Should never be here");
        }

        /// <summary>
        /// Switch key caching On
        /// </summary>
        public virtual void SignalKeyCacheOn()
        {
        }

        /// <summary>
        /// Switch key caching Off
        /// </summary>
        public virtual void SignalKeyCacheOff()
        {
        }

        /// <summary>
        /// Interface errors
        /// </summary>
        public enum Results : uint
        {
            /// <summary>
            /// Command completed succesfully
            /// </summary>
            // ReSharper disable once InconsistentNaming
            RESULT_SUCCESS = 0,

            /// <summary>
            /// Unspecified internal error
            /// </summary>
            // ReSharper disable once InconsistentNaming
            RESULT_INTERNAL_ERROR = 1,

            /// <summary>
            /// Bad parameter
            /// </summary>
            // ReSharper disable once InconsistentNaming
            RESULT_BAD_PARAMETER = 2,

            /// <summary>
            /// The command was cancelled
            /// </summary>
            // ReSharper disable once InconsistentNaming
            RESULT_COMMAND_CANCELED = 3,

            /// <summary>
            /// The command has been blocked
            /// </summary>
            /// ReShaper disable once InconsistentNaming
            RESULT_COMMAND_BLOCKED = 4
        }

    }

    /// <summary>
    /// Mode of the Tpm2 object operations.
    /// </summary>
    public enum Behavior
    {
        None = 0,

        /// <summary>
        /// If no mode flags are set, the default behavior of a Tpm2 object is to:
        /// - automatically provide auth sessions when they are necessary and not
        ///   specified explicitly;
        /// - automatically compute names of objects/NV indices;
        /// - do not do any command parameter validation before sending the command
        ///   to TPM (if only this is not necessary to prevent the client side code
        ///   from crashing).
        /// The other flags override this default behavior.  
        /// </summary>
        Default = Passthrough,

        /// <summary>
        /// When set, Tpm2 object does not issue any TPM commands of its own (i.e.
        /// commands not explicitly invoked by the user). This means that all the
        /// information required for building TPM command request and processing the
        /// command response (such as session objects, entity names) must be provided
        /// explicitly by the user whenever necessary.
        /// </summary>
        Strict = 1,

        /// <summary>
        /// Do not do any command parameter validation before sending the command to TPM.
        /// </summary>
        Passthrough = 2
    }

    /// <summary>
    /// Tpm2 provides methods to create TPM-compatible byte streams and unmarshal responses.  It is used in conjunction with a TPM device
    /// (implementing Tpm2Device) that communicates with the actual TPM device.
    /// TPM commands map 1:1 to corresponding methods in Tpm2 (with parameter translations described elsewhere).  
    /// Tpm2 also provides a few commands that are tagged with Ex (like Tpm2.StartAuthSessionEx).  These commands provide a slightly higher 
    /// level of abstraction when using the underlying native TPM command is tricky or verbose.
    /// Tpm2 also provides a few commands that are preceded by _ like _AllowErrors().  These commands are not sent to the TPM, but instead
    /// change the behavior of later TPM commands (often for the next command invocation only).
    /// Finally, Tpm2.Instrumentation provides access to TPM debug functionality (will not be
    /// available on release/production TPMs.)
    /// </summary>
    //  Note - Actual TPM command stubs are auto-generated and are in a separate file
    public sealed partial class Tpm2 : IDisposable
    {
        public class BehaviorMgr
        {
            // ReSharper disable once InconsistentNaming
            private Behavior behavior;

            BehaviorMgr (Behavior b)
            {
                behavior = b;
            }

            public static implicit operator BehaviorMgr (Behavior b)
            {
                return new BehaviorMgr(b);
            }

            public bool Strict
            {
                get
                {
                    return behavior.HasFlag(Tpm2Lib.Behavior.Strict);
                }
                set
                {
                    behavior = value ? behavior | Tpm2Lib.Behavior.Strict
                                     : behavior & ~Tpm2Lib.Behavior.Strict;
                }
            }

            public bool Passthrough
            {
                get
                {
                    return behavior.HasFlag(Tpm2Lib.Behavior.Passthrough);
                }
                set
                {
                    behavior = value ? behavior | Tpm2Lib.Behavior.Passthrough
                                     : behavior & ~Tpm2Lib.Behavior.Passthrough;
                }
            }
        }

        /// <summary>
        /// Underlying TPM device.
        /// </summary>
        internal Tpm2Device Device;

        // ReSharper disable once InconsistentNaming
        static public BehaviorMgr _TssBehavior = Behavior.Default;

        /// <summary>
        /// Behavior of this Tpm2 object. This set of modes does not affect the
        /// underlying TPM device, and only regulates which checks and transformations
        /// Tpm2 object is allowed to do while processing a command invocation.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public BehaviorMgr _Behavior;

        /// <summary>
        /// Auth value associated with the storage hierarchy (TpmRh.Owner).
        /// </summary>
        public AuthValue   OwnerAuth = new AuthValue();

        /// <summary>
        /// Auth value associated with the endorsement hierarchy (TpmRh.Endorsement).
        /// </summary>
        public AuthValue   EndorsementAuth = new AuthValue();

        /// <summary>
        /// Auth value associated with the platform hierarchy (TpmRh.Platform).
        /// </summary>
        public AuthValue   PlatformAuth = new AuthValue();

        /// <summary>
        /// Auth value associated with the dictionary attack lockout reset (TpmRh.Lockout).
        /// </summary>
        public AuthValue   LockoutAuth = new AuthValue();

        private int  TolerateErrorsPermanently = 0;

        // The following variables apply to the next command invocation. THey are typically set using the style - 
        // tpm[session].ExepectError(TpmRc.Auth).Command(parm1, parm2)
        // and are cleared when an actual command is invoked.  Note that if no command is invoked the state variables will
        // apply to the next call (probably in error)

        /// <summary>
        /// Sessions registered for the next command invocation.
        /// </summary>
        internal SessionBase[] Sessions;

        /// <summary>
        /// List of temporary session object handless created to authorize the current command.
        /// These sessions are flushed upon the command completion.
        /// </summary>
        private readonly List<SessionBase> TempSessions = new List<SessionBase>();

        /// <summary>
        /// List of handles, the associated name of which, must be reset upon the current
        /// command completion. Currently they can be only yet unwritten NV indices.
        /// </summary>
        private readonly List<TpmHandle> TempNames = new List<TpmHandle>();

        /// <summary>
        /// Hash algorithm to compute a digest of a private part that is used to index
        /// AuthValues dictionary.
        /// </summary>
        private const TpmAlgId PrivHashAlg = TpmAlgId.Sha1;

        /// <summary>
        /// A dictionary internally maintained to pass information about objects' auth
        /// values changed/set by a command to the corresponding wrapper classes managed
        /// by TSS.Net.
        /// </summary>
        private readonly Dictionary<TpmHash, AuthValue> AuthValues = new Dictionary<TpmHash, AuthValue>();

        /// <summary>
        /// A dictionary internally used to pass parameters of newly created auth sessions
        /// to the corresponding wrapper classes managed by TSS.Net.
        /// </summary>
        private Dictionary<TpmHandle, AuthSession> SessionParams = new Dictionary<TpmHandle, AuthSession>();

        /// <summary>
        /// An internal array of handles of PCRs that have an auth value assigned.
        /// </summary>
        internal TpmHandle[] PcrHandles;

        /// <summary>
        /// List of expected errors for the next command invocation.
        /// If contains TpmRc.Success value, it is always the first item of the list.
        /// </summary>
        private TpmRc[] ExpectedResponses;

        private bool TolerateErrors;

        /// <summary>
        /// Code of the command being processed at the moment.
        /// </summary>
        private TpmCc CurrentCommand = TpmCc.None;

        /// <summary>
        /// While processing a command issued by the user, the library may need to
        /// execute another TPM command (such as StartAuthSession). In this case
        /// OuterCommand is set to the original command code, which prevents the
        /// library from changing and clearing command specific state associated with
        /// the user command.
        /// </summary>
        private TpmCc OuterCommand = TpmCc.None;

        /// <summary>
        /// Error number returned by the previously completed command. Note that in
        /// general this is not exactly response code returned by TPM, just an error
        /// code extracted from it.
        /// </summary>
        private TpmRc LastError = TpmRc.Success;

        private readonly CommandModifier ActiveModifiers = new CommandModifier();

        // The following variables instruct Tpm2 to calculate the CpHash of the next command rather than actually sending it to the TPM
        private bool CpHashMode;
        private TpmHash CommandParmHash;

        /// <summary>
        /// Code of the previously completed command. This value is updated even if
        /// the command has failed.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private TpmCc _LastCommand = TpmCc.None;

        public TpmCc LastCommand
        {
            get { return _LastCommand; }
        }

        // Debugging support. Various callbacks allow a debugger, profiler or monitor 
        // to be informed of TPM commands and responses. They are called at different 
        // times and places in the conversation between the tester and the TPM.

        public delegate void TraceCallback(byte[] inBuf, byte[] outBuf);

        public delegate void ParamsTraceCallback(TpmCc commandCode, TpmStructureBase inParms, TpmStructureBase outParms);

        public delegate bool CmdParamsCallback(CommandInfo info, ref byte[] parms, TpmHandle[] handles);

        public delegate bool CmdBufCallback(ref byte[] command);

        public delegate bool CmdStatsCallback(TpmCc command, TpmRc maskedError, double executionTime);

        public delegate bool AlternateActionCallback(TpmCc ordinal,
                                                     TpmStructureBase inParms,
                                                     Type expectedResponseType,
                                                     out TpmStructureBase outParms,
                                                     out bool desiredSuccessCode);

        private TraceCallback TheTraceCallback;
        private ParamsTraceCallback TheParamsTraceCallback;
        private CmdParamsCallback TheCmdParamsCallback;
        private CmdBufCallback TheCmdBufCallback;
        private CmdStatsCallback TheCmdStatsCallback;
        private AlternateActionCallback TheAlternateActionCallback;

        /// <summary>
        /// Get the underlying TPM device
        /// </summary>
        /// <returns></returns>
        public Tpm2Device _GetUnderlyingDevice()
        {
            return Device;
        }

        /// <summary>
        /// Methods in Helpers enable quick and easy access to common TPM commands and command
        /// sequences
        /// </summary>
        public TpmHelpers Helpers;

        // If an ErrorHandler is registered then it is called instead 
        public delegate void ErrorHandler(TpmRc returnCode, TpmRc[] expectedResponses);

        private ErrorHandler TheErrorHandler;

        /// <summary>
        /// A Tpm2 must be created attached to an underlying device
        /// </summary>
        /// <param name="device"></param>
        /// <param name="mode"></param>
        public Tpm2(Tpm2Device device, Behavior b = Behavior.Default)
        {
            Device = device;
            _Behavior = b;
            Sessions = new SessionBase[0];
            Helpers = new TpmHelpers(this);
        }

        #region Sessions

        /// <summary>
        /// Specify sessions in handle-order to be used during the next command invocation.
        /// The references to the attached sessions will be cleared upon command completion,
        /// whether it is successful or not.
        /// </summary>
        public Tpm2 _SetSessions(params SessionBase[] sessions)
        {
            return this[sessions];
        }

        /// <summary>
        /// Specify sessions in handle-order to be used during the next command invocation.
        /// The references to the attached sessions will be cleared upon command completion,
        /// whether it is successful or not.
        /// </summary>
        public Tpm2 this[params SessionBase[] sessions]
        {
            get
            {
                Sessions = Globs.CopyArray(sessions);
                return this;
            }
        }

        #endregion

        /// <summary>
        /// Tpm2 will typically throw an exception if an error is returned.  If an error is expected then this command instructs the Tpm2 to 
        /// throw an exception in anything other than the expected error is returned.  This behavior is cleared when a command is invoked.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public Tpm2 _ExpectError(TpmRc errorCode)
        {
            return _ExpectResponses(errorCode);
        }

        /// <summary>
        /// If AllowError() is active on a TPM context when a command is invoked then errors are silent, but 
        /// set to the error that the TPM was generated.  This behavior is cleared when a command is executed.
        /// </summary>
        /// <param name="errorList"></param>
        /// <returns></returns>
        public Tpm2 _ExpectResponses(params object[] errorList)
        {
            // Empty responses list indicates success
            ExpectedResponses = null;

            if (errorList.Length == 0 ||
                errorList.Length == 1 && (TpmRc)errorList[0] == TpmRc.Success)
            {
                return this;
            }

            ExpectedResponses = new TpmRc[0];
            return _ExpectMoreResponses(errorList);
        }

        public Tpm2 _ExpectMoreResponses(params object[] errorList)
        {
            if (ExpectedResponses == null)
            {
                ExpectedResponses = new TpmRc[1] {TpmRc.Success};
            }
            var old = ExpectedResponses;
            ExpectedResponses = new TpmRc[errorList.Length + old.Length];
            Array.Copy(old, ExpectedResponses, old.Length);

            for (int i = 0; i < errorList.Length; ++i)
            {
                var rc = (TpmRc)errorList[i];
                if (rc == TpmRc.Success && i != 0)
                {
                    rc = ExpectedResponses[0];
                    ExpectedResponses[0] = TpmRc.Success;
                }
                ExpectedResponses[old.Length + i] = rc;
            }
            AssertExpectedResponsesValid();
            return this;
        }

        private void AssertExpectedResponsesValid()
        {
            Debug.Assert(ExpectedResponses == null ||
                         ExpectedResponses.Length == 1 && ExpectedResponses[0] != TpmRc.Success ||
                         ExpectedResponses.Length > 1);
        }

        private bool IsSuccessExpected()
        {
            return OuterCommand != TpmCc.None || 
                   ExpectedResponses == null || ExpectedResponses[0] == TpmRc.Success;
        }

        private bool AreErrorsExpected()
        {
            return OuterCommand == TpmCc.None && ExpectedResponses != null;
        }

        private bool IsErrorAllowed(TpmRc rc)
        {
            return TolerateErrors ||
                   AreErrorsExpected() && ExpectedResponses.Count(x => x == rc) > 0;
        }

        /// <summary>
        /// Cancels the effect of the _DisableExceptions() method.
        /// </summary>
        public Tpm2 _EnableExceptions()
        {
            if (--TolerateErrorsPermanently <= 0)
            {
                TolerateErrors = false;
            }
            return this;
        }

        /// <summary>
        /// Prevents this TPM object from throwing an exception if a TPM command fails.
        /// The effect of this method is permanent, and can only be canceled by the
        /// _EnableExceptions() method.
        /// </summary>
        public Tpm2 _DisableExceptions()
        {
            if (++TolerateErrorsPermanently > 0)
            {
                TolerateErrors = true;
            }
            return this;
        }

        /// <summary>
        /// Prevents this TPM context from throwing an exception if the next TPM command fails.
        /// If exceptions are not disabled permanently the effect of _AllowErrors() is nullified after the
        /// next TPM command is executed (disregarding whether it failed or succeeded).
        /// </summary>
        public Tpm2 _AllowErrors()
        {
            TolerateErrors = true;
            return this;
        }

        /// <summary>
        /// Returns the response code for the last command executed by the TPM (error or success)
        /// This is typically used in conjunction with AllowErrors() (otherwise errors cause an exception)
        /// </summary>
        /// <returns></returns>
        public TpmRc _GetLastResponseCode()
        {
            return LastError;
        }

        /// <summary>
        /// Did the last command return an error?
        /// </summary>
        /// <returns></returns>
        public bool _LastCommandSucceeded()
        {
            bool didSucceed = LastError == TpmRc.Success;
            return didSucceed;
        }

        /// <summary>
        /// Switch on or off the assertion of physical presence on the underlying device.
        /// If the device does not support PP an exception will be generated
        /// </summary>
        /// <param name="ppOn"></param>
        public void _AssertPhysicalPresence(bool ppOn)
        {
            Device.AssertPhysicalPresence(ppOn);
        }

        /// <summary>
        /// Set the locality for future commands.  The default locality is Locality0.  Not all TPM
        /// devices will be able to honor all locality requests.
        /// </summary>
        /// <param name="locality"></param>
        public Tpm2 _SetLocality(LocalityAttr locality)
        {
            switch (locality)
            {
                case LocalityAttr.TpmLocZero:
                    ActiveModifiers.ActiveLocality = 0;
                    break;
                case LocalityAttr.TpmLocOne:
                    ActiveModifiers.ActiveLocality = 1;
                    break;
                case LocalityAttr.TpmLocTwo:
                    ActiveModifiers.ActiveLocality = 2;
                    break;
                case LocalityAttr.TpmLocThree:
                    ActiveModifiers.ActiveLocality = 3;
                    break;
                case LocalityAttr.TpmLocFour:
                    ActiveModifiers.ActiveLocality = 4;
                    break;
                default:
                    if ((int)locality > 31 && (int)locality < 256)
                    {
                        ActiveModifiers.ActiveLocality = (byte)locality;
                        break;
                    }
                    throw new ArgumentException("Invalid locality");
            }
            return this;
        }

        /// <summary>
        /// Set the priority for future commands.  The default locality is Normal.  Not all TPM
        /// devices will be able to honor all locality requests.
        /// </summary>
        /// <param name="priority"></param>
        public Tpm2 _SetPriority(TbsPublicStubs.TBS_COMMAND_PRIORITY priority)
        {
            switch (priority)
            {
                case TbsPublicStubs.TBS_COMMAND_PRIORITY.LOW:
                case TbsPublicStubs.TBS_COMMAND_PRIORITY.NORMAL:
                case TbsPublicStubs.TBS_COMMAND_PRIORITY.HIGH:
                case TbsPublicStubs.TBS_COMMAND_PRIORITY.SYSTEM:
                    ActiveModifiers.ActivePriority = priority;
                    break;
                default:
                    throw new ArgumentException("Invalid priority");
            }
            return this;
        }

        private bool DoNotDispatchCommand;
        private byte[] CommandBytes;

        /// <summary>
        /// If this property modifier is called on the TPM then the next command is not dispatched 
        /// onto the TPM. Instead a copy of the command parameters is recorded which can be 
        /// obtained by calling _GetCommandBytes()
        /// </summary>
        public void _DoNotDispatchCommand()
        {
            CommandBytes = null;
            DoNotDispatchCommand = true;
        }

        /// <summary>
        /// Gets the raw command bytes of a previous command modified by _DoNotDispatchCommand()
        /// </summary>
        /// <returns></returns>
        public byte[] _GetCommandBytes()
        {
            return CommandBytes;
        }

        private TpmHash CommandAuditHash;
        private bool AuditThisCommand;

        /// <summary>
        /// Instructs this instance of Tpm2 to start collecting command audit digests (unless auditAlg is TpmAlgId.Null, 
        /// in which case auditing is terminated.
        /// Tpm2 will accumulate the command audit digest for commands tagged with _Audit().
        /// This command should also be used to reset the audit accumulation.
        /// </summary>
        /// <param name="auditAlg"></param>
        public void _SetCommandAuditAlgorithm(TpmAlgId auditAlg)
        {
            CommandAuditHash = auditAlg == TpmAlgId.Null ? null : new TpmHash(auditAlg);
        }

        public void _SetCommandAuditAlgorithm(TpmAlgId auditAlg, byte[] currentDigest)
        {
            CommandAuditHash = new TpmHash(auditAlg, currentDigest);
        }

        /// <summary>
        /// Accumulate command audit for the next command invocation (an audit algorithm must be set)
        /// </summary>
        /// <returns></returns>
        public Tpm2 _Audit()
        {
            AuditThisCommand = true;
            return this;
        }

        /// <summary>
        /// Gets the current command audit hash.
        /// </summary>
        /// <returns></returns>
        public TpmHash _GetAuditHash()
        {
            return CommandAuditHash;
        }

        /// <summary>
        /// Register a trace callback delegate.  The registered method will be called after commands are sent to the TPM with parameters
        /// describing the TPM command and response.  The delegate is only called if the command succeeds.
        /// </summary>
        /// <param name="callback"></param>
        public void _SetTraceCallback(TraceCallback callback)
        {
            TheTraceCallback = callback;
        }

        /// <summary>
        /// Install a callback that is invoked with with parms describing the
        /// command and response of the next command ONLY.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Tpm2 _SetParamsTraceCallback(ParamsTraceCallback callback)
        {
            TheParamsTraceCallback = callback;
            return this;
        }

        /// <summary>
        /// Installs a callback that allows the caller to collect basic command execution
        /// statistics, and manipulate command parameters prior to the creation of the cpHash.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="callback2"></param>
        /// <returns></returns>
        public Tpm2 _SetCommandCallbacks(CmdStatsCallback csc,
                                         CmdParamsCallback cpc = null, CmdBufCallback cbc = null)
        {
            TheCmdParamsCallback = cpc;
            TheCmdBufCallback = cbc;
            TheCmdStatsCallback = csc;
            return this;
        }

        /// <summary>
        /// Install a function to be called on every TPM command invocation.  THe callback 
        /// is called for every TPM function on this device.  If the callback returns false
        /// then standard processing occurs.  If the command returns true then the callback return 
        /// parameters are returned to the caller.
        /// </summary>
        /// <param name="theAction"></param>
        public Tpm2 _SetAlternateActionCallback(AlternateActionCallback theAction)
        {
            TheAlternateActionCallback = theAction;
            return this;
        }

        /// <summary>
        /// Trace TPM commands and responses to the debug stream.
        /// </summary>
        /// <param name="trace"></param>
        public Tpm2 _TraceCommands(bool trace)
        {
            CommandLogging = trace;
            return this;
        }

        private bool CommandLogging;

        /// <summary>
        /// Default behavior is that the TPM asserts if there is an error.  This lets the caller install an error handler (use null to 
        /// uninstall the handler.  THis is overridden by AllowError where all errors are silent.
        /// </summary>
        /// <param name="theErrorHandler"></param>
        public Tpm2 _SetErrorHandler(ErrorHandler theErrorHandler)
        {
            TheErrorHandler = theErrorHandler;
            return this;
        }

        /// <summary>
        /// GetCpHash instructs Tpm2 to calculate the cpHash of the next command rather than actually sending anything to the TPM.  
        /// The cpHash is needed for certain policy commands.
        /// THe caller should initialize cpHash.AlgId, and once a command has been issued the cpHash will be modified to contain the 
        /// actual hash value.  TPM return parameters will be null.
        /// </summary>
        /// <param name="cpHash"></param>
        /// <returns></returns>
        public Tpm2 _GetCpHash(TpmHash cpHash)
        {
            CpHashMode = true;
            CommandParmHash = cpHash;
            return this;
        }

        private bool TestCycleNv;

        /// <summary>
        /// Some implementations do not always have NV available at all times.  The TPM always tests
        /// NvIsAvailable before making any TPM state changes. If NV is not available, the TPM returns
        /// an error, without changing TPM state, and the caller can re-try later.
        /// In the reference implementation it is an implementation error (causing an assert) if the TPM calls NvCommit
        /// when NV is not available.  The reference implementation also allows NV-availability to be
        /// set programmatically.
        /// If this mode is set then all TPM commands are first submitted with NV not available and then re-submitted
        /// with NV on if an error occurs.  All tests should pass with this mode enabled.
        /// </summary>
        /// <param name="testIt"></param>
        public void _DebugTestNvIsAvailable(bool testIt = true)
        {
            TestCycleNv = testIt;
        }

        public delegate void WarningHandler(string warning);

        private WarningHandler TheWarningHandler;

        /// <summary>
        /// By default if an error returned by the TPM is not the same as the
        /// that expected (set by _ExpectError) then this is translated into 
        /// an exception by this library.  
        /// If a non-null warning handler is installed then this handler is 
        /// invoked INSTEAD and processing is as if the error was the one
        /// expected.
        /// </summary>
        /// <param name="theWarningHandler"></param>
        public void _SetWarningHandler(WarningHandler theWarningHandler)
        {
            TheWarningHandler = theWarningHandler;
        }

        // ReSharper disable once UnusedMember.Local
        private ReentrancyGuardContext MyGuard = new ReentrancyGuardContext();

        /// <summary>
        /// DispatchMethod is called by auto-generated command action code. It assembles a byte[] containing
        /// the formatted TPM command based on the params passed in explicitly, and the sessions currently attached
        /// to the TPM object.  It processes the TPM response and converts it into an instantiation of the 
        /// requested object.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="inParms"></param>
        /// <param name="expectedResponseType"></param>
        /// <param name="outParms"></param>
        /// <param name="numInHandlesNotUsed"></param>
        /// <param name="numOutHandlesNotUsed"></param>
        /// <returns></returns>
        internal bool DispatchMethod(
            TpmCc ordinal,
            TpmStructureBase inParms,
            Type expectedResponseType,
            out TpmStructureBase outParms,
            int numInHandlesNotUsed,
            int numOutHandlesNotUsed)
        {
            outParms = null;
            // todo - ClearCommandContext should be moved to the front of this
            // routine (and we should make local copies of the context-values we depend upon)
            // There are uncaught exceptions that can be generated that would skip
            // ClearCOmmandCOntext and leave the Tpm2 with some leftover state.

            byte[] response;

            if (CurrentCommand != TpmCc.None)
            {
                OuterCommand = CurrentCommand;
            }
            CurrentCommand = ordinal;

            // The AlternateActionCallback allows alternate processing (or filtering/data
            // collection on the executing command stream.
            if (TheAlternateActionCallback != null)
            {
                bool desiredSuccessCode;
                bool alternate = TheAlternateActionCallback(ordinal,
                                                            inParms,
                                                            expectedResponseType,
                                                            out outParms,
                                                            out desiredSuccessCode);
                if (alternate)
                {
                    _ClearCommandContext();
                    return desiredSuccessCode;
                }
            }

            CommandInfo commandInfo = CommandInfoFromCommandCode(ordinal);
            byte[] parms;
            TpmHandle[] inHandles;

            try
            {
                // Get the handles and the parameters from the command input structure
                CommandProcessor.Fragment(inParms, commandInfo.HandleCountIn, out inHandles, out parms);

                // Start processing sessions
                PrepareRequestSessions(commandInfo, inHandles);
            }
            catch (Exception e)
            {
                Debug.Assert(outParms == null);
                if (e is TpmException)
                {
                    if (IsErrorAllowed(((TpmException)e).RawResponse))
                    {
                        outParms = (TpmStructureBase)Activator.CreateInstance(expectedResponseType);
                    }
                }
                _ClearCommandPrelaunchContext();
                _ClearCommandContext();
                if (outParms != null)
                {
                    return false;
                }
                throw;
            }

            // The caller can install observer/modifier callbacks, and request repeated
            // execution of the same command.
            bool repeat = false;
            byte[] parmsCopy = null;

            if (TheCmdParamsCallback != null)
            {
                parmsCopy = Globs.CopyData(parms);
            }

            // Response atoms
            TpmSt   responseTag = TpmSt.None;
            TpmRc   resultCode = TpmRc.None;
            uint    responseParamSize = 0;
            byte[]  outParmsNoHandles = null,
                    outParmsWithHandles = null;
            TpmHandle[]     outHandles = null;
            SessionOut[]    outSessions = null;

            // In normal processing there is just one pass through this do-while loop
            // If command observation/modification callbacks are installed, then the
            // caller repeats the command as long as necessary.
            bool invokeCallbacks = OuterCommand == TpmCc.None &&
                                   !CpHashMode && !DoNotDispatchCommand;
            do try
            {
                if (TheCmdParamsCallback != null && invokeCallbacks)
                {
                    parms = Globs.CopyData(parmsCopy);
                    TheCmdParamsCallback(commandInfo, ref parms, inHandles);
                }

                // If there are any encrypting sessions then next we encrypt the data in place
                parms = DoParmEncryption(parms, commandInfo, 0, Direction.Command);

                // Now do the HMAC (note that the handles are needed for name-replacement)
                SessionIn[] inSessions = CreateRequestSessions(parms, inHandles);

                // CpHashMode is enabled for a single command through tpm.GetCpHash().TpmCommand(...)
                if (OuterCommand == TpmCc.None && CpHashMode)
                {
                    CommandParmHash.HashData = GetCommandHash(CommandParmHash.HashAlg, parms, inHandles);
                    outParms = (TpmStructureBase)Activator.CreateInstance(expectedResponseType);
                    CpHashMode = false;
                    _ClearCommandContext();
                    return true;
                }

                // Create the command buffer
                byte[] command = CommandProcessor.CreateCommand(ordinal, inHandles, inSessions, parms);

                // And dispatch the command
                Log(ordinal, inParms, 0);

                if (DoNotDispatchCommand)
                {
                    CommandBytes = command;
                    outParms = (TpmStructureBase)Activator.CreateInstance(expectedResponseType);
                    DoNotDispatchCommand = false;
                    _ClearCommandContext();
                    return true;
                }

                if (TheCmdBufCallback != null && invokeCallbacks)
                {
                    TheCmdBufCallback(ref command);
                    if (command == null)
                    {
                        repeat = true;
                        continue;   // retry
                    }
                }

                // And actually dispatch the command into the underlying device
                DateTime commandSentTime, responseReceivedTime;
                int nvRateRecoveryCount = 0;

                // No more than 4 retries on NV_RATE error
                for (;;)
                {
                    responseReceivedTime = commandSentTime = DateTime.Now;

                    if (!TestCycleNv)
                    {
                        commandSentTime = DateTime.Now;
                        Device.DispatchCommand(ActiveModifiers, command, out response);
                        responseReceivedTime = DateTime.Now;
                    }
                    else
                    {
                        // In TestCycleNv we submit the command with NV not-available.  If the TPM indicates that
                        // NV is not available we re-submit.
                        try
                        {
                            // Once with NV off
                            Device.SignalNvOff();
                            Device.DispatchCommand(ActiveModifiers, command, out response);
                            Device.SignalNvOn();
                            // And if it did not work, try again with NV on
                            TpmRc respCode = CommandProcessor.GetResponseCode(response);
                            if ((uint)respCode == 0x923U || respCode == TpmRc.Lockout)
                            {
                                Device.DispatchCommand(ActiveModifiers, command, out response);
                            }

                        }
                        catch (Exception)
                        {
                            Device.SignalNvOn();
                            throw;
                        }
                    }

                    // Convert the byte[] response into its constituent parts.
                    CommandProcessor.SplitResponse(response,
                                                   commandInfo.HandleCountOut,
                                                   out responseTag,
                                                   out responseParamSize,
                                                   out resultCode,
                                                   out outHandles,
                                                   out outSessions,
                                                   out outParmsNoHandles,
                                                   out outParmsWithHandles);

                    if (resultCode == TpmRc.Retry)
                    {
                        continue;
                    }
                    if (resultCode != TpmRc.NvRate || ++nvRateRecoveryCount > 4)
                    {
                        break;
                    }
                        //Console.WriteLine(">>>> NV_RATE: Retrying... Attempt {0}", nvRateRecoveryCount);
#if !NETFX_CORE
                        Thread.Sleep((int)Tpm2.GetProperty(this, Pt.NvWriteRecovery) + 100);
#endif
                } // infinite loop

                // Invoke the trace callback if installed        
                if (TheTraceCallback != null)
                {
                    TheTraceCallback(command, response);
                }

                // Collect basic statistics on command execution
                if (TheCmdStatsCallback != null && invokeCallbacks)
                {
                    repeat = TheCmdStatsCallback(ordinal, GetBaseErrorCode(resultCode),
                                        (responseReceivedTime - commandSentTime).TotalSeconds);
                }

                if (repeat && resultCode == TpmRc.Success)
                {
                    // Update session state
                    ProcessResponseSessions(outSessions);

                    int offset = (int)commandInfo.HandleCountOut * 4;
                    outParmsWithHandles = DoParmEncryption(outParmsWithHandles, commandInfo, offset, Direction.Response);
                    var m = new Marshaller(outParmsWithHandles);
                    outParms = (TpmStructureBase)m.Get(expectedResponseType, "");
#if false
                    m = new Marshaller(command);
                    TpmSt tag = m.Get<TpmSt>();
                    uint cmdSize = m.Get<uint>();
                    TpmCc actualCmd = m.Get<TpmCc>();
                    var actualHandles = new TpmHandle[inHandles.Length];
                    for (int i = 0; i < inHandles.Length; ++i)
                    {
                        actualHandles[i] = m.Get<TpmHandle>();
                    }
                    for (int i = 0; i < inSessions.Length; ++i)
                    {
                        m.Get<SessionIn>();
                    }
                    var actualParms = m.GetArray<byte>(m.GetValidLength() - m.GetGetPos());
                    if (m.GetValidLength() != cmdSize)
                    {
                        throw new Exception("Command length in header does not match input byte-stream");
                    }
#endif
                    CommandHeader actualHeader;
                    TpmHandle[] actualHandles;
                    SessionIn[] actualSessions;
                    byte[] actualParmsBuf;
                    CommandProcessor.CrackCommand(command, out actualHeader, out actualHandles, out actualSessions, out actualParmsBuf);
                    m = new Marshaller();
                    foreach (TpmHandle h in actualHandles)
                    {
                        m.Put(h, "handle");
                    }
                    m.Put(actualParmsBuf, "parms");
                    var actualParms = (TpmStructureBase)Activator.CreateInstance(inParms.GetType());
                    actualParms.ToHost(m);
                    for (int i = 0; i < actualHandles.Length; ++i)
                    {
                        for (int j = 0; j < inHandles.Length; ++j)
                        {
                            if (actualHandles[i].handle == inHandles[j].handle)
                                actualHandles[i] = inHandles[j];
                        }
                    }
                    UpdateHandleData(actualHeader.CommandCode, actualParms, actualHandles, outParms);
                    //ValidateResponseSessions(outHandles, outSessions, ordinal, resultCode, outParmsNoHandles);

                    foreach (var h in outHandles)
                    {
                        CancelSafeFlushContext(h);
                    }
                } // if (repeat && resultCode == TpmRc.Success)
            }
            catch (Exception)
            {
                _ClearCommandPrelaunchContext();
                _ClearCommandContext();
                throw;
            }
            while (repeat);

            // Update the audit session if needed
            if (AuditThisCommand)
            {
                AuditThisCommand = false;
                if (CommandAuditHash == null)
                {
                    Globs.Throw("No audit hash set for this command stream");
                    CommandAuditHash = TpmAlgId.None;
                }
                byte[] parmHash = GetCommandHash(CommandAuditHash.HashAlg, parms, inHandles);
                byte[] expectedResponseHash = GetExpectedResponseHash(CommandAuditHash.HashAlg,
                                                                      outParmsNoHandles,
                                                                      ordinal,
                                                                      resultCode);
                CommandAuditHash.Extend(Globs.Concatenate(parmHash, expectedResponseHash));
            }

            // FlushContest that may be executed as part of _ClearCommandPrelaunchContext()
            // must be executed before any command-related info is updated.
            _ClearCommandPrelaunchContext();

            // Process errors if there are any
            bool commandSucceeded = ProcessError(responseTag, responseParamSize, resultCode, inParms);
            try
            {
                if (commandSucceeded)
                {
                    ProcessResponseSessions(outSessions);
                    int offset = (int)commandInfo.HandleCountOut * 4;
                    outParmsWithHandles = DoParmEncryption(outParmsWithHandles, commandInfo, offset, Direction.Response);

                    var mt = new Marshaller(outParmsWithHandles);
                    outParms = (TpmStructureBase)mt.Get(expectedResponseType, "");
                    if (TheParamsTraceCallback != null)
                    {
                        TheParamsTraceCallback(ordinal, inParms, outParms);
                    }

                    UpdateHandleData(ordinal, inParms, inHandles, outParms);
                    ValidateResponseSessions(outHandles, outSessions, ordinal, resultCode, outParmsNoHandles);

                    foreach (var s in Sessions) if (s is AuthSession)
                    {
                        var sess = s as AuthSession;
                        if (sess.Attrs.HasFlag(SessionAttr.Audit) && !TpmHandle.IsNull(sess.BindObject))
                        {
                            sess.BindObject = TpmRh.Null;
                            break; // only one audit session is expected
                        }
                    }
                }
                else
                {
                    outParms = (TpmStructureBase)Activator.CreateInstance(expectedResponseType);
                }
            }
            finally
            {
                // Check in case an exception happened before outParms was initialized
                // ReSharper disable once CSharpWarnings::CS0183
                if (outParms is TpmStructureBase)
                {
                    Log(ordinal, outParms, 1);
                }
                // Clear all per-invocation state (e.g. sessions, errors expected) ready for next command
                _ClearCommandContext();
            }
            return commandSucceeded;
        }

        public static string GetErrorString(Type inParmsType, uint resultCode, out TpmRc theMaskedError)
        {
            // There are two encoding for errors - format 0 and format 1.  Decode the error type
            var resultCodeValue = resultCode;
            bool formatOneErrorType = ((resultCodeValue & 0x80) != 0);
            uint resultCodeMask = formatOneErrorType ? 0xBFU : 0x97FU;

            // Extract the actual error code
            uint maskedErrorVal = resultCode & resultCodeMask;
            var maskedError = (TpmRc)maskedErrorVal;
            theMaskedError = maskedError;

            string errorEntity = "Unknown";
            uint errorEntityIndex = 0;
            string errorParmName = "Unknown";
            if (formatOneErrorType)
            {
                errorEntityIndex = (resultCodeValue & 0xF00U) >> 8;
                if (errorEntityIndex == 0)
                {
                    // ReSharper disable once RedundantAssignment
                    errorEntity = "Unknown";
                }
                if ((resultCodeValue & 0x40) != 0)
                {
                    errorEntity = "Parameter";
                    errorParmName = GetParmName(inParmsType, errorEntityIndex);
                }
                else
                {
                    if (errorEntityIndex >= 8)
                    {
                        errorEntityIndex -= 8;
                        errorEntity = "Session";
                    }
                    else
                    {
                        errorEntity = "handle";
                    }
                }
            }

            string errorString = String.Format(
                                               "[Code=TpmRc.{0}],[FullVal=0x{1:X},{1}]\n" +
                                               "[ErrorEntity={2}],[ParmNum={3}]" +
                                               "[ParmName={4}]",
                                               new Object[] {
                                                   maskedError.ToString(), 
                                                   //(uint)maskedError, 
                                                   resultCodeValue,
                                                   errorEntity,
                                                   errorEntityIndex,
                                                   errorParmName
                                               });
            return errorString;
        }

        /// <summary>
        /// Handles TPM response value.
        /// Converts the error code into a human-readable form, invokes callbacks and
        /// encapsulates error info into a .Net exception.
        /// </summary>
        /// <param name="responseTag"></param>
        /// <param name="responseParamSize"></param>
        /// <param name="resultCode"></param>
        /// <param name="inParms"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedParameter.Local
        private bool ProcessError(TpmSt responseTag, uint responseParamSize,
                                  TpmRc resultCode, TpmStructureBase inParms)
        {
            string errorString;

            AssertExpectedResponsesValid();

            // Process TPM success case (both expected success, and unexpected success)
            if (resultCode == TpmRc.Success)
            {
                LastError = TpmRc.Success;
                if (IsSuccessExpected())
                {
                    return true;
                }
                // Else we have unexpectedly succeeded

                // If TolerateErrors is set, then no error indication is provided apart
                // from setting LastReponseCode (the caller must query to find that
                // the error does not match).
                if (TolerateErrors)
                {
                    return false;
                }
                // If there is an installed error handler invoke it.
                if (TheErrorHandler != null)
                {
                    TheErrorHandler(resultCode, ExpectedResponses);
                    return false;
                }
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (ExpectedResponses.Length == 1)
                {
                    errorString = string.Format("Error {0} was expected but command {1} succeeded",
                                                ExpectedResponses[0],
                                                CurrentCommand);
                }
                else
                {
                    errorString = string.Format("Errors {{{0}}} were expected but command {1} succeeded",
                                                string.Join(", ", ExpectedResponses),
                                                CurrentCommand);
                }

                _ClearCommandContext();
                throw new TssException(errorString);
            }
            // Else we have an error
            if (responseTag != TpmSt.NoSessions)
            {
                throw new Exception("Ill-formed responseTag (not NoSessions)");
            }
            if (responseParamSize != 10)
            {
                throw new Exception("Ill-formed reponseParamSize (not 10)");
            }

            // There are two encodings for errors - format 0 and format 1.
            var resultCodeValue = (uint)resultCode;
            bool formatOneErrorType = TpmErrorHelpers.IsFmt1(resultCode);

            // Extract the actual error number
            LastError = TpmErrorHelpers.ErrorNumber(resultCode);

            string errorEntity = "Unknown";
            uint errorEntityIndex = 0;
            string errorParmName = "Unknown";
            if (formatOneErrorType)
            {
                errorEntityIndex = (resultCodeValue & 0xF00U) >> 8;
                if ((resultCodeValue & 0x40) != 0)
                {
                    errorEntity = "Parameter";
                    errorParmName = GetParmName(inParms.GetType(), errorEntityIndex);
                }
                else
                {
                    if (errorEntityIndex >= 8)
                    {
                        errorEntityIndex -= 8;
                        errorEntity = "Session";
                    }
                    else
                    {
                        errorEntity = "Handle";
                    }
                }
            }
            string errorDetails = FormatString("\r\n" +
                                               "Details: \n" +
                                               "[Code=TpmRc.{0}],"+
                                               "[RawCode=0x{1:X},{1}]\n" +
                                               "[ErrorEntity={2}], [ParmNum={3}]\n" +
                                               "[ParmName={4}]",
                                               new Object[] {
                                                   LastError.ToString(), 
                                                   resultCodeValue,
                                                   errorEntity,
                                                   errorEntityIndex,
                                                   errorParmName
                                               });
            // We have found out all we can about the error.  Now process it according to the tpm context

            // AllowErrors() specifies that errors should be handled silently
            if ((OuterCommand == TpmCc.None && IsErrorAllowed(LastError)) ||
                (OuterCommand != TpmCc.None && LastError == TpmRc.Canceled))
            {
                return false;
            }

            if (OuterCommand != TpmCc.None || ExpectedResponses == null)
            {
                errorString = string.Format("Error {{{0}}} was returned for command {1}.",
                                            LastError, CurrentCommand);
            }
            else
            {
                errorString = string.Format("Error {{{0}}} was returned instead of {{{1}}} for command {2}.",
                                            LastError,
                                            string.Join(", ", ExpectedResponses),
                                            CurrentCommand);
            }

            if (AreErrorsExpected())
            {
                // We have a mismatched error. If a warning handler is installed, call it.
                if (TheWarningHandler != null)
                {
                    TheWarningHandler(errorString);
                    _ClearCommandContext();
                    return false;
                }
            }

            // Otherwise propagate the unexpected error as an exception
            _ClearCommandContext();
            errorString += errorDetails;
            throw new TpmException(resultCode, errorString);
        }

        public bool SafeFlushContext(TpmHandle ctxt)
        {
            if (ctxt == null)
            {
                return false;
            }
            if (ctxt == TpmRh.Null || ctxt.handle == 0)
            {
                _ClearCommandContext();
                return false;
            }
            FlushContext(ctxt);
            if (_LastCommandSucceeded())
                ctxt.handle = 0;

            return true;
        }

        class CancelationCtx
        {
            Tpm2    tpm;
            string  cmdName;
            bool    origTolerateErrors;
            TpmRc   origLastError;

            bool    thereWasCancelation = false;
            TpmRc   lastError;
            int     attempt = 0;

            public CancelationCtx (Tpm2 t, string cmd)
            {
                tpm = t;
                cmdName = cmd;
                origTolerateErrors = tpm.TolerateErrors;
                origLastError = tpm.LastError;
            }

            public bool NeedRetry()
            {
                lastError = tpm.LastError;
                if (lastError == TpmRc.Canceled)
                    thereWasCancelation = true;
                else
                    return false;
                return attempt++ < 25;
            }

            public bool Done()
            {
                //if (thereWasCancelation)
                //    Console.WriteLine("{0}() was canceled by {3}, and then returned {1} on attempt {2}", cmdName, lastError, attempt,
                //                      thereWasTpmCancelation ? (thereWasTbsCancelation ? "TPM & TBS" : "TPM") : "TBS");
                tpm.TolerateErrors = origTolerateErrors;
                tpm.LastError = origLastError;
                return thereWasCancelation;
            }
        } // class CancelationCtx

        public bool CancelSafeFlushContext(TpmHandle h)
        {
            if (h == null || h.handle == 0 /*|| h.handle == TpmRh.Null*/)
                return false;
            var ctx = new CancelationCtx(this, "FlushContext");
            do {
                _AllowErrors().FlushContext(h);
            } while (ctx.NeedRetry());
            return ctx.Done();
        }

        /// <summary>
        /// Create a simple unbound & unseeded session supporting session encryption.
        /// </summary>
        public AuthSession CancelSafeStartAuthSession(
            TpmSe sessionType,
            TpmAlgId authHash,
            int nonceCallerSize = 16)
        {
            byte[]  nonceTpm;
            var     EmptySalt = new byte[0];

            TpmHandle   hSess;
            var ctx = new CancelationCtx(this, "FlushContext");
            do {
                _AllowErrors();
                hSess = StartAuthSession(TpmRh.Null, TpmRh.Null,
                                         GetRandomBytes(nonceCallerSize), EmptySalt,
                                         sessionType, new SymDef(), authHash, out nonceTpm);
            } while (ctx.NeedRetry());
            ctx.Done();

            AuthSession sess = hSess + SessionAttr.ContinueSession;
            _InitializeSession(sess);
            return sess;
        }

        private string FormatString(string s, Object[] parms)
        {
            var b = new StringBuilder();
            b.AppendFormat(s, parms);
            return b.ToString();
        }

        public static TpmRc GetBaseErrorCode(TpmRc resultCode)
        {
            var resultCodeValue = (uint)resultCode;
            bool formatOneErrorType = ((resultCodeValue & 0x80) != 0);
            uint resultCodeMask = formatOneErrorType ? 0xBFU : 0x97FU;

            // Extract the actual error code
            uint maskedErrorVal = (uint)resultCode & resultCodeMask;
            var maskedError = (TpmRc)maskedErrorVal;
            return maskedError;
        }

        /// <summary>
        /// Lookup (non-handle) parameter number in input structure.
        /// </summary>
        /// <param name="inParmType"></param>
        /// <param name="parmNumber"></param>
        /// <returns></returns>
        private static string GetParmName(Type inParmType, uint parmNumber)
        {
            // Exclude prefix 'Tpm2' and suffix 'Request' from the structure containing
            // command input parameters. What remains is the command name.
            string cmdName = inParmType.Name.Substring(4, inParmType.Name.Length - 11);
            MethodInfo mi = typeof(Tpm2).GetMethod(cmdName);
            ParameterInfo[] pi = mi.GetParameters();

            int idx = 0;
            // Skip handles
            while (idx < pi.Length && pi[idx].ParameterType == typeof (TpmHandle)) ++idx;
            idx += (int)parmNumber - 1; // parmNumber is 1 based
            return idx < pi.Length ? pi[idx].Name : "Undefined (parameter index too big)";
        }

        /// <summary>
        /// Name processing for Load-style operations
        /// </summary>
        /// <param name="h"></param>
        /// <param name="tpmAssignedName"></param>
        /// <param name="publicPart"></param>
        internal void ProcessName(TpmHandle h, byte[] tpmAssignedName, TpmPublic publicPart)
        {
            // Has been configured to *not* throw an exception if the TPM returns an error.
            if (tpmAssignedName.Length == 0)
                return;

            // If the load-command fails then the name returned is NULL.
            if (!NamesEqual(publicPart.GetName(), tpmAssignedName))
            {
                Globs.Throw("TPM assigned name differs from what is expected");
            }
            h.Name = tpmAssignedName;
        }

        /// <summary>
        /// The TPM name is an opaque byte-array comprising the hashAlg concatenated with the hash value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tpmAssignedName"></param>
        /// <returns></returns>
        private static bool NamesEqual(byte[] name, byte[] tpmAssignedName)
        {
            return Globs.ArraysAreEqual(name, tpmAssignedName);
        }

        private void _ClearCommandPrelaunchContext()
        {
            if (OuterCommand != TpmCc.None)
                return;
            // Session data must not be cleared for nested commands
            foreach(SessionBase s in TempSessions)
            {
                CancelSafeFlushContext(s);
            }
            TempSessions.Clear();
            foreach(TpmHandle h in TempNames)
            {
                h.Name = null;
            }
            TempNames.Clear();
        }

        /// <summary>
        /// Clear per-invocation state like tpm._ExpectError()
        /// </summary>
        private void _ClearCommandContext()
        {
            if (OuterCommand != TpmCc.None)
            {
                CurrentCommand = OuterCommand;
                OuterCommand = TpmCc.None;
                return;
            }
            _LastCommand = CurrentCommand;
            CurrentCommand = TpmCc.None;
            ExpectedResponses = null;
            Sessions = new SessionBase[0];
            DecSession = null;
            EncSession = null;
            NonceTpmDec = null;
            NonceTpmEnc = null;

            if (TolerateErrorsPermanently <= 0)
            {
                TolerateErrors = false;
            }
        }

        private void RollNonces()
        {
            foreach (SessionBase ss in Sessions)
            {
                // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                if (ss is AuthSession)
                {
                    var sess = (AuthSession)ss;
                    sess.NewNonceCaller();
                }
            }
        }

        private void PrepareParmEncryptionSessions()
        {
            foreach (SessionBase b in Sessions)
            {
                var s = b as AuthSession;
                if ( s == null )
                    continue;
                CheckParamEncSessCandidate(s, SessionAttr.Decrypt);
                CheckParamEncSessCandidate(s, SessionAttr.Encrypt);
            }
            /// If the first auth session is followed by parameter decryption
            /// and or encryption session(s), the NonceTPM must be included
            /// into HMAC of the first auth session. This precludes encryption
            /// sessions removal by malware.
            if (DecSession != null && DecSession != Sessions[0])
            {
                NonceTpmDec = DecSession.NonceTpm;
            }
            if (EncSession != null && EncSession != Sessions[0] && EncSession != DecSession)
            {
                NonceTpmEnc = EncSession.NonceTpm;
            }
        }

        /// <summary>
        /// Copies parameters associated with the session handle encapsulated in the
        /// sess argument into the sess object. These parameters are the ones passed
        /// to the StartAuthSession command. They are remembered by this Tpm2 object,
        /// until this method is called.
        /// 
        /// Note that _InitializeSession() can be used only once for the given session
        /// handle, as the associated parameters are erased from Tpm2 Object after
        /// they were copied into AuthSession object for the first time.
        /// </summary>
        /// <param name="sess"></param>
        internal bool _InitializeSession(AuthSession sess)
        {
            if (!sess.Initialized())
            {
                if (!SessionParams.ContainsKey(sess))
                {
                    // There are no session parameters associated with the session
                    // handle (e.g., when the session was created by other Tpm2 object).
                    return false;
                }
                sess.Init(SessionParams[sess]);
                sess.CalcSessionKey();
                SessionParams.Remove(sess);
            }
            return true;
        }

        private void PrepareRequestSessions(CommandInfo commandInfo, TpmHandle[] inHandles)
        {
            if (OuterCommand != TpmCc.None)
            {
                // Nested commands must not use sessions
                return;
            }

            Debug.Assert(commandInfo.AuthHandleCountIn <= inHandles.Length);

            // Does the command require authorization and not all sessions are provided?
            if (!_Behavior.Strict && commandInfo.AuthHandleCountIn > Sessions.Length)
            {
                // Allocate missing sessions
                if (Sessions.Length == 0)
                {
                    Sessions = new SessionBase[commandInfo.AuthHandleCountIn];
                }
                else
                {
                    SessionBase[] explicitSessions = Sessions;
                    Sessions = new SessionBase[commandInfo.AuthHandleCountIn];
                    explicitSessions.CopyTo(Sessions, 0);
                }
            }

            if (Sessions.Length == 0)
            {
                return;
            }

            for (int i = 0; i < Sessions.Length; ++i)
            {
                SessionBase s = Sessions[i];

                // Is authorization for this handle explicitly suppressed?
                if (s == SessionBase.None)
                    continue;

                TpmHandle authHandle = null;
                if (i < commandInfo.AuthHandleCountIn)
                {
                    authHandle = inHandles[i];
                    switch (authHandle.handle)
                    {
                    case (uint)TpmRh.Owner:
                        authHandle.Auth = OwnerAuth;
                        break;
                    case (uint)TpmRh.Endorsement:
                        authHandle.Auth = EndorsementAuth;
                        break;
                    case (uint)TpmRh.Platform:
                        authHandle.Auth = PlatformAuth;
                        break;
                    case (uint)TpmRh.Lockout:
                        authHandle.Auth = LockoutAuth;
                        break;
                    default:
                        if (authHandle.GetType() == Ht.Pcr && PcrHandles != null)
                        {
                            int pcrId = (int)authHandle.GetOffset();
                            Debug.Assert(pcrId < PcrHandles.Length);
                            if (PcrHandles[pcrId] != null)
                            {
                                authHandle.Auth = PcrHandles[pcrId].Auth;
                            }
                        }
                        break;
                    }
                }

                if (SessionBase.IsPlaceholder(s))
                {
                    // Create missing session
                    if (s == SessionBase.Hmac ||
                        s == SessionBase.Default && _GetUnderlyingDevice().NeedsHMAC)
                    {
                        s = Sessions[i] = CancelSafeStartAuthSession(TpmSe.Hmac, TpmAlgId.Sha256);
                        // Stash away session object to flush it from TPM after the command completion
                        TempSessions.Add(Sessions[i]);
                    }
                    else
                    {
                        s = Sessions[i] = new Pwap(authHandle.Auth);
                    }
                }
                if (s.Handle != TpmRh.TpmRsPw && !_InitializeSession(s as AuthSession))
                {
                    // There are no session parameters associated with the session
                    // handle (e.g., when the session was created by other Tpm2 object).
                    Globs.Throw("Wrong session handle");
                }
                s.AuthHandle = authHandle;
            }

            foreach (TpmHandle h in inHandles)
            {
                if (h.Name == null)
                {
                    byte[] name = null;
                    // Use try-catch to intercept possible TpmRc.Handle error.
                    try
                    {
                        switch (h.GetType())
                        {
                            case Ht.Transient:
                            case Ht.Persistent:
                            {
                                byte[] qualName = null;
                                ReadPublic(h, out name, out qualName);
                                break;
                            }
                            case Ht.NvIndex:
                            {
                                NvPublic pub = NvReadPublic(h, out name);
                                if (!pub.attributes.HasFlag(NvAttr.Written) ||
                                    (pub.attributes & (NvAttr.ClearStclear | NvAttr.Orderly)) != 0)
                                {
                                    TempNames.Add(h);
                                }
                                break;
                            }
                        }
                    }
                    catch (TpmException)
                    {
                        // Failed to read public part of the object. Leave its name empty.
                    }
                    h.Name = name;
                }
            }

            // Roll the nonces.
            RollNonces();

            // Determine if parameter encryption/decryption is necessary.
            PrepareParmEncryptionSessions();
        }

        /// <summary>
        /// Create the InSessions objects.
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="inHandles"></param>
        private SessionIn[] CreateRequestSessions(byte[] parms, TpmHandle[] inHandles)
        {
            // Commands implicitly issued by TSS.Net (to prepare execution of a user
            // issued command) never require authorization.
            if (OuterCommand != TpmCc.None)
                return new SessionIn[0];

            bool firstSession = true;
            var sessions = new List<SessionIn>();
            foreach (SessionBase s in Sessions)
            {
                if (s == SessionBase.None)
                {
                    // Authorization was explicitly suppressed
                }
                // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                else if (s is Pwap)
                {
                    sessions.Add(new SessionIn(new TpmHandle(TpmRh.TpmRsPw),
                                               new byte[0],
                                               SessionAttr.ContinueSession,
                                               s.Handle.Auth ?? new byte[0]));
                }
                // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                else if (s is AuthSession)
                {
                    var sess = (AuthSession)s;

                    byte[] parmHash = GetCommandHash(sess.AuthHash, parms, inHandles);
                    byte[] hmac = sess.GetAuthHmac(parmHash,
                                                   Direction.Command,
                                                   firstSession ? NonceTpmDec : null,
                                                   firstSession ? NonceTpmEnc : null);

                    sessions.Add(new SessionIn(s, sess.NonceCaller, sess.Attrs, hmac));
                }
                else
                {
                    Globs.Throw("CreateRequestSessions: Unknown session type");
                }
                firstSession = false;
            }
            return sessions.ToArray();
        }

        /// <summary>
        /// Validate the output sessions and update the session state as needed
        /// </summary>
        /// <param name="outHandles"></param>
        /// <param name="commandCode"></param>
        /// <param name="responseCode"></param>
        /// <param name="outSessions"></param>
        /// <param name="outParmsNoHandles"></param>
        // ReSharper disable once UnusedParameter.Local
        private void ProcessResponseSessions(SessionOut[] outSessions)
        {
            int numSessions = Sessions.Length;
            if (numSessions == 0 || numSessions > outSessions.Length)
            {
                return;
            }

            int outSessionCount = 0;
            foreach (SessionBase s in Sessions)
            {
                var outSess = outSessions[outSessionCount++];
                if (s is AuthSession)
                {
                    var sess = (AuthSession)s;
                    sess.SetNonceTpm(outSess.nonceTpm);
                    sess.Attrs = outSess.attributes; // | SessionAttr.ContinueSession;
                }
            }
        } // ProcessResponseSessions

        private void ValidateResponseSessions(
            TpmHandle[] outHandles,
            SessionOut[] outSessions,
            TpmCc commandCode,
            TpmRc responseCode,
            byte[] outParmsNoHandles)
        {
            int numSessions = Sessions.Length;
            if (numSessions == 0 || numSessions > outSessions.Length)
            {
                return;
            }

            int outSessionCount = 0;
            foreach (SessionBase s in Sessions)
            {
                SessionOut outSess = outSessions[outSessionCount++];
                if (s is Pwap)
                {
                    if (outSess.nonceTpm.Length != 0)
                    {
                        throw new TpmFailure("PWAP returned non-empty nonce");
                    }
                    if (outSess.auth.Length != 0)
                    {
                        throw new TpmFailure("PWAP returned non-empty auth value");
                    }
                }
                // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                else if (s is AuthSession)
                {
                    var sess = s as AuthSession;

                    byte[] parmsHash = GetExpectedResponseHash(sess.AuthHash, outParmsNoHandles, commandCode, responseCode);
                    byte[] expectedHmac = sess.GetAuthHmac(parmsHash, Direction.Response);

                    if (outSess.auth.Length != 0)
                    {
                        if (!Globs.ArraysAreEqual(outSess.auth, expectedHmac))
                        {
                            //Globs.Throw<TpmFailure>("Bad response HMAC");
                            throw new TpmFailure("Bad response HMAC");
                        }
                    }
                }
                else
                {
                    throw new TpmFailure("Invalid response session type");
                }
            }
        } // ValidateResponseSessions

        /// <summary>
        /// Reference to parameter decryption session, or null.
        /// It is not used to manage lifetime of the associated TPM session handle.
        /// </summary>
        private AuthSession DecSession;

        /// <summary>
        /// Reference to response encryption session, or null.
        /// It is not used to manage lifetime of the associated TPM session handle.
        /// </summary>
        private AuthSession EncSession;

        /// <summary>
        /// NonceTPM of the command parameter decryption session when it is present
        /// and is not the first auth session. It is included into HMAC of the first
        /// auth session to preclude this sessions removal by malware.
        /// </summary>
        private byte[] NonceTpmDec;

        /// <summary>
        /// NonceTPM of the response parameter encryption session when it is present
        /// and is not the first auth session. It is included into HMAC of the first
        /// auth session to preclude this sessions removal by malware.
        /// </summary>
        private byte[] NonceTpmEnc;

        public void _GarbleNextSessionHmac(byte[] garbageSalt = null)
        {
            NonceTpmDec = garbageSalt ?? AuthValue.FromRandom(20);
        }

        private void CheckParamEncSessCandidate(AuthSession candidate, SessionAttr directionFlag)
        {
            if (!candidate.Attrs.HasFlag(directionFlag))
            {
                return;
            }

            bool decrypt = directionFlag == SessionAttr.Decrypt;

            if (!candidate.CanEncrypt())
            {
                Globs.Throw(string.Format("{0} session is missing symmetric algorithm",
                                          decrypt ? "Decryption" : "Encryption"));
            }
            if ((decrypt ? DecSession : EncSession) != null)
            {
                Globs.Throw(string.Format("Multiple {0} sessions",
                                          decrypt ? "decryption" : "encryption"));
            }
            if (decrypt)
            {
                DecSession = candidate;
            }
            else
            {
                EncSession = candidate;
            }
        }

        /// <summary>
        /// First determine whether parm enc/decryption is in effect for this command. If not 
        /// return an unmodified buffer.  If so then do the parm encryption.
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="commandInfo"></param>
        /// <param name="offset"></param>
        /// <param name="inOrOut"></param>
        /// <returns></returns>
        private byte[] DoParmEncryption(byte[] parms, CommandInfo commandInfo, int offset, Direction inOrOut)
        {
            if (OuterCommand != TpmCc.None)
                return parms;

            AuthSession     encSess = null;
            ParmCryptInfo   encFlag2, encFlag4;

            if (inOrOut == Direction.Command)
            {
                encSess = DecSession;
                encFlag2 = ParmCryptInfo.EncIn2;
                encFlag4 = ParmCryptInfo.EncIn4;
            }
            else
            {
                encSess = EncSession;
                encFlag2 = ParmCryptInfo.DecOut2;
                encFlag4 = ParmCryptInfo.DecOut4;
            }

            // See if encryption/decryption is needed in whatever direction we are currently going ...
            if (encSess == null)
            {
                return parms;
            }
            if ((commandInfo.TheParmCryptInfo & (encFlag2 | encFlag4)) == 0)
            {
                Globs.Throw(string.Format("Command {0} cannot use {1} session",
                                          commandInfo.CommandCode,
                                          inOrOut == Direction.Command ? "decryption" : "encryption"));
                return parms;
            }

            // ... and get the length of the size counter prepending the XCrypted parameter.
            int countSize = commandInfo.TheParmCryptInfo.HasFlag(encFlag2) ? 2 : 4;

            // Now get the encrypted data (first parm) pos and length ...
            // We are interpreting the byte-stream starting at startPos as a length (2 or 4 byte)
            // prepended buffer
            byte[] lenX = Globs.CopyData(parms, offset, countSize);
            uint len = Globs.NetToHostVar(lenX);
            byte[] firstParm = Globs.CopyData(parms, offset + countSize, (int)len);
            byte[] encFirstParm = encSess.ParmEncrypt(firstParm, inOrOut);

            for (int j = 0; j < len; j++)
            {
                parms[offset + countSize + j] = encFirstParm[j];
            }
            return parms;
        } // DoParmEncryption()


        /// <summary>
        /// Updates information associated by the library with TPM entity handles upon
        /// successful completion of a command that either creates a new entity or
        /// changes the properties of an existing one.
        /// 
        /// Some important data associated with TPM entities cannot be retrieved from
        /// TPM either because of their sensitivity or because of substantial overhead.
        /// The information of the former kind is an auth value (for permanent handles,
        /// transient and persistent objects, NV indices) and a bound handle (for
        /// sessions). Information tracked for the sake of performance optimization
        /// is objects and NV index name.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="inParms"></param>
        /// <param name="inHandles"></param>
        // ReSharper disable once UnusedParameter.Local
        private void UpdateHandleData(TpmCc ordinal, TpmStructureBase inParms, TpmHandle[] inHandles, TpmStructureBase outParms)
        {
            switch (ordinal)
            {
                case TpmCc.Create:
                {
                    var req = (Tpm2CreateRequest)inParms;
                    var resp = (Tpm2CreateResponse)outParms;
                    TpmHash priv = TpmHash.FromData(PrivHashAlg, resp.outPrivate.buffer);
                    AuthValues[priv] = Globs.CopyData(req.inSensitive.userAuth);
                    break;
                }
                case TpmCc.CreatePrimary:
                {
                    var req = (Tpm2CreatePrimaryRequest)inParms;
                    var resp = (Tpm2CreatePrimaryResponse)outParms;
                    resp.objectHandle.Auth = req.inSensitive.userAuth;
                    ProcessName(resp.objectHandle, resp.name, resp.outPublic);
                    break;
                }
                case TpmCc.Load:
                {
                    var req = (Tpm2LoadRequest)inParms;
                    var resp = (Tpm2LoadResponse)outParms;
                    TpmHash priv = TpmHash.FromData(PrivHashAlg, req.inPrivate.buffer);
                    if (AuthValues.ContainsKey(priv))
                        resp.objectHandle.Auth = AuthValues[priv];
                    ProcessName(resp.objectHandle, resp.name, req.inPublic);
                    break;
                }
                case TpmCc.LoadExternal:
                {
                    var req = (Tpm2LoadExternalRequest)inParms;

                    if (req.inPublic.nameAlg != TpmAlgId.Null)
                    {
                        var resp = (Tpm2LoadExternalResponse)outParms;
                        byte[] name = req.inPublic.GetName();
                        ProcessName(resp.objectHandle, resp.name, req.inPublic);
                    }
                    break;
                }
                case TpmCc.StartAuthSession:
                {
                    var req = (Tpm2StartAuthSessionRequest)inParms;
                    var resp = (Tpm2StartAuthSessionResponse)outParms;
                    SessionParams[resp.sessionHandle] =
                            new AuthSession(req.sessionType, req.tpmKey, req.bind,
                                            req.nonceCaller, resp.nonceTPM,
                                            req.symmetric, req.authHash);
                    break;
                }
                case TpmCc.HmacStart:
                {
                    var req = (Tpm2HmacStartRequest)inParms;
                    var resp = (Tpm2HmacStartResponse)outParms;
                    resp.sequenceHandle.Auth = req.auth;
                    resp.sequenceHandle.Name = null;
                    break;
                }
                case TpmCc.NvDefineSpace:
                {
                    var req = (Tpm2NvDefineSpaceRequest)inParms;
                    req.publicInfo.nvIndex.Auth = req.auth;
                    req.publicInfo.nvIndex.Name = null;
                    break;
                }
                case TpmCc.NvChangeAuth:
                {
                    var req = (Tpm2NvChangeAuthRequest)inParms;
                    req.nvIndex.Auth = req.newAuth;
                    break;
                }
                case TpmCc.ObjectChangeAuth:
                {
                    var req = (Tpm2ObjectChangeAuthRequest)inParms;
                    var resp = (Tpm2ObjectChangeAuthResponse)outParms;
                    TpmHash priv = TpmHash.FromData(PrivHashAlg, resp.outPrivate.buffer);
                    AuthValues[priv] = Globs.CopyData(req.newAuth);
                    break;
                }
                case TpmCc.HierarchyChangeAuth:
                {
                    var req = (Tpm2HierarchyChangeAuthRequest)inParms;
                    AuthValue auth = Globs.CopyData(req.newAuth);
                    switch (req.authHandle.handle)
                    {
                        case (uint)TpmRh.Owner: OwnerAuth = auth; break;
                        case (uint)TpmRh.Endorsement: EndorsementAuth = auth; break;
                        case (uint)TpmRh.Platform: PlatformAuth = auth; break;
                        case (uint)TpmRh.Lockout: LockoutAuth = auth; break;
                    }
                    req.authHandle.Auth = auth;
                    break;
                }
                case TpmCc.PcrSetAuthValue:
                {
                    var req = (Tpm2PcrSetAuthValueRequest)inParms;
                    req.pcrHandle.Auth = req.auth;
                    if (PcrHandles == null)
                    {
                        uint numPcrs = GetProperty(this, Pt.PcrCount);
                        PcrHandles = new TpmHandle[numPcrs];
                    }
                    int pcrId = (int)req.pcrHandle.GetOffset();
                    Debug.Assert(pcrId < PcrHandles.Length);
                    PcrHandles[pcrId] = req.pcrHandle;
                    break;
                }
                case TpmCc.EvictControl:
                {
                    var req = (Tpm2EvictControlRequest)inParms;
                    var resp = (Tpm2EvictControlResponse)outParms;
                    if (req.objectHandle.GetType() != Ht.Persistent)
                    {
                        req.persistentHandle.Auth = req.objectHandle.Auth;
                        req.persistentHandle.Name = req.objectHandle.Name;
                    }
                    break;
                }
                case TpmCc.Clear:
                {
                    OwnerAuth = new AuthValue();
                    EndorsementAuth = new AuthValue();
                    LockoutAuth = new AuthValue();
                    break;
                }
                case TpmCc.NvWrite:
                {
                    var req = (Tpm2NvWriteRequest)inParms;
                    // Force name recalculation before next use
                    req.nvIndex.Name = null;
                    break;
                }
                case TpmCc.NvWriteLock:
                {
                    var req = (Tpm2NvWriteLockRequest)inParms;
                    // Force name recalculation before next use
                    req.nvIndex.Name = null;
                    break;
                }
                case TpmCc.NvReadLock:
                {
                    var req = (Tpm2NvReadLockRequest)inParms;
                    // Force name recalculation before next use
                    req.nvIndex.Name = null;
                    break;
                }
                case TpmCc.HashSequenceStart:
                {
                    var req = (Tpm2HashSequenceStartRequest)inParms;
                    var resp = (Tpm2HashSequenceStartResponse)outParms;
                    resp.sequenceHandle.Auth = req.auth;
                    break;
                }
                case TpmCc.Startup:
                {
                    var req = (Tpm2StartupRequest)inParms;
                    if (req.startupType == Su.Clear)
                    {
                        PlatformAuth = new AuthValue();
                    }
                    break;
                }
                case TpmCc.ContextSave:
                {
                    var req = (Tpm2ContextSaveRequest)inParms;
                    var resp = (Tpm2ContextSaveResponse)outParms;
                    resp.context.savedHandle.Auth = req.saveHandle.Auth;
                    resp.context.savedHandle.Name = req.saveHandle.Name;
                    break;
                }
                case TpmCc.ContextLoad:
                {
                    var req = (Tpm2ContextLoadRequest)inParms;
                    var resp = (Tpm2ContextLoadResponse)outParms;
                    resp.loadedHandle.Auth = req.context.savedHandle.Auth;
                    resp.loadedHandle.Name = req.context.savedHandle.Name;
                    break;
                }
                case TpmCc.NvUndefineSpaceSpecial:
                {
                    var req = (Tpm2NvUndefineSpaceSpecialRequest)inParms;
                    req.nvIndex.Auth = null;
                    break;
                }
            }
        } // UpdateHandleData()

        /// <summary>
        /// Calculate the command hash.  Note that the handles are replaced by the name of the referenced object
        /// </summary>
        /// <param name="hashAlg"></param>
        /// <param name="commandParms"></param>
        /// <param name="handles"></param>
        /// <returns></returns>
        private byte[] GetCommandHash(TpmAlgId hashAlg, byte[] commandParms, TpmHandle[] handles)
        {
            var temp = new Marshaller();
            temp.Put(CurrentCommand, "ordinal");

            for (int j = 0; j < handles.Length; j++)
            {
                temp.Put(handles[j].Name, "name + " + j);
            }

            temp.Put(commandParms, "commandParms");
            byte[] parmsHash = CryptoLib.HashData(hashAlg, temp.GetBytes());

#if false
            Console.WriteLine("========= hash:{0:X2} =========", (uint)hashAlg);
            for (int j = 0; j < handles.Length; j++)
                Console.WriteLine("{0:X8}: {1}", handles[j].handle, Globs.FormatBytesCompact("", handles[j].Name));
            Console.WriteLine(Globs.FormatBytes("parms:\n", commandParms));
            Console.WriteLine(Globs.FormatBytesCompact("cpHash: ", parmsHash));
            Console.WriteLine("---------------------------");
#endif
            return parmsHash;
        }

        /// <summary>
        /// The response hash includes the command ordinal, response code, and the actual command bytes.
        /// </summary>
        /// <param name="hashAlg"></param>
        /// <param name="commandCode"></param>
        /// <param name="responseCode"></param>
        /// <param name="responseParmsNoHandles"></param>
        /// <returns></returns>
        private byte[] GetExpectedResponseHash(
            TpmAlgId hashAlg,
            byte[] responseParmsNoHandles,
            TpmCc commandCode,
            TpmRc responseCode)
        {
            var temp = new Marshaller();
            temp.Put(responseCode, "responseCode");
            temp.Put(commandCode, "currentCommand");
            temp.Put(responseParmsNoHandles, null);

            byte[] parmsHash = CryptoLib.HashData(hashAlg, temp.GetBytes());
            return parmsHash;
        }

        /// <summary>
        /// Log the command / response to the debug stream
        /// </summary>
        /// <param name="commandCode"></param>
        /// <param name="theStruct"></param>
        /// <param name="outOrIn"></param>
        private void Log(TpmCc commandCode, TpmStructureBase theStruct, int outOrIn)
        {
            if (!CommandLogging)
                return;
            Debug.WriteLine("COMMAND " + Enum.GetName(typeof (TpmCc), commandCode));
            switch (outOrIn)
            {
                case 0:
                    Debug.WriteLine("COMMAND STRUCTURE");
                    break;
                case 1:
                    Debug.WriteLine("RESPONSE STRUCTURE");
                    break;
            }
            string ss = theStruct.ToString();
            Debug.WriteLine(ss);
        }

        private byte[] GetRandomBytes(int numBytes)
        {
            // todo - caller settable
            return Globs.GetRandomBytes(numBytes);
        }

        public void Dispose()
        {
            if (Device != null)
            {
                Device.Dispose();
            }
            Device = null;
        }

        /// <summary>
        /// Return a structure describing a command given a commandCode
        /// </summary>
        /// <param name="commandCode"></param>
        /// <returns></returns>
        public static CommandInfo CommandInfoFromCommandCode(TpmCc commandCode)
        {
            // TODO: faster lookup
            CommandInfo command = null;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (CommandInfo theInfo in CommandInformation.Info)
            {
                if (theInfo.CommandCode == commandCode)
                {
                    command = theInfo;
                    break;
                }
            }

            return command;
        }

        public static bool IsTbsError(uint code)
        {
            var res = (TbsResult)code;
            return res == TbsResult.TBS_E_BLOCKED
                   || res == TbsResult.TBS_E_INTERNAL_ERROR
                   || res == TbsResult.TBS_E_BAD_PARAMETER
                   || res == TbsResult.TBS_E_COMMAND_CANCELED;
        }
    }

    // Only length prepended first-in or first-out parms can be encrypted.
    [Flags]
    public enum ParmCryptInfo
    {
        EncIn2 = 1,
        EncIn4 = 2,
        DecOut2 = 4,
        DecOut4 = 8
    }

    /// <summary>
    /// Information about a command derived from the specification
    /// </summary>
    public class CommandInfo
    {
        // TODO: add NumAuthHandles and USER/ADMIN auth requirements
        public CommandInfo(
            TpmCc theCode,
            uint inHandleCount,
            uint outHandleCount,
            uint inAuthHandleCount,
            Type inStructType,
            Type outStructType,
            uint parmCryptInfo,
            string origInputHandleTypes)
        {
            CommandCode = theCode;
            HandleCountIn = inHandleCount;
            HandleCountOut = outHandleCount;
            AuthHandleCountIn = inAuthHandleCount;
            InStructType = inStructType;
            OutStructType = outStructType;
            TheParmCryptInfo = (ParmCryptInfo)parmCryptInfo;
            InHandleOrigTypes = origInputHandleTypes;
        }

        public TpmCc CommandCode;
        public uint HandleCountIn;
        public uint AuthHandleCountIn;
        public uint HandleCountOut;
        public Type InStructType;
        public Type OutStructType;
        public ParmCryptInfo TheParmCryptInfo;
        public string InHandleOrigTypes;

        public override string ToString()
        {
            return CommandCode.ToString();
        }
    }

    public class CommandProcessor
    {
        /// <summary>
        /// Splits a TpmStructureBase command or response, and splits it into 
        /// handles and the parms data
        /// </summary>
        /// <param name="s"></param>
        /// <param name="numHandles"></param>
        /// <param name="handles"></param>
        /// <param name="parms"></param>
        public static void Fragment(TpmStructureBase s, uint numHandles, out TpmHandle[] handles, out byte[] parms)
        {
            handles = new TpmHandle[numHandles];
            // Get the handles (note we need to return the actual object because it contains the name.
            // The handles are always first, and will be simple fields or get/set props.
            MemberInfo[] fields;
            try
            {
                fields = s.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            }
            catch (Exception)
            {
                throw;
            }

            int fieldPos = 0;
            for (int j = 0; j < numHandles; j++)
            {
                MemberInfo f;
                do
                {
                    // Ignore setters
                    f = fields[fieldPos++];
                } while (f.Name.StartsWith("set_"));
                // Either a simple field
                var ff = f as FieldInfo;
                if (ff != null)
                {
                    handles[j] = (TpmHandle)ff.GetValue(s);
                }
                // A get or set accessor
                var mm = f as MethodInfo;
                if (mm != null)
                {
                    object hRep = mm.Invoke(s, null);
                    handles[j] = hRep is TpmHandle ? (TpmHandle)hRep : ((TpmHandleX)hRep).Handle;
                }
            }
            // And the rest is the parms
            byte[] b = Marshaller.GetTpmRepresentation(s);
            parms = new byte[b.Length - numHandles * 4];
            Array.Copy(b, (int)numHandles * 4, parms, 0, b.Length - (int)numHandles * 4);
        }

        public static CrackedCommand CrackCommand(byte[] command)
        {

            var c = new CrackedCommand();
            bool success = CrackCommand(command, out c.Header, out c.Handles, out c.Sessions, out c.CommandParms);
            if (!success)
            {
                return null;
            }
            return c;
        }

        /// <summary>
        /// Opens a properly-formed TPM command stream into its constituent components.
        /// Note: commandParams does NOT include handles.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="header"></param>
        /// <param name="handles"></param>
        /// <param name="sessions"></param>
        /// <param name="commandParms"></param>
        public static bool CrackCommand(
            byte[] command,
            out CommandHeader header,
            out TpmHandle[] handles,
            out SessionIn[] sessions,
            out byte[] commandParms)
        {
            var m = new Marshaller(command);
            header = m.Get<CommandHeader>();
            CommandInfo commandInfo = Tpm2.CommandInfoFromCommandCode(header.CommandCode);
            if (header.Tag == TpmSt.Null)
            {
                // A diagnostics command. Pass through unmodified
                handles = null;
                sessions = null;
                commandParms = null;
                return false;
            }
            handles = new TpmHandle[commandInfo.HandleCountIn];
            for (int j = 0; j < handles.Length; j++)
            {
                handles[j] = m.Get<TpmHandle>();
            }
            // Note sessions are only present if the command tag indicates sessions
            if (header.Tag == TpmSt.Sessions)
            {
                var sessionLength = m.Get<uint>();
                uint sessionEnd = m.GetGetPos() + sessionLength;
                var inSessions = new List<SessionIn>();
                while (m.GetGetPos() < sessionEnd)
                {
                    var s = m.Get<SessionIn>();
                    inSessions.Add(s);
                }
                sessions = inSessions.ToArray();
            }
            else
            {
                sessions = new SessionIn[0];
            }
            // And finally parameters
            commandParms = m.GetArray<byte>((int)(m.GetValidLength() - m.GetGetPos()));
            if (m.GetValidLength() != header.CommandSize)
            {
                Globs.Throw("Command length in header does not match input byte-stream");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Create a TPM command byte stream from constituent components
        /// </summary>
        /// <param name="commandCode"></param>
        /// <param name="handles"></param>
        /// <param name="sessions"></param>
        /// <param name="parmsWithoutHandles"></param>
        /// <returns></returns>
        public static byte[] CreateCommand(
            TpmCc commandCode,
            TpmHandle[] handles,
            SessionIn[] sessions,
            byte[] parmsWithoutHandles)
        {

            // ReSharper disable once UnusedVariable
            CommandInfo commandInfo = Tpm2.CommandInfoFromCommandCode(commandCode);

            var m = new Marshaller();
            TpmSt tag = sessions.Length == 0 ? TpmSt.NoSessions : TpmSt.Sessions;
            m.Put(tag, "tag");
            m.PushLength(4);
            m.Put(commandCode, "commandCode");
            foreach (TpmHandle h in handles)
            {
                m.Put(h, "handle");
            }

            if (tag == TpmSt.Sessions)
            {
                var m2 = new Marshaller();
                foreach (SessionIn s in sessions)
                {
                    m2.Put(s, "session");
                }
                m.PutUintPrependedArray(m2.GetBytes(), "sessions");
            }

            m.Put(parmsWithoutHandles, "parms");

            m.PopAndSetLengthToTotalLength();
            return m.GetBytes();
        }

        public static ResponseInfo SplitResponse(byte[] response, uint numHandles)
        {
            var r = new ResponseInfo();
            SplitResponse(response,
                          numHandles,
                          out r.Tag,
                          out r.ParamSize,
                          out r.ResponseCode,
                          out r.Handles,
                          out r.Sessions,
                          out r.ResponseParmsNoHandles,
                          out r.ResponseParmsWithHandles);
            return r;
        }

        public static void SplitResponse(
            byte[] response,
            uint numHandles,
            out TpmSt tag,
            out uint paramSize,
            out TpmRc responseCode,
            out TpmHandle[] handles,
            out SessionOut[] sessions,
            out byte[] responseParmsNoHandles,
            out byte[] responseParmsWithHandles)
        {
            var m = new Marshaller(response);
            tag = m.Get<TpmSt>();
            paramSize = m.Get<uint>();
            responseCode = m.Get<TpmRc>();
            // If error we only get the header
            if (responseCode != TpmRc.Success)
            {
                handles = new TpmHandle[0];
                sessions = new SessionOut[0];
                responseParmsNoHandles = new byte[0];
                responseParmsWithHandles = new byte[0];
                return;
            }

            handles = new TpmHandle[numHandles];
            for (int j = 0; j < numHandles; j++)
            {
                handles[j] = m.Get<TpmHandle>();
            }
            uint parmsEnd = m.GetValidLength();
            if (tag == TpmSt.Sessions)
            {
                var sessionOffset = m.Get<uint>();
                uint startOfParmsX = m.GetGetPos();
                parmsEnd = startOfParmsX + sessionOffset;
                m.SetGetPos(parmsEnd);
                var sessX = new List<SessionOut>();
                while (m.GetGetPos() < m.GetValidLength())
                {
                    var s = m.Get<SessionOut>();
                    sessX.Add(s);
                }
                sessions = sessX.ToArray();
                m.SetGetPos(startOfParmsX);
            }
            else
            {
                sessions = new SessionOut[0];
            }

            uint startOfParms = m.GetGetPos();
            uint parmsLength = parmsEnd - m.GetGetPos();

            // Get the response buf with no handles
            responseParmsNoHandles = new byte[parmsLength];
            Array.Copy(response, (int)startOfParms, responseParmsNoHandles, 0, (int)parmsLength);

            // Get the response buf with handles
            responseParmsWithHandles = new byte[parmsLength + numHandles * 4];
            Array.Copy(response, 10, responseParmsWithHandles, 0, (int)numHandles * 4);
            Array.Copy(response, (int)startOfParms, responseParmsWithHandles, (int)numHandles * 4, (int)parmsLength);
        }

        public static byte[] CreateResponse(
            TpmRc responseCode,
            TpmHandle[] handles,
            SessionOut[] sessions,
            byte[] responseParmsNoHandles)
        {
            var m = new Marshaller();
            TpmSt tag = sessions.Length == 0 ? TpmSt.NoSessions : TpmSt.Sessions;

            m.Put(tag, "tag");
            m.PushLength(4);
            m.Put(responseCode, "responseCode");

            foreach (TpmHandle h in handles)
            {
                m.Put(h, "handle");
            }

            if (tag == TpmSt.Sessions)
            {
                m.Put((uint)responseParmsNoHandles.Length, "parmsLenght");
            }

            m.Put(responseParmsNoHandles, "parms");
            foreach (SessionOut s in sessions)
                m.Put(s, "session");
            m.PopAndSetLengthToTotalLength();
            return m.GetBytes();
        }

        public static string ParseCommand(byte[] buf)
        {
            CommandHeader commandHeader;
            TpmHandle[] inHandles;
            SessionIn[] inSessions;
            byte[] commandParmsNoHandles;
            string response = "";

            bool ok = CrackCommand(buf, out commandHeader, out inHandles, out inSessions, out commandParmsNoHandles);
            if (!ok)
            {
                response = "The TPM command is not properly formatted.  Doing the best I can...\n";
            }
            CommandInfo command = Tpm2.CommandInfoFromCommandCode(commandHeader.CommandCode);
            if (command == null)
            {
                response += String.Format("The command-code {0} is not defined.  Aborting\n", commandHeader.CommandCode);
                return response;
            }
            response += "Header:\n";
            response += commandHeader + "\n";

            var m2 = new Marshaller();
            foreach (TpmHandle h in inHandles)
            {
                m2.Put(h, "");
            }

            byte[] commandParmsWithHandles = Globs.Concatenate(new[] {m2.GetBytes(), commandParmsNoHandles});
            var m = new Marshaller(commandParmsWithHandles);
            object inParms = m.Get(command.InStructType, "");
            response += "Command Parameters:\n";
            response += inParms + "\n";
            response += "Sessions [" + inSessions.Length + "]\n";
            for (int j = 0; j < inSessions.Length; j++)
            {
                // ReSharper disable once FormatStringProblem
                response += String.Format("{0}: 0x{1:x}\n", j, inSessions[j]);
            }
            return response;
        }

        public static TpmRc GetResponseCode(byte[] response)
        {
            if (response.Length > 10)
                return TpmRc.Success;

            var m = new Marshaller(response);
            // ReSharper disable once UnusedVariable
            var tag = m.Get<TpmSt>();
            // ReSharper disable once UnusedVariable
            var paramSize = m.Get<uint>();
            var responseCode = m.Get<TpmRc>();
            TpmRc maskedResponse = Tpm2.GetBaseErrorCode(responseCode);
            return maskedResponse;

        }

        public static string ParseResponse(string commandCode, byte[] buf)
        {
            TpmHandle[] outHandles;
            SessionOut[] outSessions;
            byte[] responseParmsNoHandles;
            byte[] responseParmsWithHandles;
            string response = "";
            if (1 != CommandInformation.Info.Count(item => item.CommandCode.ToString() == commandCode))
            {
                response = "Command code not recognized.  Defined command codes are:\n";
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (CommandInfo info in CommandInformation.Info)
                {
                    response += info.CommandCode.ToString() + " ";
                }
                return response;
            }

            CommandInfo command = CommandInformation.Info.First(item => item.CommandCode.ToString() == commandCode);
            TpmSt tag;
            uint paramSize;
            TpmRc responseCode;

            SplitResponse(buf,
                          command.HandleCountOut,
                          out tag,
                          out paramSize,
                          out responseCode,
                          out outHandles,
                          out outSessions,
                          out responseParmsNoHandles,
                          out responseParmsWithHandles);
            if (responseCode != TpmRc.Success)
            {
                TpmRc resultCode;
                response += "Error:\n";
                response += Tpm2.GetErrorString(command.InStructType, (uint)responseCode, out resultCode);
                return response;
            }

            // At this point in the processing stack we cannot deal with encrypted responses
            bool responseIsEncrypted = false;
            foreach (SessionOut s in outSessions)
            {
                if (s.attributes.HasFlag(SessionAttr.Encrypt)
                    &&
                    (command.TheParmCryptInfo.HasFlag(ParmCryptInfo.DecOut2) ||
                     command.TheParmCryptInfo.HasFlag(ParmCryptInfo.DecOut2))
                    )
                    responseIsEncrypted = true;
            }

            response += "Response Header:\n";
            response += "    Tag=" + tag.ToString() + "\n";
            response += "    Response code=" + responseCode.ToString() + "\n";

            response += "Response Parameters:\n";
            if (!responseIsEncrypted)
            {
                var m2 = new Marshaller(responseParmsWithHandles);
                Object inParms = m2.Get(command.OutStructType, "");
                response += inParms + "\n";
            }
            else
            {
                var m2 = new Marshaller(responseParmsWithHandles);
                Object encOutParms = null;
                switch (command.TheParmCryptInfo)
                {
                    // TODO: this is not the right type if we ever do size-checks
                    case ParmCryptInfo.DecOut2:
                        encOutParms = m2.Get(typeof (Tpm2bMaxBuffer), "");
                        break;
                    default:
                        Globs.Throw<NotImplementedException>("NOT IMPLEMENTED");
                        break;
                }
                response += "Encrypted: " + encOutParms + "\n";
            }

            response += "Sessions [" + outSessions.Length + "]\n";
            for (int j = 0; j < outSessions.Length; j++)
            {
                // ReSharper disable once FormatStringProblem
                response += String.Format("{0}: 0x{1:x}\n", j, outSessions[j]);
            }
            return response;
        }

        public static string CleanHex(string s)
        {
            // Get rid of the hex prefix
            s = s.Replace("0x", " ");
            s = s.Replace("0X", " ");

            // Get rid of the whitespace
            string[] s2 = s.Split(new[] {' ', '\n', '\r', '\t', '-'});
            // Stick it back together
            return s2.Aggregate("", (current, s3) => current + s3);
        }

        /// <summary>
        /// Interpret a HEX command string into a parsed command.  
        /// </summary>
        /// <param name="s"></param>
        public static string ParseCommand(string s)
        {
            s = CleanHex(s);
            byte[] commandBytes = Globs.ByteArrayFromHex(s);
            return ParseCommand(commandBytes);
        }

        /// <summary>
        /// Interpret a HEX command string into a parsed command.  
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="s"></param>
        public static string ParseResponse(string commandName, string s)
        {
            s = CleanHex(s);
            byte[] commandBytes = Globs.ByteArrayFromHex(s);
            return ParseResponse(commandName, commandBytes);
        }

    }

    public class ResponseInfo
    {
        public TpmSt Tag;
        public uint ParamSize;
        public TpmRc ResponseCode;
        public TpmHandle[] Handles;
        public SessionOut[] Sessions;
        public byte[] ResponseParmsNoHandles;
        public byte[] ResponseParmsWithHandles;
    }

    public class CrackedCommand
    {
        public CommandHeader Header;
        public TpmHandle[] Handles;
        public SessionIn[] Sessions;
        public byte[] CommandParms;
    }

    public class CommandModifier
    {
        public byte ActiveLocality = 0;
        public TbsPublicStubs.TBS_COMMAND_PRIORITY ActivePriority = TbsPublicStubs.TBS_COMMAND_PRIORITY.NORMAL;
    }

    internal class ReentrancyGuard : IDisposable
    {
        private readonly ReentrancyGuardContext Instance;

        internal ReentrancyGuard(ReentrancyGuardContext theInstance)
        {
            Instance = theInstance;
            int newCount = Interlocked.Increment(ref Instance.ThreadCount);
            if (newCount != 1)
            {
                throw new Exception("Illegal reentrancy/multithreading in Tpm2");
            }
        }

        public void Dispose()
        {
            Interlocked.Decrement(ref Instance.ThreadCount);
        }
    }

    internal class ReentrancyGuardContext
    {
        internal int ThreadCount;
    }
}
