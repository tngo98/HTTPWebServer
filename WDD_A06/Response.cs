/* 
 * File: Response.cs
 * Purpose: This file contains the methods in order to handle responses from client to server.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WDD_A06
{
    public class Response
    {
        // properties
        private Byte[] data = null;
        private String status;
        private String mime;

        /*
         * Function: Response()
         * Purpose: Constructor for response class
         * Parameters: NONE
         * Returns: NONE
         */
        private Response(String status, String mime, Byte[] data)
        {
            this.status = status;
            this.data = data;
            this.mime = mime;
            
        }

        /*
         * Function: Response From()
         * Purpose: Method to handle page response depending on client web page state
         * Parameters: Request request: request from web page
         * Returns: Response: to execute msg page accordingly
         */
        public static Response From(Request request)
        {
            // if request is null, make page response null
            if (request == null)
            {
                File.AppendAllText(HTTPServer.LOG_DIR, HTTPServer.time + " - Server: Page Not Found\n");
                return MakeNullRequest();
            }
            
            // if request is GET, then extract file and determine response
            if (request.Type == "GET")
            {
                // variables for ease of making condition statements
                String file = HTTPServer.WEB_DIR + request.URL;
                FileInfo f = new FileInfo(file);

                // if file exists or directory contains anything, make response from file
                if (f.Exists && f.Extension.Contains("."))
                {
                    File.AppendAllText(HTTPServer.LOG_DIR, HTTPServer.time + " - Server: Page Found\n");
                    return MakeFromFile(f);
                }
                else // if directory does not contain any file with extension, then continue checking in child directories
                {
                    DirectoryInfo di = new DirectoryInfo(f + "/");

                    // if no more directories exist, make page not found
                    if (!di.Exists)
                    {
                        File.AppendAllText(HTTPServer.LOG_DIR, HTTPServer.time + " - Server: Page Not Found\n");
                        return MakePageNotFound();
                    }

                    FileInfo[] files = di.GetFiles();
                    // if directory has no file, continue looking deeper in directory
                    foreach (FileInfo ff in files)
                    {
                        String n = ff.Name;
                        // check for specific files, and if exists, make response from file
                        if (n.Contains("default.html") || n.Contains("default.htm") || n.Contains("index.html") || n.Contains("index.htm"))
                        {
                            File.AppendAllText(HTTPServer.LOG_DIR, HTTPServer.time + " - Server: Page Found\n");
                            return MakeFromFile(ff);
                        }
                    }
                }
            }
            else // if request is not get, make method not allowed
            {
                File.AppendAllText(HTTPServer.LOG_DIR, HTTPServer.time + " - Server: Method Not Allowed\n");
                return MakeMethodNotAllowed();
            }

            File.AppendAllText(HTTPServer.LOG_DIR, HTTPServer.time + " - Server: Page Not Found\n");
            return MakePageNotFound();
        }

        /*
         * Function: MakeFromFile()
         * Purpose: To load file into web page
         * Parameters: FileInfo f: the file being read
         * Returns: a Response object
         */
        private static Response MakeFromFile(FileInfo f)
        {
            FileStream fs = f.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();

            return new Response("200 OK", "text/html", d);
        }

        /*
         * Function: MakeNullRequest()
         * Purpose: To load Null Page if file is null
         * Parameters: NONE
         * Returns: a Response object
         */
        private static Response MakeNullRequest()
        {
            String file = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "400.html";
            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();

            return new Response("400 Bad Request", "text/html", d);
        }

        /*
         * Function: MakePageNotFound()
         * Purpose: To load PageNotFound page, if file or page cannot be found
         * Parameters: NONE
         * Returns: a Response object
         */
        private static Response MakePageNotFound()
        {
            String file = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "404.html";
            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();

            return new Response("404 Page Not Found", "text/html", d);
        }

        /*
         * Function: MakeMethodNotAllowed()
         * Purpose: To load MethodNotAllowed page, if page is malformed
         * Parameters: NONE
         * Returns: a Response object
         */
        private static Response MakeMethodNotAllowed()
        {
            String file = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "405.html";
            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();

            return new Response("405 Method Not Allowed", "text/html", d);
        }

        /*
         * Function: Post()
         * Purpose: To write server information into stream
         * Parameters: NetworkStream stream: stream information of server
         * Returns: NONE
         */
        public void Post(NetworkStream stream)
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(String.Format("{0} {1}\r\nServer: {2}\r\nContent-Type: {3}\r\nAccept-Ranges: bytes\r\nContent-Length: {4}\r\n", 
                HTTPServer.VERSION, status, HTTPServer.NAME, mime, data.Length));
            writer.Flush();
            stream.Write(data, 0, data.Length);
        }
    }
}
