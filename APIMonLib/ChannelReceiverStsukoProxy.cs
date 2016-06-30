using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib
{
    /// <summary>
    /// This class is created for glueing together independent creation of objects by .NET
    /// for processing client requests and my single persistent TransferUnitReceiver.
    /// .NET might create a lot of instances of Proxy but all of them will be using 
    /// the same mediator TransferUnitReceiver.ChannelReceiverImpl from TransferUnitReceiver
    /// </summary>
    public class ChannelReceiverStsukoProxy: MarshalByRefObject, ChannelReceiver
    {
        ChannelReceiver communication_point = TransferUnitReceiver.instance.getCommunicationPoint();

        public void receiveTransferUnits(Queue<TransferUnit> tu_array)
        {
            communication_point.receiveTransferUnits(tu_array);
        }

        public void receiveTextMessage(String message)
        {
            communication_point.receiveTextMessage(message);
        }

        public void receiveRemoteHookingException(RemoteHookingException ex)
        {
            communication_point.receiveRemoteHookingException(ex);
        }

        public void ping()
        {
            communication_point.ping();
        }

        public APIFullName[] getApiCallsToIntercept()
        {
            return communication_point.getApiCallsToIntercept();
        }
    }
}
