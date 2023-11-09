using System;
using System.Windows.Forms;

namespace arm_robot
{

    public partial class Form1 : Form
    {
        Join join;
        Point point;
        Caculator caculator;
        const double convertAngle = Math.PI / 180;
        public Form1()
        {
            InitializeComponent();
            point = new Point();
            join = new Join();
            caculator = Caculator.GetCaculator();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                join.d1 = Double.Parse(txtD1.Text);
                join.theta2 = Double.Parse(txtThe2.Text) * convertAngle;
                join.theta3 = Double.Parse(txtThe3.Text) * convertAngle;
                caculator.getPoint(point, join);
                MessageBox.Show(((Double)(point.x)).ToString() + "   " + ((Double)(point.y)).ToString() + " " + ((Double)(point.z)).ToString());
                // MessageBox.Show(Math.Sin(90/).ToString());

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
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
