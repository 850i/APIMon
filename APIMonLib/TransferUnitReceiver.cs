using System;
using System.Collections.Generic;
using System.Threading;

namespace APIMonLib
{
    public class TransferUnitReceiver
    {
        private Random random = new Random();

        public delegate void ReceiveTransferUnit(TransferUnit tu);

        public delegate void ReceiveTextMessage(String message);

        public delegate void ReceiveRemoteHookingException(RemoteHookingException exception);

        public delegate APIFullName[] ReceiveGetApiCallsToIntercept();

        private ReceiveTransferUnit _receiveTransferUnit = null;

        private ReceiveTextMessage _receiveTextMessage = null;

        private ReceiveRemoteHookingException _receiveReportException = null;

        private ReceiveGetApiCallsToIntercept _receiveGetApiCallsToIntercept = null;

        private static TransferUnitReceiver transfer_unit_receiver_instance = null;

		/// <summary>
		/// Refernce to the single instance of TransferUnitReceiver class
		/// </summary>
		public static TransferUnitReceiver instance { get{return getInstance();} }

        /// <summary>
        /// Returns single instance of this class in the system
        /// </summary>
        /// <returns></returns>
        private static TransferUnitReceiver getInstance()
        {
            if (transfer_unit_receiver_instance == null) transfer_unit_receiver_instance = new TransferUnitReceiver();
            return transfer_unit_receiver_instance;
        }

        /// <summary>
        /// Private constructor deny instantiation of objects of this class directly
        /// </summary>
        private TransferUnitReceiver()
        {
			ThreadStart job = new ThreadStart(ThreadJob);
			Thread thread = new Thread(job);
			thread.Start();
        }

        public void addTransferUnitReceiver(ReceiveTransferUnit tu_receiver)
        {
            _receiveTransferUnit += tu_receiver;
        }

        public void addTextMessageReceiver(ReceiveTextMessage tm_receiver)
        {

            _receiveTextMessage += tm_receiver;

        }

        public void addExceptionReceiver(ReceiveRemoteHookingException exception_receiver)
        {
            _receiveReportException += exception_receiver;
        }

        public void addGetApiCallsToInterceptReceiver(ReceiveGetApiCallsToIntercept get_api_calls_receiver)
        {
            _receiveGetApiCallsToIntercept += get_api_calls_receiver;
        }

        private Object sync_object = new Object();

		private BlockingQueue blocking_queue=new BlockingQueue(5000);

		
		private bool _keep_running = true;
		private Object sync_keep_running = new Object();

		/// <summary>
		/// this property is set to false, when channel is scheduled for stop
		/// </summary>
		private bool keep_running {
			get {
				bool result;
				lock (sync_keep_running) { result = _keep_running; }
				return result;
			}
		}

		/// <summary>
		/// Schedules channel for stop
		/// </summary>
		public void stopReceiver() {
			lock (sync_keep_running) { _keep_running = false; }
		}

		private const int MAX_PROCESSING_FAIL_COUNT=10;

		private void ThreadJob() {
			int fail_count = 0;
			while (keep_running) {
				try {
					TransferUnit tu=(TransferUnit)blocking_queue.Dequeue();
					_receiveTransferUnit(tu);
				} catch (Exception e){
					Console.WriteLine("Exception while processing received information.");
					Console.WriteLine(e);
					fail_count++;
					if (fail_count >= MAX_PROCESSING_FAIL_COUNT) {
						Console.WriteLine("Error count: "+fail_count+" is too high. Processing stopped.");
						break;
					}
				}
			}
		}

		public void waitForAllTransferUnitsGetProcessed(){
			while (blocking_queue.Count != 0) {
				Thread.Sleep(100);
			}
		}

        private void receiveTransferUnits(Queue<TransferUnit> tu_array)
        {
			//Console.WriteLine("Array received size="+tu_array.Count);
            lock (sync_object)
            {
                foreach (TransferUnit tu in tu_array)
                {
					blocking_queue.Enqueue(tu);
                }
            }
            //if (random.Next(10000) < 10)
            //{
            //    System.GC.Collect();
            //}
        }

        /// <summary>
        /// Send debugging messages, through this method
        /// </summary>
        /// <param name="message"></param>
        private void receiveTextMessage(String message)
        {
            if (_receiveTextMessage != null) lock (sync_object) 
            { _receiveTextMessage(message); }
        }

        private void receiveRemoteHookingException(RemoteHookingException ex)
        {
            if (_receiveReportException != null) lock (sync_object)
            { _receiveReportException(ex); }
        }

        private APIFullName[] receiveGetApiCallsToIntercept()
        {
            return _receiveGetApiCallsToIntercept();
        }

        /// <summary>
        /// For connectivity testing and keeping connection alive
        /// </summary>
        private void ping()
        {
            //do nothing
			//Console.Write(".");
        }

        ChannelReceiver channel_receiver = null;

        public ChannelReceiver getCommunicationPoint()
        {
            if (channel_receiver == null)
            {
                channel_receiver = new ChannelReceiverImpl();
            }
            return channel_receiver;
        }

        private class ChannelReceiverImpl : ChannelReceiver
        {
            TransferUnitReceiver tur = getInstance();

            public void receiveTransferUnits(Queue<TransferUnit> tu_array)
            {
                tur.receiveTransferUnits(tu_array);
            }

            /// <summary>
            /// Send debugging messages, through this method
            /// </summary>
            /// <param name="message"></param>
            public void receiveTextMessage(String message)
            {
                tur.receiveTextMessage(message);
            }

            public void receiveRemoteHookingException(RemoteHookingException ex)
            {
                tur.receiveRemoteHookingException(ex);
            }

            /// <summary>
            /// For connectivity testing and keeping connection alive
            /// </summary>
            public void ping()
            {
                tur.ping();
            }

            public APIFullName[] getApiCallsToIntercept()
            {
                return tur.receiveGetApiCallsToIntercept();
            }
        }
    }
}
