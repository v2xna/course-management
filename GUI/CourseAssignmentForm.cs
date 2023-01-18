using GeorgeBrownTeacher_CourseManagement.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace GeorgeBrownTeacher_CourseManagement.GUI
{
    public partial class CourseAssignmentForm : Form
    {
        public CourseAssignmentForm()
        {
            InitializeComponent();
            loadListViewAssignment();
        }

        private void loadListViewAssignment()
        {
            using (var db = new TeacherCourseDBEntities())
            {

                var query = from assign in db.Assignments
                            select new
                            {
                                assignTeacher = assign.TeacherId,
                                assignCourse = assign.CourseNumber,
                                assignDate = assign.StartDate
                            };

                listView1.Items.Clear();

                foreach (var i in query)
                {
                    ListViewItem listItem = new ListViewItem(i.assignTeacher.ToString());
                    listItem.SubItems.Add(i.assignCourse.ToString());
                    listItem.SubItems.Add(i.assignDate.ToString());

                    listView1.Items.Add(listItem);
                }

            }
        }

        private void CourseAssignmentForm_Load(object sender, EventArgs e)
        {
            
            using (var db = new TeacherCourseDBEntities())
            {
                // ComboBox Teacher
                var tch = db.Teachers.Select(x => new {
                    Display = x.TeacherId + ", " + x.FirstName + " " + x.LastName,
                    Value = x.TeacherId
                }).ToList();

                cbTeacher.DisplayMember = "Display";
                cbTeacher.ValueMember = "Value";
                cbTeacher.DataSource = tch;

                cbTeacher.SelectedValue = -1;


                // ComboBox Teacher
                var crs = db.Courses.Select(x => new {
                    Display = x.CourseNumber + ", " + x.CourseTitle + ", " + x.Duration + "h",
                    Value = x.CourseNumber
                }).ToList();

                cbCourse.DisplayMember = "Display";
                cbCourse.ValueMember = "Value";
                cbCourse.DataSource = crs;

                cbCourse.SelectedValue = -1;


                //cbTeacher.DataSource = db.Teachers.ToList();
                //cbTeacher.ValueMember = "TeacherId";
                //cbTeacher.DisplayMember = "TeacherId";

                //cbCourse.DataSource = db.Courses.ToList();
                //cbCourse.ValueMember = "CourseNumber";
                //cbCourse.DisplayMember = "CourseNumber";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var db = new TeacherCourseDBEntities())
            {
                if (cbTeacher.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a teacher");
                    return;
                }

                if (cbCourse.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a course");
                    return;
                }

                int teacherId = Int32.Parse(cbTeacher.SelectedValue.ToString());
                string courseNumber = cbCourse.SelectedValue.ToString();

                // if teacherId and CourseNumber exist in the database
                var ifexist = (from y in db.Assignments
                               where y.TeacherId == teacherId &&
                                     y.CourseNumber == courseNumber
                               select y).FirstOrDefault();

                // Cannot add duplicate values
                if (ifexist != null)
                {
                    MessageBox.Show("This Teacher is already teaching this course",
                                    "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                }
                else
                {

                    // Returns the total number of rows for a specific teacher
                    var qryCourseCount = (from x in db.Assignments
                                          where x.TeacherId == teacherId
                                          select x).Count();

                    // Checks if teacher has more than 4 courses
                    if (qryCourseCount >= 4)
                    {
                        MessageBox.Show("Teacher cannot have more than 4 courses",
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                    else
                    {
                        // Add to database
                        Assignment newAssignment = new Assignment
                        {
                            TeacherId = teacherId,
                            CourseNumber = courseNumber,
                            StartDate = dateTimePicker1.Value
                        };


                        db.Assignments.Add(newAssignment);
                        db.SaveChanges();

                        loadListViewAssignment();
                        MessageBox.Show("Course Assignment was added!",
                                        "Information",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                }

            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (var db = new TeacherCourseDBEntities())
            {
                if (cbTeacher.SelectedIndex == -1 || cbCourse.SelectedIndex == -1)
                {
                    MessageBox.Show("In order to delete a Course Assignment you have to select in the combobox a teacher and course",
                                    "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }


                int teacherId = Int32.Parse(cbTeacher.SelectedValue.ToString());
                string courseNumber = cbCourse.SelectedValue.ToString();


                var query = (from y in db.Assignments
                         where y.TeacherId == teacherId &&
                               y.CourseNumber == courseNumber
                         select y).FirstOrDefault();

                if (query == null)
                {
                    MessageBox.Show("Please select IN THE COMBOBOX a EXISTING Course Assignment to delete",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                else
                {
                    db.Assignments.Remove(query);
                    db.SaveChanges();

                    loadListViewAssignment();
                    MessageBox.Show("Course Assignment was deleted");

                    cbTeacher.SelectedValue = -1;
                    cbCourse.SelectedValue = -1;
                }

            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            
            using (var db = new TeacherCourseDBEntities())
            {
                if (cbTeacher.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select IN THE COMBOBOX a teacher to search");
                    return;
                }

                int teacherId = Int32.Parse(cbTeacher.SelectedValue.ToString());

                var result = from a in db.Assignments
                             join t in db.Teachers on a.TeacherId equals t.TeacherId
                             join c in db.Courses on a.CourseNumber equals c.CourseNumber
                             where a.TeacherId == teacherId
                             select new
                             {
                                 joinTeacherId = t.TeacherId,
                                 joinFirstName = t.FirstName,
                                 joinLastName = t.LastName,
                                 joinEmail = t.Email,
                                 joinCourseNumber = c.CourseNumber,
                                 joinCourseTitle = c.CourseTitle,
                                 joinDuration = c.Duration
                             };


                string multipleData = "";
                string singleData = "";

                foreach (var i in result)
                {

                    singleData = "Name: " + i.joinFirstName.ToString() + " " + i.joinLastName.ToString() + "\n " + 
                                 "Email: " + i.joinEmail.ToString() + "\n " + 
                                 "Assigned Courses: \n";

                    multipleData += i.joinCourseNumber.ToString() + ", " + i.joinCourseTitle.ToString() + ", " + i.joinDuration.ToString() + "h,\n ";

                }


                if (result.Any())
                {
                    MessageBox.Show(singleData + multipleData,
                                    "Information",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    cbTeacher.SelectedValue = -1;
                }
                else
                {
                    MessageBox.Show("Select IN THE COMBOBOX a teacher that has courses",
                                    "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                }

            }
        }
    }
}
