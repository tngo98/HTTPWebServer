/*
 * File: HTTPServer.cs
 * Purpose: This file contains the HTTPServer class which contains all the methods in order to run the server in a thread.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WDD_A06
{
    class HTTPServer
    {
        // properties
        public const String MSG_DIR = "/root/msg/";

        public static String WEB_DIR = "";

        public const String VERSION = "HTTP/1.1";

        public const String NAME = "WDD HTTP Server";

        public static String LOG_DIR = "";

        private bool runState = false;

        private TcpListener listener;

        public static DateTime time = DateTime.Now;

        /*
         * Function: HTTPServer()
         * Purpose: Constuctor for HTTPServer
         * Parameters: String root: root directory
         *             IPAddress ip: ip address
         *             int port: port number
         * Return: NONE
         */
        public HTTPServer(String root, IPAddress ip, int port)
        {
            listener = new TcpListener(ip, port);
            WEB_DIR = root;
            LOG_DIR = WEB_DIR + "/myOwnWebServer.txt";
        }

        /*
         * Function: Start()
         * Purpose: Server start method
         * Parameters: NONE
         * Return: NONE
         */
        public void Start()
        {
            Thread serverThread = new Thread(new ThreadStart(Run));
            File.AppendAllText(LOG_DIR, time + " - Server: Started\n");
            serverThread.Start();
        }

        /*
         * Function: Run()
         * Purpose: Server running method
         * Parameters: NONE
         * Return: NONE
         */
        private void Run()
        {
            runState = true;
            listener.Start();

            // server running
            while (runState)
            {
                TcpClient client = listener.AcceptTcpClient();

                HandleClient(client);

                client.Close();
            }

            runState = false;

            listener.Stop();
        }

        /*
         * Function: HandleClient()
         * Purpose: Method to handle server requests and responses
         * Parameters: TcpClient client: the incoming client
         * Return: void
         */
        private void HandleClient(TcpClient client)
        {
            StreamReader reader = new StreamReader(client.GetStream());

            String msg = "";

            // reading msg according to client page
            while (reader.Peek() != -1)
            {
                msg += reader.ReadLine() + "\n";
            }

            // receving request and response information according to client
            Request req = Request.GetRequest(msg);
            Response resp = Response.From(req);
            resp.Post(client.GetStream());
        }
    }
}
