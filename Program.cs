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
            public string Category, Group, Name, Descr, Link;

            public Link(string category, string group, string name, string descr, string link)
            {
                Category = category;
                Group = group;
                Name = name;
                Descr = descr;
                Link = link;
            }

            public Link(string line)
            {
                string[] part = line.Split('|');
                Category = part[0];
                Group = part[1];
                Name = part[2];
                Descr = part[3];
                Link = part[4];
            }

            public void DisplayLinkInfo(int i)
            {
                Console.WriteLine($"|{i,-2}|{Category,-10}|{Group,-10}|{Name,-20}|{Descr,-40}|");
            }

            public void OpenLinkInDefaultBrowser()
            {
                Process.Start(new ProcessStartInfo { FileName = Link, UseShellExecute = true });
            }

            public string ToFormattedString()
            {
                return $"{Category}|{Group}|{Name}|{Descr}|{Link}";
            }
        }

        static void Main(string[] args)
        {
            string fileName = @"..\..\..\Links\Links.lis";

            using (StreamReader sr = new StreamReader(fileName))
            {
                int i = 0;
                string line = sr.ReadLine();
                while (line != null)
                {
                    Console.WriteLine(line);
                    Link L = new Link(line);
                    L.DisplayLinkInfo(i++);
                    Links.Add(L);
                    line = sr.ReadLine();
                }
            }

            Console.WriteLine("Välkommen till länklistan! Skriv 'hjälp' för hjälp!");

            do
            {
                Console.Write("> ");
                string cmd = Console.ReadLine().Trim();
                string[] arg = cmd.Split();
                string command = arg[0];

                if (command == "sluta")
                {
                    Console.WriteLine("Hej då! Välkommen åter!");
                    break; // Exit the loop to end the program
                }
                else if (command == "hjälp")
                {
                    Console.WriteLine("hjälp           - skriv ut den här hjälpen");
                    Console.WriteLine("sluta           - avsluta programmet");
                }
                else if (command == "ladda")
                {
                    if (arg.Length == 2)
                    {
                        fileName = $@"..\..\..\Links\{arg[1]}";
                    }

                    Links = new List<Link>();

                    using (StreamReader sr = new StreamReader(fileName))
                    {
                        int i = 0;
                        string line = sr.ReadLine();
                        while (line != null)
                        {
                            Console.WriteLine(line);
                            Link L = new Link(line);
                            Links.Add(L);
                            line = sr.ReadLine();
                        }
                    }
                }
                else if (command == "lista")
                {
                    Console.WriteLine("|Index|Category  |Group     |Name                |Description                              |");
                    Console.WriteLine("---------------------------------------------------------------");
                    int i = 0;
                    foreach (Link L in Links)
                        L.DisplayLinkInfo(i++);
                }
                else if (command == "ny")
                {
                    Console.WriteLine("Skapa en ny länk:");
                    Console.Write("  ange kategori: ");
                    string Category = Console.ReadLine();
                    Console.Write("  ange grupp: ");
                    string Group = Console.ReadLine();
                    Console.Write("  ange namn: ");
                    string Name = Console.ReadLine();
                    Console.Write("  ange beskrivning: ");
                    string Descr = Console.ReadLine();
                    Console.Write("  ange länk: ");
                    string Link = Console.ReadLine();
                    Link newLink = new Link(Category, Group, Name, Descr, Link);
                    Links.Add(newLink);
                }
                else if (command == "spara")
                {
                    if (arg.Length == 2)
                    {
                        fileName = $@"..\..\..\Links\{arg[1]}";
                    }

                    using (StreamWriter sr = new StreamWriter(fileName))
                    {
                        foreach (Link L in Links)
                        {
                            sr.WriteLine(L.ToFormattedString());
                        }
                    }
                }
                else if (command == "ta")
                {
                    if (arg[1] == "bort" && arg.Length == 3)
                    {
                        Links.RemoveAt(Int32.Parse(arg[2]));
                    }
                    else
                    {
                        Console.WriteLine("Felaktig användning av kommandot 'ta bort'. Använd 'ta bort <index>'.");
                    }
                }
                else if (command == "öppna")
                {
                    switch (arg.Length)
                    {
                        case 3:
                            if (arg[1] == "grupp")
                            {
                                foreach (Link L in Links)
                                {
                                    if (L.Group == arg[2])
                                    {
                                        L.OpenLinkInDefaultBrowser();
                                    }
                                }
                            }
                            else if (arg[1] == "länk")
                            {
                                int ix;
                                if (int.TryParse(arg[2], out ix))
                                {
                                    if (ix >= 0 && ix < Links.Count)
                                    {
                                        Links[ix].OpenLinkInDefaultBrowser();
                                    }
                                    else
                                    {
                                        Console.WriteLine("Indexet är utanför gränserna för länklistan.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Felaktig användning av kommandot 'öppna länk'. Använd 'öppna länk <index>'.");
                                }
                            }
                            break;

                        default:
                            Console.WriteLine("Felaktig användning av kommandot 'öppna'. Använd 'öppna grupp <gruppnamn>' eller 'öppna länk <index>'.");
                            break;
                    }
                }
            } while (true);
        }
    }
}
