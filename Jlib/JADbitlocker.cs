using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jlib
{
    class JADbitlocker
    {
        public JADbitlocker(string HostName)
        {
            SystemName = HostName;
            PasswordID = new List<string>();
            RecoveryKey = new List<string>();
        }

        public void AddKey(byte[] ID, string Key)
        {
            PasswordID.Add(ConvertID(ID));
            RecoveryKey.Add(Key);
        }

        private string ConvertID(byte[] id)
        {
            return
              id[3].ToString("X02") + id[2].ToString("X02")
            + id[1].ToString("X02") + id[0].ToString("X02") + "-"
            + id[5].ToString("X02") + id[4].ToString("X02") + "-"
            + id[7].ToString("X02") + id[6].ToString("X02") + "-"
            + id[8].ToString("X02") + id[9].ToString("X02") + "-"
            + id[10].ToString("X02") + id[11].ToString("X02")
            + id[12].ToString("X02") + id[13].ToString("X02")
            + id[14].ToString("X02") + id[15].ToString("X02")
                ;
        }

        public string SystemName { get; private set; }
        public List<string> PasswordID { get; private set; }
        public List<string> RecoveryKey { get; private set; }
    }
}
