using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace arm_robot
{
    public partial class Form3 : Form , ObserverForm3  
    {

        TransferClasscs transferClasscs;
        MyTopic topic;
        Point A;
        Point B;
        Caculator caculator;
        Join currentJoin;
        List<Join> AJoins;
        List<Join> BJoins;
        byte send = 0; 
        
        
        public Form3() 
        {
            InitializeComponent();
            transferClasscs = TransferClasscs.GetInstance () ;
            topic = new MyTopic();
            topic.set(this);
            A = new Point();
            B = new Point();
            caculator = Caculator.GetCaculator();
            currentJoin = transferClasscs.currentJoin ;
            transferClasscs = TransferClasscs.GetInstance();
            AJoins = new List<Join>();
            BJoins = new List<Join>();
            AJoins.Add(new Join());
            AJoins.Add(new Join());
            BJoins.Add(new Join());
            BJoins.Add(new Join());
            lbStatusA.Hide();
            lbStatusB.Hide();

            
        }
        private void txtD1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1) || ((e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('-') > -1)))
            {
                e.Handled = true;
            }
        }

        private void Form3_Shown(object sender, EventArgs e)
        {
           txtX_A.Text = transferClasscs.currentJoin.d1.ToString();
           txtY_A.Text = transferClasscs.currentJoin.theta2.ToString();
           txtZ_A .Text = transferClasscs.currentJoin.theta3.ToString();
        }

        private void txtY_2_TextChanged(object sender, EventArgs e)
        {

        }
        private string getStringSteps(int stepM1, int stepM2, int stepM3, char endOfString = 'e')
        {
            string str = stepM1.ToString() + " " + stepM2.ToString() + " " + stepM3.ToString() + endOfString.ToString();
            return str;

        }
        private void lockToWaitF ()
        {
            button2.Enabled = false;
            button4.Enabled = false;
        }
        private void unlock ()
        {
            button2.Enabled = true ;
            button4.Enabled = true ;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //send to move robot 
           if (  sendToMove() == 1 ) 
            lockToWaitF(); 
        }
        int  sendToMove () 
        {
            try
            {


                if (caculator.checkJoin(AJoins[0]))
                {
                    Step step = caculator.getStep(ref currentJoin, AJoins[0]);
                    currentJoin.coppy(AJoins[0]);
                    // message to user 
                    lbStA.Text = "A[0] đã được chọn ";
                   
                    transferClasscs.serial.Write(getStringSteps(step.step_1, step.step_2, step.step_3, 'f'));
                   // MessageBox.Show(getStringSteps(step.step_1, step.step_2, step.step_3, 'f'));
                    send = 2;
                    return 1;
                }
                else
                {
                    if (caculator.checkJoin(AJoins[1]))
                    {
                        Step step = caculator.getStep(ref currentJoin, AJoins[0]);
                        currentJoin.coppy(AJoins[1]);
                        lbStA.Text = "A[1] đã được chọn ";
                        transferClasscs.serial.Write(getStringSteps(step.step_1, step.step_2, step.step_3, 'f'));
                        send = 2; 
                        return 1;
                    }
                    else
                    {
                        lbStA.Text = "Không hợp lệ";
                        send = 0;
                        return 0;
                    }
                }
            } catch (Exception ex)
            {
                lbStA.Text = "Exception " + ex.Message;
                send = 0;
                return 0; 
            }
        }
        void sendToMove2()
        {
            try
            {


                if (caculator.checkJoin(BJoins[0]))
                {
                    Step step = caculator.getStep(ref currentJoin, BJoins[0]);
                    currentJoin.coppy(BJoins[0]);
                    // message to user 
                    lbStB.Text = "B[0] đã được chọn ";
                    transferClasscs.serial.Write(getStringSteps(step.step_1, step.step_2, step.step_3, 's'));
                    send = 1;
                }
                else
                {
                    if (caculator.checkJoin(BJoins[1]))
                    {
                        Step step = caculator.getStep(ref currentJoin, BJoins[1]);
                        currentJoin.coppy(BJoins[1]);
                        lbStB.Text = "B[1] đã được chọn ";
                        transferClasscs.serial.Write ( getStringSteps( step.step_1, step.step_2, step.step_3, 's') );
                        send = 1 ; 
                    }
                    else
                    {
                        lbStB.Text = "Không hợp lệ";
                        send = 0; 
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                lbStB.Text = "Exception " + ex.Message;
                send = 0; 
            }
        }
   

        void ObserverForm3.CallbaclInterface()
        {
            if (send == 0 ) 
            unlock();
            else if (send == 1 )
            {
                sendToMove();
            }else if (send == 2 )
            {
                sendToMove2();
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            topic.remove(this); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                A.x = Double.Parse(txtX_A.Text);
                A.y = Double.Parse (txtY_A.Text);
                A.z = Double.Parse(txtZ_A.Text);

                B.x = Double.Parse(txtX_B.Text);
                B.y = Double.Parse(txtY_B.Text);
                B.z = Double.Parse(txtZ_B.Text);

            if (caculator.getJoint(A, AJoins) == 1  )
                {
                    //s
                    caculator.convert(AJoins[0]);
                    caculator.convert(AJoins[1]); 
                    printSuccA(); 

                } else
                {
                     //Err
                    printErrA(); 
                }
            if (caculator.getJoint (B , BJoins) == 1 )
                {
                    caculator.convert(BJoins[0]);
                    caculator.convert(BJoins[1]);
                    printSuccB();

                }else
                {
                    printErrB();
                }

            }
            catch (Exception ex) 
            {
                MessageBox.Show("Exception "+ex.Message);
            }
        }

        void printSuccA  ()
        {
            lbStatusA.Hide();
            printJoint(txtD1_2, txtTheta2_2, txtTheta3_2, AJoins[0]);
            printJoint(txtD1_3 , txtTheta2_3 ,txtTheta3_3 , AJoins[1]);
             


        }
        void printSuccB ()
        {
            lbStatusB.Hide();
            printJoint(txtD1_2_1, txtTheta2_2_1, txtTheta3_2_1, BJoins[0]);
            printJoint(txtD1_3_1, txtTheta2_3_1, txtTheta3_3_1, BJoins[1]);
        }
        void printErrA ()
        {
            lbStatusA.Show();
        }
        void printErrB()
        {
            lbStatusB.Show(); 
        }

        void printJoint (TextBox D1 , TextBox Theta2 , TextBox Theta3 , Join join)
        {
            D1.Text     = Math.Round (join.d1,2).ToString(); 
            Theta2.Text = Math.Round (join.theta2 ,2).ToString() ;
            Theta3.Text = Math.Round (join.theta3,2).ToString() ;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            send = 0; 
        }
    }
}
