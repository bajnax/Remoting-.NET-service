using System;
using System.Runtime.Remoting.Messaging;

namespace common
{

    // delegate containing a signature of the method
    // accessible by both client and server
    public delegate void MessageArrivedHandler(Weather weath);

    public interface ISubject
    {
        void SendWeather(Weather weath);
        event MessageArrivedHandler MessageArrived;
    }


    // The wrapper is created in the client's context.
    // A delegate with encapsulated "LocallyHandleMessageArrived" 
    // method of the wrapper is subscribed to the server's "MessageArrived" event.
    // The method invokes "messageArrivedLocally" event
    // The event then forwards message to the subscribed client

    public class BroadcastEventWrapper : MarshalByRefObject
    {
        public event MessageArrivedHandler MessageArrivedLocally;

        [OneWay]
        public void LocallyHandleMessageArrived(Weather weath)
        {
            // forwards the message to the client
            MessageArrivedLocally(weath);
        }
        public override object InitializeLifetimeService()
        {
            // keeps the object alive forever
            return null;
        }
    }
}