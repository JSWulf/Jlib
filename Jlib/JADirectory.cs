using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Jlib
{
    class JADirectory
    {

        public JADirectory()
        {
            ActiveDirectory = new DirectoryEntry("LDAP://" + Environment.UserDomainName);
            Ctx = new PrincipalContext(ContextType.Domain);
        }
        public JADirectory(string Domain)
        {
            //var DirEntry = new DirectoryEntry("LDAP://RootDSE");
            //var NamingContext = DirEntry.Properties.Values.
            ActiveDirectory = new DirectoryEntry(Domain);
            Ctx = new PrincipalContext(ContextType.Domain);
        }

        public DirectoryEntry ActiveDirectory { get; private set; }
        public PrincipalContext Ctx { get; private set; }

        public void GetUserNameList(string FileWithList, string OutputFile)
        {
            var NameList = File.ReadAllLines(FileWithList);
            var Output = new List<string>();

            foreach (var name in NameList)
            {
                try
                {
                    Output.Add(getUserName(name));
                }
                catch (Exception)
                {

                    Output.Add("error: " + name);
                }

            }

            File.WriteAllLines(OutputFile, Output);
        }

        public string getUserName(string Search)
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            UserPrincipal user = new UserPrincipal(ctx)
            {
                DisplayName = Search.Trim()
            };

            PrincipalSearcher searcher = new PrincipalSearcher(user);

            return searcher.FindOne().SamAccountName;

        }

        public void getAbbreviationList(string FileWithList, string OutputFile)
        {
            var NameList = File.ReadAllLines(FileWithList);
            var Output = new List<string>();

            foreach (var name in NameList)
            {
                try
                {
                    Output.Add(getAbbreviation(name));
                }
                catch (Exception)
                {

                    Output.Add("error: " + name);
                }

            }

            File.WriteAllLines(OutputFile, Output);
        }

        public string getAbbreviation(string search)
        {
            var nl = Environment.NewLine;

            var user = new JADuser(search, JADSearchType.Fullname);

            //return user.UserName + nl
            //    + user.FullName + nl
            //    + user.MailNick + nl
            //    + user.EMail + nl
            //    + user.LockoutCount + nl
            //    + user.Drive + nl;

            return user.MailNick;

        }

        public JADbitlocker GetBitLocker(string hostname)
        {
            var output = new StringBuilder();

            DirectorySearcher directorySearcher = new DirectorySearcher(ActiveDirectory);
            directorySearcher.Filter = "(&(ObjectCategory=computer)(cn=" + hostname + "))";

            var Result = directorySearcher.FindOne();

            var Rpath = Result.Path;
            var BTsearch = new DirectorySearcher(Rpath)
            {
                SearchRoot = Result.GetDirectoryEntry(), //without this line we get every entry in AD.
                Filter = "(&(objectClass=msFVE-RecoveryInformation))"
            };

            BTsearch.PropertiesToLoad.Add("msfve-recoveryguid");
            BTsearch.PropertiesToLoad.Add("msfve-recoverypassword");

            var GetAll = BTsearch.FindAll();

            var BT = new JADbitlocker(hostname);

            foreach (SearchResult item in GetAll)
            {
                if (item.Properties.Contains("msfve-recoveryguid") && item.Properties.Contains("msfve-recoverypassword"))
                {
                    var pid = (byte[])item.Properties["msfve-recoveryguid"][0];
                    var rky = item.Properties["msfve-recoverypassword"][0].ToString();

                    BT.AddKey(pid, rky);
                    var lnth = BT.RecoveryKey.Count - 1;

                    System.Diagnostics.Debug.WriteLine("Added... " + BT.PasswordID[lnth] + " for: " + BT.RecoveryKey[lnth]);
                }
            }

            return BT;


        }


    }
}
