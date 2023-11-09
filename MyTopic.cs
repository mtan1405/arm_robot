using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arm_robot
{
    internal class MyTopic
    {
        static List<ObserverForm3> observerForm3s = new List<ObserverForm3>();
        public void set (ObserverForm3 o)
        {
            observerForm3s.Add(o);
        }
        public void  remove (ObserverForm3 o)
        {
            observerForm3s.Remove(o);
        }
        public void update ()
        {
            foreach (ObserverForm3 o in observerForm3s) {
                o.CallbaclInterface(); 
            }
        }
    }
}
