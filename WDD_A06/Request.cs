/*
 * File: Request.cs
 * Purpose: This file contains the methods in order to handle requests from client to server.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDD_A06
{
    public class Request
    {
        // properties
        public String Type { get; set; }
        public String URL { get; set; }
        public String Host { get; set; }
        public string Referer { get; set; }

        /*
         * Function: Request()
         * Purpose: Constructor for request class
         * Parameters: String type: type of request
         *             String url: url header
         *             String host: host header
         *             String referer: referer header
         * Returns: NONE
         */
        public Request(String type, String url, String host, String referer)
        {
            Type = type;
            URL = url;
            Host = host;
            Referer = referer;
        }

        /*
         * Function: GetRequest()
         * Purpose: Method responsible for sending request information
         * Parameters: String request: a request string
         * Returns: NONE
         */
        public static Request GetRequest(String request)
        {
            // to check if request is present
            if (String.IsNullOrEmpty(request))
            {
                return null;
            }

            // parsing tokens
            String[] tokens = request.Split(' ', '\n');
            String type = tokens[0];
            String url = tokens[1];
            String host = tokens[4];
            String referer = "";

            // to parse referer token
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == "Referer:")
                {
                    referer = tokens[i + 1];
                    break;
                }
            }

            // log to file
            File.AppendAllText(HTTPServer.LOG_DIR, HTTPServer.time + " - Server: " + type + " requested\n");

            return new Request(type, url, host, referer);
        }
    }
}
