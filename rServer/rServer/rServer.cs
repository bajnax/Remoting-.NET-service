using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace rServer
{
    class rServer
    {
        static void Main(string[] args)
        {
            // customizig channel to allow high-level serialization to pass "ObjRef" objects
            BinaryServerFormatterSinkProvider serverSinkProvider = new BinaryServerFormatterSinkProvider();
            serverSinkProvider.TypeFilterLevel = TypeFilterLevel.Full;
            BinaryClientFormatterSinkProvider clientSinkProvider = new BinaryClientFormatterSinkProvider();
            IDictionary properties = new Hashtable();
            properties["port"] = 8080;

            //using TCP protocol
            TcpChannel chan = new TcpChannel(properties, clientSinkProvider, serverSinkProvider);
            ChannelServices.RegisterChannel(chan, false);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServerObject), "HelloWorld", WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Press any button to quit.");
            System.Console.ReadLine();
        }
    }
}
