using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Jlib
{
    class JADuser : JADirectory
    {
        public JADuser()
        {

        }
        public JADuser(string UserSearch, JADSearchType SearchType)
        {
            //var test = ActiveDirectory;
            SetUser(UserSearch, SearchType);
        }

        public void SetUser(string UserSearch, JADSearchType SearchType)
        {
            string.Compare("", "", StringComparison.InvariantCultureIgnoreCase);
            switch (SearchType)
            {
                case JADSearchType.Username:
                    getUser("(samaccountname=" + UserSearch.Trim() + ")");
                    break;
                case JADSearchType.Fullname:
                    getUser("(displayname=" + UserSearch.Trim() + ")");
                    break;
                case JADSearchType.Mailnick:
                    getUser("(mailnickname=" + UserSearch.Trim() + ")");
                    break;
                case JADSearchType.Email:
                    getUser("(mail=" + UserSearch.Trim() + ")");
                    break;
                default:
                    break;
            }
        }

        private void getUser(string search)
        {
            try
            {
                DirectorySearcher Dsearch = new DirectorySearcher(ActiveDirectory, search);
                Dsearch.PropertiesToLoad.Add("samaccountname"); //username
                Dsearch.PropertiesToLoad.Add("displayname"); //full name
                Dsearch.PropertiesToLoad.Add("mailnickname"); //abbr
                Dsearch.PropertiesToLoad.Add("mail"); //email
                Dsearch.PropertiesToLoad.Add("badpwdcount"); //bad password count
                Dsearch.PropertiesToLoad.Add("homedirectory"); //H drive

                var searchResult = Dsearch.FindOne();
                //return searchResult.Properties["mailnickname"][0].ToString();
                UserName = searchResult.Properties["samaccountname"][0].ToString();
                FullName = searchResult.Properties["displayname"][0].ToString();
                MailNick = searchResult.Properties["mailnickname"][0].ToString();
                EMail = searchResult.Properties["mail"][0].ToString();
                LockoutCount = searchResult.Properties["badpwdcount"][0].ToString();
                Drive = searchResult.Properties["homedirectory"][0].ToString();

                ADuser = UserPrincipal.FindByIdentity(Ctx, UserName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetLockedStatus()
        {
            if (ADuser.IsAccountLockedOut())
            {
                LockedOut = true;
                return true;
            }
            else
            {
                LockedOut = false;
                return false;
            }
        }


        public string UserName { get; private set; }
        public string FullName { get; private set; }
        public string MailNick { get; private set; }
        public string EMail { get; private set; }
        public bool LockedOut { get; private set; }
        public string LockoutCount { get; private set; }
        public string Drive { get; private set; }
        public UserPrincipal ADuser { get; private set; }
    }

    public enum JADSearchType
    {
        Username = 0,
        Fullname = 1,
        Mailnick = 2,
        Email = 3
    }
}
