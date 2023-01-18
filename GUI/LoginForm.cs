using GeorgeBrownTeacher_CourseManagement.DataAccess;
using GeorgeBrownTeacher_CourseManagement.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeorgeBrownTeacher_CourseManagement
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            using (var db = new TeacherCourseDBEntities())
            {
                //int username = Int32.Parse(txtUsername.Text);
                string password = txtPassword.Text;


                var login = (from x in db.Users
                             where x.UserID == 44444 
                             && x.Password == password
                             select x).FirstOrDefault();


                if (login != null)
                {
                    this.Hide();
                    CourseAssignmentForm a = new CourseAssignmentForm();
                    a.Show();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.. Must use Program Coordinator login info..",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }

            }
        }
    }
}
