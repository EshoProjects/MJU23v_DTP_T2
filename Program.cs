using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MJU23v_DTP_T2
{
    internal class Program
    {
        static List<Link> Links = new List<Link>();

        class Link
        {
            public string Category { get; set; }
            public string Group { get; set; }
            public string Name { get; set; }
            public string Descr { get; set; }
            public string Url { get; set; }

            public Link(string category, string group, string name, string descr, string url)
            {
                Category = category;
                Group = group;
                Name = name;
                Descr = descr;
                Url = url;
            }

            public Link(string line)
            {
                string[] part = line.Split('|');
                Category = part[0];
                Group = part[1];
                Name = part[2];
                Descr = part[3];
                Url = part[4];
            }

            public void DisplayLinkInfo(int index)
            {
                Console.WriteLine($"|{index,-2}|{Category,-10}|{Group,-10}|{Name,-20}|{Descr,-40}|");
            }

            public void OpenLinkInDefaultBrowser()
            {
                Process.Start(new ProcessStartInfo { FileName = Url, UseShellExecute = true });
            }

            public string ToFormattedString()
            {
                return $"{Category}|{Group}|{Name}|{Descr}|{Url}";
            }

            public void OpenLinkByGroup(string groupName)
            {
                foreach (Link link in Links)
                {
                    if (link.Group == groupName)
                    {
                        link.OpenLinkInDefaultBrowser();
                    }
                }
            }

            public static Link CreateNewLink()
            {
                Console.WriteLine("Create a new link:");
                Console.Write("  Enter category: ");
                string category = Console.ReadLine();
                Console.Write("  Enter group: ");
                string group = Console.ReadLine();
                Console.Write("  Enter name: ");
                string name = Console.ReadLine();
                Console.Write("  Enter description: ");
                string description = Console.ReadLine();
                Console.Write("  Enter URL: ");
                string url = Console.ReadLine();
                return new Link(category, group, name, description, url);
            }

            public static void RemoveLinkByIndex(List<Link> links, int index)
            {
                if (index >= 0 && index < links.Count)
                {
                    links.RemoveAt(index);
                }
                else
                {
                    Console.WriteLine("Index is out of bounds for the link list.");
                }
            }

            public static void HandleRemoveCommand(List<Link> links, string[] args)
            {
                if (args[1] == "bort" && args.Length == 3)
                {
                    int index;
                    if (int.TryParse(args[2], out index))
                    {
                        RemoveLinkByIndex(links, index);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect usage of the 'ta bort' command. Use 'ta bort <index>'.");
                    }
                }
            }

            public static void LoadLinksFromFile(List<Link> links, string fileName)
            {
                links.Clear();

                using (StreamReader sr = new StreamReader(fileName))
                {
                    int i = 0;
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        Console.WriteLine(line);
                        Link link = new Link(line);
                        links.Add(link);
                        line = sr.ReadLine();
                    }
                }
            }

            public static void DisplayHelp()
            {
                Console.WriteLine("Available commands and variants:");
                Console.WriteLine("---------------------------------------------------------------");
                Console.WriteLine("hjälp           - display this help");
                Console.WriteLine("sluta           - exit the program");
                Console.WriteLine("ladda <file>    - load links from a specific file");
                Console.WriteLine("lista           - display the list of links");
                Console.WriteLine("ny             - create a new link");
                Console.WriteLine("spara <file>   - save links to a specific file");
                Console.WriteLine("ta bort <index> - remove a link by index");
                Console.WriteLine("öppna grupp <group> - open all links in a specific group");
                Console.WriteLine("öppna länk <index> - open a link by index");
                Console.WriteLine("---------------------------------------------------------------");
            }
        }

        static void Main(string[] args)
        {
            string fileName = @"..\..\..\Links\Links.lis";

            LinkManager.LoadLinksFromFile(Links, fileName);

            Console.WriteLine("Welcome to the link list! Type 'hjälp' for help.");

            do
            {
                Console.Write("> ");
                string command = Console.ReadLine().Trim();
                string[] arguments = command.Split();

                if (arguments.Length > 0)
                {
                    switch (arguments[0])
                    {
                        case "sluta":
                            Console.WriteLine("Goodbye! Welcome back!");
                            return;

                        case "hjälp":
                            LinkManager.DisplayHelp();
                            break;

                        case "ladda":
                            if (arguments.Length == 2)
                            {
                                fileName = $@"..\..\..\Links\{arguments[1]}";
                            }

                            LinkManager.LoadLinksFromFile(Links, fileName);
                            break;

                        case "lista":
                            Console.WriteLine("|Index|Category  |Group     |Name                |Description                              |");
                            Console.WriteLine("---------------------------------------------------------------");
                            int i = 0;
                            foreach (Link link in Links)
                                link.DisplayLinkInfo(i++);
                            break;

                        case "ny":
                            Link newLink = LinkManager.CreateNewLink();
                            Links.Add(newLink);
                            break;

                        case "spara":
                            if (arguments.Length == 2)
                            {
                                fileName = $@"..\..\..\Links\{arguments[1]}";
                            }

                            using (StreamWriter sw = new StreamWriter(fileName))
                            {
                                foreach (Link link in Links)
                                {
                                    sw.WriteLine(link.ToFormattedString());
                                }
                            }
                            break;

                        case "ta":
                            LinkManager.HandleRemoveCommand(Links, arguments);
                            break;

                        case "öppna":
                            if (arguments.Length == 3)
                            {
                                if (arguments[1] == "grupp")
                                {
                                    Links[0].OpenLinkByGroup(arguments[2]);
                                }
                                else if (arguments[1] == "länk")
                                {
                                    int index;
                                    if (int.TryParse(arguments[2], out index))
                                    {
                                        if (index >= 0 && index < Links.Count)
                                        {
                                            Links[index].OpenLinkInDefaultBrowser();
                                        }
                                        else
                                        {
                                            Console.WriteLine("Index is out of bounds for the link list.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Incorrect usage of the 'öppna länk' command. Use 'öppna länk <index>'.");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Incorrect usage of the 'öppna' command. Use 'öppna grupp <gruppnamn>' or 'öppna länk <index>'.");
                            }
                            break;

                        default:
                            Console.WriteLine("Invalid command. Type 'hjälp' for assistance.");
                            break;
                    }
                }
            } while (true);
        }
    }
}
