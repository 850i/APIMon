using System;
using System.Collections.Generic;
using System.Threading;
using EasyHook;

namespace APIMonLib
{
    /// <summary>
    /// Instance of this class should reside in the sending entity.
    /// It uses bufferisation and in general thread safe
    /// But all safe hooking guidelines should be followed before using that.
    /// </summary>
    public class TransferUnitSender
    {
        private Thread processing_thread = null;

        private ChannelReceiver remote_receiver = null;

        public TransferUnitSender(String channel_name)
        {
            remote_receiver = (ChannelReceiver)RemoteHooking.IpcConnectClient<MarshalByRefObject>(channel_name);
            remote_receiver.ping();
        }

        private Queue<TransferUnit> transfer_units_queue = new Queue<TransferUnit>();

        private int _buffer_send_period = 50;

        private const int MAX_QUEUE_LENGTH = 5000;

        /// <summary>
        /// Shows how often enqueue for sending operation will contatin yeld
        /// </summary>
        private const int YELD_PERIOD_ENQUEUE=512;

        /// <summary>
        /// Shows for how long enqueuing should be postponed.
        /// </summary>
        private const int YELD_PERIOD_TIME = 1;

        private int yeld_counter = 0;

        private Random random = new Random();

        /// <summary>
        /// Returns true when it is advisable to yeld.
        /// It has side effects, so call it from synchronized evironment
        /// </summary>
        /// <returns></returns>
        private Boolean shouldYeld()
        {
            yeld_counter++;
            return yeld_counter % YELD_PERIOD_ENQUEUE == 0;
        }


        /// <summary>
        /// Time between two consequent data send procedures (buffer flushes).
        /// </summary>
        public int buffer_send_period
        {
            get { return _buffer_send_period; }
            set { _buffer_send_period = value; }
        }

        private bool _stopped;
        private Object sync_stopped = new Object();

        /// <summary>
        /// Returns true until channel completed execution
        /// </summary>
        public bool is_running
        {
            get
            {
                bool result;
                lock (sync_stopped) { result = _stopped; }
                return !result;
            }
        }

        /// <summary>
        /// Sets appropriate field that channel is not running anymore
        /// </summary>
        private void setIsNotRunningAnymore()
        {
            lock (sync_stopped) { _stopped = true; }
        }

        private bool _keep_running = true;
        private Object sync_keep_running = new Object();

        /// <summary>
        /// this property is set to false, when channel is scheduled for stop
        /// </summary>
        private bool keep_running
        {
            get
            {
                bool result;
                lock (sync_keep_running) { result = _keep_running; }
                return result;
            }
        }

        /// <summary>
        /// Schedules channel for stop
        /// </summary>
        public void stopSender()
        {
            lock (sync_keep_running) { _keep_running = false; }
        }

        /// <summary>
        /// Schedules transfer unit provided for sending
        /// </summary>
        /// <param name="tu"></param>
        public void sendTransferUnit(TransferUnit tu)
        {
				if (!is_running) throw new Exception("Sending of messages has been interrupted. Probably no receiver");
				bool should_yeld;
				//try {
				lock (transfer_units_queue) {
					transfer_units_queue.Enqueue(tu);
					should_yeld = shouldYeld();
					if (transfer_units_queue.Count > MAX_QUEUE_LENGTH) should_yeld = true;
				}
				//} catch (System.AccessViolationException e) {
				//    Console.WriteLine(e); 
				//    should_yeld = true;
				//}
				if (should_yeld) Thread.Sleep(YELD_PERIOD_TIME);
        }

        ///// <summary>
        ///// Schedules transfer unit provided for sending and initiates 
        ///// sending procedure immediately
        ///// </summary>
        ///// <param name="tu"></param>
        //public void sendTransferUnitAndFlushBuffer(TransferUnit tu)
        //{
        //    sendTransferUnit(tu);
        //    processing_thread.Interrupt();
        //}

        /// <summary>
        ///  Uses separate synchronous channel for sending exceptions back to the
        /// host installed this hook
        /// </summary>
        /// <param name="ex"></param>
        public void sendException(RemoteHookingException ex)
        {
            remote_receiver.receiveRemoteHookingException(ex);
        }

        /// <summary>
        /// Uses separate synchronous channel for sending text messages
        /// </summary>
        /// <param name="message"></param>
        public void sendTextMessage(String message)
        {
            remote_receiver.receiveTextMessage(message);
        }

        public APIFullName[] getApiCallsToIntercept()
        {
            return remote_receiver.getApiCallsToIntercept();
        }


        /// <summary>
        /// Connectivity testing
        /// </summary>
        public void ping()
        {
            remote_receiver.ping();
        }

        /// <summary>
        /// Sends all remaining data
        /// </summary>
        /// <returns>true if some data were sent and false otherwise</returns>
        public bool flushBuffer()
        {
            Queue<TransferUnit> array_to_transfer=null;
            bool nothing_to_send = true;
            lock (transfer_units_queue)
            {
                if (transfer_units_queue.Count != 0)
                {
                    array_to_transfer = new Queue<TransferUnit>(transfer_units_queue);
                    transfer_units_queue.Clear();
                    nothing_to_send = false;
                }
            }
            if (nothing_to_send)
            {
                return false;
            }
            else{
                remote_receiver.receiveTransferUnits(array_to_transfer);
                array_to_transfer.Clear();
                array_to_transfer = null;
                return true;
            }
        }

        /// <summary>
        /// Main procedure for sending data to the receiver. Blocks until
        /// stop is scheduled and remaining data is sent.
        /// </summary>
        public void processSendRequests()
        {
            this.processing_thread = Thread.CurrentThread;
            this.processing_thread.Priority = ThreadPriority.Highest;
            try
            {
                while (keep_running)
                {

                    try
                    {
                        Thread.Sleep(buffer_send_period);
                    }
                    catch
                    {

                    }
                    //try to flush buffer. If there is nothing to send, just ping.
                    if (!flushBuffer())
                    {
                        remote_receiver.ping();
                        if (random.Next(10000) < 5)
                        {
                            System.GC.Collect();
                        }
                    }
                    else
                    {
                        if (random.Next(10000) < 10)
                        {
                            System.GC.Collect();
                        }
                    }

                }
            }
            finally
            {
                setIsNotRunningAnymore();
            }

        }
    }
}
