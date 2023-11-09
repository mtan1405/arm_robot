using System;
using System.Collections.Generic;

namespace arm_robot
{
    internal class Caculator
    {
        private const double L1 = 10;
        private const double L2 = 10;
        private const double L3 = 10;

        private const double div_1 = 1000;
        private const double div_2 = 2;
        private const double div_3 = 20.0 / 12.0;

        private const double gain2 = 2;
        private const double gain3 = 2;
        private const double gain1 = 1;

        private const double one_step = 1.8;

        private const double Min_The2 = -85;
        private const double Max_The2 = 85;
        private const double Min_The3 = -120;
        private const double Max_The3 = 120;
        private const double Max_D1 = 45;
        private const double Min_D1 = 20;

        private const double convertAngle = Math.PI / 180; 

        private Err err;
        private static Caculator caculator;
        private Caculator() { }
        public static Caculator GetCaculator()
        {
            if (caculator == null) { caculator = new Caculator(); }
            return caculator;
        }
        public void getPoint(Point point, Join join)
        {
            point.x = L1 + Math.Cos(join.theta2 + join.theta3) * L3 + Math.Cos(join.theta2) * L2;
            point.y = Math.Sin(join.theta2 + join.theta3) * L3 + Math.Sin(join.theta2) * L2;
            point.z = join.d1;
        }

        public int getJoint(Point point, List<Join> joins)
        {
            if (joins.Count == 0) return 0;
            double xC = point.x;
            double yC = point.y;
            double zC = point.z;
            double cosThe3 = (-Math.Pow(L3, 2) + -Math.Pow(L2, 2) + Math.Pow(xC - L1, 2) + Math.Pow(yC, 2)
                             ) / (2 * L3 * L2);
            double sinThe3_1 = Math.Sqrt(1 - Math.Pow(cosThe3, 2));
            double sinThe3_2 = -Math.Sqrt(1 - Math.Pow(cosThe3, 2));
            // double theta3_1     =  Math.Atan2(cosThe3, sinThe3_1);
            // double theta3_2     = Math.Atan2(cosThe3 ,sinThe3_2);
            double theta3_1 = Math.Atan2(sinThe3_1, cosThe3);
            double theta3_2 = Math.Atan2(sinThe3_2, cosThe3);

            // solve hptr 2 



            double x, y, a, b, c, d, e, z;
            a = L2 + L3 * Math.Cos(theta3_1);
            b = -L2 * Math.Sin(theta3_1);
            c = xC - L1;

            d = L3 * Math.Sin(theta3_1);
            e = L3 * Math.Cos(theta3_1) + L2;
            z = yC;

            x = (c * e - z * b) / (a * e - d * b);
            y = (c * d - z * a) / (b * d - a * e);


            double sinTh2 = y, cosTh2 = x;
            joins[0].d1 = zC;
            joins[0].theta2 = Math.Atan2(sinTh2, cosTh2);
            joins[0].theta3 = theta3_1;
            // MessageBox.Show(((Double) Math.Atan2 (y , x )  * 180/ Math.PI ).ToString() +"~~~" + ((Double)theta3_1 * 180 / Math.PI) . ToString () ) ;
            a = L2 + L3 * Math.Cos(theta3_2);
            b = -L2 * Math.Sin(theta3_2);
            c = xC - L1;
            d = L3 * Math.Sin(theta3_2);
            e = L3 * Math.Cos(theta3_2) + L2;
            z = yC;
            x = (c * e - z * b) / (a * e - d * b);
            y = (c * d - z * a) / (b * d - a * e);
            sinTh2 = y; cosTh2 = x;
            joins[1].d1 = zC;
            joins[1].theta2 = Math.Atan2(sinTh2, cosTh2);
            joins[1].theta3 = theta3_2;

            // display distance x,y 
            //   MessageBox.Show(((Double)Math.Atan2(y, x) * 180 / Math.PI).ToString() + "~~~" + ((Double)theta3_2 * 180 / Math.PI).ToString());
            if (Double.IsNaN(theta3_1) || Double.IsNaN(theta3_2))
            {
                return 0;
            }
            return 1;

        }
        public Join selectJoin(Join currentJoin, List<Join> joins)
        {
            if (joins == null || joins != null || joins.Count == 0) return null;
            double distance1 = Math.Abs(currentJoin.theta2 - joins[0].theta2);
            double distance2 = Math.Abs(currentJoin.theta2 - joins[1].theta2);
            if (distance1 < distance2) { return joins[0]; }
            else
            { return joins[1]; }
        }
        public Step getStep(ref Join currentJoin, Join desJoin)
        {
            if (currentJoin == null) currentJoin = new Join();
            Step step = new Step();
            double step2 = (((desJoin.theta2 - currentJoin.theta2) / one_step) * gain2 * div_2);
            double step3 = (((desJoin.theta3 - currentJoin.theta3) / one_step) * gain3 * div_3);
            double step1 = (desJoin.d1 - currentJoin.d1) * div_1; 
            step.step_2 = (int)step2;
            step.step_3 = (int)step3;
            step.step_1 = (int)step1;
            return step;
        }
        public bool checkJoin(Join join)
        {
            if (join.theta2 > 0 && join.theta2 > Max_The2)
            {
                err = Err.Theta2_Err;
                return false;
            }


            if (join.theta2 < 0 && join.theta2 < Min_The2) {
                err = Err.Theta2_Err;
                return false;
            }
            if (join.theta3 > 0 && join.theta3 > Max_The3)
            {
                err = Err.Theta3_Err;
                return false;
            }


            if (join.theta3 < 0 && join.theta3 < Min_The3)
            {
                err = Err.Theta3_Err;
                return false;
            }

            if (join.d1 > Max_D1 || join.d1 < Min_D1)
            {
                err = Err.D_Err;
                return false; 
            }
            err = Err.None;
            return true;

        }
        public String getErr()
        {
            String str = "Ok" ;
            switch (err)
            {
                case Err.Theta2_Err :
                    str = "Theta2 Không hợp lệ ";
                    break;
                case Err.D_Err:
                    str = "D Không hợp lệ ";
                    break;
                case Err.Theta3_Err:
                    str = "Theta3 Không hợp lệ ";
                    break; 


            }
            return str;
        }
        public void convert(Join join)
        {
            join.theta2 = join.theta2 / convertAngle;
            join.theta3 = join.theta3 / convertAngle;
        }

    }
    enum Err {
        Theta2_Err = 0,
        D_Err = 1,
        Theta3_Err = 2,
        None = 4 
    }
}
