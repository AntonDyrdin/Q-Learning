using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manipulator_simulation
{
    public class Link
    {
        public double x1=100;
        public double y1 = 100;
        public double x2 = 30;
        public double y2 = 30;
        public double l = 30;
        public double angle=1.5;
        public Link(double length)
        {
            l = length;
        }
      public void setAngle(double angle)
        {
            this.angle = angle;
            x2 = x1 + (l * Math.Cos(angle));
            y2 = y1 - (l * Math.Sin(angle ));
        }
    }
}
