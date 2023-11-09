using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arm_robot
{
    internal class TransferClasscs
    {
        private TransferClasscs() { }
        public static TransferClasscs Instance { get; private set;} = new TransferClasscs();
        public static TransferClasscs GetInstance() {  return Instance; }
        public Join   currentJoin { get; set; }
        public SerialPort serial { get; set; } 

    }
}
