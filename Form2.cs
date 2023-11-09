using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace arm_robot
{

    public partial class Form2 : Form
    {
        BackgroundWorker worker;
        Join join;
        Point point;
        Caculator caculator;
        const double convertAngle = Math.PI / 180;
        List<Join> joins;
        List<Point> points;
        Thread thread;
        char[] msg = new char[100];
        Step step;
        Join currentJoin;
        MyTopic topic;
        const int beginningOfZ = 25; 

        Join joinMassage;
        public Form2() 
        {
            InitializeComponent();
            point = new Point();
            join = new Join();
            caculator = Caculator.GetCaculator();
            joins = new List<Join>(2);
            joins.Add(new Join());
            joins.Add(new Join());
            points = new List<Point>(2);
            points.Add(new Point());
            points.Add(new Point());
            lbSatus.Visible = false;
            worker = new BackgroundWorker();
            thread = new Thread(handling_Err);
            serialPort1.ReadTimeout = 2000 ;
            serialPort1.WriteTimeout = 500;
            serialPort1.StopBits = StopBits.Two ; 
            currentJoin = new Join();
            currentJoin.d1 =  beginningOfZ ; 
            topic = new MyTopic(); 

        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                join.d1 = Double.Parse(txtD1.Text);
                join.theta2 = Double.Parse(txtThe2.Text) * convertAngle;
                join.theta3 = Double.Parse(txtThe3.Text) * convertAngle;
                caculator.getPoint(point, join);
                txtX_1.Text = point.x.ToString();
                txtY_1.Text = point.y.ToString();
                txtZ_1.Text = point.z.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }


        }

        // only press number 
        private void txtD1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1)|| ( (e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('-') > -1) ))
            {
                e.Handled = true;
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                point.x = Double.Parse(txtX_2.Text);
                point.y = Double.Parse(txtY_2.Text);
                point.z = Double.Parse(txtZ_2.Text);

                if (caculator.getJoint(point, joins) == 1)
                {
                    this.Print_txt();
                    convert(joins[0]);
                    convert(joins[1]);

                   
                    
                }
                else
                {
                    this.Print_txt_2();
                    
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }


        }
        void Print_txt()
        {
            if (joins == null) return;
            txtD1_2.Text = joins.ElementAt(0).d1.ToString();
            txtTheta2_2.Text = ((joins.ElementAt(0)).theta2 / convertAngle).ToString();
            txtTheta3_2.Text = (joins[0].theta3 / convertAngle).ToString();

            txtD1_3.Text = (joins[1].d1 / 1).ToString();
            txtTheta2_3.Text = (joins[1].theta2 / convertAngle).ToString();
            txtTheta3_3.Text = (joins[1].theta3 / convertAngle).ToString();
            lbSatus.Visible = false;
        }
        void Print_txt_2()
        {
            lbSatus.Text = "Không thể tính toán ";
            lbSatus.ForeColor = Color.Green;
            lbSatus.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String[] serialPort = SerialPort.GetPortNames();
            if (serialPort == null) return;
            //    MessageBox.Show(serialPort[0] + " " + serialPort1.PortName );
            try
            {

                serialPort1.PortName = serialPort[0];
                serialPort1.Open();
                txtCOM.Text = serialPort[0];
                if (!thread.IsAlive)
                {
                    thread.Start();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        int index = 0;
        void handling_Err()
        {
            while (true)
            {

                index++; 
                try
                {
                     
                    string msg2 = serialPort1.ReadLine();
                    //  String msg4 = serialPort1.ReadLine();
                  //  int c = serialPort1.Read(msg3, 0 , 20 ); 
                    
                    txtPrint.BeginInvoke((MethodInvoker)delegate
                    {
                        txtPrint.Text = (msg2);
                        if (msg2.Contains ("done") ) 
                        {

                         
                        unlock();
                        topic.update();
                        }
                      
                        
                    });

                    

                }
                catch (Exception e)
                {
                    txtPrint.BeginInvoke((MethodInvoker)delegate
                    {
                        txtPrint.Text = e.Message + index.ToString();
                    });
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Write(getStringSteps (-100 , -20 , 100 ) );
                 
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //  thread.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {

            Form3 form3 = new Form3 ();
            TransferClasscs.GetInstance().currentJoin = currentJoin;
            TransferClasscs.GetInstance().serial = serialPort1;
            form3.ShowDialog();
            
        }


        private string getStringSteps (int  stepM1 ,int  stepM2 , int stepM3 , char endOfString = 'e' ) 
        {
            string str = stepM1.ToString() + " " + stepM2.ToString() + " " + stepM3.ToString() + endOfString.ToString();
            return str;

        }
        private void convert (Join join )
        {
            join.theta2 = join.theta2 / convertAngle;
            join.theta3 = join.theta3 / convertAngle;   
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            { 
            if (caculator.checkJoin(joins[0])) // check whether could compute 2341ss
            {
                step = caculator.getStep(ref currentJoin, joins[0]);
                    getStringSteps(step.step_1, step.step_2, step.step_3); 
                    MessageBox.Show(step.step_2.ToString() + "     " + step.step_3.ToString() + "   " + step.step_1.ToString());
                    serialPort1.Write(getStringSteps(step.step_1, step.step_2, step.step_3));
                currentJoin.coppy(joins[0]);
                    lockToWaitToFinish();

                    MessageBox.Show(step.step_2.ToString() + "     " + step.step_3.ToString() + "   " + step.step_1.ToString () ) ;
                
            }else
            {
                MessageBox.Show(caculator.getErr () );
            }
            

            }catch (Exception ee  )
            {
                MessageBox.Show("Exception " + ee.Message );
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (caculator.checkJoin(joins[1]))
                {
                    step = caculator.getStep(ref currentJoin, joins[1]);
                    serialPort1.Write(getStringSteps(step.step_1, step.step_2, step.step_3));
                    currentJoin.coppy(joins[1]);
                    lockToWaitToFinish();
                    MessageBox.Show(step.step_2.ToString() + "     " + step.step_3.ToString());
                     
                }
                else
                {
                    MessageBox.Show(caculator.getErr () );
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("Exception " + ee.Message);
            }
        }
        private void lockToWaitToFinish ()
        {
            button6.Enabled = false; 
            button7.Enabled = false;
            button2.Enabled = false;
        }
        private void unlock ()
        {
            button6.Enabled = true ;
            button7.Enabled = true ;
            button2.Enabled = true ;
        }
    }
}
