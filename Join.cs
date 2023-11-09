using System;

namespace arm_robot
{

    internal class Join
    {
        private static Join join;
        public double theta2;
        public double theta3;
        public double d1;
        public Join()
        {
            theta3 = 0;
            theta2 = 0;
            d1 = 0;
        }
        public void  coppy (Join join)
        {
            this.theta2 = join.theta2;
            this.theta3 = join.theta3;
            this.d1 = join.d1;

        }

    }
}
