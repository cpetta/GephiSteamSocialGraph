using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace steamFriendNameCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, SteamUser> allNodes = new Dictionary<string, SteamUser>(); // Hash table

            DirectoryInfo d = new DirectoryInfo(@"[path to directory which stores steam friends]");
        
            foreach (FileInfo file in d.GetFiles())
            {
                bool blankLine = false;
                
                string[] lines = File.ReadAllLines(file.ToString());

                string name = file.Name;
                //string name = lines[0];
                int numFriends = lines.Length;

                if (!allNodes.ContainsKey(name))
                {
                    allNodes.Add(name, new SteamUser(name, numFriends));
                }

                //Console.WriteLine(lines[0]);
                foreach (string line in lines)
                {
                    if(blankLine)
                    {
                        if (line.Length == 0)
                            break;

                        if (!allNodes.ContainsKey(line))
                        {
                            allNodes.Add(line, new SteamUser(line));
                        }

                        allNodes[name].AddFriend(allNodes[line]);
                        allNodes[line].AddFriend(allNodes[name]);
                    }

                    // Check for blank line
                    if(line.Length == 0)
                    {
                        blankLine = true;
                    }
                    else
                    {
                        blankLine = false;
                    }

                }
            }
            // Testing out a friend's friendlist
            /*
            string debugFriendName = ""; // Friend's username to test
            Console.WriteLine(allNodes[debugFriendName].Friends.Count);
            foreach (SteamUser steamFriend in allNodes[debugFriendName].Friends)
            {
                Console.WriteLine($"{steamFriend.id}, {steamFriend.Name}");
            }
            */

            // Create Nodes file
            
            StringBuilder nodesString = new StringBuilder();
            nodesString.Append("id\tLabel\n");
            foreach(SteamUser node in allNodes.Values)
            {
                nodesString.Append($"{node.id}\t{node.Name}\n");
            }
            ToFile.Write("Nodes.csv", nodesString.ToString());
            

            StringBuilder edgesString = new StringBuilder();
            edgesString.Append("source, target, weight\n");

            foreach (SteamUser node in allNodes.Values)
            {
                foreach(SteamUser friend in node.Friends)
                {
                    edgesString.Append($"{node.id}, {friend.id}, 1.0\n");
                }
            }
            ToFile.Write("Edges.csv", edgesString.ToString());
            
        }
    }
    class SteamUser
    {
        public static int CurrentID = 1;
        public int id;
        public string Name;
        public List<SteamUser> Friends;
        public SteamUser(string name)
        {
            id = CurrentID++;
            Name = name;
            Friends = new List<SteamUser>();
        }
        public SteamUser(string name, int friendsListLength)
        {
            id = CurrentID++;
            Name = name;
            Friends = new List<SteamUser>(friendsListLength);
        }

        public void AddFriend(SteamUser friend)
        {
            if (!Friends.Contains(friend))
            {
                Friends.Add(friend);
            }
        }
    }
    class ToFile
    {
        public static void Write(string file, string text)
        {
            string path = @$"[path to directory which stores the result]\{file}";
            try
            {
                using (FileStream fs = File.Create(path))
                {
                    byte[] bytes = new UTF8Encoding(true).GetBytes(text);
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
