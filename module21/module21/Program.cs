using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace module21
{
    enum Role
    {
        Administrator = 1,
        Menedger = 2,
        Operator = 3
    }
    class Group
    {
        public string name;
        public List<Person> members;
        public Role role;
        public Group(String name,Role role)
        {
            this.name = name;
            this.role = role;
            members = new List<Person>();
        }

        ~Group()
        {
        }
    }

    class Person
    {
        public string login { get; set; }
        public string password { get; set; }
        Role role;

        public Person(string login, string password, Group group)
        {
            this.login = login;
            this.password = password;
            this.role = group.role;
            group.members.Add(this);
        }

        public void addPersonToGroup(Group group, Person person)
        {
            bool isExist = false;
            if (this.role == Role.Administrator)
            {
                foreach (Person p in group.members)
                {
                    if (p.login == person.login)
                    {
                        isExist = true;
                    }
                }

                if (!isExist)
                {
                    Console.WriteLine("{0} added to group", person.login);
                    group.members.Add(person);
                }
                else
                {
                    Console.WriteLine("{0} already exist",person.login);
                }
            }
            else
            {
                Console.WriteLine("You can`t do that.");
            }
        }

        public void deletePersonFromGroup(Group group, Person person)
        {
            if (this.role == Role.Administrator)
            {
                group.members.Remove(person);
            }
            else
            {
                Console.WriteLine("You can`t do that.");
            }
        }

        public void groupDelete(List<Group> groups, Group group)
        {
            if (this.role == Role.Administrator)
            {
                if (group.role != Role.Administrator)
                {
                    groups.Remove(group);
                }
                else
                {
                    Console.WriteLine("You can`t delete administration group");
                }
            }
            else
            {
                Console.WriteLine("You can`t do that.");
            }
        }

        public void groupAdd(List<Group> groups, Group group)
        {
            if (this.role == Role.Administrator)
            {
                if (group.role != Role.Administrator)
                {
                    if (!groups.Contains(group))
                    {
                        groups.Add(group);
                    }
                    else
                    {
                        Console.WriteLine("{0} already exist",group.name);
                    }
                }
                else
                {
                    Console.WriteLine("Administration group can be only one");
                }
            }
            else
            {
                Console.WriteLine("You can`t do that.");
            }
        }

        public void setGroupRole(Group group, Role role)
        {
            if (this.role == Role.Administrator)
            {
                group.role = role;
            }
            else
            {
                Console.WriteLine("You can`t do that.");
            }
        }

        public void openFile(string path, string fileName)
        {
            if (this.role == Role.Menedger || this.role == Role.Operator)
            {
                Process process = new Process();

                try
                {
                    process.StartInfo.FileName = path+"\\"+fileName;
                    process.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("You can`t do that.");
            }
        }

        public void addFile(string path, string fileName)
        {
            if (this.role == Role.Menedger || this.role == Role.Operator)
            {
                FileStream fs = File.Create(path+"\\"+fileName);
                fs.Close();
            }
            else
            {
                Console.WriteLine("You can`t do that.");
            }
        }

        public void deleteFile(string path, string fileName)
        {
            if (this.role == Role.Menedger)
            {
                File.Delete(path+"\\"+fileName);
            }
            else
            {
                Console.WriteLine("You can`t do that.");
            }
        }
    }

    class Program
    {
        static Person FindPerson(List<Group> groups, string login)
        {
            foreach (Group g in groups)
            {
                foreach (Person person in g.members)
                {
                    if (person.login == login)
                    {
                        return person;
                    }
                }
            }

            return null;
        }

        static Group FindGroup(List<Group> groups, string groupName)
        {
            foreach (Group g in groups)
            {
                if (g.name == groupName)
                {
                    return g;
                }
            }
            return null;
        }

        static void Main(string[] args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory+"Dir";

            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
                Console.WriteLine("Dir create succses at {0}", Directory.GetCreationTime(path));
            }
            else
            {
                Console.WriteLine("Directory already exist");
            }

            List<Group> groups = new List<Group>();
            Group adminGroup = new Group("administratorsGroup",Role.Administrator);
            groups.Add(adminGroup);
            Person creator = new Person("admin", "1111", adminGroup);
            bool exit = false;
            Person currentPerson = null;

            Console.WriteLine("Welcome to file system");
        
            while (true)
            {
                string command = "";

                if (!exit)
                {
                    command = Console.ReadLine();
                    switch (command)
                    {
                        case "EXIT":
                            exit = true;
                            break;
                        case "LOGIN":
                            {
                                if (currentPerson != null)
                                {
                                    Console.WriteLine("Need to logout");
                                    break;
                                }

                                Console.Write("Enter login : ");
                                string login = Console.ReadLine();
                                Console.Write("Enter password : ");
                                string password = Console.ReadLine();
                                foreach (Group group in groups)
                                {
                                    foreach (Person person in group.members)
                                    {
                                        if (person.login == login && person.password == password)
                                        {
                                            Console.WriteLine($"{login} is enter to the system");
                                            currentPerson = person;
                                        }
                                    }
                                }

                                if (currentPerson == null)
                                {
                                    Console.WriteLine("Wrong login or password");
                                    break;
                                }
                            }
                            break;
                        case "LOGOUT":
                            {
                                if (currentPerson == null)
                                {
                                    Console.WriteLine("You are already logout");
                                    break;
                                }
                                currentPerson = null;
                            }
                            break;
                        case "ADD_PERSON_TO_GROUP":
                            {
                                if (currentPerson != null)
                                {
                                    Console.Write("Name of Group : ");
                                    string groupName = Console.ReadLine();
                                    Console.Write("Person login : ");
                                    string login = Console.ReadLine();
                                    Console.Write("Person password : ");
                                    string password = Console.ReadLine();

                                    if (!groups.Contains(FindGroup(groups, groupName)))
                                    {
                                        Console.WriteLine("Wrong group");
                                        break;
                                    }
                                    Person person = new Person(login,password,FindGroup(groups,groupName));

                                    currentPerson.addPersonToGroup(FindGroup(groups, groupName), person);
                                }
                                else
                                {
                                    Console.WriteLine("Please log in");
                                }
                            }
                            break;
                        case "DELETE_PERSON_FROM_GROUP":
                            {
                                if (currentPerson != null)
                                {
                                    Console.WriteLine("Name of Group : ");
                                    string groupName = Console.ReadLine();
                                    Console.WriteLine("Persone login : ");
                                    string login = Console.ReadLine();

                                    Person user = null;
                                    user = FindPerson(groups, login);

                                    if (user == null || !groups.Contains(FindGroup(groups, groupName)))
                                    {
                                        Console.WriteLine("Wrong login or group");
                                        break;
                                    }
                                    currentPerson.deletePersonFromGroup(FindGroup(groups, groupName), user);
                                }
                                else
                                {
                                    Console.WriteLine("Please log in");
                                }
                            }
                            break;
                        case "DELETE_GROUP":
                            {
                                if (currentPerson != null)
                                {
                                    Console.Write("Enter group name : ");
                                    string groupName = Console.ReadLine();
                                    if (groups.Contains(FindGroup(groups, groupName)))
                                    {
                                        currentPerson.groupDelete(groups, FindGroup(groups, groupName));
                                    }
                                    else
                                    {
                                        Console.WriteLine("There are no such group");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Please log in");
                                }
                            }
                            break;
                        case "ADD_GROUP":
                            {
                                if (currentPerson != null)
                                {
                                    Console.Write("Enter group name : ");
                                    string groupName = Console.ReadLine();
                                    Console.Write("Enter group role : ");
                                    string groupRole = Console.ReadLine();

                                    switch (groupRole)
                                    {
                                        case "Administrator":
                                            currentPerson.groupAdd(groups, new Group(groupName, Role.Administrator));
                                            break;
                                        case "Menedger":
                                            currentPerson.groupAdd(groups, new Group(groupName, Role.Menedger));
                                            break;
                                        case "Operator":
                                            currentPerson.groupAdd(groups, new Group(groupName, Role.Operator));
                                            break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Please log in");
                                }
                                break;
                            }
                        case "SET_GROUP_ROLE":
                            {
                                if (currentPerson != null)
                                {
                                    Console.Write("Enter group name : ");
                                    string groupName = Console.ReadLine();
                                    if (groups.Contains(FindGroup(groups, groupName)))
                                    {
                                        Console.Write("Enter group new role : ");
                                        string role = Console.ReadLine();
                                        switch (role)
                                        {
                                            case "Administrator":
                                                currentPerson.setGroupRole(FindGroup(groups, groupName), Role.Administrator);
                                                break;
                                            case "Menedger":
                                                currentPerson.setGroupRole(FindGroup(groups, groupName), Role.Menedger);
                                                break;
                                            case "Operator":
                                                currentPerson.setGroupRole(FindGroup(groups, groupName), Role.Operator);
                                                break;
                                        }
                                    }

                                    else
                                    {
                                        Console.WriteLine("There are no such group");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Please log in");
                                }
                            }
                            break;
                        case "OPEN_FILE":
                            {
                                if (currentPerson != null)
                                {
                                    Console.Write("Enter file name : ");
                                    string fileName = Console.ReadLine();
                                    currentPerson.openFile(path, fileName);
                                }
                            }
                            break;
                        case "ADD_FILE":
                            {
                                if (currentPerson != null)
                                {
                                    Console.Write("Enter file name : ");
                                    string fileName = Console.ReadLine();
                                    currentPerson.addFile(path, fileName);
                                }
                                else
                                {
                                    Console.WriteLine("Please log in");
                                }
                            }
                            break;
                        case "DELETE_FILE":
                            {
                                if (currentPerson != null)
                                {
                                    Console.Write("Enter file name : ");
                                    string fileName = Console.ReadLine();
                                    currentPerson.deleteFile(path, fileName);
                                }
                                else
                                {
                                    Console.WriteLine("Please log in");
                                }
                            }
                            break;
                        case "GROUPS":
                            {
                                foreach (Group group in groups)
                                {
                                    Console.WriteLine("-{0}", group.name);
                                }
                            }
                            break;
                        case "MEMBERS":
                            {
                                Console.Write("Enter group name : ");
                                string groupName = Console.ReadLine();
                                foreach (Person person in FindGroup(groups, groupName).members)
                                {
                                    Console.WriteLine("-{0}",person.login);
                                }
                            }
                            break;
                        case "CURRENT_USER":
                            {
                                if (currentPerson != null)
                                {
                                    Console.WriteLine(currentPerson.login);
                                }
                                else
                                {
                                    Console.WriteLine("No one log in");
                                }
                            }
                            break;
                        default:
                            Console.WriteLine("Wrong command");
                            break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
