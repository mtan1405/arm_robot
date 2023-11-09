namespace arm_robot
{
    internal class Point
    {
        public double x;
        public double y;
        public double z;
        public Point()
        {
            x = 0;
            y = 0;
            z = 0;
        }
        public Point(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
