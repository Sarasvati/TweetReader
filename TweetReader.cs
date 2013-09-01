/***************************************************************************************************************************************************
 * TweetReader
 * This program will grab the last 20 tweets from a timeline. It is not intended to make use of the API.
 * 
 * Author: Nicla Rossini http://niclarossini.com
 * 
 *****************************************************************************************************************************************
* This is open source software. Open source, means you can see the source code. It also means free software. 
* However, free software doesn't mean the same as free beer. Free software is closer to free speech, free thinker, and freedom in general.
* Free software doesn't mean that you don't have to acknowledge the author and abide to their license.               
* The author is releasing this for learning/educational purposes and/or in the hope it will be useful and with NO WARRANTY OF ANY KIND.
* 
*******************************************************************************************************************************************
* This program is free software: you can redistribute it and/or modify 
* it under the terms of the GNU General Public License as published by 
* the Free Software Foundation, either version 3 of the License, or 
* (at your option) any later version. 
* **
* This program is distributed in the hope that it will be useful, 
* but WITHOUT ANY WARRANTY; without even the implied warranty of 
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
* GNU General Public License for more details.
* **
* You should have received a copy of the GNU General Public License 
* along with this program. If not, see <http://www.gnu.org/licenses/>. 
* *
 * ********************************************************************************************************************************** *
 *** ^                                                   Copyright (c) Nicla Rossini 2013                                         ^  ***
 * ********************************************************************************************************************************** *
 *          As long as you KEEP ALL NOTICES INTACT and DO NOT USE IT for COMMERCIAL software, you can do with this what you want.
 *              Please remember that PORTING counts as modification is thus subject to this same copyright and license.
 *              
 **************************************************************************************************************************************/


 // This is for console applications. If you want, it is better to use it with Windows Forms or anything else. If you plan to play with C# and VisualStudio, this goes in the Program.cs file.
  
 // Let's get started by adding some namespaces...
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Dynamic;
using System.Web;
using System.Globalization;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Json;
using System.Xml;

namespace Twitter
{
    class Program
    {


        static void Main(string[] args)
        {
        //the program starts here
            StringBuilder output = new StringBuilder();
            // this is the simples input, at least for me. We parse a twitter file
            string pat = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string patpsa = pat + @"\twitterinput.txt";
            if (!File.Exists(patpsa))
            { //if statement
                // if the input file doesn't exist, we need to alert the user, so we print a message with an alert ("\a");

                Console.WriteLine("\a\n \t **I CAN'T FIND THE PAGES YOU WANT TO SEE!!**\n\n\n Don't panic just do this:\n\n 1) Put all twitter account urls into a text (.txt) file separated by a tab \n(no new line please!).\n\n 2) Then save the file on your Desktop with the name twitterinput.txt\n\n d) Just run me again.\n\n\n\n P.S.: Finger crossing, knocks on wood, or even prayers are perfectly fine but\n won't help much \n");
                Console.ReadKey(); //this just reads the keystrokes on the keyboard. As soon as you press a key, the program closes.
            }
            else
            { //else statement

                Console.Write("\n\t Working...\t"); //show you're working on it

                //Spinner: http://www.c-sharpcorner.com/uploadfile/cbragg/console-application-waitbusy-spin-animation/
                SpinAnimation.Start(50);

                //let's set some variables for the content of the file...
                string[] tpages = File.ReadAllLines(patpsa);
                foreach (string page in tpages)
                {
                    //regex
                    string[] pages = page.Split('\t');
                    foreach (string pag in pages)
                    {
                        string[] pieces = pag.Split('/');
                        var ScreenN = pieces[3];
                        System.Threading.Thread.Sleep(5000); //sleep...

                        //Now we need a URL. Try the one below
                        UriBuilder Newur = new UriBuilder("https", "twitter.com/status/user_timeline/" + ScreenN); 
                        WebClient twitter = new WebClient();

                        string url = Newur + "?count=20"; //or count = 1, or whatever else

                        //let's call that url with an http request...

                        WebRequest request = WebRequest.Create(url);
                        request.Method = "GET";
                        System.Threading.Thread.Sleep(6000); //put it too sleep at times or you bother people

                        //response

                        WebResponse response = request.GetResponse();
                        Stream stream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(stream);
                        string xmlstuff = reader.ReadToEnd(); //read it to end

                       // Console.WriteLine("Raw xml" + xmlstuff); debug, and if you want to see the xml

                        //now we decide a path (I refer the Desktop and a name for a file, but you can print the results on the screen, or do anything else
                        string patf = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                        string filepath = patf+@"twitterfriends.xml"; //xml file

                        // if the file doesn't exist, we create one and insert the data we just grabbed only once
                        if (!File.Exists(filepath))
                        {
                            using (StreamWriter sw = File.CreateText(filepath))
                            {
                                //var dates = DateTime.Now;
                                sw.Write(xmlstuff);
                            }

                        }
                        else
                        {
                            /* Would have been nice to check if the content is already there */

                            // if (File.ReadLines(path).Any(line => line.Contains(output)))
                            //  {
                            //if the post id string is already in the file, we're about to append a duplicate. We don't want duplicates so let's skip that item
                            //  break;
                            //   }
                            // else
                            // {

                            //if there are no duplicates, we add data to what we already had (without overwriting) with AppendText
                            using (StreamWriter sw = File.AppendText(filepath))
                            {
                                // var dates = DateTime.Now;
                                sw.Write(xmlstuff);

                            }
                           
                            //and now we basically decode the xml with XmlReader. It is better if you find a way to loop through it

                            using (XmlReader read2 = XmlReader.Create(new StringReader(xmlstuff)))
                            {
                                read2.ReadToFollowing("status");


                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString()); //The ReadElementContentAsString method is not supported on node type None. Line 559, position 1 - whatever that means... I think it means that the thing is too 
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());

                                read2.ReadToNextSibling("status");
                                read2.ReadToFollowing("created_at");
                                output.Append("CreatedAt\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("id");
                                output.Append("TweetId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("text");
                                output.Append("TweetText\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("source");
                                output.Append("TweetSource\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("truncated");
                                output.Append("Truncated\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("favorited");
                                output.Append("Favorited\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_status_id");
                                output.Append("InReplyTo\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_user_id");
                                output.Append("InReplyToUserId\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("in_reply_to_screen_name");
                                output.Append("InReplyToUserScreenName\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweet_count");
                                output.Append("ReteweetsNumber\t" + read2.ReadElementContentAsString());
                                read2.ReadToFollowing("retweeted");
                                output.Append("ReteweetedYN\t" + read2.ReadElementContentAsString());
                            }
                         //let's stop the animation.
                        //Spinner: http://www.c-sharpcorner.com/uploadfile/cbragg/console-application-waitbusy-spin-animation/
                            SpinAnimation.Stop();

                            Console.WriteLine(output.ToString()); //we convert the output to string and print it 
                            
                            //now we decide a path for a file and select the Desktop (so you have a chance to see the new file)
                            string patfx = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                            //we also decice a name for a document
                            string path =  patfx+@"twitter.txt";

                            // if the file doesn't exist, we create one and insert the data we just grabbed only once
                            if (!File.Exists(path))
                            {
                                using (StreamWriter sw = File.CreateText(path))
                                {
                                   var dates = DateTime.Now;  //if you want to see the date
                                   sw.Write(/* "DATE:" + dates + */output.ToString()); /* we insert the data, coverted to string, in a text file, 
                                                                                        *  but you can od anything else */
                                }                                                       

                            }
                            else
                            {
                                /* Would have been nice to check if the content is already there */

                                // if (File.ReadLines(path).Any(line => line.Contains(output)))
                                //  {
                                //if the post id string is already in the file, we're about to append a duplicate. We don't want duplicates so let's skip that item
                                //  break;
                                //   }
                                // else
                                // {

                                //if there are no duplicates, we add data to what we already had (without overwriting) with AppendText
                                
                                using (StreamWriter sw = File.AppendText(path))
                                {
                                  //  var dates = DateTime.Now;
                                    sw.Write(/* "DATE:" + dates + */output.ToString()); //if the file is already there we append data (and duplicates)

                                }
                            }

                        }
                    }
                }
                Console.ReadKey(); //maybe let them know that to close they need to press a random keystroke on the keyboard

            }

        }

    }
}
// traditional spinner to alert you that this thing  is working (and I put a sleep fucntion to throttle a bit).
//code retrieved here (this isn't mine) http://www.c-sharpcorner.com/uploadfile/cbragg/console-application-waitbusy-spin-animation/

public static class SpinAnimation
{
    private static System.ComponentModel.BackgroundWorker spinner = initialiseBackgroundWorker();
    private static int spinnerPosition = 25;
    private static int spinWait = 25;
    private static bool isRunning;



    public static bool IsRunning { get { return isRunning; } }

    private static System.ComponentModel.BackgroundWorker initialiseBackgroundWorker()
    {



        System.ComponentModel.BackgroundWorker obj = new System.ComponentModel.BackgroundWorker();
        obj.WorkerSupportsCancellation = true;
        obj.DoWork += delegate
        {

            spinnerPosition = Console.CursorLeft;
            while (!obj.CancellationPending)
            {



                char[] spinChars = new char[] { '.', '-', '+', '^', '°', '*' }; ;


                foreach (char spinChar in spinChars)
                {

                    Console.CursorLeft = spinnerPosition;

                    Console.Write(spinChar);
                    System.Threading.Thread.Sleep(spinWait);

                }

            }

        };

        return obj;

    }


    public static void Start(int spinWait)
    {

        //Set the running flag

        isRunning = true;

        //process spinwait value

        SpinAnimation.spinWait = spinWait;

        //start the animation unless already started

        if (!spinner.IsBusy)

            spinner.RunWorkerAsync();

        else throw new InvalidOperationException("Cannot start spinner whilst spinner is already running");

    }

    public static void Start() { Start(25); }
    public static void Stop()
    {

        //Stop the animation

        spinner.CancelAsync();

        //wait for cancellation to complete

        while (spinner.IsBusy) System.Threading.Thread.Sleep(100);

        //reset the cursor position

        Console.CursorLeft = spinnerPosition;

        //set the running flag

        isRunning = false;

    }

}

 
