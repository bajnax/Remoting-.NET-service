using System;
using System.Collections.Generic;
using common;

namespace rServer
{
    // through 'singleton' pattern all clients are connected to one instance of the 'subject'
    public class ServerObject : MarshalByRefObject, ISubject
    {
        // This event will invoke "LocallyHandleMessageArrived" method of the wrapper
        // The method then triggers "MessageArrivedLocally" event
        // That invokes message updating method on the client
        public event MessageArrivedHandler MessageArrived;

        // Clients send weather details via invocation of this method through proxy
        public void SendWeather(Weather weather)
        {
            Console.WriteLine("New message: ");
            Console.WriteLine("City: " + weather.City);
            Console.WriteLine("Temperature: " + weather.Temperature);
            Console.WriteLine("Rain: " + weather.Rain);

            Console.WriteLine("Broadcasting new message.");

            SafeInvokeEvent(weather);
        }

        private void SafeInvokeEvent(Weather weath)
        {
            // looping through a list of delegates provided by clients
            if (MessageArrived == null)
            {
                Console.WriteLine("No clients");
            }
            else {
                Console.WriteLine("Number of clients: {0}", MessageArrived.GetInvocationList().Length);
                MessageArrivedHandler mah = null;
                foreach (Delegate del in MessageArrived.GetInvocationList())
                {
                    try
                    {
                        mah = (MessageArrivedHandler)del;

                        // invoking "LocallyHandleMessageArrived" method of the wrapper
                        mah(weath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Client disconnected");
                        MessageArrived -= mah;
                    }
                }
            }
        }

        public override object InitializeLifetimeService()
        {
            // keeps the object alive forever
            return null;           
        }

    }  
}
