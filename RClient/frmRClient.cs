using System;
using System.Windows.Forms;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Net.Sockets;
using common;
using System.Security;
using System.Runtime.Serialization.Formatters;
using System.Collections;

namespace Rclient
{
    public class Client : System.Windows.Forms.Form
	{
        private rServer.ServerObject serObject;

        private Button button1;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private Label label1;
        private Label label2;
        private Label label3;
        private System.ComponentModel.Container components = null;

		public Client()
		{

			InitializeComponent();

            // customizig channel to allow high-level serialization to pass "ObjRef" objects
            BinaryServerFormatterSinkProvider serverSinkProvider = new BinaryServerFormatterSinkProvider();
            serverSinkProvider.TypeFilterLevel = TypeFilterLevel.Full;
            BinaryClientFormatterSinkProvider clientSinkProvider = new BinaryClientFormatterSinkProvider();
            IDictionary properties = new Hashtable();

            // selects the first available port
            properties["port"] = 0;

            TcpChannel chan = new TcpChannel(properties, clientSinkProvider, serverSinkProvider);
			ChannelServices.RegisterChannel(chan, false);

            // activating the server object to get the proxy of the subject
            serObject = (rServer.ServerObject) Activator.GetObject(typeof(rServer.ServerObject),"tcp://localhost:8080/HelloWorld");

            if (serObject == null)
                MessageBox.Show("Failed to connect to the server");

            // creating wrapper in the client's context
            BroadcastEventWrapper eventWrapper = new BroadcastEventWrapper();

            // subscribing client's instance of the delegate with encapsulated "UpdateScreen" method
            // to the "MessageArrivedLocally" event of the wrapper
            eventWrapper.MessageArrivedLocally += new MessageArrivedHandler(UpdateScreen);

            // subcribing the instance of the delegate
            // whith encapsulated "LocallyHandleMessageArrived" method of the wrapper
            // to the server's "MessageArrived" event
            serObject.MessageArrived += new MessageArrivedHandler(eventWrapper.LocallyHandleMessageArrived);
            Console.WriteLine("Event registered. Waiting for messages.");
           
        }

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(447, 100);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Send";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(0, 1);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(265, 266);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(357, 22);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(165, 20);
            this.textBox2.TabIndex = 2;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(357, 48);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(165, 20);
            this.textBox3.TabIndex = 3;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(357, 74);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(165, 20);
            this.textBox4.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(281, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "City";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(281, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Temperature";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(281, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Rain";
            // 
            // Client
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(534, 267);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Client";
            this.Text = "RemoteClient";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        #endregion


        [STAThread]
		static void Main() 
		{
			Application.Run(new Client());
		}


        private void button1_Click(object sender, EventArgs e)
        {
            if(serObject != null)
            {
                try
                {
                    // upon the btn click, the weather data is sent to the subject
                    Weather weather = new Weather();
                    weather.City = textBox2.Text;
                    weather.Temperature = double.Parse(textBox3.Text);
                    weather.Rain = double.Parse(textBox4.Text);
                    serObject.SendWeather(weather);
                }
                catch (Exception se)
                {
                    MessageBox.Show(se.Message);
                }
            }
        }

        
        void UpdateScreen(Weather weath)
       {              
            textBox1.Text += "New message: " + Environment.NewLine
                + weath.City + Environment.NewLine
                + weath.Temperature + Environment.NewLine
                + weath.Rain + Environment.NewLine + Environment.NewLine;
       }
    }
}
