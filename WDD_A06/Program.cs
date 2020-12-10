/*
 * File: Program.cs
 * Project: WDD Assignment 6
 * Programmer: Tommy Ngo & Isaiah Andrews
 * Date: November 27, 2020
 * Purpose: This program is to act as a HTTP server to host web pages using Request and Response methods.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WDD_A06
{
    class Program
    {
        static void Main(string[] args)
        {
            // check for valid number of arguments
            if (args.Length != 3)
            {
                Console.WriteLine("Insufficient number of arguments.");
            }
            else 
            {
                // parse arguments into respective variables
                String root = args[0].Substring(args[0].LastIndexOf("=") + 1);
                IPAddress ip = IPAddress.Parse(args[1].Substring(args[1].LastIndexOf("=") + 1));
                int port = Int32.Parse(args[2].Substring(args[2].LastIndexOf("=") + 1));

                HTTPServer server = new HTTPServer(root, ip, port);
                server.Start();
            }
        }
    }
}
