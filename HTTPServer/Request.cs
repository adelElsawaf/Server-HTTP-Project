using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //parse the receivedRequest using the \r\n delimeter   
            string[] stringSeparators = new string[] { "\r\n" };
            requestLines = requestString.Split(stringSeparators, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if(requestLines[1].ToLower().Contains("host: ") == false) return false;
            // Parse Request line
            if(ParseRequestLine() == false) return false;
            // Validate blank line exists
            if (ValidateBlankLine() == false) return false;
           // Load header lines into HeaderLines dictionary
            LoadHeaderLines();
            return true;
        }

        private bool ParseRequestLine()
        {
            try
            {
                string[] M_P_V = requestLines[0].Split(' ');
                if ((M_P_V[0] == "GET") || (M_P_V[0] == "POST") || (M_P_V[0] == "HEAD"))
                {
                    switch (M_P_V[0])
                    {
                        case "GET": method = RequestMethod.GET; break;
                        case "POST": method = RequestMethod.POST; break;
                        case "HEAD": method = RequestMethod.HEAD; break;
                    }
                }
                else
                    return false;
                if (!ValidateIsURI(M_P_V[1]))
                {
                    return false;
                }
                if (M_P_V[2] == "HTTP/1.0" || M_P_V[2] == "HTTP/1.1" || M_P_V[2] == "")
                {
                    switch (M_P_V[2])
                    {
                        case "HTTP/1.0": httpVersion = HTTPVersion.HTTP10; break;
                        case "HTTP/1.1": httpVersion = HTTPVersion.HTTP11; break;
                        case "": httpVersion = HTTPVersion.HTTP09; break;
                    }

                }
                else
                    return false;

                return true; //return true if all is good
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateIsURI(string uri)
        {
             if (uri.First() == '/')
            {
                relativeURI = uri;
                return true;
            }
            return false;
        }

        private bool LoadHeaderLines()
        {
            bool result = true;
            headerLines = new Dictionary<string, string>();
            for (int i = 1; i < requestLines.Length - 2; i++)
            {
                if (requestLines[i].Contains(":"))
                {
                    string[] splitch = { ": " };
                    string[] request1 = requestLines[i].Split(splitch, StringSplitOptions.None);
                    headerLines.Add(request1[0], request1[1]);

                }
                else result = false;
            }
            return result;
        }

        private bool ValidateBlankLine()
        {
            try
            { 
                string[] BlankLineCheck = new string[] { "\r\n\r\n" };
                string[] contentbreaker = requestString.Split(BlankLineCheck, StringSplitOptions.None);
                contentLines = contentbreaker[1].Split(';');
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}