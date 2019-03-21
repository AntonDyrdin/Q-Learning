using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Manipulator_simulation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Manipulator M;
        private DecisionMakingSystem DMS;
        Random r;
        MultiParameterVisualizer MPV;
        private void Form1_Load(object sender, EventArgs e)
        {
            r = new Random();
            MPV = new MultiParameterVisualizer(pictureBox1, this);

            DMS = new DecisionMakingSystem(this);
            M = new Manipulator(this, 300, 500);
            M.AddLink(200);
            M.AddLink(150);
            M.AddLink(100);
            M.AddLink(50);

            //  M.draw(this);

            /*  DMS.addParameter("direction", "0-45,45-90,90-135,135-180,180-225,225-270,270-315,315-360");
              DMS.addParameter("f1", "0-45,45-90,90-135,135-180,180-225,225-270,270-315,315-360");
              DMS.addParameter("f2", "0-45,45-90,90-135,135-180,180-225,225-270,270-315,315-360");
              DMS.addParameter("f3", "0-45,45-90,90-135,135-180,180-225,225-270,270-315,315-360");   */

            DMS.addParameter("direction", "0-90,90-180,180-270,270-360");
            DMS.addParameter("f1", "0-90,90-180,180-270,270-360");
            DMS.addParameter("f2", "0-90,90-180,180-270,270-360");
            DMS.addParameter("f3", "0-90,90-180,180-270,270-360");
            DMS.addParameter("f4", "0-90,90-180,180-270,270-360");
            DMS.defaultActions.Add(new DMSAction("+f1"));
            DMS.defaultActions.Add(new DMSAction("-f1"));
            DMS.defaultActions.Add(new DMSAction("+f2"));
            DMS.defaultActions.Add(new DMSAction("-f2"));
            DMS.defaultActions.Add(new DMSAction("+f3"));
            DMS.defaultActions.Add(new DMSAction("-f3"));
            DMS.defaultActions.Add(new DMSAction("+f4"));
            DMS.defaultActions.Add(new DMSAction("-f4"));
            DMS.defaultActions.Add(new DMSAction("none"));
            DMS.generateStates();
            System.Threading.Tasks.Task maintask = new Task(() => { work(); });
            maintask.Start();
        }
        string angleToDiscreteStr(double angle)
        {
            string stateInString = "";

            double A = angle;



            if (A < 0)
            {
                plus360:
                A = A + 360;
                if (A < 360)
                {
                    goto plus360;
                }
            }
            if (A >= 360)
            {
                minus360:
                A = A - 360;
                if (A >= 360)
                {
                    goto minus360;
                }
            }
            if (A >= 0 & A < 90)
            {
                stateInString += "0-90";
            }
            else
            if (A >= 90 & A < 180)
            {
                stateInString += "90-180";
            }
            else
            if (A >= 180 & A < 270)
            {
                stateInString += "180-270";
            }
            else
            if (A >= 270 & A < 360)
            {
                stateInString += "270-360";
            }
            else
            {
                stateInString += "270-360";
            }
            return stateInString;
        }
        public void work()
        {
            double df = 0.01;
            double oldE = 1000;
            double e = 1000;
            MPV.enableGrid = false;
            MPV.addParameter("steps", Color.Red, 300);
            MPV.addParameter("reward", Color.Red, 300);
            MPV.addParameter("Q", Color.Red, 300);
            while (true)
            {
                // var point = new Point(MousePosition.X, MousePosition.Y);
                var point = new Point(r.Next(0, 500), r.Next(200, 500));
                e = Math.Sqrt(((point.X - M.links[M.links.Count - 1].x2) * (point.X - M.links[M.links.Count - 1].x2)) + ((point.Y - M.links[M.links.Count - 1].y2) * (point.Y - M.links[M.links.Count - 1].y2)));

                int steps = 0;

                // point.Offset(-this.Location.X - 13, -(this.Location.Y + 38));
                while (e > 20)
                {
                    var targetX = M.toLocalCS_X(point.X);
                    var targetY = M.toLocalCS_Y(point.Y);
                    double A; ;
                    /*   if (targetX > 0)
                           A = Math.Atan(targetY / targetX);
                       else
                           A = Math.Atan(targetY / targetX) - (Math.PI * 2 / 2);    */

                    A = Math.Atan(point.Y - M.links[M.links.Count - 1].y2 / point.X - M.links[M.links.Count - 1].x2);

                    var stateInString = "";

                    A = A * 180 / Math.PI;

                    stateInString = "direction:" + angleToDiscreteStr(A);
                    stateInString += ",f1:" + angleToDiscreteStr(M.links[0].angle * 180 / Math.PI)
                                    + ",f2:" + angleToDiscreteStr(M.links[1].angle * 180 / Math.PI)
                                    + ",f3:" + angleToDiscreteStr(M.links[2].angle * 180 / Math.PI)
                                     + ",f4:" + angleToDiscreteStr(M.links[3].angle * 180 / Math.PI);


                    DMS.setActualState(stateInString);


                    var action = DMS.getAction(DMS.getStateByString(stateInString));


                    if (action.type == "+f1")
                        M.setLinkAngle(0, M.links[0].angle + df);
                    if (action.type == "-f1")
                        M.setLinkAngle(0, M.links[0].angle - df);
                    if (action.type == "+f2")
                        M.setLinkAngle(1, M.links[1].angle + df);
                    if (action.type == "-f2")
                        M.setLinkAngle(1, M.links[1].angle - df);
                    if (action.type == "+f3")
                        M.setLinkAngle(2, M.links[2].angle + df);
                    if (action.type == "-f3")
                        M.setLinkAngle(2, M.links[2].angle - df);
                    if (action.type == "+f4")
                        M.setLinkAngle(3, M.links[3].angle + df);
                    if (action.type == "-f4")
                        M.setLinkAngle(3, M.links[3].angle - df);
                    if (action.type == "none")
                    { }

                    /*   stateInString = "direction:" + angleToDiscreteStr(A);
                       stateInString += ",f1:" + angleToDiscreteStr(M.links[0].angle * 180 / Math.PI)
                                       + ",f2:" + angleToDiscreteStr(M.links[1].angle * 180 / Math.PI)
                                       + ",f3:" + angleToDiscreteStr(M.links[2].angle * 180 / Math.PI)
                                        + ",f4:" + angleToDiscreteStr(M.links[3].angle * 180 / Math.PI);   */

                    //  DMS.setActualState(stateInString);

                    M.draw(this, point.X, point.Y);
                    M.refresh();
                    oldE = e;
                    e = Math.Sqrt(((point.X - M.links[M.links.Count - 1].x2) * (point.X - M.links[M.links.Count - 1].x2)) + ((point.Y - M.links[M.links.Count - 1].y2) * (point.Y - M.links[M.links.Count - 1].y2)));


                    if (e < oldE)
                    {
                        DMS.setQ((500 - e) );
                        MPV.addPoint((500 - e) , "reward");
                    }
                    else
                    {
                        DMS.setQ((-e));
                        MPV.addPoint((-e) , "reward");
                    }
                    MPV.addPoint(DMS.lastAction.Q, "Q");

                    if (MPV.parameters[2].functions[0].points.Count > 50)
                    { MPV.parameters[2].functions[0].points.RemoveAt(0);
                        MPV.parameters[2].maxY = 0;
                        MPV.parameters[2].minY = 0;
                    }
                    if (MPV.parameters[1].functions[0].points.Count > 50)
                        MPV.parameters[1].functions[0].points.RemoveAt(0);

                    MPV.refresh();
                    steps++;
                }

                MPV.addPoint(steps, "steps");
                MPV.refresh();
            }
        }


        public void log(string s)
        {
            this.logDelegate = new Form1.LogDelegate(this.delegatelog);
            this.logBox.Invoke(this.logDelegate, this.logBox, s, Color.White);
            var strings = new string[1];
            strings[0] = s;
        }
        public void delegatelog(RichTextBox richTextBox, String s, Color col)
        {
            try
            {
                richTextBox.SelectionColor = col;
                richTextBox.AppendText(s + '\n');
                richTextBox.SelectionColor = Color.White;
                richTextBox.SelectionStart = richTextBox.Text.Length;
                var strings = new string[1];
                strings[0] = s;
            }
            catch { }
        }
        public void picBoxRefresh() { picBox.Refresh(); }
        public delegate void LogDelegate(RichTextBox richTextBox, string is_completed, Color col);
        public LogDelegate logDelegate;
        public delegate void StringDelegate(string is_completed);
        public delegate void VoidDelegate();
        public StringDelegate stringDelegate;
        public VoidDelegate voidDelegate;


        private void picBox_Click(object sender, EventArgs e)
        {
            var point = new Point(MousePosition.X, MousePosition.Y);
            point.Offset(-this.Location.X - 13, -(this.Location.Y + 38));
            M.moveToThePoint(point.X, point.Y);
            M.draw(this, point.X, point.Y);
            M.refresh();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            DMS.gamma = Convert.ToDouble(trackBar2.Value) / 100;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            DMS.alpha = Convert.ToDouble(trackBar1.Value) / 100;
        }
    }
}
/*  if (A >= 0 & A < 45)
  {
      stateInString += "0-45";
  }
  else
  if (A >= 45 & A < 90)
  {
      stateInString += "45-90";
  }
  else
  if (A >= 90 & A < 135)
  {
      stateInString += "90-135";
  }
  else
  if (A >= 135 & A < 180)
  {
      stateInString += "135-180";
  }
  else
  if (A >= 180 & A < 225)
  {
      stateInString += "180-225";
  }
  else
  if (A >= 225 & A < 270)
  {
      stateInString += "225-270";
  }
  else
  if (A >= 270 & A < 315)
  {
      stateInString += "270-315";
  }
  else
  if (A >= 315 & A < 360)
  {
      stateInString += "315-360";
  }
  else
  {
      stateInString += "315-360";
  }
*/
